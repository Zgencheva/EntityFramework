
using Quiz.Services.Models;

namespace Quiz.Services
{
    public interface IQuizService
    {
        int Add(string title);
        public QuizViewModel GetQuizById(int quizId);
    }
}
