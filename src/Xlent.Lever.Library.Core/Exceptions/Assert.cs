using Xlent.Lever.Library.Core.Exceptions.Service.Server;

namespace Xlent.Lever.Library.Core.Exceptions
{
    public static class Assert
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
