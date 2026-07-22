using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva_Libre
{
    public class Function : Element
    {
        public Action command;
        public Function(string name, Action command)
        {
            this.name = name;
            this.command = command;
        }
        public override void Execute()
        {
            command?.Invoke();
        }
    }
}
