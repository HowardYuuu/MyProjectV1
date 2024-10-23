using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace MyProject.Controllers.LinePay.Service
{
    public class NonceMiddleWare
    {
        private readonly RequestDelegate _next;

        public NonceMiddleWare(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // 只攔截 HTML 回應

            var originalBodyStream = context.Response.Body;
            using var memoryStream = new MemoryStream();
            context.Response.Body = memoryStream;

            // 呼叫下一個中間件
            await _next(context);

            // 生成 nonce
            string nonce = GenerateNonce();

            // 將 CSP header 加入回應
            context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'nonce-{nonce}';");

            // 將內容從內存流轉換為字串
            memoryStream.Seek(0, SeekOrigin.Begin);
            string responseBody = new StreamReader(memoryStream).ReadToEnd();

            // 將 nonce 添加到 <script> 標籤
            responseBody = AddNonceToScripts(responseBody, nonce);

            // 重置內存流並寫回修改後的內容
            var modifiedBody = Encoding.UTF8.GetBytes(responseBody);
            context.Response.Body = originalBodyStream;
            await context.Response.Body.WriteAsync(modifiedBody, 0, modifiedBody.Length);

        }
        private string AddNonceToScripts(string html, string nonce)
        {
            // 使用正則表達式找到所有 <script> 標籤，並添加 nonce 屬性
            string pattern = "<script(?![^>]*nonce=)([^>]*)>";
            string replacement = $"<script nonce=\"{nonce}\" $1>";
            return Regex.Replace(html, pattern, replacement, RegexOptions.IgnoreCase);
        }
        private string GenerateNonce()
        {
            // 生成一個隨機 nonce
            byte[] nonceBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(nonceBytes);
            }
            return Convert.ToBase64String(nonceBytes);
        }
        //public async Task InvokeAsync(HttpContext context)
        //{
        //	// 生成一個隨機的 nonce 值
        //	var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

        //	// 將 nonce 存入 HttpContext.Items，讓後續的頁面可以訪問
        //	context.Items["Nonce"] = nonce;

        //	// 或者可以將 nonce 添加到 HttpContext.Response.Headers，依你的需求
        //	context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'nonce-{nonce}';");
        //	//context.Response.Headers.Add("Content-Security-Policy", "script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'");

        //	// 使用 ViewData
        //	//ViewData["Nonce"] = nonce;
        //	//context.Items["ViewDataNonce"] = nonce;

        //	await _next(context);
        //}
    }

}
