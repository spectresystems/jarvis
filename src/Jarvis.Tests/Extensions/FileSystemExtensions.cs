using System.IO;
using Jarvis.Tests.Utilities;
using Spectre.System.IO;
using Spectre.System.Testing;

// ReSharper disable once CheckNamespace
namespace Jarvis.Tests
{
    internal static class FileSystemExtensions
    {
        public static void CreateFileFromResource(this FakeFileSystem fileSystem, string path, string resource)
        {
            var stream = ResourceReader.GetStream(resource);
            if (path != null && stream != null)
            {
                var buffer = new byte[16 * 1024];
                using (var memory = new MemoryStream())
                {
                    int read;
                    while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        memory.Write(buffer, 0, read);
                    }
                    fileSystem.CreateFile(new FilePath(path), memory.ToArray());
                }
            }
        }
    }
}
