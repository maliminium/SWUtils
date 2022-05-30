using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.IO;
using Dna;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for SaveAsAction.xaml
    /// </summary>
    public partial class SaveAsAction : DocumentAction
    {
        //http://help.solidworks.com/2018/english/api/sldworksapi/SOLIDWORKS.Interop.sldworks~SOLIDWORKS.Interop.sldworks.IModelDocExtension~SaveAs.html
        //Do not use ModelDocExtension::SaveAs to copy assemblies, drawings, or parts with in-context references. Instead, use ISldWorks::CopyDocument or ISldWorks::ICopyDocument.
        //Saving a document as PDF when the document is open as view only is not supported.
        //A standard drawing document as a detached drawing, specify swSaveAsDetachedDrawing for Version.

        //http://help.solidworks.com/2018/english/api/sldworksapi/solidworks.interop.sldworks~solidworks.interop.sldworks.isldworks~opendoc6.html
        //OpenDoc6 does not change the current working directory to that of the opened file, whereas, interactively using the File Open dialog box does.This may affect documents with references.
        //This can be done using the ISldWorks::SetCurrentWorkingDirectory method.
        //For IGES, STEP, and so on use ISldWorks::LoadFile4.





        public const int VAL_ALREADY_OPEN = (int)swFileLoadWarning_e.swFileLoadWarning_AlreadyOpen;

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

        //private ObservableCollection<PackAndGoItem> SaveAsItems = new ObservableCollection<PackAndGoItem>();

        public ObservableCollection<ReleaseOpItem> ReleaseOpItems = new ObservableCollection<ReleaseOpItem>();

        public SaveAsAction()
        {
            SetTheControls();

            //SaveAsItems.Add(new PackAndGoItem(@"D:\mekanik\0.Miscellaneous\1.DRAFT\13.Deneme\0-1.13.1.SLDASM", @"D:\mekanik\0.Miscellaneous\1.DRAFT\13.Deneme\0-1.13.1.STEP"));
            //SaveAsItems.Add(new PackAndGoItem(@"D:\mekanik\0.Miscellaneous\1.DRAFT\13.Deneme\0-1.13.1.SLDDRW", @"D:\mekanik\0.Miscellaneous\1.DRAFT\13.Deneme\0-1.13.1.PDF"));
        }

        public SaveAsAction(ObservableCollection<ReleaseOpItem> releaseOpItems)
        {
            SetTheControls();
            ReleaseOpItems = new ObservableCollection<ReleaseOpItem>(releaseOpItems);
            dtgSaveAsItems.ItemsSource = ReleaseOpItems;
        }

        public SaveAsAction(PackAndGoAction sourceControl)
        {
            SetTheControls();
            ReleaseOpItems = new ObservableCollection<ReleaseOpItem>(sourceControl.ReleaseOpItems);
            dtgSaveAsItems.ItemsSource = ReleaseOpItems;
            edTargetDir.Value = sourceControl.edTargetDir.Value;
            sourceControl.ControlChanged += SourceControl_ControlChanged;
        }

        private void SourceControl_ControlChanged(object UIcontrol)
        {
            edTargetDir.Value = (UIcontrol as PackAndGoAction).edTargetDir.Value;
        }

        private void SetTargetDir(string targetDir)
        {
            var checker = new StringChecker();

            //var valCheck=targetDir != edTargetDir.Value;
            //var dirCheck = checker.IsDirectoryValid(targetDir);

            if (checker.IsDirectoryValid(targetDir))
            {
                for (int i = 0; i < ReleaseOpItems.Count; i++)
                {
                    ReleaseOpItems[i].TargetDir = targetDir;
                }

                dtgSaveAsItems.Items.Refresh();
                RaiseControlChanged(this);
            }
            else
            {
                Logger.Log($"problem in Target Directory for Save As Tgt:{targetDir}", MessageTypeEnum.Error);//Tc:{valCheck} Dc:{dirCheck}
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

        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                SldWorks sldWorks = SolidWorksEnvironment.Application.UnsafeObject;

                for (int i = 0; i < ReleaseOpItems.Count; i++)
                {

                    //TODO: Logger icin bir error mask decoder guzel olabilir

                    if(!ReleaseOpItems[i].TargetPath_Sas.IsNullOrEmpty())
                    {
                        var err_STR = string.Empty;
                        var war_STR = string.Empty;

                        var errOpen = 0;
                        var warOpen = 0;
                        var model = sldWorks.OpenDoc6(
                            ReleaseOpItems[i].TargetPath_PaG,
                            (int)ReleaseOpItems[i].DocType,
                            (int)swOpenDocOptions_e.swOpenDocOptions_Silent,
                            string.Empty,
                            ref errOpen,
                            ref warOpen
                            );

                        var errOpen_STR = CommonResources.GetEnumMaskString(errOpen, typeof(swFileLoadError_e));
                        if (!errOpen_STR.IsNullOrEmpty())
                            err_STR += " " + errOpen_STR;

                        var warOpen_STR = CommonResources.GetEnumMaskString(warOpen, typeof(swFileLoadWarning_e));
                        if (!warOpen_STR.IsNullOrEmpty())
                            war_STR += " " + warOpen_STR;


                        var isAlreadyOpen = (VAL_ALREADY_OPEN & warOpen) == VAL_ALREADY_OPEN;

                        var modelTitle = model.GetTitle();

                        //model.ForceReleaseLocks();

                        var errActivation = 0;
                        sldWorks.ActivateDoc3(
                            modelTitle,
                            false,
                            (int)swRebuildOnActivation_e.swRebuildActiveDoc,
                            errActivation
                            );


                        var errActivation_STR = CommonResources.GetEnumMaskString(errActivation, typeof(swActivateDocError_e));
                        if (!errActivation_STR.IsNullOrEmpty())
                            err_STR += " " + errActivation_STR;

                        model = (ModelDoc2)sldWorks.ActiveDoc;
                        model.ClearSelection2(true);

                        modelTitle = model.GetTitle();

                        //var errSave = 0;
                        //var warSave = 0;
                        //var result = model.SaveAs3(
                        //    ReleaseOpItems[i].TargetPath_Sas,
                        //    (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                        //    (int)swSaveAsOptions_e.swSaveAsOptions_Silent
                        //    );

                        

                        model.ForceReleaseLocks();

                        var nextDir = Path.GetDirectoryName(ReleaseOpItems[i].TargetPath_Sas);
                        if (!Directory.Exists(nextDir))
                            Directory.CreateDirectory(nextDir);

                        var errSave = 0;
                        var warSave = 0;
                        var result = model.Extension.SaveAs(
                            ReleaseOpItems[i].TargetPath_Sas,
                            (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                            (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                            null,
                            ref errSave,
                            ref warSave
                            );

                        var errSave_STR = CommonResources.GetEnumMaskString(errSave, typeof(swFileSaveError_e));
                        if (!errSave_STR.IsNullOrEmpty())
                            err_STR += " " + errSave_STR;

                        var warSave_STR = CommonResources.GetEnumMaskString(warSave, typeof(swFileSaveWarning_e));
                        if (!warSave_STR.IsNullOrEmpty())
                            war_STR += " " + warSave_STR;

                        if (!isAlreadyOpen)
                        {
                            //var title = model.GetTitle();
                            //sldWorks.CloseDoc(title);
                            sldWorks.CloseDoc(modelTitle);
                        }

                        if(!(err_STR.IsNullOrEmpty() && war_STR.IsNullOrEmpty()))
                        {
                            Logger.Log($"{ReleaseOpItems[i].TargetPath_Sas}", MessageTypeEnum.Exception);
                            if (!err_STR.IsNullOrEmpty())
                                Logger.Log($"{err_STR}", MessageTypeEnum.Error);
                            if (!war_STR.IsNullOrEmpty())
                                Logger.Log($"{war_STR}", MessageTypeEnum.Warning);
                            Logger.Log("");
                        }
                    }

                    //var assem = (AssemblyDoc)model;
                    //var components = (object[])assem.GetComponents(false);
                    //for (int i = 0; i < components.Length; i++)
                    //{
                    //    var nextModel = (IComponent2)components[i];
                    //    var originalPath = nextModel.GetPathName();
                    //    var replacementPath = originalPath.Replace(
                    //        @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy",
                    //        @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy_2");
                    //    var idx = replacementPath.LastIndexOf(".");
                    //    replacementPath = replacementPath.Insert(idx, COPY_SFX);
                    //    var sData = default(SelectData);
                    //    nextModel.Select4(false, sData, false);
                    //    assem.ReplaceComponents2(replacementPath, string.Empty, true, (int)swReplaceComponentsConfiguration_e.swReplaceComponentsConfiguration_MatchName, true);
                    //    Logger.Log(originalPath, MessageTypeEnum.Error);
                    //    Logger.Log(replacementPath, MessageTypeEnum.Greeting);
                    //}
                    //var docType = (swDocumentTypes_e)model.GetType();

                    //Open doc
                    //doc.SaveAs3
                    //Close doc
                }
                /*
                KATI_MODELLER_STEP
                TEKNIK_RESIMLER_PDF
                KESIM_DOSYALARI_DXF

                Set swApp = Application.SldWorks
                Set Part = swApp.ActiveDoc
                ' Save As
                longstatus = Part.SaveAs3("C:\Users\mehmet.ciftci\Desktop\S-1.1.6.Flans_celik.STEP", 0, 0)
                */
            }
        }

        private void edTargetDir_ControlChanged(object UIcontrol)
        {
            SetTargetDir(edTargetDir.Value);
            //RaiseControlChanged(this);
        }
    }
}
