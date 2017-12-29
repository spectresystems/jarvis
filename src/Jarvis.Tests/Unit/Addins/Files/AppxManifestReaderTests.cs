using Jarvis.Addin.Files.Sources.Uwp;
using Jarvis.Core.Diagnostics;
using Jarvis.Tests.Fakes;
using NSubstitute;
using Shouldly;
using Spectre.System;
using Spectre.System.IO;
using Spectre.System.Testing;
using Xunit;

namespace Jarvis.Tests.Unit.Addins.Files
{
    public sealed class AppxManifestReaderTests
    {
        [Fact]
        public void Should_Read_Package_Information()
        {
            var fixture = new Fixture();
            fixture.FileSystem.CreateFile(new FilePath("C:/app/Assets/StoreLogo.scale-200.png"));
            fixture.FileSystem.CreateFile(new FilePath("C:/app/Assets/Square44x44Logo.scale-200.png"));
            fixture.FileSystem.CreateFile(new FilePath("C:/app/Assets/Square150x150Logo.scale-200.png"));
            fixture.FileSystem.CreateFile(new FilePath("C:/app/Assets/Square310x310Logo.scale-200.png"));
            fixture.FileSystem.CreateFileFromResource("C:/app/AppxManifest.xml", "Jarvis.Tests/Data/NuGetPackageExplorer.xml");

            // When
            var result = fixture.Reader.Read(new FilePath("C:/app/AppxManifest.xml"));

            // Then
            result.Id.ShouldBe("50582LuanNguyen.NuGetPackageExplorer");
            result.Logo.Name.ShouldBe(@"Assets\StoreLogo.png");
            result.Logo.Path.FullPath.ShouldBe("C:/app/Assets/StoreLogo.scale-200.png");
            result.Description.ShouldBeNull();
            result.DisplayName.ShouldBe("NuGet Package Explorer");
            result.Publisher.ShouldBe("Oren Novotny, LLC");
        }

        private sealed class Fixture
        {
            public FakeFileSystem FileSystem { get; }
            public AppxManifestReader Reader { get; }

            public Fixture()
            {
                FileSystem = new FakeFileSystem(new FakeEnvironment(PlatformFamily.Windows));
                Reader = new AppxManifestReader(
                    FileSystem,
                    new FakeNativeStreamProvider(FileSystem),
                    Substitute.For<IJarvisLog>());
            }
        }
    }
}
