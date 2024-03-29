﻿using System.Drawing;
using System.IO;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using RecipeBook.Core.Application.Exceptions;
using RecipeBook.Core.Application.FileStorage;

namespace RecipeBook.Infrastructure.Persistence.FileStorage
{
    public class ImageFileStorer : IFileStorer
    {
        public const     string                   ImageUploadsDirectory = "recipe-image-uploads";
        private readonly ILogger<ImageFileStorer> _logger;
        private readonly IWebHostEnvironment      _env;

        public ImageFileStorer(
            ILogger<ImageFileStorer> logger,
            IWebHostEnvironment      env)
        {
            _logger = logger;
            _env = env;
        }

        private string GetFilePath(string fileName)
        {
            return Path.Combine(_env.ContentRootPath, ImageUploadsDirectory, fileName);
        }

        public bool Exists(string fileName)
        {
            return File.Exists(GetFilePath(fileName));
        }

        public FileStream LoadFile(string fileName)
        {
            return File.OpenRead(GetFilePath(fileName));
        }

        public string SaveFile(Stream stream)
        {
            /*
             * WARNING: THIS CODE IS NOT SAFE FOR ACTUAL PRODUCTION USE
             *
             * The image should be validated by some kind of malware scanner before saving.
             */

            const long maxFileSize = 1024 * 512;

            switch (stream.Length)
            {
                case 0:
                    throw new FileLengthZeroException("Length is 0");
                case > maxFileSize:
                    throw new FileTooLargeException($"File of {stream.Length} bytes " +
                                                    $"is larger than the limit of {maxFileSize} bytes");
            }

            string fileName = $"{Path.GetRandomFileName()}.jpg";
            string path = Path.Combine(_env.ContentRootPath, ImageUploadsDirectory,
                fileName);
            Image returnImage = Image.FromStream(stream);
            returnImage.Save(path);
            _logger.LogInformation("{FileName} saved at {Path}", fileName, path);

            return fileName;
        }
    }
}