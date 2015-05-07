using LocalCasino.Common.Exceptions;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;

namespace LocalCasino.Common.Tests.Exceptions
{
    [TestClass]
    public class CasinoExceptionTests
    {
        [TestMethod]
        public void TestCasinoExceptionMessage()
        {
            var ex = new CasinoException("testext");
            Assert.AreEqual("testext", ex.Message);
        }

        
    }
}