using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyProject.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }

        [DisplayName("產品名稱")]
        [Required(ErrorMessage = "產品名稱不能為空")]
        //[StringLength(50, ErrorMessage = "產品名稱長度不能超過50個字元")]
        public string? ProductName { get; set; }

        [DisplayName("品牌")]
        [Required(ErrorMessage = "產品品牌不能為空")]
        [StringLength(50, ErrorMessage = "產品品牌長度不能超過50個字元")]
        public string? Brand { get; set; }

        [DisplayName("型號")]
        [Required(ErrorMessage = "產品型號不能為空")]
        public string? Number { get; set; }


        [DisplayName("款式")]
        public string? Style { get; set; }

        [DisplayName("性別")]
        public string? Gender { get; set; }

        [DisplayName("主要色系")]
        [Required(ErrorMessage = "產品顏色不能為空")]
        [StringLength(50, ErrorMessage = "產品顏色長度不能超過50個字元")]
        public string? MainColor { get; set; }

        [DisplayName("寬度")]

        public string? Width { get; set; }


        [DisplayName("價格")]
        [Required(ErrorMessage = "產品價格不能為空")]
        public decimal? Price{ get; set; }


        [DisplayName("圖片")]
        public string? Image { get; set; }

        [DisplayName("上架狀態")]
        public bool OnSale { get; set; }

        [DisplayName("圖片檔案")]
        public IFormFile? ImageFile { get; set; }

    }
}
