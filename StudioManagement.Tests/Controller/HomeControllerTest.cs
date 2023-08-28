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
                new Studio()
                {
                    StudioID = 1
                }
            };
            //var studioList = new Mock<List<Studio>>();
            //studioList.Setup(x => x.).Returns(studio);

            //int recsCount = studioList.Count();
            int recsCount = studioList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = studioList.Skip(recSkip).Take(paper.PageSize).ToList();
            //var controller = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
            var result = _studioController.Index(strSearch, pg) as ViewResult;
            var studio = (Studio?)result?.ViewData.Model;
            Assert.Null(studio);
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
    }
}
