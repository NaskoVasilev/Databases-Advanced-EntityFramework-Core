using System.Collections.Generic;

namespace ProductShop.Dtos
{
    public class SoldProductsDto
    {
        public int Count { get; set; }

        public List<SoldProductDto> Products { get; set; }
    }
}
