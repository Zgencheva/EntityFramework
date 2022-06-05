namespace BookShop
{
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;


    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            //DbInitializer.ResetDatabase(db);
            
            Console.WriteLine(RemoveBooks(db));
        }
        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder sb = new StringBuilder();
            var ageRestriction = Enum.Parse<AgeRestriction>(command, true);
            var query = context.Books
                .Where(x => x.AgeRestriction == ageRestriction)
                .Select(x=> new { 
                    x.Title,
                    })
                .OrderBy(x=> x.Title)
                .ToList();

            foreach (var book in query)
            {
                sb.AppendLine(book.Title);
            }

            return sb.ToString().TrimEnd();
        }
        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var editionType = Enum.Parse<EditionType>("Gold", true);
            var books = context.Books.Where(x => x.EditionType == editionType && x.Copies < 5000)
                .Select(b => new {
                    b.Title,
                    b.BookId
                })
                .OrderBy(x => x.BookId)
                .ToList();
            foreach (var book in books)
            {
                sb.AppendLine(book.Title);
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var query = context.Books.Where(x => x.Price > 40)
                .Select(x => new
                {
                    x.Title,
                    x.Price
                })
                .OrderByDescending(b => b.Price)
                .ToList();

            foreach (var book in query)
            {
                sb.AppendLine($"{book.Title} - ${book.Price:f2}");
            }
            return sb.ToString().TrimEnd();
        }
        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder sb = new StringBuilder();
            var query = context.Books.Where(x => x.ReleaseDate.Value.Year != year)
                .Select(x => new
                {
                    x.Title,
                    x.BookId
                })
                .OrderBy(b => b.BookId)
                .ToList();

            foreach (var book in query)
            {
                sb.AppendLine($"{book.Title}");
            }
            return sb.ToString().TrimEnd();
        }

        public static string GetBooksByCategory(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var inputCollection = input.Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.ToLowerInvariant()).ToArray();
            var books = context.Books
                  .Where(x=> x.BookCategories.Any(b=> inputCollection.Contains(b.Category.Name.ToLower())))
                 .Select(x => x.Title)
                 .OrderBy(x=> x)
                 .ToList();

            return string.Join(Environment.NewLine, books);

        }

        public static string GetBooksReleasedBefore(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var date = DateTime.ParseExact(input, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                  .Where(x => x.ReleaseDate.Value < date)
                 .Select(x => new {
                     x.Title,
                     x.EditionType,
                     x.Price,
                     x.ReleaseDate
                 })
                 .OrderByDescending(x => x.ReleaseDate)
                 .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} - {book.EditionType} - ${book.Price:f2}");
            };
            return sb.ToString().TrimEnd();
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var authors = context.Authors.Where(x=> x.FirstName.EndsWith(input))
                .Select(x=> new { 
                fullName = x.FirstName + " " + x.LastName
                })
                .OrderBy(x=> x.fullName)
                .ToList();

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.fullName}");
            };
            return sb.ToString().TrimEnd();
        }
        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books.Where(x => x.Title.ToLower().Contains(input.ToLower()))
                .Select(x => new
                {
                    x.Title,
                })
                .OrderBy(x => x.Title)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title}");
            };
            return sb.ToString().TrimEnd();


        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder sb = new StringBuilder();
            var books = context.Books.Where(x => x.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .Select(x => new
                {
                    x.Title,
                    x.BookId,
                    FullName = x.Author.FirstName + " " + x.Author.LastName
                })
                .OrderBy(x => x.BookId)
                .ToList();

            foreach (var book in books)
            {
                sb.AppendLine($"{book.Title} ({book.FullName})");
            };
            return sb.ToString().TrimEnd();


        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            StringBuilder sb = new StringBuilder();
            var result = context.Books.Count(x => x.Title.Length > lengthCheck);


            return result;


        }
        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var authors = context.Authors.Select(x => new
            {
                FullName = x.FirstName + " " + x.LastName,
                CountOfBooks = x.Books.Sum(x => x.Copies),
            })
                .OrderByDescending(x=> x.CountOfBooks); ;

            foreach (var author in authors)
            {
                sb.AppendLine($"{author.FullName} - {author.CountOfBooks}");
            };
            return sb.ToString().TrimEnd();


        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var categories = context.Categories.Select(x => new
            {
                x.Name,
                Profit = x.CategoryBooks.Sum(x=> x.Book.Copies * x.Book.Price),
            })
                .OrderByDescending(x => x.Profit)
                .ThenBy(x=> x.Name);

            foreach (var category in categories)
            {
                sb.AppendLine($"{category.Name} ${category.Profit:f2}");
            };
            return sb.ToString().TrimEnd();


        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder sb = new StringBuilder();
            var categories = context.Categories
                .OrderByDescending(x => x.Name).Select(x => new
            {
                x.Name,
                Books = x.CategoryBooks
                    .Select(b=> new { 
                        Title = b.Book.Title,
                        ReleaseDay = b.Book.ReleaseDate,

                    })
                    .OrderByDescending(x=> x.ReleaseDay)
                    .Take(3)
                    //.ToList()
            })
                .OrderBy(x => x.Name)
                .ToList();

            foreach (var category in categories)
            {
                sb.AppendLine($"--{category.Name}");
                foreach (var book in category.Books)
                {
                    sb.AppendLine($"{book.Title} ({book.ReleaseDay.Value.Year})");
                }
            };
            return sb.ToString().TrimEnd();


        }

        public static void IncreasePrices(BookShopContext context) {
            var booksSelected = context.Books.Where(x => x.ReleaseDate.Value.Year < 2015);

            foreach (var book in booksSelected)
            {
                book.Price += 5;

            }
            context.SaveChanges();
        }

        public static int RemoveBooks(BookShopContext context)
        {
            var result = context.Books.Where(x => x.Copies < 4200).ToList();

            context.Books.RemoveRange(result);

            context.SaveChanges();
            return result.Count;
        }
    }
}
