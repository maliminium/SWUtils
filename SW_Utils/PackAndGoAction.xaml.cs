using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for PackAndGo.xaml
    /// </summary>
    public partial class PackAndGoAction : DocumentAction
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

        private Model SourceModel = null;
        private PackAndGo swPackAndGo = null;

        public ObservableCollection<ReleaseOpItem> ReleaseOpItems = new ObservableCollection<ReleaseOpItem>();

        public PackAndGoAction()
        {
            SetTheControls();
        }

        public PackAndGoAction(Model model, string targetDir)
        {
            SetTheControls();


            if (model != null)
            {
                swPackAndGo = model.UnsafeObject.Extension.GetPackAndGo();

                object docNames_obj;
                var isNameGetOk = swPackAndGo.GetDocumentNames(out docNames_obj);
                //var isNameGetOk = swPackAndGo.IGetDocumentNames(swPackAndGo.GetDocumentNamesCount(), out docNames_obj);

                if (isNameGetOk)
                {
                    var docNames = (string[])docNames_obj;

                    var items = new ObservableCollection<ReleaseOpItem>();
                    for (int i = 0; i < docNames.Length; i++)
                    {
                        items.Add(new ReleaseOpItem(docNames[i], targetDir));
                    }


                    swPackAndGo.FlattenToSingleFolder = true;
                    swPackAndGo.IncludeDrawings = true;
                    swPackAndGo.IncludeSuppressed = true;
                    swPackAndGo.IncludeToolboxComponents = true;

                    SourceModel = model;

                    ReleaseOpItems = new ObservableCollection<ReleaseOpItem>(items);
                    dtgPackAndGoItems.ItemsSource = ReleaseOpItems;

                    edTargetDir.Value = targetDir;
                }
                else
                {
                    Logger.Log("problem in getting names for Pack and Go", MessageTypeEnum.Error);
                }
                
            }
            else
            {
                Logger.Log("problem in inputs for Pack and Go", MessageTypeEnum.Error);
            }
        }

        public PackAndGoAction(Model model, string targetDir, ObservableCollection<ReleaseOpItem> items)
        {
            SetTheControls();

            if (model != null)
            {
                swPackAndGo = model.UnsafeObject.Extension.GetPackAndGo();

                swPackAndGo.FlattenToSingleFolder = true;
                swPackAndGo.IncludeDrawings = true;
                swPackAndGo.IncludeSuppressed = true;
                swPackAndGo.IncludeToolboxComponents = true;

                SourceModel = model;

                ReleaseOpItems = new ObservableCollection<ReleaseOpItem>(items);
                dtgPackAndGoItems.ItemsSource = ReleaseOpItems;

                edTargetDir.Value = targetDir;
            }
            else
            {
                Logger.Log("problem in inputs for Pack and Go", MessageTypeEnum.Error);
            }
        }

        private void SetTheControls()
        {
            InitializeComponent();

            cbAction.Checked += CbAction_Changed;
            cbAction.Unchecked += CbAction_Changed;
        }

        private void SetPackAndGoDir(string targetDir)
        {
            var checker = new StringChecker();

            if (swPackAndGo != null && checker.IsDirectoryValid(targetDir))
            {
                swPackAndGo.SetSaveToName(true, targetDir);

                for (int i = 0; i < ReleaseOpItems.Count; i++)
                {
                    ReleaseOpItems[i].TargetDir = targetDir;
                }

                dtgPackAndGoItems.Items.Refresh();
                RaiseControlChanged(this);
            }
            else
            {
                Logger.Log("problem in Target Directory for Pack and Go", MessageTypeEnum.Error);
            }
        }

        public override void ExecuteAction()
        {
            if (cbAction.IsChecked == true)
            {
                var targetNames = new string[ReleaseOpItems.Count];
                for (int i = 0; i < targetNames.Length; i++)
                {
                    targetNames[i] = ReleaseOpItems[i].TargetPath_PaG;
                }

                var setSuccesful = swPackAndGo.SetDocumentSaveToNames(targetNames);
                if (!setSuccesful)
                    Logger.Log("Problem in (SetDocumentSaveToNames)", MessageTypeEnum.Error);

                //swPackAndGo.GetDocumentSaveToNames(out object newNames_obj, out object docStat);
                //var newNames = (string[])newNames_obj;
                //for (int i = 0; i < newNames.Length; i++)
                //{
                //    Logger.Log(newNames[i]);
                //}


                var results = (int[])SourceModel.UnsafeObject.Extension.SavePackAndGo(swPackAndGo);
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
                    Logger.Log($"Pack and Go succesful");
                }
            }
        }

        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;

            RaiseControlChanged(this);
        }

        private void edTargetDir_ControlChanged(object UIcontrol)
        {
            SetPackAndGoDir(edTargetDir.Value);
        }
    }
}
