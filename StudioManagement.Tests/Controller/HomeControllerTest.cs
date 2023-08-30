using StudioManagement.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using StudioManagement.Controllers;
using Microsoft.AspNetCore.Mvc;
using StudioManagement.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StudioManagement.Repository.IRepository;
using log4net.Repository.Hierarchy;
using StudioManagement.Repository;
using FakeItEasy;
using FluentAssertions;
using DocumentFormat.OpenXml.Wordprocessing;
using StudioManagement.Models;
using Moq;
using NuGet.Protocol.Core.Types;
using log4net;
using AutoMapper;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.Extensions.Hosting;
using Syncfusion.HtmlConverter;
using Syncfusion.Pdf;
using System.Diagnostics;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.DataCollection;

namespace StudioManagement.Tests.Controller
{
    public class HomeControllerTest
    {
        private HomeController _studioController;
        private static log4net.ILog log = log4net.LogManager
           .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ILogger<HomeController> _logger;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public HomeControllerTest()
        {
            _logger = A.Fake< ILogger<HomeController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
            _logger.LogWarning("This is a MEL warning on the privacy page");

            _studioController = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
        }

        [Fact]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public void Error_Test()
        {
            ErrorViewModel error = new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
            };
            var result = _studioController.Error() as ViewResult;
            var err = result?.ViewData.Model as ErrorViewModel;
            Assert.NotNull(err);
        }

        [Fact]
        public IActionResult Privacy_Test()
        {
            log.Info("About page privacy.");
            return _studioController.Privacy();
        }

        [Fact]
        public List<Studio> Test_Index_ReturnSuccess()
        {
            //var strSearch = "";
            var pg = 1;
            var studioList = new List<Studio>();
            studioList = _unitOfWork.Studio.GetAll().ToList();
            int recsCount = studioList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = studioList.Skip(recSkip).Take(paper.PageSize).ToList();
            return (data);

        }
        [Fact]
        public void Test_Index_ReturnViewData()
        {
            var strSearch = "";
            var pg = 1;
            var studioList = new List<Studio>()
            {
                new Studio() {StudioID = 1},
                new Studio() {StudioID = 2}
            };
            var mockService = new Mock<IUnitOfWork>();
            mockService.Setup(x => x.Studio.GetAll(It.IsAny<string>())).Returns(studioList);

            int recsCount = studioList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = studioList.Skip(recSkip).Take(paper.PageSize).ToList();

            var mockLogger = new Mock<ILogger<HomeController>>();
            var controller = new HomeController(mockLogger.Object, mockService.Object, _webHostEnvironment);
            var result = _studioController.Index(strSearch, pg) as ViewResult;
            var studio = (List<Studio>)result?.ViewData.Model;
            Assert.NotNull(studio);
        }

        [Fact]
        public Studio Test_Get_ReturnSuccess()
        {
            int StudioID = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Studio.Get(u => u.StudioID == StudioID);
            var result = _studioController.Get(StudioID);
            return (studioFromDb);
        }

        [Fact]
        public Studio Test_GetStudio_ReturnSuccess()
        {
            int StudioID = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Studio.Get(u => u.StudioID == StudioID);
            var result = _studioController.GetStudio(StudioID);
            return (studioFromDb);
        }

        [Fact]
        public IActionResult Create_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            Studio studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var actual = _studioController.Create();
            return actual;
        }

        [Fact]
        public IActionResult Delete_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var StudioID = 1;
            Studio studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var actual = _studioController.Delete(StudioID);
            return actual;
        }

        [Fact]
        public IActionResult Update_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var StudioID = 1;
            Studio studio = new Studio()
            {
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };
            var actual = _studioController.Edit(StudioID);
            return actual;
        }



        [Fact]
        public IActionResult CreateStudio_ModelIsValid_ReturnCreated()
        {
            var studio = new Studio();
            var file = new Mock<IFormFile>();
            var sourceImg = Path.GetFullPath(@"source image path");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sourceImg);
            writer.Flush();
            stream.Position = 0;
            var fileName = "text.jfif";
            file.Setup(f => f.FileName).Returns(fileName).Verifiable();
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
        .Returns((Stream stream, CancellationToken token) => stream.CopyToAsync(stream))
        .Verifiable();

            var controller = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
            var inputFile = file.Object;
            _unitOfWork.Studio.Add(studio);
            _unitOfWork.Save();
            var result = _studioController.Create(studio, inputFile);
            return result;
        }

        [Fact]
        public ActionResult ExportToPDF_test()
        {
            int StudioID = 1;
            var htmlConverter = new HtmlToPdfConverter();
            var blinkConverterSettings = new BlinkConverterSettings();
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(800, 0);
            htmlConverter.ConverterSettings = blinkConverterSettings;
            var document = new PdfDocument();
            document = htmlConverter.Convert($"https://localhost:7056/Home/GetStudio?StudioID={StudioID}");
            var stream = new MemoryStream();
            document.Save(stream);
            var result = _studioController.ExportToPDF(StudioID);
            return result;
        }

        [Fact]
        public IActionResult EditStudio_ModelIsValid_ReturnCreated()
        {
            var studio = new Studio();
            var file = new Mock<IFormFile>();
            var sourceImg = Path.GetFullPath(@"source image path");
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(sourceImg);
            writer.Flush();
            stream.Position = 0;
            var fileName = "text.jfif";
            file.Setup(f => f.FileName).Returns(fileName).Verifiable();
            file.Setup(_ => _.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
        .Returns((Stream stream, CancellationToken token) => stream.CopyToAsync(stream))
        .Verifiable();

            var controller = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
            var inputFile = file.Object;
            _unitOfWork.Studio.Update(studio);
            _unitOfWork.Save();
            var result = _studioController.Edit(studio, inputFile);
            return result;
        }

        [Fact]
        public IActionResult DeleteStudio_ModelIsValid_ReturnCreated()
        {
            var StudioID = 1;
            var studio = new Studio()
            {
                StudioID = StudioID,
                StudioName = "Studio 1",
                StudioAddress = "TP.HCM",
                StudioPhone = "000000000",
                StudioPic = "https://www.eatthis.com/wp-content/uploads/sites/4/2023/08/quinoa-bowl.jpg?quality=82&strip=1&w=640"
            };

            //var controller = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, studio.StudioPic.TrimStart('\\'));
            oldImagePath.Remove(oldImagePath.Length - 1);
            _unitOfWork.Studio.Remove(studio);
            _unitOfWork.Save();
            var result = _studioController.DeletePOST(StudioID);
            return result;
        }
    }
}
