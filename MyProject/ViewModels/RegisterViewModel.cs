using System.ComponentModel.DataAnnotations;

namespace MyProject.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "姓名為必填欄位")]
        //[EmailAddress]
        public string? Name { get; set; }
        [Required(ErrorMessage = "帳號為必填欄位")]
        [EmailAddress]
        public string? Email { get; set; }
        [Required(ErrorMessage = "密碼為必填欄位")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密碼和確認密碼不相符")]
        public string? ConfirmPassword { get; set; }
    }
}
