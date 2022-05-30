using AngelSix.SolidDna;
using Dna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for FileFolderUI.xaml
    /// </summary>
    public partial class UI_FileFolder : UserControl
    {
        #region VARIABLES & C'TOR

        Model model = null; //TODO: SolidWorksEnvironment.Application.ActiveModel?

        public UI_FileFolder()
        {
            InitializeComponent();

            SetModelDependentButtonsEnability(false);
            //SetDirectoryDependentButtonsEnability(false);
            SetDirectoryDependentButtonsEnability(Directory.Exists(deDirectories.SelectedPath));

            //if (SolidWorksEnvironment.Application != null)
            //    SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
            if (SolidWorksEnvironment.Application != null)
            {
                SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
                SolidWorksEnvironment.Application.ActiveFileSaved += Application_ActiveFileSaved;
            }
                
        }

        #endregion

        #region UI & MODEL RELATED METHODS

        private void SetDirectoryDependentButtonsEnability(bool state)
        {
            btnPartNew.IsEnabled = state;
            btnAssemNew.IsEnabled = state;
            btnDrawingNew.IsEnabled = state;
        }

        private void SetModelDependentButtonsEnability(bool state)
        {
            try
            {
                btnAssemFromSolid.IsEnabled = state;
                btnAssemIntoAssem.IsEnabled = state;
                btnAssemClone.IsEnabled = state;
                btnAssemNext.IsEnabled = state;

                btnPartFromPart.IsEnabled = state;
                btnPartIntoSolid.IsEnabled = state;
                btnPartClone.IsEnabled = state;
                btnPartNext.IsEnabled = state;

                btnDrawingFromSolid.IsEnabled = state;
                btnDrawingClone.IsEnabled = state;
                btnDrawingNext.IsEnabled = state;
            }
            catch
            {
            }
        }

        public string GetActiveDirectory() => deDirectories.SelectedPath; //return model == null ? pdDraft.GetSelectedPath() : Path.GetDirectoryName(model.FilePath);

        public string GetActiveModelPrefix() => model == null ? string.Empty : CommonResources.GetPrefixFromName(Path.GetFileName(model.FilePath));

        public Material GetActiveMaterial()
        {
            //get the model's material
            var modelMaterial = model == null ? null : model.GetMaterial();

            //if even the model's material is null return AddInSettings.DFT_MATERIAL
            return modelMaterial == null ? AddInSettings.DFT_MATERIAL : modelMaterial;
        }

        #endregion

        #region EVENT HANDLERS

        private void Application_ActiveModelInformationChanged(Model obj)
        {
            SetModelDependentButtonsEnability(false);
            model = obj;

            //TODO: Tum modeller kapandiginda model null'a cekilmeli. Belki de bu islevler merkezi olmali.

            if (model != null)
            {
                if (!model.FilePath.IsNullOrEmpty())
                    deDirectories.ModelDirectory = Path.GetDirectoryName(model.FilePath);

                switch (model.ModelType)
                {
                    case ModelType.None:
                        break;
                    case ModelType.Part:
                        btnPartClone.IsEnabled = true;
                        btnPartFromPart.IsEnabled = true;
                        btnPartIntoSolid.IsEnabled = true;
                        btnPartNext.IsEnabled = true;
                        btnAssemFromSolid.IsEnabled = true;
                        btnDrawingFromSolid.IsEnabled = true;
                        cbHasSTLConjugate.IsChecked = model.GetCustomProperty(UI_Properties.CP_STLConjugate) == UI_Properties.STR_True ? true : false;
                        break;
                    case ModelType.Assembly:
                        btnAssemClone.IsEnabled = true;
                        btnAssemFromSolid.IsEnabled = true;
                        btnAssemIntoAssem.IsEnabled = true;
                        btnAssemNext.IsEnabled = true;
                        btnPartIntoSolid.IsEnabled = true;
                        btnDrawingFromSolid.IsEnabled = true;
                        cbHasSTLConjugate.IsChecked = model.GetCustomProperty(UI_Properties.CP_STLConjugate) == UI_Properties.STR_True ? true : false;
                        break;
                    case ModelType.Drawing:
                        btnDrawingClone.IsEnabled = true;
                        btnDrawingNext.IsEnabled = true;
                        break;
                    case ModelType.DocumentManager:
                        break;
                    case ModelType.ExternalFile:
                        break;
                    case ModelType.ImportedAssembly:
                        break;
                    case ModelType.ImportedPart:
                        break;
                    default:
                        break;
                }
            }
        }

        private void deDirectories_NewPathRequested(string containerPath) => CreateNewFolder(containerPath);
        private void deDirectories_DirectoryChanged(string containerPath) => SetDirectoryDependentButtonsEnability(Directory.Exists(containerPath));

        private void btnAssemFromSolid_Click(object sender, RoutedEventArgs e) => AssemblyFromSolid();
        private void btnAssemIntoAssem_Click(object sender, RoutedEventArgs e) => AssemblyIntoAssembly();
        private void btnAssemNew_Click(object sender, RoutedEventArgs e) => AssemblyNew();
        private void btnAssemClone_Click(object sender, RoutedEventArgs e) => ModelClone();
        private void btnAssemNext_Click(object sender, RoutedEventArgs e) => ModelNext();

        private void btnPartIntoSolid_Click(object sender, RoutedEventArgs e) => PartIntoSolid();
        private void btnPartFromPart_Click(object sender, RoutedEventArgs e) => PartFromPart();
        private void btnPartNew_Click(object sender, RoutedEventArgs e) => PartNew();
        private void btnPartNext_Click(object sender, RoutedEventArgs e) => ModelNext();
        private void btnPartClone_Click(object sender, RoutedEventArgs e) => ModelClone();

        private void btnDrawingFromSolid_Click(object sender, RoutedEventArgs e) => DrawingFromSolid();
        private void btnDrawingNew_Click(object sender, RoutedEventArgs e) => DrawingNew();
        private void btnDrawingClone_Click(object sender, RoutedEventArgs e) => ModelClone();
        private void btnDrawingNext_Click(object sender, RoutedEventArgs e) => ModelNext();

        private void btnCreate_Click(object sender, RoutedEventArgs e)
        {
            ExecuteActions();
            ClearActions();
        }

        #endregion

        #region ACTIONS

        #region FOLDER ACTIONS

        private void CreateNewFolder(string containerPath)
        {
            ClearActions();

            var cfPf = new CreateFolder(containerPath);

            //Project Folder is added
            trvActions.Items.Add(cfPf);

            var cA = new CreateAssembly(GetActiveMaterial());
            if (containerPath == AddInSettings.PATH_PROJECTS)
            {
                var cfPf_d = new CreateFolder() { DocText = AddInSettings.FOLDER_DRAFT };
                var cfPf_r = new CreateFolder() { DocText = AddInSettings.FOLDER_RELEASE };
                var cfDf = new CreateFolder();
                var cfDsf = new CreateFolder();
                cfPf.RegisterChild(cfPf_d);
                cfPf.RegisterChild(cfPf_r);
                cfPf_d.RegisterChild(cfDf);
                cfDf.RegisterChild(cfDsf);
                cfDsf.RegisterChild(cA);
                trvActions.Items.Add(cfPf_d);
                trvActions.Items.Add(cfPf_r);
                trvActions.Items.Add(cfDf);
                trvActions.Items.Add(cfDsf);
            }
            else if (containerPath == AddInSettings.PATH_PRINT_PROJ || (Path.GetDirectoryName(Path.GetDirectoryName(containerPath)) == AddInSettings.PATH_PROJECTS && 
                Path.GetFileName(containerPath) == $"{AddInSettings.SEQUENCE_START}{AddInSettings.SEP_FOLDER_NUMERATION}{AddInSettings.FOLDER_DRAFT}"))
            {
                var cfDsf = new CreateFolder();
                cfPf.RegisterChild(cfDsf);
                cfDsf.RegisterChild(cA);
                trvActions.Items.Add(cfDsf);
            }
            else
            {
                cfPf.RegisterChild(cA);
            }                
            trvActions.Items.Add(cA);

            var cP = new CreatePart(GetActiveMaterial(), containerPath.StartsWith(AddInSettings.PATH_PRINT_PROJ));
            cA.RegisterChild(cP);
            trvActions.Items.Add(cP);

            trvActions.Items.Add(new InsertSolidIntoAssembly(cP, cA));
        }
        //$"{AddInSettings.PATH_PROJECTS}{Path.DirectorySeparatorChar}{AddInSettings.SEQUENCE_START}{AddInSettings.SEP_FOLDER_NUMERATION}{AddInSettings.FOLDER_DRAFT}")
        #endregion


        #region ASSEMBLY ACTIONS

        private void AssemblyFromSolid()
        {
            ClearActions();

            //Create new assembly
            var dirA = GetActiveDirectory();
            var cA = new CreateAssembly(dirA, GetActiveMaterial());

            //Modify no and text
            //cA.DocNo = GetActiveModelPrefix();
            //cA.DocText = AddInSettings.CREATED_FROM_SFX;
            cA.DocText = AddInSettings.CREATED_FROM_SFX + AddInSettings.SEP_TEXT + GetActiveModelPrefix();

            //Add new assembly creation control
            trvActions.Items.Add(cA);

            //Insert
            trvActions.Items.Add(new InsertSolidIntoAssembly(model, cA));
        }

        private void AssemblyIntoAssembly()
        {
            ClearActions();

            //Create new assembly parameters
            var dirA = GetActiveDirectory();
            var parentPrefix = CommonResources.GetPrefixFromName(Path.GetFileNameWithoutExtension(model.FilePath));
            var cA = new CreateAssembly(dirA, GetActiveMaterial(), parentPrefix, null);

            //Add new assembly creation control
            trvActions.Items.Add(cA);

            //Insert Assembly into Assembly
            trvActions.Items.Add(new InsertSolidIntoAssembly(cA, model));

            //Create new Part
            var cP = new CreatePart(GetActiveMaterial());
            cA.RegisterChild(cP);
            trvActions.Items.Add(cP);

            //Insert Part into Assembly
            trvActions.Items.Add(new InsertSolidIntoAssembly(cP, cA));
        }

        private void AssemblyNew()
        {
            ClearActions();

            //Create new Assembly
            var dirA = GetActiveDirectory();
            var cA = new CreateAssembly(dirA, GetActiveMaterial());
            trvActions.Items.Add(cA);

            //Create new Part
            var cP = new CreatePart(GetActiveMaterial());
            cA.RegisterChild(cP);
            trvActions.Items.Add(cP);

            //Insert Part into Assembly
            trvActions.Items.Add(new InsertSolidIntoAssembly(cP, cA));
        }

        #endregion

        #region PART ACTIONS

        private void PartFromPart()
        {
            ClearActions();

            //Create new Part
            var dirP = GetActiveDirectory();
            var cP = new CreatePart(dirP, GetActiveMaterial(), dirP.StartsWith(AddInSettings.PATH_PRINT_PROJ));

            //Modify no and text
            //cP.DocNo = GetActiveModelPrefix();
            //cP.DocText = AddInSettings.CREATED_FROM_SFX;
            cP.DocText = AddInSettings.CREATED_FROM_SFX + AddInSettings.SEP_TEXT + GetActiveModelPrefix();

            trvActions.Items.Add(cP);

            //Insert Part into Part
            trvActions.Items.Add(new InsertPartIntoPart(model, cP, InsertationDirectionEnum.ModelIntoControl));
        }

       

        private void PartIntoSolid()
        {
            ClearActions();

            if(model.IsAssembly)
            {
                //Get assembly info
                //TODO: Aslinda bu isi C'tor'a almanin ve GetNextChildPrefix'i daha akilli yapmanin bir yolu var kesin
                var dirA = GetActiveDirectory();
                var parentPrefix = GetActiveModelPrefix();//CommonResources.GetPrefixFromName(Path.GetFileNameWithoutExtension(model.FilePath));
                var noP = CommonResources.GetNextChildPrefix(dirA, true, null, parentPrefix);

                var cP = new CreatePart(dirA, noP, GetActiveMaterial(), dirA.StartsWith(AddInSettings.PATH_PRINT_PROJ));

                trvActions.Items.Add(cP);

                //Insert part into assembly
                trvActions.Items.Add(new InsertSolidIntoAssembly(cP, model));
            }
            else if(model.IsPart)
            {
                //Create new Part
                var dirP = GetActiveDirectory();
                var cP = new CreatePart(dirP, GetActiveMaterial(), dirP.StartsWith(AddInSettings.PATH_PRINT_PROJ));

                //Modify no and text
                //cP.DocNo = GetActiveModelPrefix();
                //cP.DocText = AddInSettings.CREATED_INTO_SFX;
                cP.DocText = AddInSettings.CREATED_INTO_SFX + AddInSettings.SEP_TEXT + GetActiveModelPrefix();


                trvActions.Items.Add(cP);

                //Insert Part into Part
                trvActions.Items.Add(new InsertPartIntoPart(model, cP, InsertationDirectionEnum.ControlIntoModel));
            }
        }

        private void PartCopy()
        {
            ClearActions();

            //Create and add new part creation control
            trvActions.Items.Add(new CreatePart(model));
        }

        private void PartNew()
        {
            ClearActions();

            //Create new Part
            var dirP = GetActiveDirectory();
            var cP = new CreatePart(dirP, GetActiveMaterial());
            trvActions.Items.Add(cP);
        }

        #endregion

        #region DRAWING ACTIONS

        private void DrawingFromSolid()
        {
            ClearActions();

            //Create new drawing parameters
            var dirD = GetActiveDirectory();
            var nameD = Path.GetFileNameWithoutExtension(model.FilePath);

            //Create new drawing creation control
            var cD = new CreateDrawing(dirD, nameD);

            //Add new drawing creation control
            trvActions.Items.Add(cD);

            //Create and add new insertation control
            trvActions.Items.Add(new InsertSolidIntoDrawing(model, cD));
        }

        private void DrawingCopy()
        {
            ClearActions();

            //Create and add new drawing creation control
            trvActions.Items.Add(new CreateDrawing(model));
        }

        private void DrawingNew()
        {
            ClearActions();

            //Create new drawing parameters
            var dir = GetActiveDirectory(); 

            //Create and add new drawing creation control
            trvActions.Items.Add(new CreateDrawing(dir));
        }


        #endregion

        #region COMMON ACTIONS

        private void ModelClone()
        {
            ClearActions();

            //Create and add new assembly creation control
            trvActions.Items.Add(new CreateClone(model, GetActiveDirectory())); 
        }

        private void ModelNext()
        {
            ClearActions();

            var parentPath = Path.GetDirectoryName(GetActiveDirectory());
            var nextChild = CommonResources.GetNextChildPrefix(parentPath, false);
            var containerPath = parentPath + Path.DirectorySeparatorChar + nextChild;

            //Create and add new assembly creation control
            var cloneP = new CreateClone(model, containerPath);
            //cloneP.RegisterChild(new CreateClone());
            trvActions.Items.Add(cloneP);
        }

        #endregion

        #region TOOLBOX ASSEMBLY ACTIONS

        #endregion

        #region TOOLBOX PART ACTIONS

        #endregion

        #region PANE ACTIONS

        private void ClearActions()
        {
            trvActions.Items.Clear();
        }

        private void ExecuteActions()
        {
            for (int i = 0; i < trvActions.Items.Count; i++)
            {
                (trvActions.Items[i] as DocumentAction).ExecuteAction();
            }

        }

        private void cbHasSTLConjugate_Checked(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                model.SetCustomProperty(UI_Properties.CP_STLConjugate, UI_Properties.STR_True);
                model.UnsafeObject.SetSaveFlag();
            }
        }

        private void cbHasSTLConjugate_Unchecked(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                model.SetCustomProperty(UI_Properties.CP_STLConjugate, UI_Properties.STR_False);
                model.UnsafeObject.SetSaveFlag();
            }
        }


        bool isSaveEventActive = true;

        private void Application_ActiveFileSaved(string arg1, Model arg2)
        {
            try
            {
                if (arg2.IsPart && isSaveEventActive)
                {
                    bool hasSTLConj = arg2.GetCustomProperty(UI_Properties.CP_STLConjugate) == UI_Properties.STR_True ? true : false;
                    if (hasSTLConj)
                    {
                        var originalExt = Path.GetExtension(arg1);
                        var targetExt = ".STL";
                        var targetPath = arg1.Replace(originalExt, targetExt);

                        isSaveEventActive = false;

                        var errSave = 0;
                        var warSave = 0;
                        var result = arg2.UnsafeObject.Extension.SaveAs(
                            targetPath,
                            (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                            (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                            null,
                            ref errSave,
                            ref warSave
                            );

                        isSaveEventActive = true;

                        var title = Path.GetFileName(targetPath);
                        SolidWorksEnvironment.Application.UnsafeObject.CloseDoc(title);
                    }
                }
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }









        #endregion

        #endregion








        //private CreateAssembly CreateNewAssembly(string assemName, bool isCreateFirstPart, DocumentAction dirSource = null, string directoryPath = null)
        //{
        //    //Set directoryPath to Selected Draft Directory if needed
        //    if (dirSource == null && directoryPath.IsNullOrEmpty())
        //        directoryPath = pdDraft.GetSelectedPath();

        //    //Create new Assembly
        //    var noA = UI_Properties.DFT_Value;
        //    var descriptionA = UI_Properties.DFT_Value;
        //    var cA = dirSource == null ?
        //        new CreateAssembly(assemName, directoryPath, noA, descriptionA, AddInSettings.DFT_MATERIAL) :
        //        new CreateAssembly(noA, descriptionA, AddInSettings.DFT_MATERIAL, dirSource);
        //    trvActions.Items.Add(cA);

        //    //Create new Assembly Folder
        //    var nameAf = assemName;
        //    var cfA = dirSource == null ?
        //        new CreateFolder(nameAf, directoryPath) :
        //        new CreateFolder(nameAf, dirSource);
        //    trvActions.Items.Add(cfA);

        //    //Create new Part
        //    var nameP = assemName + AddInSettings.SEP_NUMBER + AddInSettings.SEQUENCE_START.ToString();
        //    var cP = CreateNewPart(nameP, cfA);

        //    //Insert Part into Assembly
        //    if (isCreateFirstPart)
        //        trvActions.Items.Add(new InsertSolidIntoAssembly(cP, cA));

        //    return cA;
        //}

        //private CreatePart CreateNewPart(string partName, DocumentAction dirSource = null, string directoryPath = null)
        //{
        //    //Set directoryPath to Selected Draft Directory if needed
        //    if (dirSource == null && directoryPath.IsNullOrEmpty())
        //        directoryPath = pdDraft.GetSelectedPath();

        //    //Create new Part
        //    var nameP = partName;
        //    var noP = UI_Properties.DFT_Value;
        //    var descriptionP = UI_Properties.DFT_Value;
        //    var cP = dirSource == null ?
        //        new CreatePart(nameP, directoryPath, noP, descriptionP, AddInSettings.DFT_MATERIAL) :
        //        new CreatePart(noP, descriptionP, AddInSettings.DFT_MATERIAL, dirSource);
        //    trvActions.Items.Add(cP);

        //    return cP;
        //}

        //private void CreateNewDrawing(string drawName, DocumentAction dirSource = null, string directoryPath = null)
        //{

        //}



    }
}
