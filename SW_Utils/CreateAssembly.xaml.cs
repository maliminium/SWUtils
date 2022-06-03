using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CreateAssembly.xaml
    /// </summary>
    public partial class CreateAssembly : DocumentAction
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

        private List<DocumentAction> ChildActions = new List<DocumentAction>();

        public CreateAssembly() => SetTheControls();

        /// <summary>
        /// C'tor to be used in Assem into Assem
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="material"></param>
        /// <param name="parentPrefix"></param>
        /// <param name="docExt"></param>
        /// <param name="hasSTLConjugate"></param>
        public CreateAssembly(string dir, Material material, string parentPrefix, string docExt, bool hasSTLConjugate = false)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = CommonResources.GetNextChildPrefix(DocDir, IsFile, docExt, parentPrefix);
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreateAssembly(string dir, Material material, bool hasSTLConjugate = false)
        {
            SetTheControls();
            DocDir = dir;
            DocNo = CommonResources.GetNextChildPrefix(DocDir, IsFile, DocExt);
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreateAssembly(Material material, bool hasSTLConjugate = false)
        {
            SetTheControls();
            edMaterial.Value = material;
            HasSTLConjugate = hasSTLConjugate;
        }

        public CreateAssembly(Model modelFile)
        {
            SetTheControls();

            //TODO: Renumeration'a gore duzenlenecek

            edName.Value = CommonResources.GetCopyFileName(Path.GetFileNameWithoutExtension(modelFile.FilePath));
            edDir.Value = Path.GetDirectoryName(modelFile.FilePath);
            edTemplate.Value = modelFile.FilePath;
            var materialText = modelFile.GetCustomProperty(UI_Properties.CP_DftMaterial, modelFile.ActiveConfiguration.UnsafeObject.Name);
            edMaterial.Value = CommonResources.GetMaterialFromText(materialText);
        }
                

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
                var model = new Model((ModelDoc2)SolidWorksEnvironment.Application.UnsafeObject.NewDocument(edTemplate.Value, 0, 0, 0));
                UI_Properties.RecreateDefaultProps(model);
                model.SetCustomProperty(UI_Properties.CP_STLConjugate, HasSTLConjugate ? UI_Properties.STR_True : UI_Properties.STR_False);
                UI_Properties.SetCustomPropForAllConf(model, UI_Properties.CP_DftMaterial, edMaterial.Value.ToString());
                var docPath = GetFullPath();
                var res = model.SaveAs(docPath, SaveAsVersion.CurrentVersion, SaveAsOptions.SaveReferenced);

                AssignedModel = model.UnsafeObject;

                Logger.Log(docPath + " is saved " + res.ToString() + "\r", MessageTypeEnum.Greeting);

                //SolidWorksEnvironment.Application.UnsafeObject.OpenDoc7()

            }
        }
        private void DirSource_ControlChanged(object UIcontrol)
        {
            var dirSource = UIcontrol as DocumentAction;
            edDir.Value = dirSource.GetFullPath();
            IsActive = dirSource.IsActive;
        }

        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;
            RaiseControlChanged(this);
        }
        private void editor_ControlChanged(object UIcontrol) => RaiseControlChanged(this);
        private void editor_PathChanged(object UIcontrol)
        {
            edName.IsWarning = File.Exists(GetFullPath());
            RaiseControlChanged(this);
        }
    }
}
