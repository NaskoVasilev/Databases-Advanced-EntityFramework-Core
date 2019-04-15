using System.Collections.Generic;

namespace ProductShop.Dtos
{
    public class SellerProductsDto
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? Age { get; set; }

        public SoldProductsDto SoldProducts { get; set; }
    }
}
