using System;
using System.Collections.Generic;
using System.Globalization;
using Morpher.Generic;
using Morpher.Russian;

namespace EasyDox
{
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Предоставляет функции склонения по падежам и прописи денежных сумм на русском языке.
    /// </summary>
    public class MorpherFunctionPack
    {
        private readonly IDeclension declension;
        private readonly INumberSpelling numberSpelling;

        /// <summary>
        /// Creates a new instance of the <see cref="MorpherFunctionPack"/>.
        /// </summary>
        public MorpherFunctionPack (IDeclension declension, INumberSpelling numberSpelling)
        {
            this.declension = declension;
            this.numberSpelling = numberSpelling;
        }

        /// <summary>
        /// 
        /// </summary>
        public Dictionary <string, Delegate> Functions => new Dictionary <string, Delegate>
        {
            {"родительный", new Func<string, string>(Родительный)},
            {"цифрами и прописью", new Func<string, string> (Пропись)},
            {"фамилия и. о.", new Func<string, string> (ФамилияИнициалы)},
        };

        static string ФамилияИнициалы (string fieldValue)
        {
            if (string.IsNullOrWhiteSpace(fieldValue))
            {
                throw new ArgumentException(fieldValue);
            }

            var parts = fieldValue.Split (" .".ToCharArray (), StringSplitOptions.RemoveEmptyEntries).ToList();
            if (parts.Count == 0)
            {
                throw new ArgumentException("Нет частей ФИО");
            }

            if (parts[0].ToLowerInvariant() == "ип")
            {
                parts.RemoveAt(0);
            }

            if (parts.Count == 0)
            {
                throw new ArgumentException("После удаления ИП нет частей ФИО");
            }

            StringBuilder fioBuilder = new StringBuilder();
            fioBuilder.Append(parts[0]);
            if (parts.Count > 1)
            {
                foreach (var part in parts.Skip(1))
                {
                    fioBuilder.AppendFormat(" {0}.", part[0]);
                }
            }

            return fioBuilder.ToString();
        }

        string Родительный (string fieldValue)
        {
            var paradigm = declension.Parse(fieldValue);

            return paradigm == null 
                ? fieldValue // TODO: return a warning / error?
                : paradigm.Genitive; 
        }

        string Пропись (string fieldValue)
        {
            return new ДенежнаяЕдиница 
                       {
                           ПолноеНаименованиеЦелойЧасти = "рубль",
                           ПолноеНаименованиеДробнойЧасти = "копейка",
                           СокращенноеНаименованиеЦелойЧасти = "руб.",
                           СокращенноеНаименованиеДробнойЧасти = "коп."
                       }
                .СуммаПрописью (Decimal.Parse (fieldValue, CultureInfo.GetCultureInfo ("en-US")), numberSpelling.AsGeneric().DefaultCase());
        }
    }
}