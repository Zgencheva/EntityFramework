using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quiz.Data;
using Quiz.Services;
using System;
using System.IO;

namespace Quiz.ConsoleUI
{
    class Program
    {
        static void Main(string[] args)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureService(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var quizService = serviceProvider.GetService<IQuizService>();
            var quiz = quizService.GetQuizById(1);
            Console.WriteLine(quiz.Title);
            foreach (var question in quiz.Questions)
            {
                Console.WriteLine(question.Title);
                foreach (var answer in question.Answers)
                {
                    Console.WriteLine(answer.Title);
                }
            }
            //quizService.Add("C# DB");

            //var questionService = serviceProvider.GetService<IQuestionService>();
            //questionService.Add("What is Ef Core", 1);

            //var answerService = serviceProvider.GetService<IAnswerService>();
            ////answerService.Add("It is on ORM", 5, true, 1);
            //answerService.Add("It is a MicroORM", 0, false, 1);
            //var userAnswerService = serviceProvider.GetService<IUserAnswerService>();
            //userAnswerService.AddUserAnswer("c5173dd5-d71e-4b23-a503-a599a7212588", 1,1, 2);


        }

        private static void ConfigureService(IServiceCollection services) 
        {
            var configurations = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();
            services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configurations.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<IdentityUser>(options => 
            options.SignIn.RequireConfirmedAccount = true)
              .AddEntityFrameworkStores<ApplicationDbContext>();

            services.AddTransient<IQuizService, QuizService>();
            services.AddTransient<IQuestionService, QuestionService>();
            services.AddTransient<IAnswerService, AnswerService>(); 
            services.AddTransient<IUserAnswerService, UserAnswerService>();


        }
    }
}
