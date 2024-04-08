using System.Globalization;
using System.Threading;
using NUnit.Framework;

namespace Epos.Utilities;

[SetUpFixture]
public class TestSuite
{
    [OneTimeSetUp]
    public void SetUp() {
        Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
    }
}
