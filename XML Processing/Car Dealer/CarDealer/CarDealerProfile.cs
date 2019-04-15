using AutoMapper;
using CarDealer.Dtos.Export;
using CarDealer.Dtos.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            this.CreateMap<ImportSuppliersDto, Supplier>();

            this.CreateMap<ImportPartDto, Part>();

            this.CreateMap<ImportCustomerDto, Customer>();

            this.CreateMap < ImportSaleDto, Sale>();

            this.CreateMap<CarImportDto, Car>();

            this.CreateMap<Car, CarInfoDto>();

            this.CreateMap<Car, CarSaleDto>();
        }
    }
}
