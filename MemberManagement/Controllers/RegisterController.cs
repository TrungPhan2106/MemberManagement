using Microsoft.AspNetCore.Mvc;
using StudioManagement.Data;
using StudioManagement.Models;
using StudioManagement.Repository.IRepository;

namespace StudioManagement.Controllers
{
    public class RegisterController : Controller
    {
        private readonly ILogger<RegisterController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public RegisterController(IUnitOfWork unitOfWork, ILogger<RegisterController> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }
        public IActionResult Index(int pg = 1)
        {
            List<StudioRegister> registList = _unitOfWork.Register.GetAll(includeProperties: "Studio").ToList();
            registList = _unitOfWork.Register.GetAll(includeProperties: "Member").ToList();
            const int pageSize = 9;
            if (pg < 1) pg = 1;
            int recsCount = registList.Count();
            var paper = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = registList.Skip(recSkip).Take(paper.PageSize).ToList();

            this.ViewBag.Pager = paper;
            return View(data);
        }

    }
}
