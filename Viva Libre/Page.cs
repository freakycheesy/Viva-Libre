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
        public Action onOpen;
        public Page previousPage;
        public Page(string name, ModderPlayer player, Element[] elements, Action onOpen = null)
        {
            this.name = name;
            this.player = player;
            this.elements = elements;
            this.onOpen = onOpen;
        }
        public override void Execute()
        {
            if (player != null) {
                previousPage = player.currentPage;
                player.currentPage = this;
            }
            onOpen?.Invoke();
        }
    }
}
