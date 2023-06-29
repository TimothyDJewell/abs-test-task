namespace ABS.FileGeneration
{
    public class FileGenerationException : Exception
    {
        public FileGenerationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
