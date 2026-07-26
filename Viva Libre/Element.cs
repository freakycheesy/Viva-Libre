namespace Viva_Libre
{
    public abstract class Element
    {
        public string name;
        public abstract void Execute();
        public Action onPreGUI;
        public Action onPostGUI;
    }
}
