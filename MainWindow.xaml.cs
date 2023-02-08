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
                        INC(words);
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
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_n_r(words[1], true));
                int.TryParse(var.val, out param_x);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_n_r(words[1]), out param_x);

            //TODO : Géstion des erreurs
            leds[param_x].On();
        }
        public void RESET(string[] words)
        {
            int param_x = 0;

            //vérifier si words[1] n'est pas une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_n_r(words[1], true));
                int.TryParse(var.val, out param_x);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_n_r(words[1]), out param_x);

            //TODO : Géstion des erreurs
            leds[param_x].Off();
        }
        public void WAIT(string[] words)
        {
            int param_xxx = 0;

            //vérifier si words[1] n'est pas une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_n_r(words[1], true));
                int.TryParse(var.val, out param_xxx);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_n_r(words[1]), out param_xxx);

            //TODO : Géstion des erreurs
            Thread.Sleep(param_xxx);
        }
        public void LET(string[] words) //TODO: géréer l'ajout d'une variable qui éxiste déja
        {
            int param_val = 0;
            Variable newVar = null;
            
            //vérifier si words[1] est bien une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                //vérifier si words[2] n'est pas une variable ($___)
                if (words[2].Count() > 1 && words[2][0] == '$')
                {
                    Variable var = Variable.SearchVar(variables, RemoveBackslash_t_n_r(words[2], true));
                    int.TryParse(var.val, out param_val);
                }
                else
                    int.TryParse(RemoveBackslash_t_n_r(words[2]), out param_val);
                newVar = new Variable(variables.Count(), RemoveBackslash_t_n_r(words[1], true), param_val);
                variables.Add(newVar);
            }
        }
        public void INC(string[] words)
        {
            int param_val = 0;

            //vérifier si words[1] est bien une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                int varId = Variable.SearchVar(variables, RemoveBackslash_t_n_r(words[1], true)).id;

                //vérifier si words[2] peut etre convertie en int
                if (int.TryParse(RemoveBackslash_t_n_r(words[2]), out param_val))
                {
                    variables[varId].val = (int.Parse(variables[varId].val) + param_val).ToString();
                }                
            }
        }
        public string RemoveBackslash_t_n_r(string s, bool dollar = false)
        {
            string newS = s.Replace("\r", string.Empty);
            newS = newS.Replace("\n", string.Empty);
            newS = newS.Replace("\t", string.Empty);
            if (dollar)
                newS = newS.Replace("$", string.Empty);

            return newS;
        }
    }
}
