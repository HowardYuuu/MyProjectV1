using System.Collections.Specialized;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Res
{
    public class CusResDTO
    {
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int CustomerId { get; set; }
        public bool? Success { get; set; }
        public List<string>? Errors { get; set; }

        public string? RedirectUrl { get; set; }
        public string? CurrentUrl { get; set; }


    }
}
