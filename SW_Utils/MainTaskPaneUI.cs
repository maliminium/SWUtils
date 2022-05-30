using AngelSix.SolidDna;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace SW_Utils
{
    [ProgId(UniqueProgId)]
    public partial class MainTaskPaneUI : UserControl, ITaskpaneControl
    {
        public string ProgId => UniqueProgId;
        public const string UniqueProgId = "MainTaskPaneUICOMUniqueProgID";

        public MainTaskPaneUI()
        {
            InitializeComponent();
        }
    }
}
