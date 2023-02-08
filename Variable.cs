using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace myProgLanguage
{
    internal class Variable
    {
        public int id { get; set; }
        public string name {get; set;}
        public string type { get; set; }
        public string val { get; set; }

        public Variable(int id, string name, string val, string type = "STRING")
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.val = val;
        }
        public Variable(int id, string name, int val, string type = "INT")
        {
            this.id = id;
            this.name = name;
            this.type = type;
            this.val = val.ToString();
        }
        public static Variable SearchVar(List<Variable> vars ,string varName)
        {
            Variable var = null;

            vars.ForEach(v =>
            {
                if (v.name == varName)
                {
                    var = v;
                }
            });
            return var;
        }
    }
}
