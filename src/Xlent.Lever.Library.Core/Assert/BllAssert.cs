using Xlent.Lever.Library.Core.Exceptions;

namespace Xlent.Lever.Library.Core.Assert
{
    public static class BllAssert
    {
        public static void IsTrue(bool value, string message)
        {
            if (!value) Fail(message);
        }

        public static void Fail(string message)
        {
            throw new AssertionFailedException(message);
        }
    }
}
