using AutoMapper;
using MemberManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using MemberManagement.Models;


namespace MemberManagement.Controllers
{
    public class MemberController : Controller
    {
        
        private readonly ILogger<MemberController> _logger;
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        public MemberController(IMapper mapper,ILogger<MemberController> logger, MyDbContext db)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
        }
        public IActionResult MemberIndex(string strSearch, int pg=1)
        {
            List<Member> objMemberList = _db.Member.ToList();
            if (!string.IsNullOrEmpty(strSearch))
            {
                objMemberList = objMemberList.Where(x => x.UserName.Contains(strSearch)).ToList();
            }
            const int pageSize = 9;
            if (pg < 1) pg = 1;
            int recsCount = objMemberList.Count();
            var paper = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg -1) * pageSize;
            var data = objMemberList.Skip(recSkip).Take(paper.PageSize).ToList();
            this.ViewBag.Pager = paper;
            return View(data);
        }
    
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Member member)
        {
            if (ModelState.IsValid)
            {
                member.MemberUUId = Guid.NewGuid().ToString();
                _db.Member.Add(member);
                _db.SaveChanges();
                TempData["success"] = "Member created successfully";
                return RedirectToAction("MemberIndex");
            }
            return View();
        }

        public IActionResult Edit(int? MemberId) 
        {
            if(MemberId == null || MemberId == 0)
            {
                return NotFound();
            }
            Member? memberFromDb = _db.Member.Find(MemberId);
            if (memberFromDb == null)
            {
                return NotFound();
            }
            return View(memberFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Member member)
        {
            if (ModelState.IsValid)
            {
                member.UpdatedDate = DateTime.Now;
                _db.Member.Update(member);
                _db.SaveChanges();
                TempData["success"] = "Member updated successfully";
                return RedirectToAction("MemberIndex");
            }
            return View();
        }
        public IActionResult Delete(int? MemberId)
        {
            if (MemberId == null || MemberId == 0)
            {
                return NotFound();
            }
            Member? memberFromDb = _db.Member.Find(MemberId);
            if (memberFromDb == null)
            {
                return NotFound();
            }
            return View(memberFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? MemberId)
        {
            Member? obj = _db.Member.Find(MemberId);
            if(obj == null)
            {
                return NotFound();
            }
            _db.Member.Remove(obj);
            _db.SaveChanges();
            TempData["success"] = "Member deleted successfully";
            return RedirectToAction("MemberIndex");
        }
        
        public ActionResult Get(int? MemberId)
        {
            if (MemberId == null || MemberId == 0)
            {
                return NotFound();
            }
            Member? memberFromDb = _db.Member.Find(MemberId);
            if (memberFromDb == null)
            {
                return NotFound();
            }
            return View(memberFromDb);
        }
        
    }
}
