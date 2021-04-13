using System.IO;

namespace RecipeBook.Core.Application.FileStorage
{
    public interface IFileStorer
    {
        public bool Exists(string fileName);

        public FileStream LoadFile(string fileName);

        public string SaveFile(Stream stream);
    }
}