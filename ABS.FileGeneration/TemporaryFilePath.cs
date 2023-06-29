using System;
using System.Collections.Generic;
using System.Text;

namespace ABS.FileGeneration
{
    /// <summary>
    /// A wrapper class for a file path to a temporary/short-lived file.
    /// </summary>
    /// <remarks>
    /// This could implement an interface to provide common abstraction if the need should arise for file access to non-temporary files.
    /// </remarks>
    public class TemporaryFilePath
    {
        public string Value { get; }

        internal TemporaryFilePath(string filePath)
        {
            this.Value = filePath;
        }
    }
}
