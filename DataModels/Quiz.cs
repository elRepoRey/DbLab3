using Lab3.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using Lab3.DBServices;
using System.Windows;


namespace Lab3.DataModels
{
    public class Quiz
    {
        
        private IEnumerable<Question> _questions;
        public string _title = string.Empty;
        public IEnumerable<Question> Questions => _questions;
        public string Title => _title;
        private Random _random = new Random();
        private List<Question> _usedQuestions = new List<Question>(); 

        public Quiz()
        {
            _questions = new List<Question>();                        
        }

        public Question GetRandomQuestion()
        {
           try
           {
                var availableQuestions = _questions.Where(q => !_usedQuestions.Contains(q)).ToList();

                if (!availableQuestions.Any())
                {
                    _usedQuestions.Clear();
                    return null;
                }

                var randomQuestion = availableQuestions[_random.Next(availableQuestions.Count)];
                _usedQuestions.Add(randomQuestion);
                return randomQuestion;
           }
            catch (Exception ex)
           {
                throw new Exception(ex.Message);
           }
                     
        }


        public void AddQuestion(string statement, int correctAnswer, string category, params string[] answers)
        {
            try
            {
                var question = new Question(statement, answers, correctAnswer, category);
                var questionsList = _questions as List<Question>;
                questionsList.Add(question);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
          
        }

        public void RemoveQuestion(int index)
        {
            try
            {
                var questionsList = _questions as List<Question>;
                if (index >= 0 && index < questionsList.Count)
                {
                    questionsList.RemoveAt(index);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The provided index is out of range.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
         
        }
        public void InsertQuestion(int index, Question question)
        {
            try
            {
                if (index >= 0 && index < _questions.Count())
                {
                    var questionsList = _questions as List<Question>;
                    questionsList.Insert(index, question);
                }
                else
                {
                    throw new ArgumentOutOfRangeException("The provided index is out of range.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
  
}
