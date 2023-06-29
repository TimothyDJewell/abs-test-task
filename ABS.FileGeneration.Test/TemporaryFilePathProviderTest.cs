namespace ABS.FileGeneration.Test
{
    [TestFixture]
    public class TemporaryFilePathProviderTest
    {

        [TestCase(@"some\fake\path", null, @"some\\fake\\path\\[a-f0-9]{32}")]
        [TestCase(@"some\fake\path", "", @"some\\fake\\path\\[a-f0-9]{32}")]
        [TestCase(@"some\fake\path", ".xlsx", @"some\\fake\\path\\[a-f0-9]{32}\.xlsx")]
        [TestCase(@"some\fake\path", ".txt", @"some\\fake\\path\\[a-f0-9]{32}\.txt")]
        [TestCase(@"C:\AnotherPlace\", ".txt", @"C:\\AnotherPlace\\[a-f0-9]{32}\.txt")]
        public void Create_ParameterizedValues_ReturnsExpectedValue(string folder, string extension, string expectedFilePathRegex)
        {
            var subject = new TemporaryFilePathProvider(folder);

            TemporaryFilePath result = subject.Create(extension);

            Assert.That(result.Value, Does.Match(expectedFilePathRegex));
        }

        [TestCase(@"")]
        [TestCase(null)]
        [TestCase(@"      ")]
        public void Ctor_WhiteSpacePath_ThrowsArgumentException(string folderPath)
        {
            Assert.That(
                () => new TemporaryFilePathProvider(folderPath),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("folder"));
        }

        [Test]
        public void Create_InvalidExtension_ThrowsArgumentException()
        {
            var subject = new TemporaryFilePathProvider(@"some\fake\path");

            Assert.That(
                () => subject.Create("invalid extension"),
                Throws.ArgumentException.With.Property(nameof(ArgumentException.ParamName)).EqualTo("fileExtension"));
        }

        [TestCase]
        [TestCase(".xlsx")]
        [TestCase(null, ".txt", ".xlsx")]
        public void Dispose_SpecifiedCreateCallsAlreadyProcessed_DeletesFiles(params string[] createdFiles)
        {
            var subject = Substitute.ForPartsOf<TemporaryFilePathProvider>(@"some\fake\path");
            subject.When(o => o.DeleteFile(Arg.Any<string>())).DoNotCallBase();
            foreach (string extension in createdFiles)
            {
                subject.Create(extension);
            }

            subject.ClearReceivedCalls();

            subject.Dispose();

            subject.Received(createdFiles.Length).DeleteFile(Arg.Any<string>());
        }
    }
}
