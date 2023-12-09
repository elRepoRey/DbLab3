using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Lab3.View
{
    public partial class QuizListView : UserControl
    {
        public event EventHandler<string> PlayQuizRequested;
        public event EventHandler<string> EditQuizRequested;
        public event EventHandler<string> DeleteQuizRequested;

        public QuizListView()
        {
            InitializeComponent();          
        
            PlayCommand = new RelayCommand<string>(title => PlayQuizRequested?.Invoke(this, title));
            EditCommand = new RelayCommand<string>(title => EditQuizRequested?.Invoke(this, title));
            DeleteComand = new RelayCommand<string>(title => DeleteQuizRequested?.Invoke(this, title));

        }

        public ICommand PlayCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteComand { get; }
    }
}
