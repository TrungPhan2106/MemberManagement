using AutoMapper;
using StudioManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace StudioManagement.Controllers
{
    public class MemberController : Controller
    {
        
        private readonly ILogger<MemberController> _logger;
        private readonly IMapper _mapper;
        private readonly MyDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MemberController(IMapper mapper,ILogger<MemberController> logger, MyDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _db = db;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
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
            IEnumerable<SelectListItem> StudioList = _db.Studio.Select(u => new SelectListItem
            {
                Text = u.StudioName,
                Value = u.StudioID.ToString()
            });
            ViewBag.StudioList = StudioList;
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Member member, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string memberPath = Path.Combine(wwwRootPath, @"images/member");

                    using (var fileStream = new FileStream(Path.Combine(memberPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    member.ImageUrl = @"\images\member\" + fileName;
                }

                member.MemberUUId = Guid.NewGuid().ToString();
                _db.Member.Add(member);
                _db.SaveChanges();
                TempData["success"] = "Member updated successfully";
                return RedirectToAction("MemberIndex");
            }
            else
            {
                IEnumerable<SelectListItem> StudioList = _db.Studio.Select(u => new SelectListItem
                {
                    Text = u.StudioName,
                    Value = u.StudioID.ToString()
                });
                ViewBag.StudioList = StudioList;
                return View();
            }
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
            IEnumerable<SelectListItem> StudioList = _db.Studio.Select(u => new SelectListItem
            {
                Text = u.StudioName,
                Value = u.StudioID.ToString()
            });
            ViewBag.StudioList = StudioList;
            return View(memberFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Member member, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string memberPath = Path.Combine(wwwRootPath, @"images/member");

                    if(!string.IsNullOrEmpty(member.ImageUrl)) 
                    { 
                        //delete ola image
                        var oldImagePath = 
                            Path.Combine(wwwRootPath, member.ImageUrl.TrimStart('\\'));

                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(memberPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    member.ImageUrl = @"\images\member\" + fileName;
                }

                member.UpdatedDate = DateTime.Now;
                _db.Member.Update(member);
                _db.SaveChanges();
                TempData["success"] = "Member updated successfully";
                return RedirectToAction("MemberIndex");
            }
            else
            {
                IEnumerable<SelectListItem> StudioList = _db.Studio.Select(u => new SelectListItem
                {
                    Text = u.StudioName,
                    Value = u.StudioID.ToString()
                });
                ViewBag.StudioList = StudioList;
                return View();
            }
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

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
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
