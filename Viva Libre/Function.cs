using MelonLoader;

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
            try
            {
                command?.Invoke();
            }
            catch (Exception ex)
            {
                MelonLogger.Error(ex);
            }
        }
    }
}
