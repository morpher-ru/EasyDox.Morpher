namespace EasyDox.Morpher.UnitTests
{
    using System;
    using System.Reflection;
    using NUnit.Framework;

    [TestFixture]
    public class UnitTest
    {
        [Test]
        public void ИП_Test() => Assert.AreEqual("Слепов С. Н.", MorpherFunctionPack.ФамилияИнициалы("ИП Слепов Сергей Николаевич"));

        [Test]
        public void ИП_Exception() => Assert.Throws<ArgumentException>(() => MorpherFunctionPack.ФамилияИнициалы("ИП"));

        [Test]
        public void Dot_Test() => Assert.AreEqual("Слепов С.", MorpherFunctionPack.ФамилияИнициалы("Слепов С. ."));

        [Test]
        public void DoubleDot_Test() => Assert.AreEqual("Слепов С.", MorpherFunctionPack.ФамилияИнициалы("Слепов С.."));
    }
}
