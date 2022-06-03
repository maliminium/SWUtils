﻿using SolidWorks.Interop.sldworks;
using System.Windows;
using System.IO;
using AngelSix.SolidDna;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for InsertSolidIntoAssembly.xaml
    /// </summary>
    public partial class InsertSolidIntoAssembly : DocumentAction
    {
        public override bool IsFile { get => false; }
        public override string DocDir { get => string.Empty; set { } }
        public override string DocNo { get => string.Empty; set { } }
        public override string DocText { get => string.Empty; set { } }
        public override string DocExt { get => string.Empty; set { } }
        public override bool IsActive 
        { 
            get => (cbAction.IsChecked == true); 
            set 
            { 
                if (cbAction.IsChecked != value) 
                    cbAction.IsChecked = value; 
            } 
        }

        Model SolidModel = null;
        //Model AssemModel = null;
        //Cunku Model saklayinca Execute noktasinda UnsafeObject=null oluyor
        ModelDoc2 AssemModel = null;
        DocumentAction ControlSolid = null;
        CreateAssembly ControlAssem = null;

        public InsertSolidIntoAssembly() => SetTheControls();
        public InsertSolidIntoAssembly(DocumentAction solidControl, CreateAssembly assemblyControl)
        {
            SetTheControls();

            ControlSolid = solidControl;
            ControlAssem = assemblyControl;

            solidControl.ControlChanged += SolidControl_ControlChanged;
            assemblyControl.ControlChanged += AssemblyControl_ControlChanged;

            RenewSolidPath(solidControl.GetFullPath());
            RenewAssemblyPath(assemblyControl.GetFullPath());
        }
        public InsertSolidIntoAssembly(Model solidModel, CreateAssembly assemblyControl)
        {
            SetTheControls();

            SolidModel = solidModel;
            ControlAssem = assemblyControl;

            solidModel.ModelSaved += SolidModel_ModelSaved;
            assemblyControl.ControlChanged += AssemblyControl_ControlChanged;

            RenewSolidPath(solidModel.FilePath);
            RenewAssemblyPath(assemblyControl.GetFullPath());
        }
        public InsertSolidIntoAssembly(DocumentAction solidControl, Model assemblyModel)
        {
            SetTheControls();

            ControlSolid = solidControl;
            //AssemModel = assemblyModel;
            AssemModel = assemblyModel.UnsafeObject;

            solidControl.ControlChanged += SolidControl_ControlChanged;
            assemblyModel.ModelSaved += AssemblyModel_ModelSaved;

            RenewSolidPath(solidControl.GetFullPath());
            //RenewAssemblyPath(AssemModel.FilePath);
            RenewAssemblyPath(AssemModel.GetPathName());
        }
        private void SetTheControls()
        {
            InitializeComponent();
            cbAction.Checked += CbAction_Changed;
            cbAction.Unchecked += CbAction_Changed;
        }
        private void RenewSolidPath(string fullPath)
        {
            txtSolid.Text = Path.GetFileName(fullPath);
            txtSolid.ToolTip = fullPath;
        }
        private void RenewAssemblyPath(string fullPath)
        {
            txtAssembly.Text = Path.GetFileName(fullPath);
            txtAssembly.ToolTip = fullPath;
        }
        private void CheckForEnability()
        {
            var solidState = ControlSolid != null ? ControlSolid.IsActive : (SolidModel != null);
            var assemState = ControlAssem != null ? ControlAssem.IsActive : (AssemModel != null);

            IsActive = (solidState && assemState);

              
            //if enabling is meaningless, deactivate control
            cbAction.IsEnabled = IsActive;
        }
        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                //var assemDoc = ControlAssem == null ? (AssemblyDoc)AssemModel.UnsafeObject : (AssemblyDoc)ControlAssem.GetModel();

                //var assemDoc = ControlAssem == null ? (AssemblyDoc)AssemModel : (AssemblyDoc)ControlAssem.GetModel();

                var modelDoc = ControlAssem == null ? AssemModel : ControlAssem.GetModel();
                var assemDoc = (AssemblyDoc)modelDoc;

                var solidPath = txtSolid.ToolTip as string;

                if (assemDoc != null && File.Exists(solidPath))
                {
                    string[] compNames = new string[1];
                    string[] compCoordSysNames = new string[1];
                    compNames[0] = solidPath;
                    var firstComponent = assemDoc.GetComponentCount(true) > 0 ? false : true;
                    assemDoc.AddComponents3(compNames, null, compCoordSysNames);
                    if (firstComponent)
                    {
                        //AssemblyDoc swAssy = (AssemblyDoc)AssemModel;
                        //ModelDocExtension swModelDocExt = modelDoc.Extension;
                        //SelectionMgr swSelMgr = (SelectionMgr)modelDoc.SelectionManager;

                        //swModelDocExt.SelectAll();
                        modelDoc.Extension.SelectAll();
                        assemDoc.FixComponent();

                        //string compName = Path.GetFileNameWithoutExtension(solidPath) + "-1@" + Path.GetFileNameWithoutExtension(modelDoc.GetPathName());

                        // Fix the selected component
                        //bool status = swModelDocExt.SelectByID2("Pad_1-1@key pad_1", "COMPONENT", 0, 0, 0, false, 0, null, 0);
                        //swAssy.FixComponent();



                        //ModelDoc2 swModel;
                        //AssemblyDoc swAssy;
                        //ModelDocExtension swModelDocExt;
                        //Component2 swComp;
                        //SelectionMgr swSelMgr;
                        //string fileName;
                        //int errors = 0;
                        //int warnings = 0;
                        //bool status;

                        //fileName = "C:\\Users\\Public\\Documents\\SOLIDWORKS\\SOLIDWORKS 2018\\samples\\tutorial\\api\\key pad_1.sldasm";
                        //swModel = (ModelDoc2)swApp.OpenDoc6(fileName, (int)swDocumentTypes_e.swDocASSEMBLY, (int)swOpenDocOptions_e.swOpenDocOptions_Silent, "", ref errors, ref warnings);
                        //swAssy = (AssemblyDoc)swModel;
                        //swModelDocExt = (ModelDocExtension)swModel.Extension;
                        //swSelMgr = (SelectionMgr)swModel.SelectionManager;

                        //// Fix the selected component
                        //status = swModelDocExt.SelectByID2("Pad_1-1@key pad_1", "COMPONENT", 0, 0, 0, false, 0, null, 0);
                        //swAssy.FixComponent();
                        //status = swModelDocExt.SelectByID2("Pad_1-1@key pad_1", "COMPONENT", 0, 0, 0, false, 0, null, 0);
                        //swComp = swSelMgr.GetSelectedObjectsComponent3(1, -1);
                        //Debug.Print("Selected component fixed? " + swComp.IsFixed());




                        //assemDoc.FixComponent();
                    }
                }
                else
                {
                    var msg = assemDoc == null ? "Assembly is null, " : string.Empty;
                    msg += "Solid Path: " + solidPath;

                    Logger.Log("Problem in InsertSolidIntoAssembly: " + msg, MessageTypeEnum.Error);
                } 
            }
        }
        private void AssemblyModel_ModelSaved()
        {
            //RenewAssemblyPath(AssemModel.FilePath);
            RenewAssemblyPath(AssemModel.GetPathName());
        }
        private void SolidModel_ModelSaved() => RenewSolidPath(SolidModel.FilePath);
        private void AssemblyControl_ControlChanged(object UIcontrol)
        {
            RenewAssemblyPath((UIcontrol as CreateAssembly).GetFullPath());
            CheckForEnability(); 
        }
        private void SolidControl_ControlChanged(object UIcontrol)
        {
            RenewSolidPath((UIcontrol as DocumentAction).GetFullPath());
            CheckForEnability();
        }
        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;

            RaiseControlChanged(this);
        }
    }
}
