using GalaSoft.MvvmLight.Command;
using Lab3.DataModels;
using Lab3.DBServices;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Lab3.Utils;

namespace Lab3.View
{
    public class QuizViewModel : INotifyPropertyChanged
    {
        private readonly MongodbService _dbService = new();
        private QuizService _quizService = new();

        public ObservableCollection<string> QuizTitles { get; set; }
        private QuizDAO _currentQuizDAO;
        public QuizDAO CurrentQuizDAO
        {
            get { return _currentQuizDAO; }
            set
            {
                _currentQuizDAO = value;
                OnPropertyChanged(nameof(CurrentQuizDAO));
            }
        }
        private UserControl _currentView;
        
        public UserControl CurrentView
        {
            get { return _currentView; }
            set
            {
                _currentView = value;
                OnPropertyChanged(nameof(CurrentView));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand PlayCommand { get; private set; }
        public ICommand EditCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }
        public ICommand ListCommand { get; private set; }

        public QuizViewModel()
        {
            
            UpdateQuizList();
           
            
        }

        private async  void PlayQuiz(string quizTitle)
        {           
            await _quizService.SetupQuizAsync(quizTitle);

            PlayQuizViewModel playQuizViewModel = new PlayQuizViewModel();
            var playQuizView = new PlayQuizView(playQuizViewModel);
            ((PlayQuizViewModel)playQuizView.DataContext).QuizCompleted += HandleQuizCompleted;
            CurrentView = playQuizView;
        }

        private void HandleQuizCompleted()
        {            
            UpdateQuizList();
        }

        private async  void EditQuiz(string quizTitle)
        {
            await _quizService.SetupQuizAsync(quizTitle);

            EditQuizViewModel editViewModel = new EditQuizViewModel();
            var editQuizView = new EditQuizView(editViewModel);

            editViewModel.QuizEditCompleted += HandleQuizCompleted;
            CurrentView = editQuizView;
        }

        private  void DeleteQuiz(string quizTitle)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this quiz?", "Delete Quiz", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
               _dbService.DeleteData(quizTitle);
                QuizTitles.Remove(quizTitle);
            }
        }

        private async void UpdateQuizList()
        {
              
            QuizTitles = new ObservableCollection<string>();
            var titles = await _dbService.GetAllQuizTitles();
            QuizTitles = new ObservableCollection<string>(titles);
            PlayCommand = new RelayCommand<string>(PlayQuiz);
            EditCommand = new RelayCommand<string>(EditQuiz);
            DeleteCommand = new RelayCommand<string>(DeleteQuiz);
            CurrentView = new QuizListView();
        }


    }
}
