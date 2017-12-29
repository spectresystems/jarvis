using System;
using System.Runtime.InteropServices.ComTypes;
using Jarvis.Addin.Files.Sources.Uwp;
using Jarvis.Tests.Utilities;
using Spectre.System.IO;

namespace Jarvis.Tests.Fakes
{
    public sealed class FakeNativeStreamProvider : INativeStreamProvider
    {
        private readonly IFileSystem _fileSystem;

        public FakeNativeStreamProvider(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public IStream GetStream(FilePath path)
        {
            var file = _fileSystem.GetFile(path);
            if (!file.Exists)
            {
                throw new InvalidOperationException("File does not exist.");
            }
            return new StreamWrapper(file.OpenRead());
        }

        public void ReleaseStream(IStream stream)
        {
        }
    }
}
