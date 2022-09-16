using System;

namespace DataAccess.Exceptions
{
    public class FileDataLoadException : Exception
    {
        public FileDataLoadException(string filePath) : base($"File {filePath} cannot be loaded.") { }
    }
}
