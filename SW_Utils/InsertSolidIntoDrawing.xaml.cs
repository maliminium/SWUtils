using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using System;
using System.IO;
using System.Windows;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for InsertSolidIntoDrawing.xaml
    /// DrawingPalette is not produced for an empty part. Therefore no "create & insert solid into drawing" functionality.
    /// </summary>
    public partial class InsertSolidIntoDrawing : DocumentAction
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
        CreateDrawing ControlDrawing = null;

        public InsertSolidIntoDrawing() => SetTheControls();
        public InsertSolidIntoDrawing(Model solidModel, CreateDrawing drawingControl)
        {
            SetTheControls();

            SolidModel = solidModel;
            ControlDrawing = drawingControl;

            solidModel.ModelSaved += SolidModel_ModelSaved;
            drawingControl.ControlChanged += DrawingControl_ControlChanged;

            RenewSolidPath(solidModel.FilePath);
            RenewDrawingPath(drawingControl.GetFullPath());
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
        private void RenewDrawingPath(string fullPath)
        {
            txtDrawing.Text = Path.GetFileName(fullPath);
            txtDrawing.ToolTip = fullPath;
        }
        private void SolidModel_ModelSaved()
        {
            RenewSolidPath(SolidModel.FilePath);
        }
        private void DrawingControl_ControlChanged(object UIcontrol)
        {
            RenewDrawingPath((UIcontrol as CreateDrawing).GetFullPath());
            CheckForEnability();
        }
        private void CheckForEnability()
        {
            IsActive = (ControlDrawing.IsActive && SolidModel != null);

            //if enabling is meaningless, deactivate control
            cbAction.IsEnabled = IsActive;
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
                try
                {
                    var drawDoc = (DrawingDoc)ControlDrawing.GetModel();
                    var solidPath = txtSolid.ToolTip as string;

                    if (drawDoc != null && File.Exists(solidPath))
                    {

                        var isPaletteCreated = drawDoc.GenerateViewPaletteViews(solidPath);

                        var paletteViewNames = (string[])drawDoc.GetDrawingPaletteViewNames();

                        if (isPaletteCreated)
                        {
                            var sheet = (Sheet)drawDoc.GetCurrentSheet();
                            //m cinsinden kagit boyutlari
                            double H = 0;
                            double W = 0;
                            sheet.GetSize(ref W, ref H);

                            drawDoc.DropDrawingViewFromPalette2(paletteViewNames[0], W / 2, H / 2, 0);
                        }
                        else
                        {
                            Logger.Log("Solid Model seems to be empty", MessageTypeEnum.Warning);
                        }
                    }
                    else
                    {
                        var msg = drawDoc == null ? "Drawing is null, " : string.Empty;
                        msg += "Solid Path: " + solidPath;

                        Logger.Log("Problem in InsertIntoDrawing :" + msg, MessageTypeEnum.Error);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
        }
    }
}
