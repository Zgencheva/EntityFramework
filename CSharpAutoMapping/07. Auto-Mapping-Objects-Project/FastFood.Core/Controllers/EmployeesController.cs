namespace FastFood.Core.Controllers
{
    using System;
    using System.Linq;
    using AutoMapper;
    using Data;
    using Microsoft.AspNetCore.Mvc;
    using AutoMapper.QueryableExtensions;
    using ViewModels.Employees;
    using FastFood.Models;

    public class EmployeesController : Controller
    {
        private readonly FastFoodContext context;
        private readonly IMapper mapper;

        public EmployeesController(FastFoodContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public IActionResult Register()
        {
            var possitions = this.context.Positions
                .ProjectTo<RegisterEmployeeViewModel>(mapper.ConfigurationProvider)
                .ToList();
            return this.View(possitions);
        }

        [HttpPost]
        public IActionResult Register(RegisterEmployeeInputModel model)
        {
            var employee = this.mapper.Map<Employee>(model);

            this.context.Employees.Add(employee);

            context.SaveChanges();

            return this.RedirectToAction("All", "Employees");
        }

        public IActionResult All()
        {
            var employees = this.context.Employees
               .ProjectTo<EmployeesAllViewModel>(mapper.ConfigurationProvider)
               .ToList();

            return this.View(employees);
        }
    }
}
