using Lab3.DataModels;
using Lab3.DBServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using System.ComponentModel;
using Lab3.Utils;
using System.Collections.ObjectModel;

namespace Lab3.View
{
    public class GenerateQuizViewModel : INotifyPropertyChanged
    {
        private readonly MongodbService _dbService = new();
        private readonly Random _random = new();
        public event PropertyChangedEventHandler PropertyChanged;
        public event Action PlayGeneratedQuiz;
        public ObservableCollection<string> SelectedCategories { get; set; } = new ObservableCollection<string>();

        public List<int> NumberOfQuestionsOptions { get; } = new List<int> { 5, 10, 25};
        public ICommand PlayGeneratedQuizCommand { get; private set; }
        private IAsyncEnumerable<List<Question>> AllQuestions { get; set; }

        public bool CanPlayQuiz => IsRandomSelected || IsByCategorySelected;
        private bool _isRandomSelected;
        public bool IsRandomSelected
        {
            get => _isRandomSelected;
            set
            {
                _isRandomSelected = value;
                OnPropertyChanged(nameof(IsRandomSelected));
                OnPropertyChanged(nameof(CanPlayQuiz));
            }
        }
        private bool _isByCategorySelected;
        public bool IsByCategorySelected
        {
            get => _isByCategorySelected;
            set
            {
                _isByCategorySelected = value;
                OnPropertyChanged(nameof(IsByCategorySelected));
                OnPropertyChanged(nameof(CanPlayQuiz));
            }
        }

        private int _selectedNumberOfQuestions;
        public int SelectedNumberOfQuestions
        {
            get => _selectedNumberOfQuestions;
            set
            {
                _selectedNumberOfQuestions = value;
                OnPropertyChanged(nameof(SelectedNumberOfQuestions));
            }
        }

        private List<string> _availableCategories;
        public List<string> AvailableCategories
        {
            get => _availableCategories;
            set
            {
                _availableCategories = value;
                OnPropertyChanged(nameof(AvailableCategories));
            }
        }

        public GenerateQuizViewModel()
        {
                                    
            PlayGeneratedQuizCommand = new RelayCommand(OnPlayGeneratedQuiz);
            AllQuestions = _dbService.GetAllQuestionsAsync();
            _availableCategories = new List<string>();
            GetAvailableCategories();
            SelectedNumberOfQuestions = NumberOfQuestionsOptions[0];
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void GetAvailableCategories()
        {
            var uniqueCategories = new HashSet<string>();

            await foreach (var questionList in AllQuestions)
            {
                foreach (var question in questionList)
                {
                    uniqueCategories.Add(question.Category);
                }
            }

            AvailableCategories = uniqueCategories.ToList();
        }

        public async void OnPlayGeneratedQuiz()
        {
            if (IsRandomSelected)
            {
                await RandomQuiz(SelectedNumberOfQuestions);
            }
            else if (IsByCategorySelected)
            {
                if (SelectedCategories == null || !SelectedCategories.Any())
                {
                    MessageBox.Show("Please select at least one category.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                await ByCategoryQuiz();
            }
            else
            {
                MessageBox.Show("Please select a quiz type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PlayGeneratedQuiz?.Invoke();
            PlayGeneratedQuiz = () => { };
        }

        public async Task RandomQuiz(int numberOfquestions)
        {
            if (numberOfquestions <= 0) return;

            var allQuestionsList = new List<Question>();
            await foreach (var questionList in AllQuestions)
            {
                allQuestionsList.AddRange(questionList);
            }

            var selectedIndices = new HashSet<int>();
            while (selectedIndices.Count < numberOfquestions && selectedIndices.Count < allQuestionsList.Count)
            {
                int randomIndex = _random.Next(allQuestionsList.Count);
                selectedIndices.Add(randomIndex);
            }

            Global.CurrentQuiz = new Quiz();
            foreach (var index in selectedIndices)
            {
                var q = allQuestionsList[index];
                Global.CurrentQuiz.AddQuestion(q.Statement, q.CorrectAnswer, q.Category, q.Answers.ToArray());
            }
        }

        private async Task ByCategoryQuiz()
        {
            Global.CurrentQuiz = new Quiz();
            if (SelectedCategories.Count <= 0) return;

            var allQuestionsFromSelectedCategories = new List<Question>();

            await foreach (var questionlist in AllQuestions)
            {
                foreach (var question in questionlist)
                {
                    if (!SelectedCategories.Contains(question.Category)) continue;
                    allQuestionsFromSelectedCategories.Add(question);
                }
            }
           
            var shuffledQuestions = allQuestionsFromSelectedCategories.OrderBy(q => _random.Next()).ToList();
            
            var selectedQuestions = shuffledQuestions.Take(SelectedNumberOfQuestions).ToList();

            foreach (var q in selectedQuestions)
            {
                Global.CurrentQuiz.AddQuestion(q.Statement, q.CorrectAnswer, q.Category,  q.Answers.ToArray());
            }
        }


    }
}
