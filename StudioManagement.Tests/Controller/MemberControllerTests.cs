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
            int? StudioID = 0;
            var objMemberList = new List<Member>();
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, 8);
            int recSkip = (pg - 1) * 8;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();
            //var controller = new HomeController(_logger, _unitOfWork, _webHostEnvironment);
            var result = _memberController.MemberIndex(strSearch, pg, StudioID) as ViewResult;
            var member = (Member?)result?.ViewData.Model;
            Assert.Null(member);
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
    }
}
