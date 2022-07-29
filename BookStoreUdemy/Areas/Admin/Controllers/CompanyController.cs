using BookStore.DataAccess.Repositories.IRepositories;
using BookStore.Models;
using BookStore.Models.ViewModels;
using BookStore.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BookStoreUdemy.Areas.Admin.Controllers
{

    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            Company? company = new();
            if (id == null || id == 0)
            {
                // pass empty company object
                return View(company);
            }
            else
            {
                company = await _unitOfWork.Company.GetFirstOrDefault(x => x.Id == id);
            }

            return View(company);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(Company company)
        {
            if (ModelState.IsValid)
            {
                if(company.Id == 0) 
                {
                    // Add new peoduct
                    await _unitOfWork.Company.Add(company);
                    TempData["success"] = "Company created successfilly!!";
                }
                else
                {
                    // Edit Existing Product
                    _unitOfWork.Company.Update(company);
                    TempData["success"] = "Company updated successfilly!!";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            
            return View(company);
        }



        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _unitOfWork.Company.GetAll();
                return Json(new { data = products });
            }
            catch(Exception error) 
            {
                return NoContent();
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int? companyId)
        {
            var company = await _unitOfWork.Product.GetFirstOrDefault(x => x.Id == companyId);
            if (company == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            _unitOfWork.Product.Remove(company);
            _unitOfWork.Save();
            //TempData["success"] = "Company deleted succesfully!!";

            return Json(new { success = true, message = "Product deleted successfully" });
        }
        #endregion
    }
}


