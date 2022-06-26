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
           // context.Database.EnsureDeleted();
           // context.Database.EnsureCreated();

           // var xmlUsers = File.ReadAllText("../../../Datasets/users.xml");
           //var xmlProducts = File.ReadAllText("../../../Datasets/products.xml");
           // var xmlCategories = File.ReadAllText("../../../Datasets/categories.xml");
           // var xmlCatProd = File.ReadAllText("../../../Datasets/categories-products.xml");
          
           // Console.WriteLine(ImportUsers(context, xmlUsers));
           // Console.WriteLine(ImportProducts(context, xmlProducts));
           // Console.WriteLine(ImportCategories(context, xmlCategories));
           // Console.WriteLine(ImportCategoryProducts(context, xmlCatProd));
            //Console.WriteLine(GetProductsInRange(context));

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
            //context.SaveChanges();

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
            //context.SaveChanges();

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
                if (allCategoriesProducts.Any(x=> x.CategoryId == catProd.CategoryId && x.ProductId == catProd.ProductId))
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
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);
            StringBuilder sb = new StringBuilder();
            var products = context.Products
                        .Include(x => x.Buyer)
                        .Where(x => x.Price >= 500 && x.Price <=1000)
                        .Select(p=> new ProductsInangeExportDTO
                        { 
                            Name = p.Name,
                            Price = p.Price,
                            Buyer = p.Buyer.FirstName + " " + p.Buyer.LastName
                        })
                        .OrderBy(x=> x.Price)
                        .Take(10)
                        .ToArray();

            var serializer = new XmlSerializer(typeof(ProductsInangeExportDTO[]), new XmlRootAttribute("Products"));

            serializer.Serialize(new StringWriter(sb), products, namespaces);

            return sb.ToString().TrimEnd();
        }
    }
}