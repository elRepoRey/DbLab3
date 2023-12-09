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
    /// <summary>
    /// Interaction logic for GenerateQuizView.xaml
    /// </summary>
    public partial class GenerateQuizView : UserControl
    {
        public GenerateQuizView(GenerateQuizViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null && DataContext is GenerateQuizViewModel viewModel)
            {
                viewModel.SelectedCategories.Add(checkbox.Content.ToString());
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            var checkbox = sender as CheckBox;
            if (checkbox != null && DataContext is GenerateQuizViewModel viewModel)
            {
                viewModel.SelectedCategories.Remove(checkbox.Content.ToString());
            }
        }

    }
}
