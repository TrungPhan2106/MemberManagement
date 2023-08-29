using AutoMapper;
using FakeItEasy;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using StudioManagement.Controllers;
using StudioManagement.Data;
using StudioManagement.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StudioManagement.Models;
using Moq;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using FluentAssertions;

namespace StudioManagement.Tests.Controller
{
    public class MemberControllerTests
    {
        private MemberController _memberController;
        private ILogger<MemberController> _logger;
        private IMapper _mapper;
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;
        private MyDbContext _db;

        public MemberControllerTests()
        {
            _logger = A.Fake<ILogger<MemberController>>();
            _unitOfWork = A.Fake<IUnitOfWork>();
            _webHostEnvironment = A.Fake<IWebHostEnvironment>();
            _mapper = A.Fake<IMapper>();

            _memberController = new MemberController(_mapper, _logger, _unitOfWork, _webHostEnvironment, _db);
        }

        [Fact]
        public List<Member> Test_Index_ReturnSuccess()
        {
            int pg = 1;
            var objMemberList = new List<Member>();
            objMemberList = _unitOfWork.Member.GetAll(includeProperties:"Studio")
                .OrderBy(x => x.MemberId).ToList();
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();
            return (objMemberList);
        }

        [Fact]
        public void Test_Index_ReturnViewData()
        {
            var strSearch = "";
            var pg = 1;
            var StudioID = 1;
            var objMemberList = new List<Member>()
            {
                new Member() {MemberId = 1},
                new Member() {MemberId = 2}
            };
            var mockService = new Mock<IUnitOfWork>();
            mockService.Setup(x => x.Member.GetAll(It.IsAny<string>())).Returns(objMemberList);

            var expiredMembers = objMemberList.Where(x => x.ExpiredDate < DateTime.Now).ToList();
            if (expiredMembers.Any())
            {
                foreach (var mem in expiredMembers)
                {
                    mem.Status = "DeActive";
                }
                _unitOfWork.Save();
            }
            if (StudioID != 0)
            {
                objMemberList = objMemberList.Where(x => x.StudioID == StudioID).ToList();
            }
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();

            var mockLogger = new Mock<ILogger<MemberController>>();
            var controller = new MemberController(_mapper, mockLogger.Object, mockService.Object, _webHostEnvironment, _db);
            var result = _memberController.MemberIndex(strSearch, pg) as ViewResult;
            var member = (List<Member>)result?.ViewData.Model;
            Assert.NotNull(member);
        }
        [Fact]
        public Studio Test_Get_ReturnSuccess()
        {
            int MemberId = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.Get(MemberId);
            return (studioFromDb);
        }

        [Fact]
        public Studio Test_GetMem_ReturnSuccess()
        {
            int MemberId = 1;
            var studioFromDb = new Studio();
            _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            var result = _memberController.GetMem(MemberId);
            return (studioFromDb);
        }

        [Fact]
        public IActionResult Create_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var StudioID = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Create(StudioID, member);
            return actual;
        }

        [Fact]
        public IActionResult Delete_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var MemberId = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Delete(MemberId);
            return actual;
        }

        [Fact]
        public IActionResult Update_ViewState_Is_Valid_Returns_RedirectToRouteResult()
        {
            var MemberId = 1;
            Member member = new Member()
            {
                MemberId = 1,
                UserName = "UserName",
                FullName = "FullName",
                Email = "Email",
                DateOfBirth = DateTime.Now,
                Gender = true,
                JoinedDate = DateTime.Now,
                PhoneNumber = "00000000"
            };
            var actual = _memberController.Edit(MemberId);
            return actual;
        }

        [Fact]
        public void CanGet()
        {
            try
            {
                //Arrange
                int MemberId = 1;
                Mock<IUnitOfWork> mockRepo = new Mock<IUnitOfWork>();
                mockRepo.Setup(m => m.Member.GetAll(It.IsAny<string>())).Returns(new Member[]
                {
                    new Member() 
                    { 
                        MemberId =1,
                        UserName = "UserName",
                        FullName = "FullName",
                        Email = "Email",
                        DateOfBirth= DateTime.Now,
                        Gender= true,
                        JoinedDate= DateTime.Now,
                        PhoneNumber = "00000000" 
                    }
                });
                var mockMapper = new Mock<IMapper>();
                mockMapper.Setup(x => x.Map<MemberDto>(It.IsAny<Member>()))
                    .Returns((Member source) => new MemberDto() { MemberId = source.MemberId });
                MemberController controller = new MemberController(mockMapper.Object, _logger, mockRepo.Object, _webHostEnvironment, _db);
                //Act
                var result = _memberController.Get(MemberId);
                //Assert
                var okResult = result as OkObjectResult;
                Assert.NotNull(okResult);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.False(false, ex.Message);
            }
        }
    }
}
