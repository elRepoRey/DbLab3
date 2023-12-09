using Lab3.DataModels;
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
   
    public partial class EditQuizView : UserControl
    {
        private int i;
        private int j;
        public EditQuizView(EditQuizViewModel viewModel)
        {
            i = 0;
            j = 0;
            InitializeComponent();
            this.DataContext = viewModel;
        }
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            
            var viewModel = DataContext as EditQuizViewModel;
            if (viewModel != null)
            {   
                if(i > 4)
                {
                    viewModel.IsModified = true;
                }     
                i++;
            }
        }

        private void OnQuizTitleChanged(object sender, TextChangedEventArgs e)
        {
            var viewModel = DataContext as EditQuizViewModel;
            if (viewModel != null)
            {
                if (j > 0)
                {
                    viewModel.CanSaveQuiz = true;
                }
                j++;
            }
        }

    }
}
