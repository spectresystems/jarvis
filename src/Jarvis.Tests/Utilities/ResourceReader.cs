using System.IO;

namespace Jarvis.Tests.Utilities
{
    public static class ResourceReader
    {
        public static Stream GetStream(string path)
        {
            path = path.Replace("/", ".");
            return typeof(ResourceReader).Assembly.GetManifestResourceStream(path);
        }
    }
}
