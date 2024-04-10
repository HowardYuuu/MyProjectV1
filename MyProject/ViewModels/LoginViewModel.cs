using System.ComponentModel.DataAnnotations;

namespace MyProject.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "帳號為必填欄位")]
        [EmailAddress]
        public string? Email { get; set; }

        [Required(ErrorMessage = "密碼為必填欄位")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
    }
}
