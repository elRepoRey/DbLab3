using System;
using System.Collections.Generic;
using System.Configuration;
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
using Lab3.DBServices;
using Lab3.View;


// Add your connection string to DBServices/MongodbServices

namespace Lab3
{
    public partial class MainWindow : Window
    {
        private MongodbService _mongodbService;
        public MainWindow()
        {
            InitializeComponent();
            dbconnection();
            this.DataContext = new MainViewModel(); 

        }

        private async void dbconnection()
        {

            _mongodbService = new MongodbService();
            await _mongodbService.SeedFromData();
        }
    }
}
