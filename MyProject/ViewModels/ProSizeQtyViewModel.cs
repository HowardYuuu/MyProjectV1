using MyProject.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MyProject.ViewModels
{
    public class ProSizeQtyViewModel
    {
       public List<TProduct>? Product { get; set; }
        public List<TSizeQty>? SizeQty { get; set; }

    }
}
