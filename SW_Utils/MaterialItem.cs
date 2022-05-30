using AngelSix.SolidDna;
using System.Windows.Controls;

namespace SW_Utils
{
    public class MaterialItem : TreeViewItem
    {
        private Material _AssignedMaterial;
        public Material AssignedMaterial
        {
            get => _AssignedMaterial;
            set
            {
                _AssignedMaterial = value;
                Header = value.DisplayName;
            }
        }

        public MaterialItem()
        {
            MouseDoubleClick += MaterialItem_MouseDoubleClick;
        }

        private void MaterialItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            MoveFocus(new System.Windows.Input.TraversalRequest(System.Windows.Input.FocusNavigationDirection.Next));
        }
    }
}
