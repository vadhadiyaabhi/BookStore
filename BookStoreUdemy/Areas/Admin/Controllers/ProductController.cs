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
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            IEnumerable<Product> products = await _unitOfWork.Product.GetAll();
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            //IEnumerable<Category> categories = await _unitOfWork.Category.GetAll();
            //IEnumerable<SelectListItem> categoryList = categories.Select(
            //        u => new SelectListItem
            //        {
            //            Text = u.Name,
            //            Value = u.Id.ToString(),
            //        }
            //    );

            // MIMP NOTE
            // Take a loook at following line, Braces are required with await keyword                        // MIMP NOTE
            //IEnumerable <SelectListItem> categoryList = (await _unitOfWork.Category.GetAll()).Select(
            //        u => new SelectListItem
            //        {
            //            Text = u.Name,
            //            Value = u.Id.ToString(),
            //        }
            //    );


            ProductViewModel productViewModel = new()
            {
                Product = new Product(),
                CategoryList = (await _unitOfWork.Category.GetAll()).Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }),
                CoverTypeList = (await _unitOfWork.CoverType.GetAll()).Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    }),
            };

            if (id == null || id == 0)
            {
                // pass empty productViewModel object
                return View(productViewModel);
            }
            else
            {
                productViewModel.Product = await _unitOfWork.Product.GetFirstOrDefault(x => x.Id == id);
            }

            return View(productViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert(ProductViewModel productViewModel, IFormFile? image = null)
        {

            // Instead of defining image as separaet argument, add the field in Model itself as datatype - IFormFile and make it nullable based on requirment
            Console.WriteLine(productViewModel.Product.Id);
            if (ModelState.IsValid)
            {
                // First handling image
                string wwwRootPath = _webHostEnvironment.WebRootPath;               // This will return path to our public folder - wwwroot
                //Console.WriteLine(wwwRootPath);
                string? uniqueFileName = null;
                if (image != null)
                {
                    if (productViewModel.Product.ImageUrl != null)
                    {
                        var oldPath = Path.Combine(wwwRootPath, productViewModel.Product.ImageUrl);
                        if (System.IO.File.Exists(oldPath))
                        {
                            System.IO.File.Delete(oldPath);
                        }
                    }

                    string fileGuidId = Guid.NewGuid().ToString();
                    var uploadsFolder = Path.Combine(wwwRootPath, @"Images\Products");            // path to products folder, @ symbol use - remove special chars from its special meaning and will be treated as normal char isnide string, EX: \, /
                    uniqueFileName = fileGuidId + image.FileName;

                    // --------------------------------- Following two lines are same as using block
                    string FilePath = Path.Combine(uploadsFolder, uniqueFileName);
                    //image.CopyTo(new FileStream(FilePath, FileMode.Create));

                    using (var fileStreams = new FileStream(Path.Combine(uploadsFolder, uniqueFileName), FileMode.Create))
                    {
                        image.CopyTo(fileStreams);
                    }

                    //productViewModel.Product.ImageUrl = FilePath;
                    productViewModel.Product.ImageUrl = Path.Combine(@"Images\Products", uniqueFileName);

                }
                //else
                //{
                //    ModelState.AddModelError("image", "Please provide a product image");
                //    return View(productViewModel);
                //}


                if(productViewModel.Product.Id == 0) 
                {
                    // Add new peoduct
                    await _unitOfWork.Product.Add(productViewModel.Product);
                    TempData["success"] = "Product created successfilly!!";
                }
                else
                {
                    // Edit Existing Product
                    _unitOfWork.Product.Update(productViewModel.Product);
                    TempData["success"] = "Product updated successfilly!!";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            productViewModel.CategoryList = (await _unitOfWork.Category.GetAll()).Select(
                    u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString(),
                    });
            productViewModel.CoverTypeList = (await _unitOfWork.CoverType.GetAll()).Select(
                u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                });
            return View(productViewModel);
        }



        #region API Calls
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var products = await _unitOfWork.Product.GetAll(includeProperties: "Category,CoverType");
                return Json(new { data = products });
            }
            catch(Exception error) 
            {
                return NoContent();
            }
            
            
        }

        [HttpDelete]
        //[Route("Admin/Product/Delete/{productId}")]
        public async Task<IActionResult> Delete(int? productId)
        {
            var product = await _unitOfWork.Product.GetFirstOrDefault(x => x.Id == productId);
            //var category = await _categoryRepository.GetFirstOrDefault(x => x.Id == id);
            if (product == null)
            {
                return Json(new { success = false, message = "Error While Deleting" });
            }

            if (product.ImageUrl != null)
            {
                var oldPath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl);
                if (System.IO.File.Exists(oldPath))
                {
                    System.IO.File.Delete(oldPath);
                }
            }

            _unitOfWork.Product.Remove(product);
            _unitOfWork.Save();
            //TempData["success"] = "Product deleted succesfully!!";

            return Json(new { success = true, message = "Product deleted successfully" });
            //return RedirectToAction("Index");
        }
        #endregion
    }
}


