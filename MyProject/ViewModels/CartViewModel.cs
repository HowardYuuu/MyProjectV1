using MyProject.Models;

namespace MyProject.ViewModels
{
	public class CartViewModel
	{
		public List<TProduct> Products { get; set; }
		public List<TSizeQty> Sizes { get; set; }
		public List<TShoppingCart> ShoppingCarts { get; set;}
	}
}
