﻿
using Quiz.Services.Models;

namespace Quiz.Services
{
    public interface IQuizService
    {
        void Add(string title);
        public QuizViewModel GetQuizById(int quizId);
    }
}