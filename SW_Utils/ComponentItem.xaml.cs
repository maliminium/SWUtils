using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for ComponentItem.xaml
    /// </summary>
    public partial class ComponentItem : TreeViewItem
    {
        public delegate void OnStateChanged();
        public event OnStateChanged StateChanged;
        private void RaiseStateChanged()
        {
            StateChanged?.Invoke();
        }

        public static Brush BR_BACK_EDIT = Brushes.White;
        public static Brush BR_BACK_TRANSPARENT = Brushes.Transparent;
        public static System.Windows.Thickness BRD_T_EDIT = new System.Windows.Thickness(1);
        public static System.Windows.Thickness BRD_T_STANDART = new System.Windows.Thickness(0);
        public static Brush BR_BACK_CHANGED = Brushes.SlateGray;

        public bool IsBlank { get; private set; } = true;

        private string _TargetPath_old = string.Empty;
        public bool IsEdit
        {
            get => cbIsActive.Visibility == System.Windows.Visibility.Collapsed;
            set
            {
                if(value)
                {
                    _TargetPath_old = TargetPath;

                    cbIsActive.Visibility = System.Windows.Visibility.Collapsed;

                    txtSourcePfx.Visibility = System.Windows.Visibility.Collapsed;
                    txtSource.Visibility = System.Windows.Visibility.Collapsed;
                    txtSourceSfx.Visibility = System.Windows.Visibility.Collapsed;

                    btnCancel.Visibility = System.Windows.Visibility.Visible;

                    btnDeleteNo.Visibility = System.Windows.Visibility.Visible;
                    btnDeleteText.Visibility = System.Windows.Visibility.Visible;
                    edTargetDir.Visibility = System.Windows.Visibility.Visible;

                    txtTargetText.IsReadOnly = false;
                    txtTargetText.Background = BR_BACK_EDIT;
                    txtTargetText.BorderThickness = BRD_T_EDIT; //IsReadOnly="True" Background="Transparent" BorderThickness="0" 
                }
                else
                {
                    btnCancel.Visibility = System.Windows.Visibility.Collapsed;

                    btnDeleteNo.Visibility = System.Windows.Visibility.Collapsed;
                    btnDeleteText.Visibility = System.Windows.Visibility.Collapsed;
                    edTargetDir.Visibility = System.Windows.Visibility.Collapsed;

                    cbIsActive.Visibility = System.Windows.Visibility.Visible;

                    txtSourcePfx.Visibility = System.Windows.Visibility.Visible;
                    txtSource.Visibility = System.Windows.Visibility.Visible;
                    txtSourceSfx.Visibility = System.Windows.Visibility.Visible;

                    txtTargetText.IsReadOnly = true;
                    txtTargetText.Background = BR_BACK_TRANSPARENT;
                    txtTargetText.BorderThickness = BRD_T_STANDART;

                    if(TargetPath!=_TargetPath_old)
                    {
                        dpItem.Background = BR_BACK_CHANGED;
                        RaiseStateChanged();
                    }

                    //var newTargetPath = edTargetDir.Value;
                    //if (txtTargetNo.Text == string.Empty)
                    //{
                    //    newTargetPath += Path.DirectorySeparatorChar + txtTargetText.Text;
                    //}
                    //else
                    //{
                    //    if (txtTargetText.Text == string.Empty)
                    //    {
                    //        newTargetPath += Path.DirectorySeparatorChar + txtTargetNo.Text;
                    //    }
                    //    else
                    //    {
                    //        newTargetPath += Path.DirectorySeparatorChar + txtTargetNo.Text + AddInSettings.SEP_TEXT + txtTargetText.Text;
                    //    }
                    //}
                    //newTargetPath += txtExtText.Text;


                    ////TODO: Acaba bu islemler burada mi olmali?
                    //if (TargetPath != newTargetPath)
                    //{
                    //    TargetPath = newTargetPath;
                    //    dpItem.Background = BR_BACK_CHANGED;
                    //    RaiseStateChanged();
                    //}
                }
            }
        }

        public bool IsActive { get => cbIsActive.IsChecked == true; set => cbIsActive.IsChecked = value; }
        public bool IsDrawing { get => SourcePath.EndsWith(AddInSettings.EXT_DRAW); }   //TODO: Ya lowercase ise?

        private string _SourceName = string.Empty;
        private string _SourceDir = string.Empty;

        public string SourcePath
        {
            get
            {
                //return txtSource.ToolTip.ToString();
                return _SourceDir + Path.DirectorySeparatorChar + _SourceName;
            }
            set
            {
                //txtSource.Text = Path.GetFileName(value);
                //if (CommonResources.IsToolboxComponentPath(value))
                //    IsActive = false;
                //txtSource.ToolTip = value;

                _SourceName = Path.GetFileName(value);
                _SourceDir = Path.GetDirectoryName(value);
                
                if (CommonResources.IsToolboxComponentPath(value))
                    IsActive = false;

                txtSource.Text = _SourceName;                
            }
        }

        public string TargetPath
        {
            get
            {
                //return txtTarget.ToolTip.ToString();
                //return txtTargetNo.ToolTip == null ? string.Empty : txtTargetNo.ToolTip.ToString();

                var fileName = string.Empty;

                if (txtTargetNo.Text != string.Empty && txtTargetText.Text != string.Empty)
                    fileName = txtTargetNo.Text + AddInSettings.SEP_TEXT + txtTargetText.Text;
                else
                    fileName = txtTargetNo.Text != string.Empty ? txtTargetNo.Text : txtTargetText.Text;

                return edTargetDir.Value + Path.DirectorySeparatorChar + fileName + txtExtText.Text;
            }
            set
            {
                //txtTarget.Text = Path.GetFileName(value);
                //txtTarget.ToolTip = value;

                edTargetDir.Value = Path.GetDirectoryName(value);

                var fileName = Path.GetFileName(value);
                txtTargetNo.Text = CommonResources.GetPrefixFromName(fileName);
                var nameBlock = CommonResources.GetNameBlock(fileName);
                txtSepText.Visibility = nameBlock.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                txtTargetText.Text = nameBlock;
                txtExtText.Text = Path.GetExtension(fileName);
                //var nameBlock = CommonResources.GetNameBlock(value);
                //txtTargetNo.Text = CommonResources.GetPrefixFromName(value);
                //txtSepText.Visibility = nameBlock.Length > 0 ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
                //txtTargetText.Text = nameBlock;
                //txtExtText.Text = Path.GetExtension(value);
                //if (nameBlock.StartsWith(AddInSettings.SEP_TEXT))
                //{
                //    txtSepText.Visibility = System.Windows.Visibility.Visible;
                //    //txtTargetText.Text = nameBlock.Substring(1);
                //    nameBlock = nameBlock.Substring(1);
                //}
                //else
                //{
                //    txtSepText.Visibility = System.Windows.Visibility.Collapsed;
                //    //txtTargetText.Text = nameBlock;
                //}

                //txtTargetText.Text = Path.GetFileNameWithoutExtension(nameBlock);
                //txtExtText.Text = Path.GetExtension(nameBlock);

                //txtTargetNo.ToolTip = value;
                //txtTargetText.ToolTip = value;
                //edTargetDir.Value = Path.GetDirectoryName(value);


                ToolTip = $"TargetName: {fileName}\nTargetPath: {edTargetDir.Value}\n\nSourceName: {_SourceName}\nSourcePath: {_SourceDir}";

                //Updating child target paths accordingly
                var childCounter = AddInSettings.SEQUENCE_START;
                //var startBlock = Path.GetDirectoryName(value) + Path.DirectorySeparatorChar + CommonResources.GetPrefixFromName(txtTarget.Text);
                var startBlock = Path.GetDirectoryName(value) + Path.DirectorySeparatorChar + txtTargetNo.Text;
                for (int i = 0; i < Items.Count; i++)
                {
                    var child = Items[i] as ComponentItem;
                    if (child.IsActive && child.IsEnabled)
                    {
                        //var endBlock = CommonResources.GetNameBlock(child.SourcePath);
                        //var endBlock = IsBlank ? CommonResources.GetNameBlock(child.SourcePath) : CommonResources.GetNameBlock(child.TargetPath);
                        var childName = IsBlank ? Path.GetFileName(child.SourcePath) : Path.GetFileName(child.TargetPath);
                        var childNameBlock = CommonResources.GetNameBlock(childName);
                        var endBlock = childNameBlock.Length > 0 ? AddInSettings.SEP_TEXT + childNameBlock : string.Empty;
                        endBlock += Path.GetExtension(childName);
                        if (child.IsDrawing)
                        {
                            child.TargetPath = GetDrawingPath(value);
                        }
                        else
                        {
                            child.TargetPath = startBlock + AddInSettings.SEP_FILE_NUMERATION + childCounter.ToString() + endBlock;
                            childCounter++;
                        }
                    }
                    else
                    {
                        //TODO: Burasi PackAndGo'ya etki etmez ama dogru oldugundan emin degilim
                        child.TargetPath = Path.GetDirectoryName(value) + Path.DirectorySeparatorChar + Path.GetFileName(child.SourcePath);
                    }
                }

                if (TargetPath != value)
                    Logger.Log(TargetPath + "\n" + value + "\n", MessageTypeEnum.Error);
                //else
                //    Logger.Log(TargetPath+ "\n", MessageTypeEnum.Greeting);

                IsBlank = false;

                //if (value!=_TargetPath_old)
                //{

                //    _TargetPath_old = value;
                //    //dpItem.Background = BR_BACK_CHANGED;
                //    //RaiseStateChanged();

                //}

                
            }
        }

        public ComponentItem()
        {
            InitializeComponent();
            SetTheControls();
        }

        public ComponentItem(string sourcePath)
        {
            InitializeComponent();
            SourcePath = sourcePath;
            SetTheControls();
        }

        public ComponentItem(Model motherModel, bool includeDrawings)
        {
            InitializeComponent();
            SourcePath = motherModel.FilePath;

            if (motherModel.IsDrawing)
            {
                try
                {
                    var swDrawDoc = ((DrawingDoc)motherModel.UnsafeObject);
                    var sheets = (object[])swDrawDoc.GetViews();
                    var views = new List<View>();
                    for (int i = 0; i < sheets.Length; i++)
                    {
                        var nextView_arr = (object[])sheets[i];
                        if(nextView_arr.Length>1)
                        {
                            for (int j = 1; j < nextView_arr.Length; j++)
                            {
                                views.Add((View)nextView_arr[j]);
                            }
                        }
                    }
                    for (int i = 0; i < views.Count; i++)
                    {
                        //Items.Add(new ComponentItem(views[i].ReferencedDocument, includeDrawings));
                        AddItem(new ComponentItem(views[i].ReferencedDocument, includeDrawings));
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.LogException(ex);
                }
            }
            else
            {
                var children = GetComponents(motherModel.UnsafeObject.FirstFeature(), includeDrawings);
                for (int i = 0; i < children.Count; i++)
                {
                    //Items.Add(children[i]);
                    AddItem(children[i]);
                }

                if (includeDrawings)
                {
                    var drawingPath = GetDrawingPath(SourcePath);
                    if (File.Exists(drawingPath))
                    {
                        //Items.Add(new ComponentItem(drawingPath));
                        AddItem(new ComponentItem(drawingPath));
                    }
                        
                }
            }
            SetTheControls();
        }

        public ComponentItem(ModelDoc2 motherModelDoc, bool includeDrawings)
        {
            InitializeComponent();
            SourcePath = motherModelDoc.GetPathName();

            var children = GetComponents(motherModelDoc.FirstFeature(), includeDrawings);
            for (int i = 0; i < children.Count; i++)
            {
                //Items.Add(children[i]);
                AddItem(children[i]);
            }

            if (includeDrawings)
            {
                var drawingPath = GetDrawingPath(SourcePath);
                if (File.Exists(drawingPath))
                {
                    //Items.Add(new ComponentItem(drawingPath));
                    AddItem(new ComponentItem(drawingPath));
                }

            }

            SetTheControls();
        }

        public ComponentItem(IComponent2 underlyingComponent, bool includeDrawings)
        {
            InitializeComponent();
            SourcePath = underlyingComponent.GetPathName();

            var children = GetComponents(underlyingComponent.FirstFeature(), includeDrawings);
            for (int i = 0; i < children.Count; i++)
            {
                //Items.Add(children[i]);
                AddItem(children[i]);
            }

            if (includeDrawings)
            {
                var drawingPath = Path.GetDirectoryName(SourcePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(SourcePath) + AddInSettings.EXT_DRAW;
                if (File.Exists(drawingPath))
                {
                    //Items.Add(new ComponentItem(drawingPath));
                    AddItem(new ComponentItem(drawingPath));
                }
            }
            SetTheControls();
        }

        public ComponentItem(IDerivedPartFeatureData featureData, bool includeDrawings)
        {
            InitializeComponent();
            SourcePath = featureData.PathName;

            if (includeDrawings)
            {
                var drawingPath = Path.GetDirectoryName(SourcePath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(SourcePath) + AddInSettings.EXT_DRAW;
                if (File.Exists(drawingPath))
                {
                    //Items.Add(new ComponentItem(drawingPath));
                    AddItem(new ComponentItem(drawingPath));
                }
                    
            }

            SetTheControls();
        }

        private void SetTheControls()
        {
            IsEdit = false;
            dpItem.Background = BR_BACK_TRANSPARENT;
        }

        private void AddItem(ComponentItem item)
        {
            Items.Add(item);
            item.StateChanged += Item_StateChanged;
        }

        private void Item_StateChanged()
        {
            RaiseStateChanged();
        }

        private static List<ComponentItem> GetComponents(object feature_obj, bool includeDrawings)
        {
            var results = new List<ComponentItem>();

            if (feature_obj != null)
            {
                var feature = (Feature)feature_obj;

                var typeName = feature.GetTypeName2();
                if (typeName == "Reference")
                {
                    var comp = feature.GetSpecificFeature2() as IComponent2;
                    if (comp == null)
                        Logger.Log("RefF- " + feature.Name, MessageTypeEnum.Error);
                    else
                    {
                        results.Add(new ComponentItem(comp, includeDrawings));
                    }

                }
                else if (typeName == "Stock")
                {
                    var comp_obj = feature.GetDefinition();
                    var comp = comp_obj as IDerivedPartFeatureData;

                    if (comp == null)
                        Logger.Log("StoF- " + feature.Name, MessageTypeEnum.Error);
                    else
                    {

                        results.Add(new ComponentItem(comp, includeDrawings)); 
                        //results.Add(new ComponentItem(comp.PathName));
                        //results.Add(new ComponentItem(comp.GetModelDoc(), includeDrawings)); //.GetModelDoc() hep null geliyor. Pes ediyorum. DerivedPartlarin hiyerarsisi yok.
                    }
                }

                results.AddRange(GetComponents(feature.GetNextFeature(), includeDrawings));
            }

            return results;
        }

        public static string GetDrawingPath(string modelPath)
        {
            return Path.GetDirectoryName(modelPath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(modelPath) + AddInSettings.EXT_DRAW;
        }

        public List<string> GetDocNames(List<string> currentList)
        {
            var nextList = new List<string>(currentList);
            
            if (nextList.IndexOf(SourcePath) < 0)
            {
                nextList.Add(SourcePath);
            }
            else
            {
                IsEnabled = false;
                IsActive = false;
            }

            for (int i = 0; i < Items.Count; i++)
            {
                nextList = (Items[i] as ComponentItem).GetDocNames(nextList);
            }

            return nextList;
        }

        public List<string> GetSaveToNames(List<string> currentList)
        {
            var nextList = new List<string>(currentList);

            if (IsEnabled)
            {
                nextList.Add(IsActive ? TargetPath : string.Empty);
            }

            for (int i = 0; i < Items.Count; i++)
            {
                nextList = (Items[i] as ComponentItem).GetSaveToNames(nextList);
            }

            return nextList;
        }

        private void txtTarget_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (IsActive && e.ClickCount > 1 && (!IsEdit))
            {
                //Logger.Log("Double Click");
                IsEdit = true;
                e.Handled = true;
            }
        }

        private void cbIsActive_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            //TODO: Burasi eski isareti hairlayacak sekilde gelistirilebilir

            RaiseStateChanged();
        }

        private void cbIsActive_Unchecked(object sender, System.Windows.RoutedEventArgs e)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                (Items[i] as ComponentItem).IsActive = false;
            }
            RaiseStateChanged();
        }

        /// <summary>
        /// Crops the text part (part after <see cref="AddInSettings.SEP_TEXT"/>) of the file name of this item and all its children
        /// </summary>
        /// <param name="cropAfter">cropStart index (0 means full clear)</param>
        public void CropText(int cropAfter)
        {
            if(txtTargetNo.Text.Length>0)
            {
                IsEdit = true;
                if (txtTargetText.Text.Length > cropAfter)
                    txtTargetText.Text = txtTargetText.Text.Substring(0, cropAfter);
                IsEdit = false;
            }

            foreach (ComponentItem item in Items)
            {
                item.CropText(cropAfter);
            }
        }

        private void btnDeleteText_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtTargetText.Text = string.Empty;
        }

        private void btnDeleteNo_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            txtTargetNo.Text = string.Empty;
        }

        private void txtTargetText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                IsEdit = false;
            else if (e.Key == Key.Escape)
                CancelEdit();
        }

        private void CancelEdit()
        {
            TargetPath = TargetPath;
            IsEdit = false;
        }

        private void btnCancel_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CancelEdit();
        }

        private void TreeViewItem_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key==Key.Enter)
            {
                IsEdit = false;
            }
        }
    }
}
