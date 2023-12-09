using GalaSoft.MvvmLight.Command;
using Lab3.DataModels;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Lab3.Utils;
using Lab3.DBServices;
using System.IO;
using System.Windows.Media.Imaging;

namespace Lab3.View
{
    public class PlayQuizViewModel : INotifyPropertyChanged

    {
       
        public Quiz CurrentQuiz { get; }
        public Question CurrentQuestion { get; private set; }
        public int CurrentQuestionIndex { get; private set; }
        public event Action? QuizCompleted;
        
        public event PropertyChangedEventHandler PropertyChanged;
        public bool IsAnswerSelected => SelectedAnswerIndex.HasValue;

        public string CurrentQuestionNumberText => $"{CurrentQuestionIndex + 1} of {CurrentQuiz.Questions.Count()}";

        private int _correctAnswersCount;
        public int CorrectAnswersCount
        {
            get { return _correctAnswersCount; }
            private set
            {
                if (_correctAnswersCount != value)
                {
                    _correctAnswersCount = value;
                    OnPropertyChanged(nameof(CorrectAnswersCount));
                    OnPropertyChanged(nameof(PercentageCorrect));
                }
            }
        }

        public int PercentageCorrect
        {
            get
            {
                return (int)((float)CorrectAnswersCount / CurrentQuiz.Questions.Count() * 100);
            }
        }         

        public bool IsNextQuestionAvailable => CurrentQuestionIndex < CurrentQuiz.Questions.Count() - 1;

        private int? _selectedAnswerIndex;

        public int? SelectedAnswerIndex
        {
            get { return _selectedAnswerIndex; }
            set
            {
                if (_selectedAnswerIndex != value)
                {
                    _selectedAnswerIndex = value;
                    OnPropertyChanged(nameof(PercentageCorrect));
                    OnPropertyChanged(nameof(SelectedAnswerIndex));
                    OnPropertyChanged(nameof(IsAnswerSelected));                    
                }
            }
        }       
                
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand NextQuestionCommand { get; }

        public PlayQuizViewModel()
        {
            CurrentQuiz = new();
            CurrentQuiz = Global.CurrentQuiz;
            CurrentQuestionIndex = 0;
            LoadQuestion();
            NextQuestionCommand = new RelayCommand(NextQuestion);   
            
        }

        public void NextQuestion()
        {
           try
            {
                if (SelectedAnswerIndex == CurrentQuestion.CorrectAnswer)
                {
                    CorrectAnswersCount++;
                }

                if (IsNextQuestionAvailable)
                {
                    CurrentQuestionIndex++;
                    LoadQuestion();
                    OnPropertyChanged(nameof(CurrentQuestionNumberText));
                    OnPropertyChanged(nameof(IsNextQuestionAvailable));
                }
                else
                {
                    MessageBox.Show($"You have answered {CorrectAnswersCount} questions correctly out of {CurrentQuiz.Questions.Count()}.", "Quiz Completed", MessageBoxButton.OK, MessageBoxImage.Information);
                    QuizCompleted?.Invoke();


                    QuizCompleted = null;
                }

                SelectedAnswerIndex = null;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }

        private  void LoadQuestion()
        {
            try
            {
                CurrentQuestion = CurrentQuiz.GetRandomQuestion();
           
                OnPropertyChanged(nameof(CurrentQuestion));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
        }
      
    }
}
