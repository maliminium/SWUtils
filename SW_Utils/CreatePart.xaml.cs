using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using System.IO;
using System.Windows;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CreatePart.xaml
    /// </summary>
    public partial class CreatePart : DocumentAction
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
        public bool HasSTLConjugate { get => cbSTL.IsChecked == true; set => cbSTL.IsChecked = value; }

        public CreatePart()
        {
            SetTheControls();
        }

        public CreatePart(string dir, Material material, bool hasSTLConjugate = false)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = CommonResources.GetNextChildPrefix(DocDir, IsFile, DocExt);
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreatePart(string dir, string no, Material material, bool hasSTLConjugate = false)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = no;
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreatePart(Material material, bool hasSTLConjugate = false)
        {
            SetTheControls();
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreatePart(Model modelFile)
        {
            //TODO: Renumeration'a gore duzenlenecek
            SetTheControls();
            edName.Value = CommonResources.GetCopyFileName(Path.GetFileNameWithoutExtension(modelFile.FilePath));
            edDir.Value = Path.GetDirectoryName(modelFile.FilePath);
            edTemplate.Value = modelFile.FilePath;
            edMaterial.Value = modelFile.GetMaterial();
        }

        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                var model = new Model((ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.NewDocument(edTemplate.Value, 0, 0, 0));
                UI_Properties.RecreateDefaultProps(model);
                model.SetCustomProperty(UI_Properties.CP_STLConjugate, HasSTLConjugate ? UI_Properties.STR_True : UI_Properties.STR_False);
                model.SetMaterial(edMaterial.Value);
                var docPath = GetFullPath();
                model.SaveAs(docPath, SaveAsVersion.CurrentVersion, SaveAsOptions.SaveReferenced);

                AssignedModel = model.UnsafeObject;

                Logger.Log(docPath + " is saved\r", MessageTypeEnum.Greeting);
            }
        }

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

        private void editor_ControlChanged(object UIcontrol) => RaiseControlChanged(this);

        private void editor_PathChanged(object UIcontrol)
        {
            var path = GetFullPath();
            edName.IsWarning = File.Exists(path);
            RaiseControlChanged(this);
        }
    }
}
