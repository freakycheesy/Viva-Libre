using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Viva_Libre
{
    public class Page : Element
    {
        public ModderPlayer player;
        public Element[] elements;
        public Page previousPage;
        public Page(string name, ModderPlayer player, Element[] elements)
        {
            this.name = name;
            this.player = player;
            this.elements = elements;
        }
        public override void Execute()
        {
            previousPage = player.currentPage;
            player.currentPage = this;
        }
    }
}
