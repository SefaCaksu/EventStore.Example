using System;
namespace Product.App.Models.ViewModels
{
	public class ListProductVM
	{
        public string Id { get; set; }
        public string ProductName { get; set; }
        public int Count { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Price { get; set; }
    }
}

