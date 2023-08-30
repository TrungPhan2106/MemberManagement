using StudioManagement.Data;
using StudioManagement.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using AutoMapper.Execution;
using System.Diagnostics.Metrics;
using StudioManagement.Repository.IRepository;
using Syncfusion.Pdf;
using Syncfusion.HtmlConverter;

namespace StudioManagement.Controllers
{
    public class HomeController : Controller
    {
        private static readonly log4net.ILog log = log4net.LogManager
            .GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            logger.LogWarning("This is a MEL warning on the privacy page");
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index(string strSearch, int pg = 1)
        {
            List<Studio> studioList = _unitOfWork.Studio.GetAll().ToList();
            if (!string.IsNullOrEmpty(strSearch))
            {
                studioList = studioList.Where(x => x.StudioName.Contains(strSearch)).ToList();
            }
            const int pageSize = 8;
            if (pg < 1) pg = 1;
            int recsCount = studioList.Count();
            var paper = new Pager(recsCount, pg, pageSize);
            int recSkip = (pg - 1) * pageSize;
            var data = studioList.Skip(recSkip).Take(paper.PageSize).ToList();
            this.ViewBag.Pager = paper;
            return View(data);
        }

        public IActionResult Privacy()
        {
            log.Info("About page privacy.");
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var t = Activity.Current?.Id;
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Studio stu, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string studioPath = Path.Combine(wwwRootPath, @"images\studio");

                    using (var fileStream = new FileStream(Path.Combine(studioPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    stu.StudioPic = @"\images\studio\" + fileName;
                }

                _unitOfWork.Studio.Add(stu);
                _unitOfWork.Save();
                TempData["success"] = "New Studio created successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Edit(int? StudioID)
        {
            if (StudioID == null || StudioID == 0)
            {
                return NotFound();
            }
            Studio? studioFromDb = _unitOfWork.Studio.Get(u => u.StudioID == StudioID);
            if (studioFromDb == null)
            {
                return NotFound();
            }
            return View(studioFromDb);
        }
        [HttpPost]
        public IActionResult Edit(Studio stu, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string studioPath = Path.Combine(wwwRootPath, @"images\studio");

                    if (!string.IsNullOrEmpty(stu.StudioPic))
                    {
                        //delete ola image
                        var oldImagePath =
                            Path.Combine(wwwRootPath, stu.StudioPic.TrimStart('\\'));

                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(Path.Combine(studioPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    stu.StudioPic = @"\images\studio\" + fileName;
                }

                _unitOfWork.Studio.Update(stu);
                _unitOfWork.Save();
                TempData["success"] = "Studio updated successfully";
                return RedirectToAction("Index");
            }
            return View();
        }

        public IActionResult Delete(int? StudioID)
        {
            if (StudioID == null || StudioID == 0)
            {
                return NotFound();
            }
            Studio? studioFromDb = _unitOfWork.Studio.Get(u => u.StudioID == StudioID);
            if (studioFromDb == null)
            {
                return NotFound();
            }
            return View(studioFromDb);
        }
        //[Route("studio/Delete")]
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePOST(int? StudioID)
        {
            Studio? obj = _unitOfWork.Studio.Get(u => u.StudioID == StudioID);
            if (obj == null)
            {
                return NotFound();
            }
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, obj.StudioPic.TrimStart('\\'));
            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }
            _unitOfWork.Studio.Remove(obj);
            _unitOfWork.Save();
            TempData["success"] = "Studio deleted successfully";
            return RedirectToAction("Index");
        }

        public ActionResult Get(int? StudioID)
        {
            if (StudioID == null || StudioID == 0)
            {
                return NotFound();
            }
            Studio? studioFromDb = _unitOfWork.Studio.Get(u => u.StudioID == StudioID);

            if (studioFromDb == null)
            {
                return NotFound();
            }
            return View(studioFromDb);
        }
        public ActionResult GetStudio(int? StudioID)
        {
            if (StudioID == null || StudioID == 0)
            {
                return NotFound();
            }
            Studio? studioFromDb = _unitOfWork.Studio.Get(u => u.StudioID == StudioID);

            if (studioFromDb == null)
            {
                return NotFound();
            }
            return View(studioFromDb);
        }
        [HttpGet]
        public ActionResult ExportToPDF(int StudioID)
        {
            //Initialize HTML to PDF converter.
            HtmlToPdfConverter htmlConverter = new HtmlToPdfConverter();
            BlinkConverterSettings blinkConverterSettings = new BlinkConverterSettings();
            ////Set Blink viewport size.
            blinkConverterSettings.ViewPortSize = new Syncfusion.Drawing.Size(800, 0);
            //Assign Blink converter settings to HTML converter.
            htmlConverter.ConverterSettings = blinkConverterSettings;
            //Convert URL to PDF document.
            PdfDocument document = htmlConverter.Convert($"https://localhost:7056/Home/GetStudio?StudioID={StudioID}");
            //Create memory stream.
            MemoryStream stream = new MemoryStream();
            //Save the document to memory stream.
            document.Save(stream);
            return File(stream.ToArray(), System.Net.Mime.MediaTypeNames.Application.Pdf, "StudioInfo.pdf");
        }
    }
}
