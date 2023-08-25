using Castle.Core.Logging;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using StudioManagement.Data;
using StudioManagement.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudioManagement.Tests.Controller
{
    public class HomeControllerTests
    {
        private ILogger _logger;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        public HomeControllerTests()
        {
            //Depen
            _logger = A.Fake<ILogger>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
        }

        [Fact]
        public void HomeController_Index_ReturnsSuccess()
        {
            //Arrange - What do I need to bring in?
            var studioList = A.Fake<List<Studio>>();
            A.CallTo(() => _unitOfWork);
            //Act

            //Assert
        }
    }
}
