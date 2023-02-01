using System;
using System.Collections.Generic;
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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace myProgLanguage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }



        public void LedOn(int ledId)
        {
            Ellipse Led = (Ellipse)FindName($"LED{ledId}");
            Led.Fill = Brushes.Green;
            InvalidateVisual();
        }
        public void LedOff(int ledId)
        {
            Ellipse Led = (Ellipse)FindName($"LED{ledId}");
            Led.Fill = Brushes.Red;
        }
        public void Restart()
        {
            for (int i = 0; i < 9; i++)
            {
                LedOff(i);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Restart();
            
            //Lire le code
            string text = TextZone.Text;
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
                        break;
                    case "INC":
                        break;
                }
            }
        }
        public void SET(string[] words)
        {
            int param_x = 0;

            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (int.TryParse(words[1], out param_x))
            {
                LedOn(param_x);
            }
        }

        public void RESET(string[] words)
        {
            int param_x = 0;

            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (int.TryParse(words[1], out param_x))
            {
                LedOff(param_x);
            }
        }
        public void WAIT(string[] words)
        {
            int param_xxx = 0;

            //TODO : vérifier si words[1] n'est pas une variable ($___)

            //vérifier si words[1] peut etre convertie en int
            if (int.TryParse(words[1], out param_xxx))
            {
                Thread.Sleep(param_xxx);
            }
        }
    }
}
