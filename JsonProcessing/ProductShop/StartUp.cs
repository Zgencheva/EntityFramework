using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        static IMapper mapper;
        public static void Main(string[] args)
        {
            var context = new ProductShopContext();
            //context.Database.EnsureDeleted();
            //context.Database.EnsureCreated();


            //string inputUsersJson = File.ReadAllText("../../../Datasets/users.json");
            //string inputProductsJson = File.ReadAllText("../../../Datasets/products.json");
            //string inputCategoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            //string inputCatProdJson = File.ReadAllText("../../../Datasets/categories-products.json");
            //var usersInput = ImportUsers(context, inputUsersJson);
            //var productsInput = ImportProducts(context, inputProductsJson);
            //var inputCategories = ImportCategories(context, inputCategoriesJson);
            //Console.WriteLine(ImportCategoryProducts(context, inputCatProdJson));

            Console.WriteLine(GetUsersWithProducts(context));

        }

        private static void InitializeAutoMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            });
            mapper = config.CreateMapper();
        }

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();
            var dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            var users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();
        
            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson) 
        {
            InitializeAutoMapper();
            var dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductsInputModel>>(inputJson);

            var products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            InitializeAutoMapper();
            var categoriesDto = JsonConvert
                .DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson)
                .Where(x=> x.Name != null)
                .ToList();

            var categories = mapper.Map<IEnumerable<Category>>(categoriesDto);
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {

            InitializeAutoMapper();
            var categoryProductDto = JsonConvert
                .DeserializeObject<IEnumerable<CategoryProductsInputModel>>(inputJson)
                .ToList();

            var categoryProducts = mapper.Map<IEnumerable<CategoryProduct>>(categoryProductDto);

            context.CategoryProducts.AddRange(categoryProducts);
            
            return $"Successfully imported {context.SaveChanges()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            InitializeAutoMapper();
            var productsInRange = context.Products
                .Where(x => 500 <= x.Price && x.Price <= 1000)
                //.Select(x => new ProductExportDto
                //{
                //    name = x.Name,
                //    price = x.Price,
                //    seller = x.Seller.FirstName + " " + x.Seller.LastName,
                //})
                .OrderBy(x => x.Price)
                .ToList();

            

            var productsDTOs = mapper.Map<IEnumerable<ProductExportDto>>(productsInRange);

            var outputResult = JsonConvert.SerializeObject(productsDTOs, Formatting.Indented);


            return outputResult;
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            InitializeAutoMapper();
            var usersWithSoldItem = context.Users
                .Where(x => x.ProductsSold.Any(p=> p.Buyer != null))
                .Select(x => new
                {
                    firstName = x.FirstName,
                    lastName = x.LastName,
                    soldProducts = x.ProductsSold.Where(p=> p.BuyerId != null)
                    .Select(s => new
                    {
                        name = s.Name,
                        price = s.Price,
                        buyerFirstName = s.Buyer.FirstName,
                        buyerLastName = s.Buyer.LastName
                    })
                    .ToList()
                })
                .OrderBy(x=> x.lastName)
                .ThenBy(x=> x.firstName)
                .ToList();
            var outputResult = JsonConvert.SerializeObject(usersWithSoldItem, Formatting.Indented);

            return outputResult;
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            InitializeAutoMapper();

            var categories = context.Categories
               
                .Select(x => new
                {
                    category = x.Name,
                    productsCount = x.CategoryProducts.Count(),
                    averagePrice = $"{(x.CategoryProducts.Sum(p => p.Product.Price) / x.CategoryProducts.Count()):f2}",
                    totalRevenue = $"{ x.CategoryProducts.Sum(p => p.Product.Price):f2}",
                })
                .OrderByDescending(x=> x.productsCount);

            var outputResult = JsonConvert.SerializeObject(categories, Formatting.Indented);

            return outputResult;
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
        
            var users = context.Users
                .Include(x=> x.ProductsSold)
                .ToList()
                .Where(x => x.ProductsSold.Any(p => p.Buyer != null))
                .Select(user => new
                {
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    age = user.Age,
                    soldProducts =  new
                    {
                        count = user.ProductsSold.Where(x=> x.Buyer != null).Count(),
                        products = user.ProductsSold
                        .Where(x=> x.BuyerId != null)
                        .Select(x => new
                        {
                            name = x.Name,
                            price = x.Price,
                        })
                   
                    } 
                })
                .OrderByDescending(x=> x.soldProducts.count)
                .ToList();

            var outputObj = new
            {
                usersCount = users.Count,
                users = users,
            };

            var jsonserializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented,
            };
            var outputResult = JsonConvert.SerializeObject(outputObj, jsonserializerSettings);

            return outputResult;
        }
    }
}