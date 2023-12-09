using GalaSoft.MvvmLight.Command;
using Lab3.DataModels;
using Lab3.DBServices;
using Lab3.Utils;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3.View
{
    public class EditQuizViewModel : INotifyPropertyChanged
    {        
        private Quiz _originalQuiz;
        
        public event Action? QuizEditCompleted;

        private readonly MongodbService _dbService = new();
        private QuizService _quizService;
   
        public bool IsNextQuestionAvailable => CurrentQuestionIndex < Global.CurrentQuiz.Questions.Count() - 1;
        public bool IsPreviousQuestionAvailable => CurrentQuestionIndex > 0;
        public string CurrentQuestionNumberText => $"{CurrentQuestionIndex + 1} of {Global.CurrentQuiz.Questions.Count()}";

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

                LoadQuestionData();
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
   
        private Question CurrentQuestion;

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
                IsModified = true;
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

        public ICommand SaveCommand { get; private set; }
        public ICommand NextQuestionCommand { get; private set; }
        public ICommand PreviousQuestionCommand { get; private set; }
        public ICommand UpdateQuestionCommand { get; private set; }
      


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public   EditQuizViewModel() {           
                   
              
            _quizService = new QuizService();
            SaveCommand = new RelayCommand(SaveQuiz);
            NextQuestionCommand = new RelayCommand(NextQuestion);
            PreviousQuestionCommand = new RelayCommand(PreviousQuestion);
            UpdateQuestionCommand = new RelayCommand(UpdateCurrentQuestion);      

            InitializeQuizData();

        }     

        private async void SaveQuiz()
        {
            try {
               
             
                if (QuizTitle.Equals(string.Empty))
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
                    Questions = Global.CurrentQuiz.Questions.ToList()
                };
                if (QuizTitle.ToLower() != _originalQuiz.Title.ToLower())
                {
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

                    _dbService.DeleteData(_originalQuiz.Title); 
                }            

                if(QuizTitle != _originalQuiz.Title)
                {
                    _dbService.DeleteData(_originalQuiz.Title);
                }

                await _dbService.WriteData(quizDAO, QuizTitle);           

           
                MessageBox.Show("Quiz saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            
                QuizEditCompleted?.Invoke();

                QuizEditCompleted = null;     
                }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private void NextQuestion()
        {
            if (IsNextQuestionAvailable)
            {
                CurrentQuestionIndex++;
                LoadQuestionData();
            }
        }

        private void PreviousQuestion()
        {
            if (IsPreviousQuestionAvailable)
            {
                CurrentQuestionIndex--;
                LoadQuestionData();
            }
        }

        private void UpdateCurrentQuestion()
        {
            try
            {                
                if (Answers.Any(string.IsNullOrEmpty) || Category.Equals(string.Empty) || QuestionStatement.Equals(string.Empty))
                {
                    MessageBox.Show("Please fill all the fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                var newQuestion = new Question(QuestionStatement, Answers.ToArray(), CorrectAnswer, Category);

                

                if (Global.CurrentQuiz.Questions.Count() == 1 ||  Global.CurrentQuiz.Questions.Count() == CurrentQuestionIndex + 1)
                {
                    Global.CurrentQuiz.RemoveQuestion(_currentQuestionIndex);
                    Global.CurrentQuiz.AddQuestion(newQuestion.Statement, newQuestion.CorrectAnswer, newQuestion.Category, newQuestion.Answers);

                }
              else
                {
                    Global.CurrentQuiz.RemoveQuestion(_currentQuestionIndex);
                    Global.CurrentQuiz.InsertQuestion(CurrentQuestionIndex, newQuestion);
                }

                    
                MessageBox.Show("Question updated successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CanSaveQuiz = true;
                IsModified = false;

            }
            catch ( Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }                   
            
        }

        public void InitializeQuizData()
        {
           try
            {
                _originalQuiz = Global.CurrentQuiz;
                QuizTitle = Global.CurrentQuiz.Title;
                LoadQuestionData();
                CanSaveQuiz = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }      


        public void LoadQuestionData()
        {
            try
            {


                CurrentQuestion = Global.CurrentQuiz.Questions.ElementAt(_currentQuestionIndex);
                QuestionStatement = CurrentQuestion.Statement;
                Answers = CurrentQuestion.Answers.ToList();
                CorrectAnswer = CurrentQuestion.CorrectAnswer;
                Category = CurrentQuestion.Category;                
                IsModified = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

    }
}