using System.ComponentModel;

namespace MyProject.ViewModels
{
    public class SizeQtyViewModel
    {
        public int Fid { get; set; }

        [DisplayName("產品編號")]
        public int? FProductId { get; set; }

        [DisplayName("尺寸")]
        public decimal? FSize { get; set; }

        [DisplayName("庫存")]
        public int? FQuantity { get; set; }
    }
}
