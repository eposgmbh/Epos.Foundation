using System.Globalization;
using NUnit.Framework;

namespace Epos.Utilities
{
    [TestFixture]
    public class ValueConversionExtensionsTest
    {
        [Test]
        public void TryConvert() {
            bool isSuccess = "33".TryConvert(typeof(int), CultureInfo.InvariantCulture, out object theObject);
            Assert.That(theObject, Is.EqualTo(33));
            Assert.That(isSuccess, Is.True);

            isSuccess = "XY".TryConvert(typeof(int), CultureInfo.InvariantCulture, out theObject);
            Assert.That(theObject, Is.Null);
            Assert.That(isSuccess, Is.False);

            isSuccess = "XY".TryConvert(typeof(StringExtensions), CultureInfo.InvariantCulture, out theObject);
            Assert.That(theObject, Is.Null);
            Assert.That(isSuccess, Is.False);

            isSuccess = "33".TryConvert(CultureInfo.InvariantCulture, out int theInteger);
            Assert.That(theInteger, Is.EqualTo(33));
            Assert.That(isSuccess, Is.True);

            isSuccess = "ABC".TryConvert(CultureInfo.InvariantCulture, out double theDouble);
            Assert.That(theDouble, Is.EqualTo(0.0));
            Assert.That(isSuccess, Is.False);
        }
    }
}