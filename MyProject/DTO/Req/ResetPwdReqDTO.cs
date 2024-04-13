using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Req
{
    public class ResetPwdReqDTO
    {
        [Required(ErrorMessage ="密碼為必填欄位")]
        public string? Password { get; set; }

        public string? Token { get; set; }
        public int UserId { get; set; }
    }
}
