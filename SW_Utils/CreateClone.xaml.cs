using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CreateClone.xaml
    /// </summary>
    public partial class CreateClone : DocumentAction
    {
        public override bool IsFile { get => false; }
        public override string DocDir { get => edDir.Value; set => edDir.Value = value; }
        public override string DocNo { get => edName.NumberText; set => edName.NumberText = value; }
        public override string DocText { get => edName.Value; set => edName.Value = value; }
        public override string DocExt { get => edName.ExtensionText; set => edName.ExtensionText = value; }
        public override bool IsActive
        {
            get => (cbAction.IsChecked == true);
            set
            {
                if (cbAction.IsChecked != value)
                    cbAction.IsChecked = value;
            }
        }

        private bool IsIncludingDrawings { get => cbIncludeDrawings.IsChecked == true; set => cbIncludeDrawings.IsChecked = value; }

        private bool _RefreshFlag = false;
        public bool RefreshFlag
        {
            get => _RefreshFlag;
            set
            {
                _RefreshFlag = value;
                stpHeader.Background = value ? Brushes.Red : Brushes.Transparent;
            }
        }

        public string TargetPath
        {
            get
            {
                //return edDir.Value;
                //return edDir.Value + Path.DirectorySeparatorChar + edName.Value;
                return GetFullPath();
            }
            set
            {
                //edDir.Value = value;
                //edName.Value = Path.GetFileName(value);
                //edDir.Value = Path.GetDirectoryName(value);
                var folderName = Path.GetFileName(value);
                DocNo = CommonResources.GetPrefixFromName(folderName);

                if (DocNo != folderName)
                {
                    if (DocNo.Length > 0 && folderName.Length > (DocNo.Length + 1))
                    {
                        DocText = folderName.Remove(0, DocNo.Length + 1);
                    }
                    else
                    {
                        DocText = folderName;
                    }
                }

                DocDir = Path.GetDirectoryName(value);
            }
        }


        private ComponentItem CompTree = null;

        private Model MotherModel = null;
        private PackAndGo PackAndGoElement = null;
        private string[] targetNames = null;

        public CreateClone()
        {
            InitializeComponent();
        }

        public CreateClone(Model motherModel, string targetDir)
        {
            InitializeComponent();

            //sirasi onemli
            MotherModel = motherModel;
            //UpdateCompTree();
            if (MotherModel != null)
            {
                //Update tree
                trvComponents.Items.Clear();
                CompTree = new ComponentItem(MotherModel, IsIncludingDrawings);
                trvComponents.Items.Add(CompTree);
                CompTree.StateChanged += CompTree_StateChanged;
            }
            //edDir.Value = targetDir;
            TargetPath = targetDir;
            edName.ControlChanged += edDir_ControlChanged;
            edDir.ControlChanged += edDir_ControlChanged;
            RefreshTargetInfo();
        }

        //private void UpdateCompTree()
        //{
        //    if (MotherModel != null)
        //    {
        //        //Update tree
        //        trvComponents.Items.Clear();
        //        CompTree = new ComponentItem(MotherModel, IsIncludingDrawings);
        //        trvComponents.Items.Add(CompTree);
        //        CompTree.StateChanged += CompTree_StateChanged;
        //    }
        //}

        private void CompTree_StateChanged()
        {
            RefreshFlag = true;
            //RefreshTargetInfo(); burada bu olunca her alt uncheck icin liste refresh oluyor
        }

        public static int GetCaseInsensitiveIndex(string item, List<string> list)
        {
            return list.FindIndex(x => x.Equals(item, StringComparison.OrdinalIgnoreCase));
        }

        private void RefreshTargetInfo()
        {
            if (MotherModel != null)
            {
                var targetDir = TargetPath;
                var motherNo = CommonResources.GetNextChildPrefix(targetDir, true);
                //var assemblyNameBlock = CompTree.IsBlank ? CommonResources.GetNameBlock(MotherModel.FilePath) : CommonResources.GetNameBlock(CompTree.TargetPath);
                var motherPath = CompTree.IsBlank ? MotherModel.FilePath : CompTree.TargetPath;
                var assemblyNameBlock = CommonResources.GetNameBlock(motherPath);
                if (assemblyNameBlock.Length > 0)
                    assemblyNameBlock = AddInSettings.SEP_TEXT + assemblyNameBlock;
                //var motherNo = CommonResources.GetNextChildPrefix(targetDir, true);
                var extText = Path.GetExtension(motherPath);

                //var sourceNames_CL = CompTree.GetDocNames(new List<string>());

                CompTree.TargetPath = targetDir + Path.DirectorySeparatorChar + motherNo + assemblyNameBlock + extText;
                var sourceNames_CL = CompTree.GetDocNames(new List<string>());
                var targetNames_CL = CompTree.GetSaveToNames(new List<string>());

                //Check with PaG
                PackAndGoElement = MotherModel.UnsafeObject.Extension.GetPackAndGo();
                PackAndGoElement.SetSaveToName(true, targetDir);
                PackAndGoElement.FlattenToSingleFolder = true;
                PackAndGoElement.IncludeSimulationResults = true;
                PackAndGoElement.IncludeDrawings = IsIncludingDrawings;
                PackAndGoElement.IncludeSuppressed = true;
                PackAndGoElement.IncludeToolboxComponents = true;
                PackAndGoElement.GetDocumentNames(out object pag_obj);
                var pag_arr = (string[])pag_obj;
                var sourceNames_PaG = new List<string>(pag_arr);

                if ((sourceNames_CL.Count != targetNames_CL.Count) || (sourceNames_CL.Count != sourceNames_PaG.Count))
                {
                    Logger.Log("List sizes doesn't match", MessageTypeEnum.Exception);
                    Logger.Log(
                        "Clone Source List (" + sourceNames_CL.Count.ToString() + 
                        ")\nPaG Source List (" + sourceNames_PaG.Count.ToString() + ")" +
                        ")\nClone Target List (" + targetNames_CL.Count.ToString() + ")", MessageTypeEnum.Exception);
                }

                //Construct targetNames array
                targetNames = new string[pag_arr.Length];
                for (int i = 0; i < sourceNames_PaG.Count; i++)
                {
                    var idx = GetCaseInsensitiveIndex(sourceNames_PaG[i], sourceNames_CL);
                    if (idx > -1)
                    {
                        targetNames[i] = targetNames_CL[idx];
                    }
                    else
                    {
                        targetNames[i] = string.Empty;
                        Logger.Log(sourceNames_PaG[i] + " is missing in the Clone Source List", MessageTypeEnum.Error);
                    }   
                }



                var ctrAssembly = 0;
                var ctrPart = 0;
                var ctrDrawing = 0;
                var ctrOther = 0;
                var ctrTotal = 0;

                for (int i = 0; i < sourceNames_CL.Count; i++)
                {
                    var idx = GetCaseInsensitiveIndex(sourceNames_CL[i], sourceNames_PaG);
                    if (idx < 0)
                        Logger.Log(sourceNames_CL[i] + " is missing in the PaG DocNames List", MessageTypeEnum.Information);


                    switch (Path.GetExtension(sourceNames_CL[i]).ToUpper())
                    {
                        case AddInSettings.EXT_ASSM:
                            ctrAssembly++;
                            break;
                        case AddInSettings.EXT_PART:
                            ctrPart++;
                            break;
                        case AddInSettings.EXT_DRAW:
                            ctrDrawing++;
                            break;
                        default:
                            ctrOther++;
                            break;
                    }
                }

                ctrTotal = ctrAssembly + ctrPart + ctrDrawing + ctrOther;
                txtSummary.Text = $"(A:{ctrAssembly} P:{ctrPart} D:{ctrDrawing} O:{ctrOther} T:{ctrTotal})";
                RefreshFlag = false;
            }
            else
            {
                Logger.Log("AssemblyModel = null", MessageTypeEnum.Error);
                targetNames = null;
            }
        }
       
        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                if (targetNames != null)
                {
                    if (RefreshFlag)
                    {
                        RefreshTargetInfo();
                        Logger.Log("TargetInfo is refreshed before Pack and Go", MessageTypeEnum.Information);
                    }   
                    var setSuccesful = PackAndGoElement.SetDocumentSaveToNames(targetNames);
                    if (!setSuccesful)
                        Logger.Log("Problem in (SetDocumentSaveToNames)", MessageTypeEnum.Error);

                    var results = (int[])MotherModel.UnsafeObject.Extension.SavePackAndGo(PackAndGoElement);
                    var faultyResults = new List<int>();
                    for (int i = 0; i < results.Length; i++)
                    {
                        if ((swPackAndGoSaveStatus_e)results[i] != swPackAndGoSaveStatus_e.swPackAndGoSaveStatus_Succeed)
                            faultyResults.Add(i);
                    }
                    if (faultyResults.Count > 0)
                    {
                        for (int i = 0; i < faultyResults.Count; i++)
                        {
                            Logger.Log($"{(swPackAndGoSaveStatus_e)results[faultyResults[i]]} for {targetNames[i]}", MessageTypeEnum.Error);
                        }
                    }
                    else
                    {
                        Logger.Log($"Pack and Go succesful", MessageTypeEnum.Information);
                    }
                }
                else
                {
                    Logger.Log("targetNames[] = null", MessageTypeEnum.Error);
                }
            }
        }

        private void edDir_ControlChanged(object UIcontrol)
        {
            RefreshTargetInfo();
        }

        private void btnRefreshNumeration_Click(object sender, RoutedEventArgs e)
        {
            RefreshTargetInfo();
        }

        private void btnCrop_Click(object sender, RoutedEventArgs e)
        {
            var parsingOk = int.TryParse(txtCrop.Text, out int charCt);
            if (parsingOk && charCt > -1)
                CompTree.CropText(charCt);
            else
                Logger.Log(txtCrop.Text + " is not a valid count");
        }
    }
}
