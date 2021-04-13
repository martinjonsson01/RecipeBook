using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application.Exceptions;
using RecipeBook.Infrastructure.Persistence.Repositories;

using Xunit;

namespace Tests.Infrastructure.Persistence.Repositories
{
    public class ImageFileStorerTests
    {
        public ImageFileStorerTests()
        {
            var mockedEnvironment = new Mock<IWebHostEnvironment>();
            Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), ImageFileStorer.ImageUploadsDirectory));
            mockedEnvironment.SetupGet(env => env.ContentRootPath).Returns(Path.GetTempPath);
            mockedEnvironment.SetupGet(env => env.EnvironmentName).Returns("environmentName");
            _repo = new ImageFileStorer(
                new Mock<ILogger<ImageFileStorer>>().Object,
                mockedEnvironment.Object);
        }

        private readonly ImageFileStorer _repo;

        private static Stream MockImageStream()
        {
            return new MemoryStream(GenerateImageByteArray());
        }

        private static byte[] GenerateImageByteArray(int width = 1, int height = 1)
        {
            var      bitmapImage = new Bitmap(width, height);
            Graphics imageData   = Graphics.FromImage(bitmapImage);
            imageData.DrawLine(new Pen(Color.Blue), 0, 0, width, height);

            var    memoryStream = new MemoryStream();
            byte[] byteArray;

            using (memoryStream)
            {
                bitmapImage.Save(memoryStream, ImageFormat.Jpeg);
                byteArray = memoryStream.ToArray();
            }
            return byteArray;
        }


        [Fact]
        public void SaveFile_ReturnsResourceName_WithValidImage()
        {
            // Arrange
            Stream imageStream = MockImageStream();

            // Act
            string resourceName = _repo.SaveFile(imageStream);

            // Assert
            resourceName.Should().NotBeEmpty();
        }

        [Fact]
        public void SaveFile_ThrowsFileLengthZeroException_WithEmptyFile()
        {
            // Arrange
            var emptyStream = new Mock<Stream>();
            emptyStream.SetupGet(stream => stream.Length).Returns(0);

            // Act
            Func<string> method = () => _repo.SaveFile(emptyStream.Object);

            // Assert
            method.Should().Throw<FileLengthZeroException>();
        }

        [Fact]
        public void SaveFile_ThrowsFileTooLargeException_WithTooLargeFile()
        {
            // Arrange
            var tooLargeStream = new Mock<Stream>();
            tooLargeStream.SetupGet(stream => stream.Length).Returns(1024 * 1024 * 1024);

            // Act
            Func<string> method = () => _repo.SaveFile(tooLargeStream.Object);

            // Assert
            method.Should().Throw<FileTooLargeException>();
        }
    }
}