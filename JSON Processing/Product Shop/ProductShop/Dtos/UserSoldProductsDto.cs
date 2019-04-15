using System.Collections.Generic;

namespace ProductShop.Dtos
{
    public class UserSoldProductsDto
    {
        public UserSoldProductsDto()
        {
            this.SoldProducts = new List<ProductBuyerDto>();
        }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public List<ProductBuyerDto> SoldProducts { get; set; }
    }
}
