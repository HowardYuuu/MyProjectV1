using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyProject.DTO.Order;
using MyProject.Models;
using MyProject.ViewModels;
using System.Diagnostics;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using prjTravelPlatform_release.Areas.Customer.LinePay.DTO;
using prjTravelPlatform_release.Areas.Customer.LinePay.Service;

namespace MyProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly HttpClient _httpClient;
        private readonly MyProjectContext _context;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor contextAccessor, MyProjectContext projectContext, HttpClient httpClient)
        {
            _logger = logger;
            _httpContextAccessor = contextAccessor;
            _context = projectContext;
            _httpClient = httpClient;
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
        public IActionResult GetQuantity(int? productId, decimal? size)
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

            var cartItem = _context.TShoppingCarts.FirstOrDefault(x => x.FSize == size && x.FCustomerId == customerID && x.FProductId == productID);
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
            decimal? subtotal = shoppingCarts.FirstOrDefault(x => x.FSize == size && x.FProductId == productID)?.FSubtotal;

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
            try
            {
                if (paymentInfo != null)
                {
                    //新增訂單
                    TOrder order = new TOrder
                    {
                        FOrderId = GetOrderID(),
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
                    .Where(x => x.FCustomerId == customerID).ToList();
                    
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
                    }
                    
                    // 清空購物車
                    _context.TShoppingCarts.RemoveRange(shoppingCarts);
                    await _context.SaveChangesAsync();

                    // 如果選擇 LINE Pay，發起付款請求
                    if (paymentInfo.Payment == "Linepay")
                    {
                        var linePayService = new LinePayService();
                        
                        // 準備商品列表
                        var products = new List<LinePayProductDTO>();
                        var orderDetails = _context.TOrderDetails
                            .Where(x => x.FOrderId == order.FOrderId)
                            .ToList();
                        
                        foreach (var detail in orderDetails)
                        {
                            var product = _context.TProducts.FirstOrDefault(p => p.FProductId == detail.FProductId);
                            products.Add(new LinePayProductDTO
                            {
                                Name = product?.FProductName ?? "商品",
                                Quantity = detail.FQuantity ?? 1,
                                Price = Convert.ToInt32(detail.FUnitPrice ?? 0)
                            });
                        }

                        // 建立 LINE Pay 付款請求
                        var paymentRequest = new PaymentRequestDTO
                        {
                            Amount = Convert.ToInt32(paymentInfo.TotalAmount ?? 0),
                            Currency = "TWD",
                            OrderId = order.FOrderId,
                            Packages = new List<PackageDTO>
                            {
                                new PackageDTO
                                {
                                    Id = order.FOrderId,
                                    Amount = Convert.ToInt32(paymentInfo.TotalAmount ?? 0),
                                    Name = "訂單商品",
                                    Products = products
                                }
                            },
                            RedirectUrls = new RedirectUrlsDTO
                            {
                                ConfirmUrl = $"{Request.Scheme}://{Request.Host}/Home/LinePayConfirm",
                                CancelUrl = $"{Request.Scheme}://{Request.Host}/Home/LinePayCancel"
                            }
                        };

                        var paymentResponse = await linePayService.SendPaymentRequest(paymentRequest);
                        
                        if (paymentResponse.ReturnCode == "0000")
                        {
                            // 返回 LINE Pay 付款網址
                            return Ok(new 
                            { 
                                success = true, 
                                paymentUrl = paymentResponse.Info.PaymentUrl.Web,
                                transactionId = paymentResponse.Info.TransactionId,
                                orderId = order.FOrderId
                            });
                        }
                        else
                        {
                            return BadRequest(new { message = "LINE Pay 付款請求失敗：" + paymentResponse.ReturnMessage });
                        }
                    }
                    else
                    {
                        // 取貨付款，直接完成
                        return Ok(new { success = true, message = "訂單建立成功", orderId = order.FOrderId });
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

        public string GetOrderID()
        {
            string orderID = string.Empty;

            // 取得當天日期
            string todayDate = DateTime.Now.ToString("yyyyMMdd");

            // 取得當天已有的訂單數量
            //int orderCount = _context.TOrders
            //                        .Count(o => o.FOrderId.StartsWith(todayDate));
            // 取得當天已有的訂單數量
            int orderCount = _context.TOrders
                                    .Where(o => o.FOrderId.StartsWith(todayDate))
                                    .ToList() // 立即執行查詢，關閉資料庫連線
                                    .Count();

            // 設定新的訂單 ID
            orderID = todayDate + (orderCount + 1).ToString().PadLeft(5, '0');

            return orderID;
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

		//[HttpPost]
		//public IActionResult Calculation(int n)
		//{
		//    try
		//    {
		//        long n0 = 0; // 邊界條件1 : 第0位等於0
		//        long n1 = 1; // 邊界條件2 : 第1位等於1
		//        long result = 0; // 先設結果變數初值
		//        string numberList = n0.ToString(); // 顯示數列
		//        for (int i = 0; i < n; i++)
		//        {
		//            if (n <= 1) // 邊界條件情況
		//            {
		//                if (n == 0)
		//                {
		//                    result = n;
		//                    numberList = n.ToString();
		//                }
		//                else if (n == 1)
		//                {
		//                    result = n;
		//                    numberList += ", " + n.ToString();
		//                }
		//            }
		//            else // n >= 2 的情況
		//            {
		//                result = n0 + n1; // 第 n 位 = 前兩項加總
		//                n0 = n1; // 讓第 n-2 位等於n-1位
		//                n1 = result; // 讓第 n-1 位等於前一個結果
		//                numberList += ", " + result; // 串接數列
		//            }
		//        }
		//        return Json(new { number = result, numberList = numberList });
		//    }
		//    catch
		//    {
		//        return BadRequest();
		//    }
		//}
		[HttpPost]
		public IActionResult Calculation(int n)
		{
			try
			{
				string numberList = string.Empty;
				long result = Fibonacci(n, out numberList);
				return Json(new { number = result, numberList = numberList });
			}
			catch
			{
				return BadRequest();
			}
		}



		private long Fibonacci(int n, out string numberList)
		{
			if (n == 0)
			{
				numberList = "0";
				return 0;
			}
			else if (n == 1)
			{
				numberList = "0, 1";
				return 1;
			}
			else
			{
				string prevNumberList;
				long prevResult = Fibonacci(n - 1, out prevNumberList);
				long prevPrevResult = Fibonacci(n - 2, out numberList);
				long result = prevResult + prevPrevResult;
				numberList = $"{prevNumberList}, {result}";
				return result;
			}
		}


		//[HttpPost]
		//public IActionResult Calculation(int n) //正確的
		//{
		//	try
		//	{
		//		int number1 = 0; // 邊界條件1:第0位等於0
		//		int number2 = 1; // 邊界條件2:第1位等於1
		//		int result = 0; // 先設結果變數初值
		//		string numberList = number1.ToString(); // 初始數列

		//		if (n == 0)
		//		{
		//			result = n;
		//         numberList = n.ToString();
		//		}
		//		else if (n == 1)
		//		{
		//			result = n;
		//         numberList += "," + n.ToString();
		//		}

		//		for (int i = 0; i < n - 1; i++)
		//		{
		//			result = number1 + number2; // 第n位=前兩項加總
		//			number1 = number2; // 讓第n-2位等於n-1位
		//			number2 = result; // 讓第n-1位等於前一個結果
		//			numberList += "," + result; // 串接數列
		//		}

		//		return Json(new { number = result, numberList = numberList });
		//	}
		//	catch
		//	{
		//		return BadRequest();
		//	}
		//}

		// LINE Pay 確認付款回調
		[HttpGet]
		public async Task<IActionResult> LinePayConfirm(string transactionId, string orderId)
		{
			try
			{
				// 查詢訂單
				var order = await _context.TOrders.FirstOrDefaultAsync(o => o.FOrderId == orderId);
				if (order == null)
				{
					return RedirectToAction("PaymentResult", new { success = false, message = "訂單不存在" });
				}

				// 確認付款
				var linePayService = new LinePayService();
				var confirmDTO = new PaymentConfirmDTO
				{
					Amount = Convert.ToInt32(order.FTotalAmount ?? 0),
					Currency = "TWD"
				};

				var confirmResponse = await linePayService.ConfirmPayment(transactionId, orderId, confirmDTO);

				if (confirmResponse.ReturnCode == "0000")
				{
					// 更新訂單狀態為已付款
					order.FStatus = "已付款";
					await _context.SaveChangesAsync();

					return RedirectToAction("PaymentResult", new 
					{ 
						success = true, 
						orderId = orderId, 
						transactionId = transactionId,
						amount = order.FTotalAmount 
					});
				}
				else
				{
					return RedirectToAction("PaymentResult", new 
					{ 
						success = false, 
						message = "付款確認失敗：" + confirmResponse.ReturnMessage 
					});
				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("PaymentResult", new 
				{ 
					success = false, 
					message = "付款處理錯誤：" + ex.Message 
				});
			}
		}

		// LINE Pay 取消付款回調
		[HttpGet]
		public IActionResult LinePayCancel(string transactionId, string orderId)
		{
			return RedirectToAction("PaymentResult", new 
			{ 
				success = false, 
				message = "付款已取消", 
				orderId = orderId 
			});
		}

		// 付款結果頁面
		[HttpGet]
		public IActionResult PaymentResult(bool success, string orderId = "", string transactionId = "", decimal? amount = null, string message = "")
		{
			ViewBag.Success = success;
			ViewBag.OrderId = orderId;
			ViewBag.TransactionId = transactionId;
			ViewBag.Amount = amount;
			ViewBag.Message = message;
			return View();
		}

	}
}