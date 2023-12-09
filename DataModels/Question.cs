using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace Lab3.DataModels;
public class Question
{
    public string Statement { get; }
    public string[] Answers { get; }
    public int CorrectAnswer { get; }
    public string Category { get; }
    
    

    public Question(string statement, string[] answers, int correctAnswer, string category)
    {
        
        Statement = statement ?? throw new ArgumentNullException(nameof(statement));
        Answers = answers ?? throw new ArgumentNullException(nameof(answers));
        CorrectAnswer = correctAnswer;
        Category = category ?? throw new ArgumentNullException(nameof(category));
      
    }

  

    



}
