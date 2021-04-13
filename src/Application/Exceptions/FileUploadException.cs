using System;

namespace RecipeBook.Core.Application.Exceptions
{
    public abstract class FileUploadException : Exception
    {
        public FileUploadException()
        {
        }

        public FileUploadException(string message)
            : base(message)
        {
        }

        public FileUploadException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class FileLengthZeroException : FileUploadException
    {
        public FileLengthZeroException()
        {
        }

        public FileLengthZeroException(string message)
            : base(message)
        {
        }

        public FileLengthZeroException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }

    public class FileTooLargeException : FileUploadException
    {
        public FileTooLargeException()
        {
        }

        public FileTooLargeException(string message)
            : base(message)
        {
        }

        public FileTooLargeException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}