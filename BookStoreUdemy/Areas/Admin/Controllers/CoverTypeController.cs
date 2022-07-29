using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookStoreUdemy.Areas.Admin.Controllers
{
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public class CovertypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public CovertypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<CoverType> coverTypes = await _unitOfWork.CoverType.GetAll();
            return View(coverTypes);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // This can also be applied at class level, As applied here, so no need to apply on each method = [AutoValidateAntiforgeryToken]
        //[ValidateAntiForgeryToken]                        // To validate form data, make it secure from CrossSite Scripting Attacks
        public async Task<IActionResult> Create(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.CoverType.Add(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type created successfully!!";    // TempData - to store one time notification, Will get destroyed after one refresh (of any page)
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var filteredCoverType = await _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            //var filteredCategory = await _categoryRepository.GetFirstOrDefault(c => c.Id == id);

            if (filteredCoverType == null)
            {
                return NotFound();
            }

            return View(filteredCoverType);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.CoverType.Update(coverType);
                _unitOfWork.Save();
                TempData["success"] = "Cover Type updated successfully!!";    // TempData - to store one time notification, Will get destroyed after one refresh (of any page)
                return RedirectToAction("Index");
            }
            return View(coverType);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var filteredCoverType = await _unitOfWork.CoverType.GetFirstOrDefault(c => c.Id == id);
            //var filteredCategory = await _categoryRepository.GetFirstOrDefault(c => c.Id == id);

            if (filteredCoverType == null)
            {
                return NotFound();
            }

            return View(filteredCoverType);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var coverType = await _unitOfWork.CoverType.GetFirstOrDefault(x => x.Id == id);
            //var category = await _categoryRepository.GetFirstOrDefault(x => x.Id == id);
            if (coverType == null)
            {
                return NotFound();
            }
            _unitOfWork.CoverType.Remove(coverType);
            _unitOfWork.Save();
            TempData["success"] = "Cover Type deleted succesfully!!";

            return RedirectToAction("Index");
        }




    }
}
