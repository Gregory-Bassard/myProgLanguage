using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace myProgLanguage
{
    internal class LED
    {
        public int Id { get; set; }
        public Ellipse ellipse { get; set; }
        public bool isOn { get; set; } = false;
        public void Update()
        {
            if (isOn)
                ellipse.Fill = Brushes.Green;
            else
                ellipse.Fill = Brushes.Red;
        }
    }
}
