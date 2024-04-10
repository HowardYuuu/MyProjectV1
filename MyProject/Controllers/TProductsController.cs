using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MyProject.Models;
using MyProject.ViewModels;

namespace MyProject.Controllers
{
    public class TProductsController : Controller
    {
        private readonly MyProjectContext _context;
        private readonly IWebHostEnvironment _environment;
        public TProductsController(MyProjectContext context, IWebHostEnvironment webHost)
        {
            _context = context;
            _environment = webHost;
        }

		#region 檢視Views========================================================
		
		// GET: TProducts
		public async Task<IActionResult> Index()
        {
            //var VP = await _context.VproductSizeQties.ToListAsync();
            var p = await _context.TProducts.ToListAsync();
            var pv = p.Select(product => new ProductViewModel
            {
                ProductId = product.FProductId,
                ProductName = product.FProductName,
                Brand = product.FBrand,
                Number = product.FNumber,
                Style = product.FStyle,
                Gender = product.FGender,
                MainColor = product.FMainColor,
                Width = product.FWidth,
                Price = product.FPrice,
                Image = product.FImgPath,
                OnSale = (bool)product.FOnSale
            }).ToList();
            //var onSaleStatus = p.Select(x=>x.FOnSale)? "true" : "false";
            //ViewData["OnSaleStatus"] = onSaleStatus;

            return pv != null ?
                        View(pv) : Problem("Entity set 'MyProjectContext.TProducts'  is null.");
            //return VP != null ?
            //                       View(VP) :
            //                        Problem("Entity set 'MyProjectContext.TProducts'  is null.");
        }

		
		public IActionResult Create()
		{
			return View();
		}

		// GET: TProducts/Edit
		public IActionResult Edit(int id)
		{
			var product = _context.TProducts.FirstOrDefault(p => p.FProductId == id);


			if (product == null)
			{
				return NotFound();
			}
			var pv = new ProductViewModel
			{
				ProductId = product.FProductId,
				ProductName = product.FProductName,
				Brand = product.FBrand,
				Number = product.FNumber,
				Style = product.FStyle,
				Gender = product.FGender,
				MainColor = product.FMainColor,
				Width = product.FWidth,
				Price = product.FPrice,
				Image = product.FImgPath
			};
			return View(pv);
		}

		// GET: TProducts/Details/5
		public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.TProducts == null)
            {
                return NotFound();
            }

            var tProduct = await _context.TProducts
                .FirstOrDefaultAsync(m => m.FProductId == id);
            if (tProduct == null)
            {
                return NotFound();
            }

            return View(tProduct);
        }



        public async Task<IActionResult> SizeEdit(int? id)
        {
            if (id == null || _context.TSizeQties == null)
            {
                return NotFound();
            }
            ViewBag.ProductID = id;
            var sq = await _context.TSizeQties.Where(x => x.FProductId == id).Distinct().OrderBy(x=>x.FSize).ToListAsync();
            var PS = sq.Select(s => new SizeQtyViewModel
            {
                FProductId = s.FProductId,
                FSize = s.FSize,
                FQuantity = s.FQuantity,

            }).ToList();
            

            return View(PS);
        }
