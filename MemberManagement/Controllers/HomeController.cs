using MemberManagement.Data;
using MemberManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;

namespace MemberManagement.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MyDbContext _context;
        
        public HomeController(ILogger<HomeController> logger, MyDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(string strSearch)
        {
            var members = _context.Member.ToList();
            if (!String.IsNullOrEmpty(strSearch))
            {
                members = members.Where(x => x.UserName.Contains(strSearch)).ToList();
            }
            return View(members);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /*
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Member member)
        {
            if (ModelState.IsValid)
            {
                MemberList memberList = new MemberList();
                memberList.AddMember(member);
                return RedirectToAction("Index");
            }
            return View();
        }
        public ActionResult Edit(string memberId = "")
        {
            MemberList memberList = new MemberList();
            List<Member> members = memberList.getMember(memberId);
            return View(members.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Edit(Member member)
        {
            MemberList memlist = new MemberList();
            memlist.UpdateMember(member);
            return RedirectToAction("Index");
        }
        public ActionResult Detail(string memberId = "")
        {
            MemberList memberList = new MemberList();
            List<Member> obj = memberList.getMember(memberId);
            return View(obj.FirstOrDefault());
        }
        public ActionResult Delete(string memberId = "")
        {
            MemberList memberList = new MemberList();
            List<Member> obj = memberList.getMember(memberId);
            return View(obj.FirstOrDefault());
        }
        [HttpPost]
        public ActionResult Delete(Member mem)
        {
            MemberList memberList = new MemberList();
            memberList.DeleteMember(mem);
            return RedirectToAction("Index");
        }*/
    }
}
