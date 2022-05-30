using Dna;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for EditorName.xaml
    /// </summary>
    public partial class EditorText : PropertyEditor
    {
        public const int DFT_CHAR_LIMIT = 40;

        public static Brush BR_NORMAL = Brushes.White;
        public static Brush BR_WARNING = Brushes.PaleVioletRed;

        private StringChecker TextChecker = new StringChecker();

        public const double COL_W_EXT = 45;
        public const double COL_W_CHAR = 5;

        public bool IsTextValidationActive { get; set; } = true;
        public int CharLimit { get => TextChecker.CharLimit; set => TextChecker.CharLimit = value; }
        public string InvalidChars { get => TextChecker.InvalidChars; set => TextChecker.InvalidChars = value; }
        public string TrimChars { get => TextChecker.TrimChars; set => TextChecker.TrimChars = value; }
        public bool IsSpaceAllowed { get => TextChecker.IsSpaceAllowed; set => TextChecker.IsSpaceAllowed = value; }
        public bool IsTurkishCharsAllowed { get => TextChecker.IsTurkishCharsAllowed; set => TextChecker.IsTurkishCharsAllowed= value; }
        public bool IsSystemName { get => TextChecker.IsSystemName; set => TextChecker.IsSystemName = value; }
        public bool IsEmptyStringAllowed { get => TextChecker.IsEmptyStringAllowed; set => TextChecker.IsEmptyStringAllowed = value; }

        public string SepName { get; set; } = string.Empty;

        public string NumberText
        {
            get => txtNumber.Text;
            set
            {
                if(txtNumber.Text!=value)
                {
                    txtNumber.Text = value;
                    RaiseControlChanged(this);
                }
            }   
        }

        public string ExtensionText
        {
            get => txtExtension.Text;
            set
            {
                if(txtExtension.Text!=value)
                {
                    txtExtension.Text = value;
                    colExt.Width =
                        value.IsNullOrEmpty() ?
                        new GridLength(0) :
                        new GridLength(COL_W_EXT);
                    //RaiseControlChanged(this);    //Exception yaratiyor
                }
            }
        }
        public bool IsWarning 
        {
            get { return txtValue.Background == BR_WARNING ? true : false; }
            set 
            { 
                txtValue.Background = value ?  BR_WARNING : BR_NORMAL;
                txtValue.ToolTip = value ? "File will be overwritten" : null;
            }
        }

        public string Title { get => txtLabel.Text; set => txtLabel.Text = value; }
        public string Value 
        { 
            get => txtValue.Text;
            set
            {
                if (IsTextValidationActive)
                {
                    if (value==string.Empty && IsEmptyStringAllowed)
                    {
                        txtValue.Text = value;
                    }
                    else
                    {
                        var cleanValue = TextChecker.CleanString(value);
                        txtValue.Text = cleanValue.IsNullOrEmpty() ? oldValue : cleanValue;
                    }                    
                }
                else
                {
                    txtValue.Text = value;
                }

                txtValue.CaretIndex = txtValue.Text.Length;

                if (txtValue.Text != oldValue)
                    RaiseControlChanged(this);

                txtSeperator.Text = value.IsNullOrEmpty() ? string.Empty : SepName;
            }
        }

        private string oldValue = string.Empty;

        public EditorText()
        {
            InitializeComponent();
            CharLimit = DFT_CHAR_LIMIT;
            oldValue = Value;
        }

        private void txtValue_GotFocus(object sender, RoutedEventArgs e) => oldValue = Value;

        private void txtValue_LostFocus(object sender, RoutedEventArgs e) => Value = txtValue.Text;

        private void txtValue_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                Value = txtValue.Text;
        }
    }
}
