namespace VaporStore
{
	using AutoMapper;
    using System.Linq;
    using VaporStore.Data.Models;
    using VaporStore.DataProcessor.Dto.Export;

    public class VaporStoreProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public VaporStoreProfile()
		{
			this.CreateMap<Game, GameExportDto>()
			   .ForMember(x => x.Title,
			   y => y.MapFrom(s => s.Name));
			this.CreateMap<Purchase, PurchasesExportDto>()
				   .ForMember(x => x.Card,
				   y => y.MapFrom(s => s.Card.Number))
				   .ForMember(x=> x.Cvc, y=> y.MapFrom(s=> s.Card.Cvc))
				   .ForMember(x=> x.Date, y=> y.MapFrom(s=> s.Date.ToString("yyyy-MM-dd HH:mm")));
			this.CreateMap<User, UserPurchasesExportDto>()
				.ForMember(x=> x.TotalSpent, y=> y.MapFrom(s=> s.Cards.Sum(x=> x.Purchases.Count())));
		
		}
	}
}