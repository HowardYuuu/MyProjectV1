using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Req
{
    public class CusSignUpReqDTO
    {
        [Required(ErrorMessage = "姓名為必填欄位")]
        public string? Name { get; set; }


        [Required(ErrorMessage = "帳號為必填欄位")]
        [EmailAddress(ErrorMessage = "Email格式錯誤")]
        public string? Email { get; set; }


        [Required(ErrorMessage = "密碼為必填欄位")]
        //[DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        //ErrorMessage = "密碼必須包含至少一個小寫字母、一個大寫字母、一個數字、一個特殊字符並且至少8個字符")]
        public string? Password { get; set; }

        
        public string? ReturnUrl { get; set; }
        public string? CurrentUrl { get; set; }
    }
}
