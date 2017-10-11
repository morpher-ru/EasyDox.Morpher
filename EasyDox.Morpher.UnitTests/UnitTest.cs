namespace EasyDox.Morpher.UnitTests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class UnitTest
    {
        private string ФамилияИнициалы(string s)
        {
            MorpherFunctionPack functionPack = new MorpherFunctionPack(null, null);
            var func = functionPack.Functions["фамилия и. о."];
            return (string) func.DynamicInvoke(s);
        }

        [Test]
        public void ИП_Test() => Assert.AreEqual("Слепов С. Н.", ФамилияИнициалы("ИП Слепов Сергей Николаевич"));

        [Test]
        public void ИП_Exception()
        {
            try
            {
                ФамилияИнициалы("ИП");
                throw new Exception();
            }
            catch (TargetInvocationException exc)
            {
                Assert.AreEqual(typeof(ArgumentException), exc.InnerException.GetType());
            }
        }

        [Test]
        public void Dot_Test() => Assert.AreEqual("Слепов С.", ФамилияИнициалы("Слепов С. ."));

        [Test]
        public void DoubleDot_Test() => Assert.AreEqual("Слепов С.", ФамилияИнициалы("Слепов С.."));
    }
}
