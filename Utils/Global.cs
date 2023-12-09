using Lab3.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Utils
{
    public static  class Global
    {
        private static Quiz _currentQuiz = new();
        public static Quiz CurrentQuiz
        {
            get => _currentQuiz;
            set
            {
                if (_currentQuiz != value)
                {
                    _currentQuiz = value;
                }
            }
          
        }     

    }
}
