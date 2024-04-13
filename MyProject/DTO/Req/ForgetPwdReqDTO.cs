using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Req
{
    public class ForgetPwdReqDTO
    {
        [Required(ErrorMessage = "Email為必填欄位")]
        public string? Email { get; set; }
    }
}
