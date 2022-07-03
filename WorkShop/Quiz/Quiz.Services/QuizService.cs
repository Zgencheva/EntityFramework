
using Quiz.Data;

namespace Quiz.Services
{
    public class QuizService : IQuizService
    {
        private readonly ApplicationDbContext applicationDbContext;
        public QuizService(ApplicationDbContext dbContext)
        {
            this.applicationDbContext = dbContext;
        }
        public void Add(string title)
        {
            var quiz = new Models.Quiz { Title = title };
            this.applicationDbContext.Quizzes.Add(quiz);
            this.applicationDbContext.SaveChanges();
        }

        
    }
}
