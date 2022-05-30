using System.Windows.Controls;

namespace SW_Utils
{
    public abstract class PropertyEditor : UserControl
    {
        public event CommonResources.UIControlChanged ControlChanged;
        public void RaiseControlChanged(object sender) => ControlChanged?.Invoke(sender);
    }
}