#endregion

		#region Product功能=======================================================
		// POST: TProducts/Create
		[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var createProduct = new TProduct
                {
                    // 新增產品的屬性
                    FProductName = model.ProductName,
                    FBrand = model.Brand,
                    FNumber = model.Number,
                    FGender = model.Gender,
                    //existingProduct.FImgPath = model.圖片;
                    FMainColor = model.MainColor,
                    FPrice = model.Price,
                    FStyle = model.Style,
                    FWidth = model.Width,

                };
                if (model.ImageFile != null && model.ImageFile.Length > 0)
                {
                    // 確定文件夾存在，如果不存在，則創建它
                    string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "Products");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // 取得文件名稱
                    var fileName = Path.GetFileName(model.ImageFile.FileName);
                    // 建立新的檔案路徑
                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // 寫入文件到指定路徑
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.ImageFile.CopyToAsync(stream);
                    }

                    // 新增圖片路徑
                    createProduct.FImgPath = fileName;
                }

                // 更新產品數量及尺寸的相關信息
                _context.TProducts.Add(createProduct);
                await _context.SaveChangesAsync();

                // 重新導向到編輯頁面或其他頁面
                return RedirectToAction("Index");

            }

            // 如果模型狀態無效，傳回目前視圖以顯示驗證錯誤
            return View(model);
        }

        
        //==============================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var editProduct = await _context.TProducts.FindAsync(model.ProductId);
                if (editProduct != null)
                {
                    // 更新現有產品的屬性
                    editProduct.FProductName = model.ProductName;
                    editProduct.FBrand = model.Brand;
                    editProduct.FNumber = model.Number;
                    editProduct.FGender = model.Gender;
                    //existingProduct.FImgPath = model.圖片;
                    editProduct.FMainColor = model.MainColor;
                    editProduct.FPrice = model.Price;
                    editProduct.FStyle = model.Style;
                    editProduct.FWidth = model.Width;

                    if (model.ImageFile != null && model.ImageFile.Length > 0)
                    {
                        // 確定文件夾存在，如果不存在，則創建它
                        string uploadsFolder = Path.Combine(_environment.WebRootPath, "images", "Products");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // 取得文件名稱
                        var fileName = Path.GetFileName(model.ImageFile.FileName);
                        // 建立新的檔案路徑
                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // 寫入文件到指定路徑
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(stream);
                        }

                        // 更新圖片路徑
                        editProduct.FImgPath = fileName;
                    }


                    // 更新產品數量及尺寸的相關信息
                    _context.TProducts.Update(editProduct);
                    await _context.SaveChangesAsync();

                    // 重新導向到編輯頁面或其他頁面
                    return RedirectToAction("Index");
                }
                else
                {
                    // 找不到對應的產品，可能是錯誤處理邏輯
                    return NotFound();
                }
            }

            // 如果模型狀態無效，傳回目前視圖以顯示驗證錯誤
            return View(model);
        }
        //=================================================================

     
        [HttpPost]
        public IActionResult ProductDelete(int? id)
        {
            try
            {
				var productToDelete = _context.TProducts.FirstOrDefault(x => x.FProductId == id);

				if (productToDelete == null)
				{
					return Json(new { success = false, errorMessage = "找不到要刪除的產品！" });
				}

				var sizeQty = _context.TSizeQties.Where(x => x.FProductId == id).ToList();
				if (sizeQty.Any())
				{
					_context.TSizeQties.RemoveRange(sizeQty);
                    _context.SaveChanges();
                }

				_context.TProducts.Remove(productToDelete);
				_context.SaveChanges();

				return Json(new { success = true, message = "產品及其相關庫存已成功刪除。" });
			}
            catch (Exception ex)
            {
                return Json(new { success = false, errorMessage = "刪除產品時出現錯誤，請稍後在試。" });
            }
        }


        private bool TProductExists(int id)
        {
            return (_context.TProducts?.Any(e => e.FProductId == id)).GetValueOrDefault();
        }

		[HttpPost]
		public IActionResult UpdateOnSaleStatus(int productId, bool onSale)
		{
			var product = _context.TProducts.FirstOrDefault(p => p.FProductId == productId);
			if (product == null)
			{
				return NotFound(); // 如果找不到相應的商品，返回404錯誤
			}

			product.FOnSale = onSale;
			_context.SaveChanges(); // 更新資料庫中的商品上下架狀態

			return Ok(); // 返回200 OK表示更新成功
		}
#endregion

		#region SizeQty功能=======================================================
		public async Task<IActionResult> SizeCreate(decimal? size, int? qty, int? id)
		{
			try
			{
				if (_context.TSizeQties.Where(p => p.FProductId == id).Any(s => s.FSize == size))
				{
					return Json(new { success = false, errorMessage = "尺寸已存在" });
				}

				var newSizeQty = new TSizeQty
				{
					FSize = size,
					FQuantity = qty,
					FProductId = id
				};
				_context.TSizeQties.Add(newSizeQty);
				await _context.SaveChangesAsync();

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, errorMessage = "伺服器目前異常：" + ex.Message });
			}
		}

		[HttpPost]
		public IActionResult SizeDelete(decimal? size, int? id)
		{
			try
			{
				var productToDelete = _context.TSizeQties.FirstOrDefault(x => x.FProductId == id && x.FSize == size);
				if (productToDelete == null)
				{
					return Json(new { success = false, errorMessage = "找不到要刪除的產品！" });
				}

				_context.TSizeQties.Remove(productToDelete);
				_context.SaveChanges();

				return Json(new { success = true });
			}
			catch (Exception ex)
			{
				return Json(new { success = false, errorMessage = "刪除產品時出現錯誤：" + ex.Message });
			}
		}

		[HttpPost]
        public ActionResult UpdateQuantity(int? id, decimal? size, int? qty)
        {
            try
            {
                // 檢查是否有對應的記錄需要更新
                var existingRecord = _context.TSizeQties.FirstOrDefault(p => p.FProductId == id && p.FSize == size);

                if (existingRecord != null)
                {
                    // 更新現有記錄的庫存量
                    existingRecord.FQuantity = qty;
                    _context.TSizeQties.Update(existingRecord);
                    _context.SaveChanges();

                    // 回傳成功的JSON回應
                    return Json(new { success = true });
                }
                else
                {
                    // 如果記錄不存在，回傳更新失敗的JSON回應
                    return Json(new { success = false, message = "找不到需要更新的記錄" });
                }
            }
            catch (Exception ex)
            {
                // 如果在更新過程中發生異常，回傳帶有錯誤訊息的JSON回應
                return Json(new { success = false, message = ex.Message });
            }
        }
     #endregion

    }
}
