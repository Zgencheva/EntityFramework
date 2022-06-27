using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.Dtos;
using ProductShop.Dtos.Import;
using ProductShop.Dtos.Export;
using ProductShop.Models;
using System.Text;
using ProductShop.XMLHelper;

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

            //var xmlUsers = File.ReadAllText("../../../Datasets/users.xml");
            //var xmlProducts = File.ReadAllText("../../../Datasets/products.xml");
            //var xmlCategories = File.ReadAllText("../../../Datasets/categories.xml");
            //var xmlCatProd = File.ReadAllText("../../../Datasets/categories-products.xml");

            //Console.WriteLine(ImportUsers(context, xmlUsers));
            //Console.WriteLine(ImportProducts(context, xmlProducts));
            //Console.WriteLine(ImportCategories(context, xmlCategories));
            //Console.WriteLine(ImportCategoryProducts(context, xmlCatProd));
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

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Users";
            var usersDtos = XmlConverter.Deserializer<UserInputModel>(inputXml, root);
            //var xmlSerializer = new XmlSerializer(typeof(UserInputModel[]),
            //    new XmlRootAttribute(root));
            //var textRead = new StringReader(inputXml);
            //var usersDtos = (UserInputModel[])xmlSerializer.Deserialize(textRead);
            var users = mapper.Map<User[]>(usersDtos);
            context.Users.AddRange(users);

            return $"Successfully imported {context.SaveChanges()}"; ;
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Products";
            var productsDtos = XmlConverter.Deserializer<ProductImputModel>(inputXml, root);
            //var xmlSerializer = new XmlSerializer(typeof(ProductImputModel[]),
            //    new XmlRootAttribute(root));
            //var textRead = new StringReader(inputXml);
            //var productsDtos = (ProductImputModel[])xmlSerializer.Deserialize(textRead);
            var products = mapper.Map<Product[]>(productsDtos);
            context.Products.AddRange(products);

            return $"Successfully imported {context.SaveChanges()}"; ;
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "Categories";
            var categoriesDtos = XmlConverter.Deserializer<CategoriesInputModel>(inputXml, root);
            //var xmlSerializer = new XmlSerializer(typeof(CategoriesInputModel[]),
            //    new XmlRootAttribute(root));
            //var textRead = new StringReader(inputXml);
            //var categoriesDtos = (CategoriesInputModel[])xmlSerializer.Deserialize(textRead);
            var categories = mapper.Map<Category[]>(categoriesDtos);
            context.Categories.AddRange(categories);

            return $"Successfully imported {context.SaveChanges()}"; ;
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            InitializeAutoMapper();
            const string root = "CategoryProducts";
            var catProdsDtos = XmlConverter.Deserializer<CategoriesProductsInputModels>(inputXml, root);

            //var xmlSerializer = new XmlSerializer(typeof(CategoriesProductsInputModels[]),
            //    new XmlRootAttribute(root));
            //var textRead = new StringReader(inputXml);
            //var catProdsDtos = (CategoriesProductsInputModels[])xmlSerializer.Deserialize(textRead);
            var catProds = mapper.Map<CategoryProduct[]>(catProdsDtos);
            var categoriesForBD = new List<CategoryProduct>();
            var allCategoriesProducts = context.CategoryProducts.ToList();
            foreach (var catProd in catProds)
            {
                if (allCategoriesProducts.Any(x => x.CategoryId == catProd.CategoryId && x.ProductId == catProd.ProductId))
                {
                    continue;
                }
                if (categoriesForBD.Any(x => x.CategoryId == catProd.CategoryId && x.ProductId == catProd.ProductId))
                {
                    continue;
                }
                categoriesForBD.Add(catProd);
            }
            context.CategoryProducts.AddRange(categoriesForBD);


            return $"Successfully imported {context.SaveChanges()}"; ;
        }
        public static string GetProductsInRange(ProductShopContext context)
        {
            //var namespaces = new XmlSerializerNamespaces();
            //namespaces.Add(string.Empty, string.Empty);

            var products = context.Products
                        .Include(x => x.Buyer)
                        .Where(x => x.Price >= 500 && x.Price <= 1000)
                        .Select(p => new ProductsInangeExportDTO
                        {
                            Name = p.Name,
                            Price = p.Price,
                            Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                        })
                        .OrderBy(x => x.Price)
                        .Take(10)
                        .ToArray();
            var result = XmlConverter.Serialize<ProductsInangeExportDTO>(products, "Products");
            //var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), 
            //    new XmlRootAttribute("Products"));
            //var texWriter = new StringWriter();
            //serializer.Serialize(texWriter, products, namespaces);

            return result.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            //var namespaces = new XmlSerializerNamespaces();
            //namespaces.Add(string.Empty, string.Empty);
            var users = context.Users
                .Include(x => x.ProductsSold)
                .ThenInclude(x => x.CategoryProducts)
                .ThenInclude(x => x.Product)
                .Where(x => x.ProductsSold.Count > 0)
                .Select(u => new GetSoldProductsDTO {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ExportProductDTO {
                        Name = p.Name,
                        Price = p.Price
                    }).ToArray()
                })
                .OrderBy(x => x.LastName)
                .ThenBy(x => x.FirstName)
                .Take(5)
                .ToArray();

            var result = XmlConverter.Serialize(users, "Users");

            return result.ToString().TrimEnd();
        }
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            //var namespaces = new XmlSerializerNamespaces();
            //namespaces.Add(string.Empty, string.Empty);
            var categories = context.Categories
                .Include(x => x.CategoryProducts)
                .ThenInclude(x => x.Product)
                .Select(c => new CategoryByProducDTO
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(x => x.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(x => x.Product.Price)
                })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.TotalRevenue)
                .ToArray();

            var result = XmlConverter.Serialize(categories, "Categories");

            return result.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {

            var users = context.Users
                .Include(x=> x.ProductsSold)
                .ThenInclude(x=> x.CategoryProducts)
                .ThenInclude(x=> x.Product)
             .Where(u => u.ProductsSold.Count >= 1) 
            .OrderByDescending(u => u.ProductsSold.Count)
            .Take(10).ToArray()
            .Select(u => new UsersExportDto()
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Age = u.Age.Value,
                SoldProducts = new SoldPRoductsDTo()
                {
                    Count = u.ProductsSold.Count,
                    Products = u.ProductsSold
                    //.Where(ps => ps.Buyer != null)
                       .Select(ps => new ExportProductDTO()
                       {
                           Name = ps.Name,
                           Price = ps.Price
                       })
                       .OrderByDescending(p => p.Price)
                       .ToArray()
                }
            });



            //var namespaces = new XmlSerializerNamespaces();
            //namespaces.Add(string.Empty, string.Empty);
            UsersWithProductsDto usersWithPRoducts = new UsersWithProductsDto
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = users.ToArray()
            };


            var result = XmlConverter.Serialize(usersWithPRoducts, "Users");
            //var resultASchararrat = result.ToString();
            //Console.WriteLine(resultASchararrat.Substring(115, 152));
            
            ////Console.WriteLine(result.ToCharArray().charAt(123));
            //return null;
            return result.ToString().TrimEnd();
        }
    }
}