using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MyProject.Controllers.Helper
{
    public class ScriptNonceTagHelper : TagHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScriptNonceTagHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (output.TagName == "script")
            {
                // 從 HttpContext 中取得 nonce
                var nonce = _httpContextAccessor.HttpContext.Items["Nonce"]?.ToString();

                // 為 <script> 標籤添加 nonce 屬性
                if (!string.IsNullOrEmpty(nonce))
                {
                    output.Attributes.SetAttribute("nonce", nonce);
                }
            }
        }
    }
}
