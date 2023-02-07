using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace myProgLanguage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string text;
        List<LED> leds;
        List<Variable> variables;

        public MainWindow()
        {
            InitializeComponent();
            leds = new List<LED>();
            variables = new List<Variable>();

            for (int i = 0; (Ellipse)FindName($"LED{i}") != null; i++)
            {
                Ellipse ellipse = (Ellipse)FindName($"LED{i}");
                LED newLed = new LED();
                newLed.Id = i;
                newLed.ellipse = ellipse;
                leds.Add(newLed);
            }
        }
        public void Restart()
        {
            for (int i = 0; i < leds.Count; i++)
                leds[i].Off();
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //Lire le code
            text = TextZone.Text;
            string[] lines = text.Split("\n");
            foreach (string line in lines)
            {
                string[] words = line.Split(" ");
                switch (words[0])
                {
                    case "SET":
                        SET(words);
                        break;
                    case "RESET":
                        RESET(words);
                        break;
                    case "WAIT":
                        WAIT(words);
                        break;
                    case "LET":
                        LET(words);
                        break;
                    case "INC":
                        break;
                }
            }
        }
        public void SET(string[] words)
        {
            int param_x = -1;
            //vérifier si words[1] est une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                variables.ForEach(var =>
                {
                    if (var.name == words[1].Remove(0, 1))
                    {
                        int.TryParse(var.val, out param_x);
                        return;
                    }
                });
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(words[1], out param_x);
            leds[param_x].On();
        }
        public void RESET(string[] words)
        {
            int param_x = 0;
            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (int.TryParse(words[1], out param_x))
                leds[param_x].Off();
        }
        public void WAIT(string[] words)
        {
            int param_xxx = 0;
            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (int.TryParse(words[1], out param_xxx))
                Thread.Sleep(param_xxx);
        }
        public void LET(string[] words)
        {
            int param_val = 0;
            Variable newVar = new Variable("name", 0);
            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                if (int.TryParse(words[2], out param_val))
                    newVar = new Variable(words[1].Remove(0, 1), param_val);
                else
                    newVar = new Variable(words[1].Remove(0, 1), words[2], "STRING");
                variables.Add(newVar);
            }
        }
    }
}
