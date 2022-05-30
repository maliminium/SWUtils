using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CheckListItem.xaml
    /// </summary>
    public partial class CheckListItem : UserControl
    {
        public const string STR_CHECKED = "C";
        public const string STR_UNCHECKED = "U";

        public static Brush BR_ACTIVE = Brushes.White;
        public static Brush BR_DEACTIVE = Brushes.Transparent;

        private const double W_BTN = 20;

        private string OldContent = string.Empty;

        public event EventHandler DeleteRequested;
        private void RaiseDeleteRequested() => DeleteRequested?.Invoke(this, new EventArgs());

        public event EventHandler ValueChanged;
        private void RaiseValueChanged() => ValueChanged?.Invoke(this, new EventArgs());


        public string ItemText
        {
            get => txtItem.Text;
            set => txtItem.Text = value;
        }

        public bool IsChecked
        {
            get => (bool)cbItem.IsChecked;
            set => cbItem.IsChecked = value;
        }

        public CheckListItem()
        {
            InitializeComponent();
            LeaveItem();
        }

        public CheckListItem(string itemContent, bool isChecked)
        {
            InitializeComponent();

            txtItem.Text = itemContent;
            IsChecked = isChecked;

            LeaveItem();
        }

        public string GetItemString()
        {
            var checkString = cbItem.IsChecked == true ? STR_CHECKED : STR_UNCHECKED;
            return checkString + txtItem.Text;
        }

        public void EditItem()
        {
            OldContent = txtItem.Text;
            txtItem.IsReadOnly = false;
            txtItem.Background = BR_ACTIVE;

            btnDelete.Width = 0;
            btnSave.Width = W_BTN;
            btnCancel.Width = W_BTN;

            txtItem.Focus();
        }

        public void CancelChanges()
        {
            txtItem.Text = OldContent;
            SelectItem();
        }

        public void SelectItem()
        {
            LeaveItem();
            Focus();
        }

        public void LeaveItem()
        {
            txtItem.IsReadOnly = true;
            txtItem.Background = BR_DEACTIVE;

            btnDelete.Width = W_BTN;
            btnSave.Width = 0;
            btnCancel.Width = 0;

            if (txtItem.Text == string.Empty && OldContent != string.Empty)
            {
                RaiseDeleteRequested();
            }
            else if (OldContent != txtItem.Text)
            {
                RaiseValueChanged();
            }
        }

        private void cbItem_Checked(object sender, RoutedEventArgs e) => RaiseValueChanged();

        private void cbItem_Unchecked(object sender, RoutedEventArgs e) => RaiseValueChanged();


        private void btnDelete_Click(object sender, RoutedEventArgs e) => RaiseDeleteRequested();

        private void btnSave_Click(object sender, RoutedEventArgs e) => SelectItem();

        private void btnCancel_Click(object sender, RoutedEventArgs e) => CancelChanges();

        private void txtItem_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e) => EditItem();

        private void txtItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SelectItem();
            }
            else if (e.Key == Key.Escape)
            {
                CancelChanges();
            }
        }

        private void UserControl_LostFocus(object sender, RoutedEventArgs e) => LeaveItem();
    }
}
