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
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace myProgLanguage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string text;
        bool flag = true;
        int count = 0;
        List<LED> LEDs = new List<LED>();
        public MainWindow()
        {
            InitializeComponent();
            while (flag)
            {
                if ((Ellipse)FindName($"LED{count}") != null)
                {
                    Ellipse ellipse = (Ellipse)FindName($"LED{count}");
                    LED newLed = new LED();
                    newLed.Id = count;
                    newLed.ellipse = ellipse;
                    LEDs.Add(newLed);
                }
                else
                    flag = false;
                count++;
            }
        }



        public void LedOn(int ledId)
        {
            LEDs.ForEach(led =>
            {
                if (led.Id == ledId)
                    led.isOn = true;
            });
        }
        public void LedOff(int ledId)
        {
            LEDs.ForEach(led =>
            {
                if (led.Id == ledId)
                    led.isOn = false;
            });
        }
        public void Restart()
        {
            for (int i = 0; i < LEDs.Count; i++)
            {
                LedOff(i);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Restart();
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
                        break;
                    case "INC":
                        break;
                }
                Refresh();
            }
            Refresh();
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
        private void Refresh()
        {
            this.Dispatcher.Invoke(() =>
            {
                LEDs.ForEach(led =>
                {
                    led.Update();
                });
            });
        }

    }
}
