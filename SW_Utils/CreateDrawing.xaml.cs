using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using System.IO;
using System.Windows;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CreateDrawing.xaml
    /// </summary>
    public partial class CreateDrawing : DocumentAction
    {
        public override bool IsActive
        {
            get => (cbAction.IsChecked == true);
            set
            {
                if (cbAction.IsChecked != value)
                    cbAction.IsChecked = value;
            }
        }

        public override bool IsFile { get => true; }
        public override string DocDir { get => edDir.Value; set => edDir.Value = value; }
        public override string DocNo { get => edName.NumberText; set => edName.NumberText = value; }
        public override string DocText { get => edName.Value; set => edName.Value = value; }
        public override string DocExt { get => edName.ExtensionText; set => edName.ExtensionText = value; }

        public CreateDrawing()
        {
            SetTheControls();
        }

        public CreateDrawing(string dir)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = CommonResources.GetNextChildPrefix(DocDir, IsFile, DocExt);
        }

        public CreateDrawing(string dir, string solidName)
        {
            SetTheControls();
            DocDir = dir;
            var sepIdx = solidName.IndexOf(AddInSettings.SEP_TEXT);
            if(sepIdx>-1)
            {
                DocNo = solidName.Substring(0, sepIdx);
                DocText = solidName.Remove(0, sepIdx);
            }
            else
            {
                DocNo = solidName;
            }
        }

        //public CreateDrawing(string name, string dir, string description)
        //{
        //    SetTheControls();
        //    edName.Value = name;
        //    edDir.Value = dir;
        //    edDescription.Value = description;
        //}

        //public CreateDrawing(string name, string description, DocumentAction dirSource)
        //{
        //    SetTheControls();
        //    edName.Value = name;
        //    edDir.Value = dirSource.PointingPath;
        //    edDescription.Value = description;
        //    dirSource.ControlChanged += DirSource_ControlChanged;
        //}

        private void DirSource_ControlChanged(object UIcontrol)
        {
            var dirSource = UIcontrol as DocumentAction;
            edDir.Value = dirSource.GetPointingPath();
            IsActive = dirSource.IsActive;
        }

        public CreateDrawing(Model modelFile)
        {
            SetTheControls();
            //TODO: Renumeration'a gore duzenlenecek
            edName.Value = CommonResources.GetCopyFileName(Path.GetFileNameWithoutExtension(modelFile.FilePath));
            edDir.Value = Path.GetDirectoryName(modelFile.FilePath);
            edTemplate.Value = modelFile.FilePath;
            edDescription.Value = modelFile.GetCustomProperty(UI_Properties.CP_Description, modelFile.ActiveConfiguration.UnsafeObject.Name);
        }

        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                var model = new Model((ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.NewDocument(edTemplate.Value, 0, 0, 0));
                //TODO: Drawing icin default prop yazinca burada olustur
                var docPath = GetFullPath();
                model.SaveAs(docPath, SaveAsVersion.CurrentVersion, SaveAsOptions.SaveReferenced);

                AssignedModel = model.UnsafeObject;

                Logger.Log(docPath + " is saved\r", MessageTypeEnum.Greeting);

            }
        }

        //public override string FullPath => edDir.Value + Path.DirectorySeparatorChar + edName.Value + edName.ExtensionText;
        //public override string PointingPath => edDir.Value;

        private void SetTheControls()
        {
            InitializeComponent();
            cbAction.Checked += CbAction_Changed;
            cbAction.Unchecked += CbAction_Changed;
        }

        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;
            RaiseControlChanged(this);
        }

        private void editor_ControlChanged(object UIcontrol)
        {
            RaiseControlChanged(this);
        }

        private void editor_PathChanged(object UIcontrol)
        {
            edName.IsWarning = File.Exists(GetFullPath());
            RaiseControlChanged(this);
        }
    }
}
