using System;
using System.Collections.Generic;
using System.Globalization;
using Morpher.Generic;
using Morpher.Russian;

namespace EasyDox
{
    /// <summary>
    /// Предоставляет функции склонения по падежам и прописи денежных сумм на русском языке.
    /// </summary>
    public class MorpherFunctionPack : IFunctionPack
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

        Dictionary <string, FunctionDefinition> IFunctionPack.Functions
        {
            get
            {
                return new Dictionary <string, FunctionDefinition>
                           {
                               {"родительный", new FunctionDefinition (Родительный)},
                               {"цифрами и прописью", new FunctionDefinition (Пропись)},
                               {"фамилия и. о.", new FunctionDefinition (ФамилияИнициалы)},
                           };
            }
        }

        static string ФамилияИнициалы (string fieldValue)
        {
            var parts = fieldValue.Split (" .".ToCharArray (), StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != 3) throw new Exception ("Ожидалось 3 части ФИО.");

            return parts [0] + " " + parts [1] [0] + ". " + parts [2] [0] + ".";
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