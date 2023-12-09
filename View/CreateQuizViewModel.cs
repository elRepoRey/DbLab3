using GalaSoft.MvvmLight.Command;
using Lab3.DataModels;
using Lab3.DBServices;
using Lab3.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace Lab3.View
{
    public class CreateQuizViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly QuizService _quizService = new QuizService();        
        public Quiz CurrentQuiz { get; private set; }
        public Question CurrentQuestion;      
        public event Action? QuizCreatedCompleted;
        private readonly MongodbService _dbService = new();
        public bool IsNextQuestionAvailable => CurrentQuestionIndex < CurrentQuiz.Questions.Count() - 1;
        public bool IsPreviousQuestionAvailable => CurrentQuestionIndex > 0;
        public string CurrentQuestionNumberText => $"{CurrentQuestionIndex + 1 } of {CurrentQuiz.Questions.Count() + 1}";
        private int _currentQuestionIndex = 0;
        public int CurrentQuestionIndex
        {
            get => _currentQuestionIndex;
            set
            {
                _currentQuestionIndex = value;
                OnPropertyChanged(nameof(CurrentQuestionIndex));
                OnPropertyChanged(nameof(IsNextQuestionAvailable));
                OnPropertyChanged(nameof(IsPreviousQuestionAvailable));
                OnPropertyChanged(nameof(CurrentQuestionNumberText));
            }
        }

        private string _quizTitle;

        public string QuizTitle
        {
            get => _quizTitle;
            set
            {
                _quizTitle = value;
                OnPropertyChanged(nameof(QuizTitle));
            }
        }      

        private string _questionStatement;
        public string QuestionStatement
        {
            get => _questionStatement;
            set
            {
                _questionStatement = value;
                OnPropertyChanged(nameof(QuestionStatement));
            }
        }
        private List<string> _answers;
        public List<string> Answers
        {
            get => _answers;
            set
            {
                _answers = value;
                OnPropertyChanged(nameof(Answers));
            }
        }
        private int _correctAnswer;
        public int CorrectAnswer
        {
            get => _correctAnswer;
            set
            {
                _correctAnswer = value;
                OnPropertyChanged(nameof(CorrectAnswer));
            }
        }
        private string _category;
        public string Category
        {
            get => _category;
            set
            {
                _category = value;
                OnPropertyChanged(nameof(Category));
            }
        }

        private bool _isModified;
        public bool IsModified
        {
            get => _isModified;
            set
            {
                _isModified = value;
                OnPropertyChanged(nameof(IsModified));
            }
        }

        private bool _canSaveQuestion;
        public bool CanSaveQuestion
        {
            get => _canSaveQuestion;
            set
            {
                _canSaveQuestion = value;
                OnPropertyChanged(nameof(CanSaveQuestion));
            }
        }

        private bool _canSaveQuiz;
        public bool CanSaveQuiz
        {
            get => _canSaveQuiz;
            set
            {
                _canSaveQuiz = value;
                OnPropertyChanged(nameof(CanSaveQuiz));
            }
        }

        private bool _hasQuestions;
        public bool HasQuestions
        {
            get => _hasQuestions;
            set
            {
                _hasQuestions = value;
                OnPropertyChanged(nameof(HasQuestions));
            }
        }
  
        public ICommand SaveCommand { get; private set; }  
        public ICommand UpdateQuestionCommand { get; private set; }
        public ICommand NextQuestionCommand { get; private set; }
        public ICommand PreviousQuestionCommand { get; private set; }
        public ICommand BrowseImageCommand { get; private set; }
       
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public CreateQuizViewModel( )
        {
            CurrentQuiz = new Quiz();
                  
            Answers = new[] { " ", " ", " " }.ToList();
            SaveCommand = new RelayCommand(SaveQuiz);           
            UpdateQuestionCommand = new RelayCommand(UpdateCurrentQuestion);
            NextQuestionCommand = new RelayCommand(NextQuestion);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion);
                      
            IsModified = false;
            CanSaveQuiz = false;
            CanSaveQuestion = true;
            HasQuestions = false;
            
        }

        private async void SaveQuiz()
        {
            try
            {
                if (string.IsNullOrEmpty(QuizTitle))
                {
                    MessageBox.Show("Please enter a title for the quiz.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (!_quizService.IsValidFilename(QuizTitle))
                {
                    MessageBox.Show("The quiz title is not a valid filename.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                var quizDAO = new QuizDAO
                {
                    Title = QuizTitle,
                    Questions = CurrentQuiz.Questions.ToList()
                };

                List<string> Files = new List<string>();
                Files = await _dbService.GetAllQuizTitles();

                foreach (var file in Files)
                {

                    if (file.Equals(QuizTitle, StringComparison.OrdinalIgnoreCase))
                    {
                        MessageBox.Show("Quiz with this title already exists. Edit the existing quiz or choose a different title.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                }

                await _dbService.WriteData(quizDAO, QuizTitle);

                MessageBox.Show("Quiz saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                QuizCreatedCompleted?.Invoke();
                CurrentQuiz = new Quiz();
                QuizCreatedCompleted = null;
            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
              
            
        }
   
      

        private void UpdateCurrentQuestion()
        {
            try
            {
                if (Answers.Any(answer => string.IsNullOrEmpty(answer)) || string.IsNullOrEmpty(Category) || string.IsNullOrEmpty(QuestionStatement))

                {
                    MessageBox.Show("Please fill all the fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                if (string.IsNullOrEmpty(QuizTitle))
                {
                    MessageBox.Show("Please enter a title for the quiz.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newQuestion = new Question(QuestionStatement, Answers.ToArray(), CorrectAnswer, Category);             

                CurrentQuiz.AddQuestion(newQuestion.Statement, newQuestion.CorrectAnswer, newQuestion.Category, newQuestion.Answers);

              

                MessageBox.Show("Question updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CanSaveQuiz = true;

                ClearQuestionData();
                CurrentQuestionIndex++;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }   
        }
        private void ClearQuestionData()
        {
            try
            {
                QuestionStatement = string.Empty;
                Answers = new[] { " ", " ", " " }.ToList();
                CorrectAnswer = 0;
                Category = string.Empty;
                HasQuestions = false;
                CanSaveQuestion = true;
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

        }

        private void UpdateQuizData()
        {
            try
            {
                CurrentQuestion = CurrentQuiz.Questions.ElementAt(_currentQuestionIndex);
                QuestionStatement = CurrentQuestion.Statement;
                Answers = CurrentQuestion.Answers.ToList();
                CorrectAnswer = CurrentQuestion.CorrectAnswer;
                Category = CurrentQuestion.Category;
             

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

    

        private void NextQuestion()
        {
            if (HasQuestions)
            {
                if (IsNextQuestionAvailable)
                {
                    CurrentQuestionIndex++;
                    UpdateQuizData();
                }
                else
                {
                    ClearQuestionData();
                    CurrentQuestionIndex++;
                }

            }
        }

        private void PreviousQuestion()
        {
            if (IsPreviousQuestionAvailable)
            {
                CurrentQuestionIndex--;
                UpdateQuizData();
                CanSaveQuestion = false;
                HasQuestions = true;
            }
        }
    }
}
