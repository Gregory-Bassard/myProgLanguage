using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myProgLanguage
{
    internal class Variable
    {
        public string name {get; set;}
        public string type { get; set; }
        public string val { get; set; }

        public Variable(string name, string val, string type = "STRING")
        {
            this.name = name;
            this.type = type;
            this.val = val;
        }
        public Variable(string name, int val, string type = "INT")
        {
            this.name = name;
            this.type = type;
            this.val = val.ToString();
        }
    }
}
