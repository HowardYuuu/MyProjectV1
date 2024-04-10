using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using MyProject.Models;
using System.Data.Common;
using System.Data;
using System.Diagnostics;
using System.Text;
using Microsoft.DotNet.Scaffolding.Shared.ProjectModel;
using System.Net.Mail;
using MyProject.ViewModels;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Azure;

namespace MyProject.Controllers
{
    public class MemberController : Controller
    {

        private readonly MyProjectContext _context;

        public MemberController(MyProjectContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }
        //註冊畫面
        public IActionResult Register()
        {
            return View();
        }
        // 註冊成功提示頁面
        public IActionResult RegistrationSuccess()
        {
            return View();
        }
        //登入畫面
        public IActionResult Login()
        {
            return View();
        }
        //註冊
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            var response = new Dictionary<string, object>();
            response["success"] = false;
            response["errors"] = new Dictionary<string, string>(); // 將錯誤訊息存儲在字典中
            // 檢查是否已經存在相同的帳號
            if (_context.TCustomers.Any(m => m.FEmail == model.Email))
            {
                ((Dictionary<string, string>)response["errors"]).Add("Email", "此Email已被使用");
                return Json(response);
            }

            if (ModelState.IsValid)
            {

                // 密碼加密
                var hashedPassword = HashPassword(model.Password);

                // 建立會員
                var member = new TCustomer
                {
                    FName = model.Name,
                    FAccount = model.Email,
                    FEmail = model.Email,
                    FPwd = hashedPassword.hashedPassword,
                    FSalt = hashedPassword.salt,
                    FEnabled = false // 設置為未驗證
                };

                _context.TCustomers.Add(member);
                await _context.SaveChangesAsync();

                // 寄送驗證信
                //await SendVerificationEmail(model.Email);

                response["success"] = true;
                return Json(response);
                // 返回註冊成功提示頁面
                //return RedirectToAction("RegistrationSuccess");
                //return RedirectToAction("Login");
            }
            // 驗證失敗，返回錯誤信息
            // 將ModelState的錯誤訊息轉換為字典
            foreach (var key in ModelState.Keys)
            {
                var error = ModelState[key].Errors.FirstOrDefault();
                if (error != null)
                {
                    ((Dictionary<string, string>)response["errors"]).Add(key, error.ErrorMessage);
                }
            }
            return Json(response);
            //return View(model);
        }

        //登入
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var member = _context.TCustomers.FirstOrDefault(m => m.FEmail == model.Email);

                if (member != null && VerifyPassword(model.Password, member.FPwd, member.FSalt))
                {
                    // 登入成功，將用戶名字存入 Cookie 中
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, member.FName),
                    //new Claim("UserName", member.FName),
                    new Claim(ClaimTypes.Email, member.FEmail),
                    //new Claim("UserEmail", member.FEmail),
                    new Claim(ClaimTypes.NameIdentifier, member.FUserId.ToString()),
                    //new Claim("UserID", member.FUserId.ToString()),
                    //new Claim(ClaimTypes.Role, "Administrator"),
                };
                    var authProperties = new AuthenticationProperties
                    {
                        //AllowRefresh = <bool>,
                        // Refreshing the authentication session should be allowed.

                        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                        // The time at which the authentication ticket expires. A 
                        // value set here overrides the ExpireTimeSpan option of 
                        // CookieAuthenticationOptions set with AddCookie.

                        //IsPersistent = true,
                        // Whether the authentication session is persisted across 
                        // multiple requests. When used with cookies, controls
                        // whether the cookie's lifetime is absolute (matching the
                        // lifetime of the authentication ticket) or session-based.

                        //IssuedUtc = <DateTimeOffset>,
                        // The time at which the authentication ticket was issued.

                        //RedirectUri = <string>
                        // The full path or absolute URI to be used as an http 
                        // redirect response value.
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                    


                    //await HttpContext.SignInAsync(
                    //	CookieAuthenticationDefaults.AuthenticationScheme,
                    //	new ClaimsPrincipal(claimsIdentity),
                    //	authProperties);

                    //_logger.LogInformation("User {Email} logged in at {Time}.",
                    //	user.Email, DateTime.UtcNow);

                    //return LocalRedirect(Url.GetLocalUrl(returnUrl));

                    // 重定向到首頁
                    //return RedirectToAction("Index", "Home");
                    return Json(new { success = true });
                }
                else
                {
                    //return Json(new { success = false, errorMessage = "帳號或密碼錯誤！" });
                    return Json(new { success = false, errorMessage = "帳號或密碼錯誤" });
                    //ModelState.AddModelError(string.Empty, "帳號或密碼錯誤");
                }
            }
            return Json(new { success = false, errorMessage = "帳號或密碼空白" });
		}


        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            // 重定向到首頁
            return RedirectToAction("Index", "Home");
        }

        private (string hashedPassword, string salt) HashPassword(string password)
        {
            // 產生隨機鹽值
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            string salt = Convert.ToBase64String(saltBytes);

            // 使用鹽值和密碼進行雜湊處理
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                string hashedPassword = Convert.ToBase64String(hashedBytes);

                return (hashedPassword, salt);
            }
        }
        private bool VerifyPassword(string password, string hashedPassword, string salt)
        {
            // 使用儲存的鹽值和輸入的密碼進行雜湊處理
            using (var sha256 = SHA256.Create())
            {
                byte[] combinedBytes = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashedBytes = sha256.ComputeHash(combinedBytes);
                string inputHashedPassword = Convert.ToBase64String(hashedBytes);

                // 比較雜湊後的密碼是否匹配
                return inputHashedPassword == hashedPassword;
            }
        }

        #region 寄信驗證
        //private async Task SendVerificationEmail(string email)
        //{
        //    var smtpClient = new SmtpClient("smtp.gmail.com")
        //    {
        //        Port = 587,
        //        Credentials = new System.Net.NetworkCredential("l26101272@gs.ncku.edu.tw", "aa870920"),
        //        EnableSsl = true,
        //    };
        //    string body = "請點擊以下連結完成註冊：<a style=\"text-decoration:none;color:inherit;\" href=\"https://localhost:7063/\">點擊這裡</a>";
        //    var mailMessage = new MailMessage
        //    {

        //        From = new MailAddress("l26101272@gs.ncku.edu.tw"),
        //        Subject = "會員註冊驗證信",
        //        Body = "請點擊以下連結完成註冊：<a style=\"text-decoration:none;color:inherit;\" href=\"https://localhost:7063/\">點擊這裡</a>"
        //    };

        //    mailMessage.To.Add(email);

        //    await smtpClient.SendMailAsync(mailMessage);
        //}
        #endregion


    }
}
