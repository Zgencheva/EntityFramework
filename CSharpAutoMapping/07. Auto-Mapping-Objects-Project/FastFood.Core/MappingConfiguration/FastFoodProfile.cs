namespace FastFood.Core.MappingConfiguration
{
    using AutoMapper;
    using FastFood.Core.ViewModels.Categories;
    using FastFood.Core.ViewModels.Employees;
    using FastFood.Core.ViewModels.Items;
    using FastFood.Core.ViewModels.Orders;
    using FastFood.Models;
    using System;
    using ViewModels.Positions;

    public class FastFoodProfile : Profile
    {
        public FastFoodProfile()
        {
            //Positions
            this.CreateMap<CreatePositionInputModel, Position>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.PositionName));

            this.CreateMap<Position, PositionsAllViewModel>()
                .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));
            //Employees:
            this.CreateMap<Position, RegisterEmployeeViewModel>()
                .ForMember(x => x.PositionId, y => y.MapFrom(p => p.Id));


            this.CreateMap<RegisterEmployeeInputModel, Employee>();

            this.CreateMap<Employee, EmployeesAllViewModel>()
                .ForMember(z=>z.Position, y=> y.MapFrom(x=> x.Position.Name));

            //Categories:
            this.CreateMap<CreateCategoryInputModel, Category>()
                .ForMember(x => x.Name, y => y.MapFrom(c => c.CategoryName));
            this.CreateMap<Category, CategoryAllViewModel>()
             .ForMember(x => x.Name, y => y.MapFrom(s => s.Name));

            //Items:

            this.CreateMap<Category, CreateItemViewModel>()
                .ForMember(x=> x.CategoryId, y=> y.MapFrom(c=> c.Id));

            this.CreateMap<CreateItemInputModel, Item>();

            this.CreateMap<Item, ItemsAllViewModels>()
                .ForMember(x=> x.Category, y=> y.MapFrom(c=> c.Category.Name));

            //Orders:

            //this.CreateMap<CreateOrderViewModel,Order>()
             

            this.CreateMap<Order, OrderAllViewModel>()
                .ForMember(x => x.Employee, y => y.MapFrom(c => c.Employee.Name))
                .ForMember(x=>x.OrderId, y=> y.MapFrom(o=> o.Id))
                .ForMember(x=> x.DateTime, y=> y.MapFrom(d=> d.DateTime.ToString("d")));

            this.CreateMap<CreateOrderInputModel, Order>()
                .ForMember(x=> x.DateTime, y=> y.MapFrom(s=> DateTime.Now));
        }
    }
}
