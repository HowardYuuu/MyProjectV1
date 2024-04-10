using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyProject.Models;
using MyProject.ViewModels;
using System.Diagnostics;
using System.Security.Claims;

namespace MyProject.Controllers
{
	public class HomeController : Controller
	{
		private readonly ILogger<HomeController> _logger;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly MyProjectContext _myProjectContext;

		public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor, MyProjectContext projectContext)
		{
			_logger = logger;
			_httpContextAccessor = contextAccessor;
			_myProjectContext = projectContext;
		}

		#region 檢視Views========================================================

		public async Task<IActionResult> Index()
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			var p = await _myProjectContext.TProducts.Where(x => x.FOnSale == true).ToListAsync();
			var s = await _myProjectContext.TSizeQties.ToListAsync();
			var ps = new ProSizeQtyViewModel
			{
				Product = p,
				SizeQty = s
			};

			return View(ps);
		}

		public async Task<IActionResult> ShoppingCart(int? id)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			var s = await _myProjectContext.TShoppingCarts.Where(x => x.FCustomerId == customerID).ToListAsync();
			var size = await _myProjectContext.TSizeQties.ToListAsync();
			var product = await _myProjectContext.TProducts.ToListAsync();

			var shop = new CartViewModel
			{
				Products = product,
				Sizes = size,
				ShoppingCarts = s
			};


			return View(shop);

		}


		public async Task<IActionResult> ProductDetail(int id)
		{

			////int productId = Convert.ToInt32(id);
			//var p =_myProjectContext.TProducts.Where(p=>p.FProductId == id);
			//         var s = _myProjectContext.TSizeQties.Where(p => p.FProductId == id);
			//         var model = new VproductSizeQty
			//{
			//             產品id = id,
			//             名稱=p.Select(x=>x.FProductName).ToString(),
			//             風格 = p.Select(x => x.FStyle).ToString(),
			//             價格 = p.Select(x => x.FPrice).ToString(),
			//             品牌 = p.Select(x => x.FProductName).ToString(),
			//             圖片 = p.Select(x => x.FProductName).ToString(),
			//             型號 = p.Select(x => x.FProductName).ToString(),
			//             寬度 = p.Select(x => x.FProductName).ToString(),
			//             尺寸 = s.Select(x => x.FProductName).ToString(),
			//};

			return View();
		}




		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}


		#endregion

		#region 功能相關=========================================================
		public ActionResult GetQuantity(int? productId, decimal? size)
		{
			// 在此處進行資料庫查詢，根據產品ID（productId）和尺寸（size）檢索 FQuantity
			// 假設您使用 Entity Framework，這可能會類似於以下代碼

			var quantity = _myProjectContext.TSizeQties
				.Where(item => item.FProductId == productId && item.FSize == size)
				.Select(item => item.FQuantity)
				.FirstOrDefault();

			// 返回 FQuantity 數量，將透過 AJAX 傳送到前端
			return Content(quantity.ToString());


		}


		// 添加商品到購物車的動作
		[HttpPost]
		public IActionResult AddToCart(int? productId, decimal? size, decimal price)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			try
			{
				if (customerID != 0)
				{
					var existCartItem = _myProjectContext.TShoppingCarts.FirstOrDefault(x =>
					x.FCustomerId == customerID &&
					x.FProductId == productId &&
					x.FSize == size
					);

					if (existCartItem != null)
					{
						// 如果客戶購物車中已存在相同尺寸的商品，則更新該商品的數量
						existCartItem.FQuantity += 1;
					}
					else
					{
						// 如果客戶購物車中不存在相同尺寸的商品，則將新商品添加到購物車中
						var shoppingCart = new TShoppingCart()
						{
							FCustomerId = customerID,
							FProductId = productId,
							FSize = size,
							FUnitPrice = price,
							FQuantity = 1,
						};
						_myProjectContext.TShoppingCarts.Add(shoppingCart);
					}

					_myProjectContext.SaveChanges();

					return Ok();
				}
				return RedirectToAction("Login");

			}
			catch (Exception ex)
			{
				// 如果出現錯誤，返回錯誤狀態碼和錯誤信息
				return BadRequest(ex.Message);
			}
		}

		// 獲取購物車中的商品數量的動作
		[HttpGet]
		public IActionResult GetCartCount()
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var p = _myProjectContext.TShoppingCarts.Where(s => s.FCustomerId == customerID).Count();

			return Ok(p);
		}

		[HttpPost]
		public IActionResult RemoveCart(decimal? size)
		{
			try
			{
				ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				int customerID = Convert.ToInt32(ViewBag.CusID);

				var p = _myProjectContext.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID);

				if (p != null)
				{
					_myProjectContext.TShoppingCarts.Remove(p);
					_myProjectContext.SaveChanges(); // 確保變更已保存到數據庫
					return Json(new { success = true });
				}
				else
				{
					return Json(new { success = false, message = "錯誤" });
				}
			}
			catch (Exception ex)
			{
				return Json(new { success = false, message = ex.Message });
			}
		}

		[HttpGet]
		public IActionResult GetTotalAmountForCustomer()
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var shoppingCarts = _myProjectContext.TShoppingCarts
			.Where(x => x.FCustomerId == customerID)
			.AsEnumerable(); // 轉換為記憶體中運行的操作

			decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));

			ViewBag.TotalAmount = totalAmount;

			return Ok(totalAmount);
		}

		[HttpPost]
		public IActionResult UpdateCartItemQuantity(decimal? size, int newQuantity)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var cartItem = _myProjectContext.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID);
			if (cartItem != null)
			{
				cartItem.FQuantity = newQuantity;
				_myProjectContext.SaveChanges();

				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false });
			}
		}

		[HttpGet]
		public IActionResult GetTotalAmount(decimal? size)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var shoppingCarts = _myProjectContext.TShoppingCarts
				.Where(x => x.FCustomerId == customerID)
				.ToList(); // 或者 .ToListAsync()，根據您的情況

			decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));
			decimal? subtotal = shoppingCarts.FirstOrDefault(x => x.FSize == size)?.FSubtotal;

			ViewBag.TotalAmount = totalAmount;

			ViewBag.Subtotal = subtotal;

			return Json(new { success = true, totalAmount = totalAmount, subtotal = subtotal });

			//return Json(new { totalAmount = totalAmount });
		}





		//[HttpPost]
		//public IActionResult UpdateCartItemQuantity(decimal? size, int newQuantity)
		//{
		//	ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
		//	int customerID = Convert.ToInt32(ViewBag.CusID);

		//	var cartItem = _myProjectContext.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID);
		//	if (cartItem != null)
		//	{
		//		cartItem.FQuantity = newQuantity;
		//		_myProjectContext.SaveChanges();

		//		// 重新計算總金額
		//		var shoppingCarts = _myProjectContext.TShoppingCarts
		//		.Where(x => x.FCustomerId == customerID)
		//		.ToList(); // 或者 .ToListAsync()，根據您的情況

		//		decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));
		//		decimal? subtotal = shoppingCarts.FirstOrDefault(x => x.FSize == size)?.FSubtotal;

		//		ViewBag.TotalAmount = totalAmount;

		//		ViewBag.Subtotal = subtotal;

		//		return Json(new { success = true, totalAmount = totalAmount, subtotal = subtotal });
		//	}
		//	else
		//	{
		//		return Json(new { success = false });
		//	}
		//}
		#endregion
	}





}