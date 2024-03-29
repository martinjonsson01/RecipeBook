﻿using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using FluentAssertions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Moq;

using RecipeBook.Core.Application.Exceptions;
using RecipeBook.Core.Application.FileStorage;
using RecipeBook.Presentation.WebApp.Server.Controllers.v1;

using Xunit;

namespace Tests.WebApp.Server.Controllers
{
    public class ImagesControllerTests
    {
        public ImagesControllerTests()
        {
            var mockedEnvironment = new Mock<IWebHostEnvironment>();
            mockedEnvironment.SetupGet(env => env.ContentRootPath).Returns("root/path");
            mockedEnvironment.SetupGet(env => env.EnvironmentName).Returns("environmentName");
            var mockedHttpContext = new Mock<HttpContext>();
            var mockedRequest     = new Mock<HttpRequest>();
            mockedRequest.SetupGet(request => request.Scheme).Returns("https");
            mockedRequest.SetupGet(request => request.Host).Returns(new HostString("localhost"));
            mockedHttpContext.SetupGet(context => context.Request).Returns(mockedRequest.Object);
            var controllerContext = new ControllerContext
            {
                HttpContext = mockedHttpContext.Object
            };
            _fileStore = new Mock<IFileStorer>();
            _controller = new ImagesController(
                new Mock<ILogger<ImagesController>>().Object,
                _fileStore.Object)
            {
                ControllerContext = controllerContext
            };
        }

        private readonly ImagesController  _controller;
        private readonly Mock<IFileStorer> _fileStore;

        private string MockFileName() => $"{Path.GetRandomFileName()}.jpg";

        private IFormFile MockImageFile(string? name = default)
        {
            name ??= MockFileName();
            var imageStream = new MemoryStream(GenerateImageByteArray());
            var validImage = new FormFile(imageStream, 0, imageStream.Length, name, name)
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
            return validImage;
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
        public void PostImage_ReturnsStoredImageLocation_WithValidImage()
        {
            // Arrange
            IFormFile validImageFile = MockImageFile();

            // Act
            IActionResult response = _controller.PostImage(validImageFile, string.Empty);

            // Assert
            response.Should().BeAssignableTo<CreatedAtActionResult>();
            var objectResult = (CreatedAtActionResult) response;

            // Needs to contain these keys so resource location is correct.
            objectResult.RouteValues.Keys.Should().Contain("imageName");
            objectResult.RouteValues.Keys.Should().Contain("recipeName");
            objectResult.ActionName.Should().Be(nameof(ImagesController.GetImage));
        }

        [Fact]
        public void PostImage_Returns406NotAcceptable_WhenFileStoreThrowsFileLengthZeroException()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            _fileStore.Setup(fileStore => fileStore.SaveFile(It.IsAny<Stream>()))
                      .Throws<FileLengthZeroException>();

            // Act
            IActionResult response = _controller.PostImage(file.Object, string.Empty);

            // Assert
            response.Should().BeAssignableTo<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult) response;

            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status406NotAcceptable);
        }

        [Fact]
        public void PostImage_Returns413RequestEntityTooLarge_WhenFileStoreThrowsFileTooLargeException()
        {
            // Arrange
            var file = new Mock<IFormFile>();
            _fileStore.Setup(fileStore => fileStore.SaveFile(It.IsAny<Stream>()))
                      .Throws<FileTooLargeException>();

            // Act
            IActionResult response = _controller.PostImage(file.Object, string.Empty);

            // Assert
            response.Should().BeAssignableTo<StatusCodeResult>();
            var statusCodeResult = (StatusCodeResult) response;

            statusCodeResult.StatusCode.Should().Be(StatusCodes.Status413RequestEntityTooLarge);
        }

        [Fact]
        public void GetImage_Returns404NotFound_WhenFileDoesNotExist()
        {
            // Arrange
            _fileStore.Setup(fileStore => fileStore.Exists(It.IsAny<string>()))
                      .Returns(false);

            // Act
            IActionResult response = _controller.GetImage(string.Empty, "nonexistent-file");

            // Assert
            response.Should().BeAssignableTo<NotFoundResult>();
        }

        [Fact]
        public void GetImage_ReturnsJpegFile_WhenFileExists()
        {
            // Arrange
            const string existingFile = "existing-file.jpg";
            _fileStore.Setup(fileStore => fileStore.Exists(existingFile))
                      .Returns(true);
            string filePath = Path.GetTempFileName();
            _fileStore.Setup(fileStore => fileStore.LoadFile(existingFile))
                      .Returns(File.OpenRead(filePath));
            
            // Act
            IActionResult response = _controller.GetImage(string.Empty, existingFile);

            // Assert
            response.Should().BeAssignableTo<FileResult>();
            var fileResult = (FileResult) response;

            fileResult.ContentType.Should().Be("image/jpeg");
        }
    }
}