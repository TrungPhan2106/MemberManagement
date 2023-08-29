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

namespace StudioManagement.Tests.Controller
{
    public class HomeControllerTest
    {
        private HomeController _studioController;
        private ILogger<HomeController> _logger;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public HomeControllerTest(/*ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment*/)
        {
            _logger = A.Fake< ILogger<HomeController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();

            _studioController = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
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
            //var controller = new HomeController( _logger,  _unitOfWork,  _webHostEnvironment);
            //var result = _studioController.Index(strSearch, pg) /*as ViewResult*/;
            //Assert.Equal("Index", result?.ViewName);
            //result.Should().BeOfType<IActionResult>();

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
        public IActionResult CreateEquipment_ModelIsValid_ReturnCreated()
        {
            var studio = new Studio();
            string wwwRootPath = _webHostEnvironment.WebRootPath;
            string filePath = Path.Combine(wwwRootPath, @"images\studio");
            string fileName = Guid.NewGuid().ToString();
            string contentType = "application/octet-stream";
            var stream = new FileStream(filePath, FileMode.Open);
            IFormFile? file= new FormFile(stream, 0, stream.Length, null, fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };
            _unitOfWork.Studio.Add(studio);
            _unitOfWork.Save();
            var result = _studioController.Create(studio, file);
            return result;
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
    }
}
