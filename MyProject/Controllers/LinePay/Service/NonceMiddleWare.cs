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
			// 生成一個隨機的 nonce 值
			var nonce = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

			// 將 nonce 存入 HttpContext.Items，讓後續的頁面可以訪問
			context.Items["Nonce"] = nonce;

			// 或者可以將 nonce 添加到 HttpContext.Response.Headers，依你的需求
			//context.Response.Headers.Add("Content-Security-Policy", $"script-src 'self' 'nonce-{nonce}';");
			//context.Response.Headers.Add("Content-Security-Policy", "script-src 'self' 'unsafe-inline' 'unsafe-eval'; style-src 'self' 'unsafe-inline'");

			// 使用 ViewData
			//ViewData["Nonce"] = nonce;
			//context.Items["ViewDataNonce"] = nonce;

			await _next(context);
		}
	}

}
