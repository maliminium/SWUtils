using Dna;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for EditorFolder.xaml
    /// </summary>
    public partial class EditorDirectory : PropertyEditor
    {
        public const int CHAR_LIMIT = 260;

        public static Brush BR_EXISTING_DIR = Brushes.White;
        public static Brush BR_NONEXISTING_DIR = Brushes.PaleTurquoise;

        private StringChecker DirectoryChecker = new StringChecker() { IsSpaceAllowed = true };

        public string Value
        {
            get => txtValue.Text;
            set
            {
                var cleanDir = DirectoryChecker.CleanDirectoryString(value);
                txtValue.Text = cleanDir.IsNullOrEmpty() ? oldValue : cleanDir;
                if (txtValue.Text != oldValue)
                {
                    IsWarning = !Directory.Exists(value);
                    RaiseControlChanged(this);
                }
            }
        }

        private string oldValue = string.Empty;

        public bool IsWarning
        {
            get { return txtValue.Background == BR_NONEXISTING_DIR ? true : false; }
            set
            {
                txtValue.Background = value ? BR_NONEXISTING_DIR : BR_EXISTING_DIR;
                txtValue.ToolTip = value ? "Directory does not exist" : null;
            }
        }

        public EditorDirectory()
        {
            InitializeComponent();
            Value = "";
        }

        private void txtValue_GotFocus(object sender, RoutedEventArgs e)
        {
            oldValue = Value;
        }

        private void txtValue_LostFocus(object sender, RoutedEventArgs e)
        {
            Value = txtValue.Text;
            //RefreshDir();
        }

        private void txtValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Value = txtValue.Text;
            //RefreshDir();
        }

        private void btnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new System.Windows.Forms.FolderBrowserDialog() {
                ShowNewFolderButton = true,
                SelectedPath=Value
            };

            var result = fbd.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
                Value = fbd.SelectedPath;
        }
    }
}
