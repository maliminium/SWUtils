using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using System.Windows;
using System.IO;
using System.Windows.Controls;
using SolidWorks.Interop.swconst;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for InsertPartIntoPart.xaml
    /// </summary>
    public partial class InsertPartIntoPart : DocumentAction
    {
        public const int INSERT_OPT = 17;   //(swInsertPartImportSolids = 1) + (swInsertPartImportCosmeticThreads = 16)

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

        Model ExistingPart = null;
        PartDoc ExistingPartDoc = null;
        CreatePart ControlNewPart = null;
        InsertationDirectionEnum InsertationDirection;

        //TextBlock txtInsertationSubject = new TextBlock();
        //TextBlock txtInsertationTarget = new TextBlock();


        public InsertPartIntoPart() => SetTheControls();

        public InsertPartIntoPart(Model existingPart, CreatePart newPartCreator, InsertationDirectionEnum insertationDirection)
        {
            SetTheControls();

            InsertationDirection = insertationDirection;

            ExistingPart = existingPart;
            ExistingPartDoc = existingPart.AsPart();
            //existingPart.ModelSaved += MainPart_ModelSaved;

            ControlNewPart = newPartCreator;
            newPartCreator.ControlChanged += NewPartCreator_ControlChanged;

            switch (InsertationDirection)
            {
                case InsertationDirectionEnum.ModelIntoControl:
                    RenewPartPath(txtInsertationSubject, existingPart.FilePath);
                    RenewPartPath(txtInsertationTarget, newPartCreator.GetFullPath());
                    break;
                case InsertationDirectionEnum.ControlIntoModel:
                    RenewPartPath(txtInsertationSubject, newPartCreator.GetFullPath());
                    RenewPartPath(txtInsertationTarget, existingPart.FilePath);
                    break;
                default:
                    break;
            }

            
        }

        private void MainPart_ModelSaved()
        {
            switch (InsertationDirection)
            {
                case InsertationDirectionEnum.ModelIntoControl:
                    RenewPartPath(txtInsertationSubject, ExistingPart.FilePath);
                    break;
                case InsertationDirectionEnum.ControlIntoModel:
                    RenewPartPath(txtInsertationTarget, ExistingPart.FilePath);
                    break;
                default:
                    break;
            }
        }

        private void NewPartCreator_ControlChanged(object UIcontrol)
        {
            switch (InsertationDirection)
            {
                case InsertationDirectionEnum.ModelIntoControl:
                    RenewPartPath(txtInsertationTarget, (UIcontrol as CreatePart).GetFullPath());
                    break;
                case InsertationDirectionEnum.ControlIntoModel:
                    RenewPartPath(txtInsertationSubject, (UIcontrol as CreatePart).GetFullPath());
                    break;
                default:
                    break;
            }            
            CheckForEnability();
        }

        private void RenewPartPath(TextBlock txtBlock, string fullPath)
        {
            txtBlock.Text = Path.GetFileName(fullPath);
            txtBlock.ToolTip = fullPath;
        }

        private void CheckForEnability()
        {
            IsActive = ControlNewPart == null ? false : ControlNewPart.IsActive;
            cbAction.IsEnabled = (ControlNewPart != null);
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
                PartDoc derivedPart = null;
                var corePartPath = txtInsertationSubject.ToolTip as string;
                switch (InsertationDirection)
                {
                    case InsertationDirectionEnum.ModelIntoControl:
                        derivedPart = (PartDoc)ControlNewPart.GetModel();
                        break;
                    case InsertationDirectionEnum.ControlIntoModel:
                        //derivedPart = ExistingPart.AsPart();
                        derivedPart = ExistingPartDoc;
                        break;
                    default:
                        break;
                }

                if (derivedPart != null && File.Exists(corePartPath))
                {
                    derivedPart.InsertPart2(corePartPath, INSERT_OPT);
                    
                    if(InsertationDirection==InsertationDirectionEnum.ControlIntoModel)
                    {
                        var err = 0;
                        SolidWorksEnvironment.Application.UnsafeObject.ActivateDoc3(Path.GetFileName(ControlNewPart.GetFullPath()), true, (int)swRebuildOnActivation_e.swRebuildActiveDoc, ref err);
                    }
                }
                else
                {
                    var msg = derivedPart == null ? "Target Part is null, " : string.Empty;
                    msg += "Core Part Path: " + corePartPath;
                    Logger.Log("Problem in InsertPartIntoPart: " + msg, MessageTypeEnum.Error);
                }

                ExistingPartDoc = null;

                //switch (InsertationDirection)
                //{
                //    case InsertationDirectionEnum.ModelIntoControl:
                //        PartDoc derivedPart = (PartDoc)ControlNewPart.GetModel();
                //        var corePartPath = txtInsertationSubject.ToolTip as string;

                //        if (derivedPart != null && File.Exists(corePartPath))
                //        {
                //            derivedPart.InsertPart2(corePartPath, INSERT_OPT);
                //        }
                //        else
                //        {
                //            var msg = derivedPart == null ? "Target Part is null, " : string.Empty;
                //            msg += "Core Part Path: " + corePartPath;

                //            Logger.Log("Problem in InsertPartIntoPart: " + msg, MessageTypeEnum.Error);
                //        }
                //        break;
                //    case InsertationDirectionEnum.ControlIntoModel:
                //        //PartDoc corePart = (PartDoc)ControlNewPart.GetModel();
                //        //var derivedPartPath = txtInsertationSubject.ToolTip as string;

                //        //if (corePart != null && File.Exists(derivedPartPath))
                //        //{
                //        //    corePart.InsertPart2(derivedPartPath, INSERT_OPT);
                //        //}
                //        //else
                //        //{
                //        //    var msg = corePart == null ? "Core Part is null, " : string.Empty;
                //        //    msg += "Target Part Path: " + derivedPartPath;

                //        //    Logger.Log("Problem in InsertPartIntoPart: " + msg, MessageTypeEnum.Error);
                //        //}
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        private void CbAction_Changed(object sender, RoutedEventArgs e)
        {
            stpDetails.IsEnabled = (bool)cbAction.IsChecked;

            RaiseControlChanged(this);
        }
    }
}
