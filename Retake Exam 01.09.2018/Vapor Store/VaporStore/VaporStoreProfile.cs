namespace VaporStore
{
	using AutoMapper;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dtos.Export;

    public class VaporStoreProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public VaporStoreProfile()
		{
            this.CreateMap<Game, GameDto>()
                .ForMember(x => x.Genre, y=> y.MapFrom(s => s.Genre.Name))
                .ForMember(x => x.Title, y=> y.MapFrom(s => s.Name));
        }
	}
}