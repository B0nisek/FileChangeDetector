using Microsoft.VisualStudio.TestTools.UnitTesting;
using BL.Helpers;

namespace Tests.BL
{
    [TestClass]
    public class SizeConverterTests
    {
        private const long size = 100000000;
        private const double expectedOutput = 95.367431640625;

        [TestMethod]
        public void ConvertBytesToMegaBytesCorrectResult()
        {
            var result = SizeConverter.ConvertBytesToMegaBytes(size);
            Assert.AreEqual(expectedOutput, result);
        }
    }
}
