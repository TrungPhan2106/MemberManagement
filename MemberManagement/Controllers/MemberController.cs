using AutoMapper;
using StudioManagement.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using StudioManagement.Repository.IRepository;
using System.Drawing;

namespace StudioManagement.Controllers
{
    public class MemberController : Controller
    {
        
        private readonly ILogger<MemberController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public MemberController(IMapper mapper,ILogger<MemberController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult MemberIndex(string strSearch, int pg=1, int? StudioID=0)
        {
            List<Member> objMemberList = _unitOfWork.Member.GetAll(includeProperties:"Studio").OrderBy(x => x.MemberId).ToList();
            if (!string.IsNullOrEmpty(strSearch))
            {
                objMemberList = objMemberList.Where(x => x.UserName.Contains(strSearch)).ToList();
            }
            if (StudioID != 0)
            {
                objMemberList = objMemberList.Where(x => x.StudioID == StudioID).ToList();
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
            IEnumerable<SelectListItem> StudioList = _unitOfWork.Studio
                .GetAll().Select(u => new SelectListItem
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
                    string memberPath = Path.Combine(wwwRootPath, @"images\member");

                    using (var fileStream = new FileStream(Path.Combine(memberPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    member.ImageUrl = @"\images\member\" + fileName;
                }
                member.MemberUUId = Guid.NewGuid().ToString();
                _unitOfWork.Member.Add(member);
                _unitOfWork.Save();
                TempData["success"] = "Member updated successfully";
                return RedirectToAction("MemberIndex");
            }
            else
            {
                IEnumerable<SelectListItem> StudioList = _unitOfWork.Studio
                    .GetAll().Select(u => new SelectListItem
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
            Member? memberFromDb = _unitOfWork.Member.Get(u => u.MemberId == MemberId, includeProperties: "Studio");
            if (memberFromDb == null)
            {
                return NotFound();
            }
            IEnumerable<SelectListItem> StudioList = _unitOfWork.Studio
                .GetAll().Select(u => new SelectListItem
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
                    string memberPath = Path.Combine(wwwRootPath, @"images\member");

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
                _unitOfWork.Member.Update(member);
                _unitOfWork.Save();
                TempData["success"] = "Member updated successfully";
                return RedirectToAction("MemberIndex");
            }
            else
            {
                IEnumerable<SelectListItem> StudioList = _unitOfWork.Studio
                    .GetAll().Select(u => new SelectListItem
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
            Member? memberFromDb = _unitOfWork.Member.Get(u => u.MemberId == MemberId, includeProperties: "Studio");
            if (memberFromDb == null)
            {
                return NotFound();
            }
            return View(memberFromDb);
        }
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? MemberId)
        {
            Member? obj = _unitOfWork.Member.Get(u => u.MemberId == MemberId);
            if(obj == null)
            {
                return NotFound();
            }

            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Member.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Member deleted successfully";
            return RedirectToAction("MemberIndex");
        }
        
        public ActionResult Get(int? MemberId)
        {
            if (MemberId == null || MemberId == 0)
            {
                return NotFound();
            }
            Member? memberFromDb = _unitOfWork.Member.Get(u => u.MemberId == MemberId, includeProperties: "Studio");
            if (memberFromDb == null)
            {
                return NotFound();
            }
            return View(memberFromDb);
        }

    }
}
