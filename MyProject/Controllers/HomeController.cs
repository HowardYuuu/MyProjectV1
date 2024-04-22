using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.DTO.Order;
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
		private readonly MyProjectContext _context;

		public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor, MyProjectContext projectContext)
		{
			_logger = logger;
			_httpContextAccessor = contextAccessor;
			_context = projectContext;
		}

		#region 檢視Views========================================================

		public async Task<IActionResult> Index()
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			var p = await _context.TProducts.Where(x => x.FOnSale == true).ToListAsync();
			var s = await _context.TSizeQties.ToListAsync();
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
			var s = await _context.TShoppingCarts.Where(x => x.FCustomerId == customerID).ToListAsync();
			var size = await _context.TSizeQties.ToListAsync();
			var product = await _context.TProducts.ToListAsync();

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

		public async Task<IActionResult> CheckOut(int? id)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			var s = await _context.TShoppingCarts.Where(x => x.FCustomerId == customerID).ToListAsync();
			var size = await _context.TSizeQties.ToListAsync();
			var product = await _context.TProducts.ToListAsync();

			var shop = new CartViewModel
			{
				Products = product,
				Sizes = size,
				ShoppingCarts = s
			};




			return View(shop);
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

			var quantity = _context.TSizeQties
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
					var existCartItem = _context.TShoppingCarts.FirstOrDefault(x =>
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
						_context.TShoppingCarts.Add(shoppingCart);
					}

					_context.SaveChanges();

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

			var p = _context.TShoppingCarts.Where(s => s.FCustomerId == customerID).Count();

			return Ok(p);
		}

		[HttpPost]
		public IActionResult RemoveCart(decimal? size)
		{
			try
			{
				ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
				int customerID = Convert.ToInt32(ViewBag.CusID);

				var p = _context.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID);

				if (p != null)
				{
					_context.TShoppingCarts.Remove(p);
					_context.SaveChanges(); // 確保變更已保存到數據庫
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

			var shoppingCarts = _context.TShoppingCarts
			.Where(x => x.FCustomerId == customerID)
			.AsEnumerable(); // 轉換為記憶體中運行的操作

			decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));

			ViewBag.TotalAmount = totalAmount;

			return Ok(totalAmount);
		}

		[HttpPost]
		public IActionResult UpdateCartItemQuantity(decimal? size, int newQuantity, int? productID)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var cartItem = _context.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID&&x.FProductId == productID);
			if (cartItem != null)
			{
				cartItem.FQuantity = newQuantity;
				_context.SaveChanges();

				return Json(new { success = true });
			}
			else
			{
				return Json(new { success = false });
			}
		}

		[HttpGet]
		public IActionResult GetTotalAmount(decimal? size, int? productID)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var shoppingCarts = _context.TShoppingCarts
				.Where(x => x.FCustomerId == customerID)
				.ToList(); // 或者 .ToListAsync()，根據您的情況

			decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));
			decimal? subtotal = shoppingCarts.FirstOrDefault(x => x.FSize == size&&x.FProductId == productID)?.FSubtotal;

			ViewBag.TotalAmount = totalAmount;

			ViewBag.Subtotal = subtotal;

			return Json(new { success = true, totalAmount = totalAmount, subtotal = subtotal });

			//return Json(new { totalAmount = totalAmount });
		}

		[HttpGet]
		public IActionResult GetCartInfo()
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);

			var shoppingCarts = _context.TShoppingCarts
				.Where(x => x.FCustomerId == customerID)
				.ToList(); // 或者 .ToListAsync()，根據您的情況

			decimal totalAmount = shoppingCarts.Sum(x => Convert.ToDecimal(x.FSubtotal));


			ViewBag.TotalAmount = totalAmount;



			return Json(new { success = true, totalAmount = totalAmount });

			//return Json(new { totalAmount = totalAmount });
		}


		[HttpPost]
		public async Task<IActionResult> ConfirmPayment([FromBody] PaymentInfo paymentInfo)
		{
			ViewBag.CusID = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
			int customerID = Convert.ToInt32(ViewBag.CusID);
			List<string> createdOrderIds = new List<string>(); // 用於存儲已創建訂單的 FOrderId
			try
			{
				if (paymentInfo != null)
				{
					//ADO.NET
					//string connectionString = "Data Source=.;Initial Catalog=MyProject;Integrated Security=True;TrustServerCertificate=true";
					//using (var connection = new SqlConnection(connectionString))
					//{
					//	// 創建 SQL 命令
					//	string sql = @"INSERT INTO TOrder (FCustomerId, FCusName, FCusEmail, FCusAddress, FCusPhone, FPaymentMethod, FTotalAmount, FOrderDate, FEndDate, FStatus)
					//               VALUES (@CustomerId, @CusName, @CusEmail, @CusAddress, @CusPhone, @PaymentMethod, @TotalAmount, @OrderDate, @EndDate, @Status)";

					//	SqlCommand command = new SqlCommand(sql, connection);

					//	// 添加參數
					//	command.Parameters.AddWithValue("@CustomerId", customerID);
					//	command.Parameters.AddWithValue("@CusName", paymentInfo.Name);
					//	command.Parameters.AddWithValue("@CusEmail", paymentInfo.Email);
					//	command.Parameters.AddWithValue("@CusAddress", paymentInfo.Address);
					//	command.Parameters.AddWithValue("@CusPhone", paymentInfo.Phone);
					//	command.Parameters.AddWithValue("@PaymentMethod", paymentInfo.Payment);
					//	command.Parameters.AddWithValue("@TotalAmount", paymentInfo.TotalAmount);
					//	command.Parameters.AddWithValue("@OrderDate", DateTime.Now);
					//	command.Parameters.AddWithValue("@EndDate", DateTime.Now.AddDays(3));
					//	command.Parameters.AddWithValue("@Status", "未付款");

					//	// 打開資料庫連接
					//	connection.Open();
					//	command.ExecuteNonQuery();
					//	// 關閉資料庫連接
					//	connection.Close(); // 或使用 connection.Dispose();

					//}
					int? maxOrderId = Convert.ToInt32(await _context.TOrders.Where(x => x.FCustomerId == customerID && x.FOrderDate == DateTime.Today).MaxAsync(o => o.FOrderId));
					if (maxOrderId != null)
					{
						//新增訂單
						TOrder order = new TOrder
						{
							FCustomerId = customerID,
							FCusName = paymentInfo.Name,
							FCusEmail = paymentInfo.Email,
							FCusAddress = paymentInfo.Address,
							FCusPhone = paymentInfo.Phone,
							FPaymentMethod = paymentInfo.Payment,
							FTotalAmount = paymentInfo.TotalAmount,
							FOrderDate = DateTime.Now,
							FEndDate = DateTime.Now.AddDays(3),
							FStatus = "未付款"
						};
						_context.TOrders.Add(order);
						await _context.SaveChangesAsync();

						//新增訂單明細
						var shoppingCarts = _context.TShoppingCarts
						.Where(x => x.FCustomerId == customerID);
						foreach (var s in shoppingCarts)
						{
							TOrderDetail orderDetail = new TOrderDetail()
							{
								FOrderId = order.FOrderId,
								FProductId = s.FProductId,
								FSize = s.FSize,
								FQuantity = s.FQuantity,
								FUnitPrice = s.FUnitPrice,
								FSubTotal = s.FSubtotal
							};
							_context.TOrderDetails.Add(orderDetail);
							_context.TShoppingCarts.Remove(s);
							await _context.SaveChangesAsync();
						}







						createdOrderIds.Add(order.FOrderId);
						return Ok(new { message = "付款確認成功" });

					}
					else
					{
						//新增訂單
						int? newOrderId = maxOrderId + 1;
						TOrder order = new TOrder
						{
							FOrderId = newOrderId.ToString(),
							FCustomerId = customerID,
							FCusName = paymentInfo.Name,
							FCusEmail = paymentInfo.Email,
							FCusAddress = paymentInfo.Address,
							FCusPhone = paymentInfo.Phone,
							FPaymentMethod = paymentInfo.Payment,
							FTotalAmount = paymentInfo.TotalAmount,
							FOrderDate = DateTime.Now,
							FEndDate = DateTime.Now.AddDays(3),
							FStatus = "未付款"
						};
						_context.TOrders.Add(order);
						await _context.SaveChangesAsync();

						//新增訂單明細
						var shoppingCarts = _context.TShoppingCarts
						.Where(x => x.FCustomerId == customerID);
						foreach (var s in shoppingCarts)
						{
							TOrderDetail orderDetail = new TOrderDetail()
							{
								FOrderId = order.FOrderId,
								FProductId = s.FProductId,
								FSize = s.FSize,
								FQuantity = s.FQuantity,
								FUnitPrice = s.FUnitPrice,
								FSubTotal = s.FSubtotal
							};
							_context.TOrderDetails.Add(orderDetail);
							_context.TShoppingCarts.Remove(s);
							await _context.SaveChangesAsync();
						}


						createdOrderIds.Add(order.FOrderId);
						return Ok(new { message = "付款確認成功" });

					}


				}
				// 在這裡處理後端資料庫存儲的邏輯
				// 例如，將 paymentInfo 中的資料寫入到您的資料庫中

				return BadRequest(new { message = "資料填寫未完全" });
			}
			catch (Exception ex)
			{
				// 處理錯誤
				return StatusCode(500, new { error = "內部錯誤：" + ex.Message });
			}
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