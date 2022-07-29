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
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        //private readonly ICategoryRepository _categoryRepository;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //_categoryRepository = categoryRepository;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Category> categories = await _unitOfWork.Category.GetAll();
            //IEnumerable<Category> categories = await _categoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        // This can also be applied at class level, As applied here, so no need to apply on each method = [AutoValidateAntiforgeryToken]
        //[ValidateAntiForgeryToken]                        // To validate form data, make it secure from CrossSite Scripting Attacks
        public async Task<IActionResult> Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("notEqual", "Name should not be same as Display Order.");           // Add custom validation to ModelState
            }
            if (ModelState.IsValid)
            {
                //await _categoryRepository.Add(category);
                //_categoryRepository.Save();
                await _unitOfWork.Category.Add(category);
                _unitOfWork.Save();
                TempData["success"] = "Category created successfully!!";    // TempData - to store one time notification, Will get destroyed after one refresh (of any page)
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var filteredCategory = await _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            //var filteredCategory = await _categoryRepository.GetFirstOrDefault(c => c.Id == id);

            if (filteredCategory == null)
            {
                return NotFound();
            }

            return View(filteredCategory);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("notEqual", "Name should not be same as Display Order.");           // Add custom validation to ModelState
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(category);
                _unitOfWork.Save();
                //await _categoryRepository.Update(category);
                //_categoryRepository.Save();
                TempData["success"] = "Category updated successfully!!";    // TempData - to store one time notification, Will get destroyed after one refresh (of any page)
                return RedirectToAction("Index");
            }
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var filteredCategory = await _unitOfWork.Category.GetFirstOrDefault(c => c.Id == id);
            //var filteredCategory = await _categoryRepository.GetFirstOrDefault(c => c.Id == id);

            if (filteredCategory == null)
            {
                return NotFound();
            }

            return View(filteredCategory);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeletePost(int? id)
        {
            var category = await _unitOfWork.Category.GetFirstOrDefault(x => x.Id == id);
            //var category = await _categoryRepository.GetFirstOrDefault(x => x.Id == id);
            if (category == null)
            {
                //return NotFound();
                return Json(new { success = false, message = "Error While Deleting" });
            }
            _unitOfWork.Category.Remove(category);
            _unitOfWork.Save();
            //_categoryRepository.Remove(category);
            //_categoryRepository.Save();
            TempData["success"] = "Category deleted succesfully!!";

            return RedirectToAction("Index");
        }




    }
}
