using NUnit.Framework;
using WereMakingFunOfYou.Editor;

namespace WereMakingFunOfYou.Tests.Editor
{
    public sealed class LegacyInputHandlingSetupTests
    {
        [Test]
        public void ActiveInputHandlerCanBeReadWithoutChangingIt()
        {
            bool found = LegacyInputHandlingSetup.TryGetActiveInputHandler(out int activeInputHandler);

            Assert.That(found, Is.True, "The active Player Settings object and activeInputHandler property should be available.");
            Assert.That(activeInputHandler, Is.InRange(0, 2), "Expected Old (0), New (1), or Both (2).");
        }
    }
}
