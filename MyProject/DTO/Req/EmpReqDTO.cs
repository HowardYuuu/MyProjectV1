using System.ComponentModel.DataAnnotations;

namespace prjTravelPlatform_release.Areas.CustomizedIdentity.DTO.Req
{
    public class EmpReqDTO
    {
        [Required(ErrorMessage = "帳號為必填欄位")]
        public string? Account { get; set; }

        [Required(ErrorMessage = "密碼為必填欄位")]
        //[DataType(DataType.Password)]
        //[RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$",
        //ErrorMessage = "密碼必須包含至少一個小寫字母、一個大寫字母、一個數字、一個特殊字符並且至少8個字符")]
        public string? Password { get; set; }      
    }
}
