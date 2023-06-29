namespace ABS.FileGeneration.Test
{
    [TestFixture]
    public class FileGenerationServiceTest
    {
        private TemporaryFilePathProvider subTempFileProvider;
        private IWorkbookGenerator subWorkbookGenerator;


        [SetUp]
        public void Setup()
        {
            this.subTempFileProvider = new TemporaryFilePathProvider(@"C:\Some\Nonsense\Path");
            this.subWorkbookGenerator = Substitute.For<IWorkbookGenerator>();
        }

        private FileGenerationService CreateSubject() => new FileGenerationService(this.subTempFileProvider, this.subWorkbookGenerator);

        [Test]
        public void GenerateFile_HappyPath_ReturnsUsedPath()
        {
            var subject = CreateSubject();

            TemporaryFilePath result = subject.GenerateFile();

            Assert.That(result.Value, Does.StartWith(@"C:\Some\Nonsense\Path\"));
            this.subWorkbookGenerator.Received(1).Create(result);
        }

        [TestCase(typeof(FileNotFoundException))]
        [TestCase(typeof(IOException))]
        [TestCase(typeof(UnauthorizedAccessException))]
        [TestCase(typeof(System.Security.SecurityException))]
        public void GenerateFile_KnownExceptionDuringWorkbookGeneration_ThrowsWrappedException(Type exceptionType)
        {
            Exception exception = (Exception)Activator.CreateInstance(exceptionType)!;
            this.subWorkbookGenerator.When(g => g.Create(Arg.Any<TemporaryFilePath>())).Throw(exception);
            var subject = CreateSubject();

            Assert.That(() => subject.GenerateFile(), Throws.Exception.TypeOf<FileGenerationException>().And.InnerException.EqualTo(exception));
        }
    }
}