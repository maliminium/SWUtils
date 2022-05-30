using System.IO;
using System.Windows;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CreateFolder.xaml
    /// </summary>
    public partial class CreateFolder : DocumentAction
    {
        //public override string FullPath => edDir.Value + Path.DirectorySeparatorChar + edName.Value;
        //public override string PointingPath => GetFullPath();
        public override bool IsActive
        {
            get => (cbAction.IsChecked == true);
            set
            {
                if (cbAction.IsChecked != value)
                    cbAction.IsChecked = value;
            }
        }

        public override bool IsFile { get => false; }
        public override string DocDir { get => edDir.Value; set => edDir.Value = value; }
        public override string DocNo { get => edName.NumberText; set => edName.NumberText = value; }
        public override string DocText { get => edName.Value; set => edName.Value = value; }
        public override string DocExt { get => edName.ExtensionText; set => edName.ExtensionText = value; }

        public CreateFolder() => SetTheControls();

        public CreateFolder(string dir)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = CommonResources.GetNextChildPrefix(DocDir, IsFile, DocExt);
        }
        ////Project
        ////TODO: Folder c'tor + Versioning file
        //public CreateFolder(string name, string dir)
        //{
        //    SetTheControls();
        //    edName.Value = name;
        //    edDir.Value = dir;
        //}
        //public CreateFolder(string name, DocumentAction dirSource)
        //{
        //    SetTheControls();
        //    edName.Value = name;
        //    edDir.Value = dirSource.GetPointingPath();
        //    dirSource.ControlChanged += DirSource_ControlChanged;
        //}
        private void SetTheControls()
        {
            InitializeComponent();
            cbAction.Checked += CbAction_Changed;
            cbAction.Unchecked += CbAction_Changed;
        }
        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                try
                {
                    //var path = edDir.Value + AddInSettings.SEP_FOLDER + edName.Value;
                    var path = GetFullPath();
                    if (!Directory.Exists(path))
                        Directory.CreateDirectory(path);
                }
                catch (System.Exception ex)
                {
                    Logger.LogException(ex);
                }
                
            }
        }
        private void DirSource_ControlChanged(object UIcontrol)
        {
            var dirSource = UIcontrol as DocumentAction;
            edDir.Value = dirSource.GetPointingPath();
            IsActive = dirSource.IsActive;
        }

        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;
            RaiseControlChanged(this);
        }

        private void editor_ControlChanged(object UIcontrol) => RaiseControlChanged(this);
    }
}
