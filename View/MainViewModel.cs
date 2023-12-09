using GalaSoft.MvvmLight.Command;
using Lab3.DataModels;
using Lab3.DBServices;
using Lab3.Utils;
using Lab3.View;
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Input;

namespace Lab3.View
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ICommand NavigateCommand { get; private set; }
        public UserControl NavbarView { get; private set; }

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get { return _currentView; }
            set
            {
                if (_currentView != value)
                {
                    _currentView = value;
                    OnPropertyChanged(nameof(CurrentView));
                }
            }
        }

        public MainViewModel()
        {
            NavigateCommand = new RelayCommand<EnumNavbarItems>(NavigateTo);
            NavbarView = new Navbar();
            NavbarView.DataContext = this;            
            CurrentView = new WelcomeView();       

        }
       
        private void NavigateTo(EnumNavbarItems item)
        {            
            switch (item)
            {
                case EnumNavbarItems.Quizzes:
                    CurrentView = new QuizView();
                    break;
                case EnumNavbarItems.CreateQuiz:
                    CreateQuizViewModel createQuizViewModel = new CreateQuizViewModel();
                    createQuizViewModel.QuizCreatedCompleted += HandleQuizCreated; 
                    CurrentView = new CreateQuizView(createQuizViewModel);
                    break;
                case EnumNavbarItems.GenerateQuiz:                    
                    GenerateQuizViewModel generateQuizViewModel = new GenerateQuizViewModel();
                    generateQuizViewModel.PlayGeneratedQuiz += HandleOnPlayGeneratedQuiz;                    
                    CurrentView = new GenerateQuizView(generateQuizViewModel);
                    break;
                case EnumNavbarItems.Welcome:
                    CurrentView = new WelcomeView();
                    break;
            }   
        }

        private void HandleQuizCreated()
        {
            CurrentView = new QuizView();
        }

        private void HandleOnPlayGeneratedQuiz()
        {
            PlayQuizViewModel playQuizViewModel = new PlayQuizViewModel();
            playQuizViewModel.QuizCompleted += HandleQuizCompleted;
            CurrentView = new PlayQuizView(playQuizViewModel);
        }

        private void HandleQuizCompleted()
        {
             CurrentView = new QuizView();
        }        
    }
}
