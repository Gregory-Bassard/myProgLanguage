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
        string[] lines = new string[25];
        List<LED> leds;
        List<Variable> variables;
        List<Tuple<string, int>> labels = new List<Tuple<string, int>>();
        private DispatcherTimer _timer;
        bool endText = true;
        int indexLine = 0;
        DebugWindow debugWindow = new DebugWindow();
        bool debugMode = false;

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

            tbTextCode.Text = "#Petit Code 1\nLABEL @Start\nLET $POS 0\nLABEL @LOOP\nSET $POS\nWAIT 300\nRESET $POS\nINC $POS 1\nIF $POS EQ 9 @START\nGOTO @LOOP";

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Run_Click(object sender, RoutedEventArgs e)
        {
            Restart();
            //Lire le code
            text = tbTextCode.Text;
            lines = text.Split("\n");
            endText = false;
        }
        private void Debug_Click(object sender, RoutedEventArgs e)
        {
            debugMode = true;
            Restart();
            text = tbTextCode.Text;
            lines = text.Split("\n");

            DebugMode();
        }
        private void btnStop_Click(object sender, RoutedEventArgs e)
        {
            debugWindow.Close();
            debugWindow = new DebugWindow();
            Restart();
            debugMode = false;
        }
        private void btnNext_Click(object sender, RoutedEventArgs e)
        {
            while (lines[indexLine].Split(" ")[0].StartsWith("#"))
                indexLine++;

            foreach (Label lab in spListLabels.Children)
            {
                if (lab.Name == $"lb{indexLine}")
                    lab.Background = Brushes.Beige;
                else
                    lab.Background = Brushes.Transparent;
            }
            ReadLine();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!debugMode)
            {
                if (!endText)
                {
                    string[] words = lines[indexLine].Split(" ");
                    if (words[0].StartsWith("=>") && !debugMode)
                    {
                        lines[indexLine].Remove(2);
                        debugMode = true;
                        DebugMode();
                    }
                    ReadLine();
                }
                else if (indexLine >= lines.Length - 1)
                    indexLine = 0;
            }
            else
            {
                List<string> items = new List<string>();
                foreach (Variable var in variables)
                {
                    string item = $"{var.name} : {var.val}";
                    items.Add(item);
                }
                debugWindow.lbVariables.ItemsSource = items;
            }
        }
        public void DebugMode()
        {
            btnRun.Visibility = Visibility.Hidden;
            btnDebug.Visibility = Visibility.Hidden;
            btnStop.Visibility = Visibility.Visible;
            btnNext.Visibility = Visibility.Visible;

            spTextBox.Visibility = Visibility.Hidden;
            spListLabels.Visibility = Visibility.Visible;

            //Label x:Name = "..." Content = "..." HorizontalAlignment = "Center" VerticalAlignment = "Top" Width = "450" Height = "Auto"
            int count = 0;

            foreach (var line in lines)
            {
                Label label = new Label();
                label.HorizontalAlignment = HorizontalAlignment.Center;
                label.VerticalAlignment = VerticalAlignment.Top;
                label.VerticalContentAlignment = VerticalAlignment.Center;
                label.Width = 450;
                label.Height = 25;

                label.Name = $"lb{count}";
                label.Content = line;
                spListLabels.Children.Add(label);
                count++;
            }

            while (lines[indexLine].Split(" ")[0].StartsWith("#"))
                indexLine++;

            foreach (Label lab in spListLabels.Children)
            {
                if (lab.Name == $"lb{indexLine}")
                {
                    lab.Background = Brushes.Beige;
                }
            }
            debugWindow.Show();
        }
        public void ReadLine()
        {
            string[] words = lines[indexLine].Split(" ");
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
                case "LABEL":
                    LABEL(words, indexLine);
                    break;
                case "GOTO":
                    GOTO(words);
                    break;
                case "IF":
                    IF(words);
                    break;
            }
            if (indexLine >= lines.Length -1)
                endText = true;
            else
                indexLine++;
        }
        public void Restart()
        {
            indexLine = 0;

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
            
            btnRun.Visibility = Visibility.Visible;
            btnDebug.Visibility = Visibility.Visible;
            btnStop.Visibility = Visibility.Hidden;
            btnNext.Visibility = Visibility.Hidden;

            spTextBox.Visibility = Visibility.Visible;
            spListLabels.Visibility = Visibility.Hidden;

            spListLabels.Children.Clear();
        }
        
        public void SET(string[] words)
        {
            int param_x = -1;

            //vérifier si words[1] est une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_r(words[1], true));
                int.TryParse(var.val, out param_x);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_r(words[1]), out param_x);

            //TODO : Géstion des erreurs
            leds[param_x].On();
        }
        public void RESET(string[] words)
        {
            int param_x = 0;

            //vérifier si words[1] n'est pas une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_r(words[1], true));
                int.TryParse(var.val, out param_x);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_r(words[1]), out param_x);

            //TODO : Géstion des erreurs
            leds[param_x].Off();
        }
        public void WAIT(string[] words)
        {
            int param_xxx = 0;

            //vérifier si words[1] n'est pas une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_r(words[1], true));
                int.TryParse(var.val, out param_xxx);
            }
            //vérifier si words[1] peut etre convertie en int
            else
                int.TryParse(RemoveBackslash_t_r(words[1]), out param_xxx);

            //TODO : Géstion des erreurs
            Thread.Sleep(param_xxx);
        }
        public void LET(string[] words) //TODO: gérer l'ajout d'une variable qui éxiste déja
        {
            int param_val = 0;
            Variable newVar = null;
            
            //vérifier si words[1] est bien une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                //vérifier si words[2] n'est pas une variable ($___)
                if (words[2].Count() > 1 && words[2][0] == '$')
                {
                    Variable var = Variable.SearchVar(variables, RemoveBackslash_t_r(words[2], true));
                    int.TryParse(var.val, out param_val);
                }
                else
                    int.TryParse(RemoveBackslash_t_r(words[2]), out param_val);
                newVar = new Variable(variables.Count(), RemoveBackslash_t_r(words[1], true), param_val);
                variables.Add(newVar);
            }
        }
        public void INC(string[] words)
        {
            int param_val = 0;

            //vérifier si words[1] est bien une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                int varId = Variable.SearchVar(variables, RemoveBackslash_t_r(words[1], true)).id;

                //vérifier si words[2] peut etre convertie en int
                if (int.TryParse(RemoveBackslash_t_r(words[2]), out param_val))
                    variables[varId].val = (int.Parse(variables[varId].val) + param_val).ToString();
            }
        }
        public void LABEL(string[] words, int ligneNum)
        {
            if (words[1].Count() > 1 && words[1][0] == '@')
                labels.Add(new Tuple<string, int>(RemoveBackslash_t_r(words[1], at : true), ligneNum));
        }
        public void GOTO(string[] words)
        {
            if (words[1].Count() > 1 && words[1][0] == '@')
            {
                labels.ForEach(lab => {
                    if (lab.Item1 == RemoveBackslash_t_r(words[1], at:true))
                        indexLine = lab.Item2;
                });
            }
        }
        public void IF(string[] words)
        {
            string cond = "";
            int comp = -1;
            int labelIndexLigne = -1;
            //vérifier si words[1] est bien une variable ($___)
            if (words[1].Count() > 1 && words[1][0] == '$')
            {
                //Chercher la variable
                Variable var = Variable.SearchVar(variables, RemoveBackslash_t_r(words[1], true));

                //Récupérer le comparateur
                cond = RemoveBackslash_t_r(words[2]);

                //vérifier si words[3] est peut etre une variable ($___)
                if (words[3].Count() > 1 && words[3][0] == '$')
                    comp = int.Parse(Variable.SearchVar(variables, RemoveBackslash_t_r(words[3], dollar: true)).val);
                else//sinon : vérifier si words[3] peut etre convertie en int
                {
                    if (!int.TryParse(RemoveBackslash_t_r(words[3]), out comp))
                        throw new InvalidOperationException("le comparateur doit être un INT");
                }

                //vérifier si words[4] est bien un label (@___)
                if (words[1].Count() > 1 && words[1][0] == '@')
                {
                    labels.ForEach(lab =>
                    {
                        if (lab.Item1 == RemoveBackslash_t_r(words[3], at: true))
                            labelIndexLigne = lab.Item2;
                    });
                }
                switch (cond){
                    case "EQ":
                        if (int.Parse(var.val) == comp)
                            indexLine = labelIndexLigne;
                        break;
                    case "NEQ":
                        if (int.Parse(var.val) != comp)
                            indexLine = labelIndexLigne;
                        break;
                    case "LT":
                        if (int.Parse(var.val) < comp)
                            indexLine = labelIndexLigne;
                        break;
                    case "GT":
                        if (int.Parse(var.val) > comp)
                            indexLine = labelIndexLigne;
                        break;
                    default: throw new InvalidOperationException("la condition doit être EQ/NEQ/LT/GT");
                }
            }
        }
        public string RemoveBackslash_t_r(string s, bool dollar = false, bool at = false)
        {
            string newS = s.Replace("\r", string.Empty);
            newS = newS.Replace("\t", string.Empty);
            if (dollar)
                newS = newS.Replace("$", string.Empty);
            if (at)
                newS = newS.Replace("@", string.Empty);

            return newS;
        }
    }
}