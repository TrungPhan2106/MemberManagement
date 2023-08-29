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
using System.Data;
using ClosedXML.Excel;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;


namespace StudioManagement.Controllers
{
    public class MemberController : Controller
    {
        
        private readonly ILogger<MemberController> _logger;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly MyDbContext _db;
        public MemberController(IMapper mapper,ILogger<MemberController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, MyDbContext db)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
            _db = db;
        }
        public IActionResult MemberIndex(string strSearch, int pg=1, int? StudioID=0)
        {
            List<Member> objMemberList = _unitOfWork.Member.GetAll(includeProperties:"Studio")
                .OrderBy(x => x.MemberId).ToList();
            
            var expiredMembers = objMemberList.Where(x => x.ExpiredDate < DateTime.Now).ToList();
            if (expiredMembers.Any())
            {
                foreach (var mem in expiredMembers)
                {
                    mem.Status = "DeActive";
                }
                _unitOfWork.Save();
            }
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

        [HttpGet]
        public async Task<FileResult> ExportMembersInExcel(string strSearch, int? StudioID = 0)
        {
            var objMemberList = await _db.Member.ToListAsync();
            if (!string.IsNullOrEmpty(strSearch))
            {
                objMemberList = objMemberList.Where(x => x.UserName.Contains(strSearch)).ToList();
            }
            if (StudioID != 0)
            {
                objMemberList = objMemberList.Where(x => x.StudioID == StudioID).ToList();
            }
            var fileName = "members.xlsx";
            return GenerateExcel(fileName, objMemberList);
        }

        private FileResult GenerateExcel(string fileName, IEnumerable<Member> objMemberList)
        {
            DataTable dataTable = new DataTable("objMemberList");
            dataTable.Columns.AddRange(new DataColumn[]
            {
                new DataColumn("MemberId"),
                new DataColumn("UserName"),
                new DataColumn("FullName"),
                new DataColumn("DateOfBirth"),
                new DataColumn("Gender"),
                new DataColumn("PhoneNumber"),
                new DataColumn("Email"),
                new DataColumn("Address"),
                new DataColumn("JoinedDate"),
                new DataColumn("StudioID")
            });

            foreach (var member in objMemberList)
            {
                dataTable.Rows.Add(member.MemberId, member.UserName, member.FullName,
                    member.DateOfBirth.ToString("MM/dd/yyyy"), member.Gender ? "Male" : "Female", member.PhoneNumber, member.Email,
                    member.Address, member.JoinedDate.ToString("MM/dd/yyyy"), member.StudioID);
            }

            using (XLWorkbook wb = new XLWorkbook())
            {
                wb.Worksheets.Add(dataTable);
                using(MemoryStream stream = new MemoryStream())
                {
                    wb.SaveAs(stream);
                    return File(stream.ToArray(),
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }
            }
        }
        
        public IActionResult Create(int? StudioID, Member member)
        {
            if (StudioID != 0)
            {
                member.StudioID = (int)StudioID;
            }
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
                        //delete old image
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
                if (member.ExpiredDate < DateTime.Now)
                { member.Status = "DeActive"; }
                else
                { member.Status = "Active"; }
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

        public ActionResult GetMem(int? MemberId)
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

        [HttpGet]
        public ActionResult ExportToPDF(int MemberId)
        {
            //Initialize HTML to PDF converter.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            ////Set Blink viewport size.
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(800, 0);
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;
            //Convert URL to PDF document.
            PdfDocument document = htmlConverter.Convert($"https://localhost:7056/Member/GetMem?MemberId={MemberId}");
            //Create memory stream.
            MemoryStream stream = new MemoryStream();
            //Save the document to memory stream.
            document.Save(stream);
            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "MemberInfo.pdf");
        }    
    }
}
