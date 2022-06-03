using AngelSix.SolidDna;
using Dna;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for DirectoryEditor.xaml
    /// </summary>
    public partial class DirectoryEditor : UserControl
    {
        public delegate void OnTextAction(string containerPath);
        private void RaiseNewPathRequested(string containerPath) => NewPathRequested?.Invoke(containerPath);
        public event OnTextAction NewPathRequested;
        private void RaiseDirectoryChanged(string containerPath) => DirectoryChanged?.Invoke(containerPath);
        public event OnTextAction DirectoryChanged;

        private ObservableCollection<PickerDirectory> pickerDirectories = new ObservableCollection<PickerDirectory>();
        private List<string> DirectoryHistory = new List<string>();

        public int DirectoryIndex 
        { 
            get => _DirectoryIndex;
            private set 
            {
                _DirectoryIndex = value;

                if (value > -1 && value < DirectoryHistory.Count)
                    SetDirectory(DirectoryHistory[DirectoryIndex]);
                else
                    Logger.Log("Invalid DirectoryIndex = " + DirectoryIndex.ToString(), MessageTypeEnum.Error);

                btnPrev.IsEnabled = value > 0;
                btnNext.IsEnabled = value < (DirectoryHistory.Count - 1);
            }
        }
        private int _DirectoryIndex = -1;
        public string ModelDirectory
        {
            get { return _ModelDirectory; }
            set 
            {
                if (SelectedPath != value)
                {
                    _ModelDirectory = value;

                    //Clean DirectoryHistory members after current DirectoryIndex
                    if (DirectoryIndex > -1)
                    {
                        var lastIndex = DirectoryHistory.Count - 1;

                        if (DirectoryIndex < lastIndex)
                            DirectoryHistory.RemoveRange(DirectoryIndex, (lastIndex - DirectoryIndex));
                    }

                    //Add new dir to the DirectoryHistory
                    DirectoryHistory.Add(value);
                    //update DirectoryIndex
                    DirectoryIndex++;
                    //select directory
                    SetDirectory(DirectoryHistory[DirectoryIndex]);
                }
            }
        }
        private string _ModelDirectory = string.Empty;
        //public string SelectedPath
        //{
        //    get => LastUseData.Default.SelectedPath;
        //    private set
        //    {
        //        LastUseData.Default.SelectedPath = value;
        //        LastUseData.Default.DeepestPath = pickerDirectories.Last().GetSelectedPath();
        //        LastUseData.Default.Save();
        //    }
        //}
        private string _SelectedPath = string.Empty;
        public string SelectedPath
        {
            get => _SelectedPath;
            private set
            {
                LastUseData.Default.SelectedPath = value;
                LastUseData.Default.DeepestPath = pickerDirectories.Last().GetSelectedPath();
                LastUseData.Default.Save();
                _SelectedPath = value;
                SolidWorksEnvironment.Application.UnsafeObject.SetCurrentWorkingDirectory(value);
            }
        }

        private int SelectedPickerIdx = -1;

        public DirectoryEditor()
        {
            InitializeComponent();
            pickerDirectories.Add(pdRoot);

            //populate list with the possible dirs
            var directoryList = new List<PathData>();
            directoryList.Add(new PathData(AddInSettings.PATH_PROJECTS, "PROJECTS"));
            directoryList.Add(new PathData(AddInSettings.PATH_TOOLBOX, "TOOLBOX"));
            directoryList.Add(new PathData(AddInSettings.PATH_PRINT_PROJ, "3DP MODELS")); 
            pdRoot.SetDirectories(directoryList);

            //fill dir picker list as it was in last session
            if (!LastUseData.Default.DeepestPath.IsNullOrEmpty())
                SetDirectory(LastUseData.Default.DeepestPath, false);
            //SetDirectory(LastUseData.Default.DeepestPath);
            //ModelDirectory = LastUseData.Default.DeepestPath;

            //select dir of the last session
            if (!LastUseData.Default.SelectedPath.IsNullOrEmpty())
                ModelDirectory = LastUseData.Default.SelectedPath;
            //SetDirectory(LastUseData.Default.SelectedPath);

        }

        private void AddDirectory(string newDirectory)
        {
            if(Directory.Exists(newDirectory))
            {
                var newDirPicker = new PickerDirectory();
                newDirPicker.Title = "Sub " + pickerDirectories.Count.ToString();
                newDirPicker.ControlChanged += DirPicker_ControlChanged;
                newDirPicker.SubPathRequested += DirPicker_SubPathRequested;
                newDirPicker.Selected += DirPicker_Selected;
                pickerDirectories.Add(newDirPicker);
                stpSubFolders.Children.Add(newDirPicker);
                newDirPicker.RefreshTargetDirectory(newDirectory, true);
                 
                //eger klasorde numarali alt klasor yoksa
                if (newDirPicker.cmbValue.Items.Count < 1)
                    RemoveDirectoryPicker(newDirPicker);
            }
        }
        private void RemoveDirectoryPicker(PickerDirectory picker)
        {
            picker.ControlChanged -= DirPicker_ControlChanged;
            picker.SubPathRequested -= DirPicker_SubPathRequested;
            stpSubFolders.Children.Remove(picker);
            pickerDirectories.Remove(picker);
        }
        public void SetDirectory(string directory, bool SelectMatch = true)
        {
            //update picker list and find if any picker matches the directory
            var selectionIdx = -1;
            for (int i = 0; i < pickerDirectories.Count; i++)
            {
                pickerDirectories[i].SetSelectedDirectory(directory);
                if (pickerDirectories[i].GetSelectedPath() == directory)
                {
                    selectionIdx = i;
                    //SelectedPath = directory;   //aslinda buna gerek olmamali ama bunsuz olmuyor
                }

            }

            if(SelectMatch)
            {
                //if a match is found, select that picker 
                if (selectionIdx > -1 && selectionIdx < pickerDirectories.Count)
                {
                    pickerDirectories[selectionIdx].IsSelected = true;
                    RaiseDirectoryChanged(directory);
                }
                else
                {
                    RaiseDirectoryChanged(null);
                    //RaiseDirectoryChanged(directory);
                }
            }
        }

        private void DirPicker_Selected(object UIcontrol)
        {
            var selectedControl = UIcontrol as PickerDirectory;
            int newIdx = pickerDirectories.IndexOf(selectedControl);

            if(newIdx!=SelectedPickerIdx)
            {
                //deselect previously selected picker
                if (SelectedPickerIdx > -1 && SelectedPickerIdx < pickerDirectories.Count)
                    pickerDirectories[SelectedPickerIdx].IsSelected = false;

                //update selected picker
                SelectedPickerIdx = newIdx;
            }
            SelectedPath = selectedControl.GetSelectedPath();

            ////deselect previously selected picker
            //if (SelectedPickerIdx > -1 && SelectedPickerIdx < pickerDirectories.Count)
            //    pickerDirectories[SelectedPickerIdx].IsSelected = false;

            ////update selected picker
            //var selectedControl = UIcontrol as PickerDirectory;
            //SelectedPickerIdx = pickerDirectories.IndexOf(selectedControl);
            //SelectedPath = selectedControl.GetSelectedPath();
        }        
        private void DirPicker_SubPathRequested(object UIcontrol)
        {
            RaiseNewPathRequested((UIcontrol as PickerDirectory).GetSelectedPath());
        }        
        private void DirPicker_ControlChanged(object UIcontrol)
        {
            var picker = UIcontrol as PickerDirectory;
            var idx = pickerDirectories.IndexOf(picker);
            if (idx > -1)
            {
                for (int i = idx + 1; i < pickerDirectories.Count; i++)
                {
                    RemoveDirectoryPicker(pickerDirectories[i]);
                    i--;
                }
            }
            AddDirectory(picker.GetSelectedPath());
        }
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            //SetDirectory(SelectedPath);
            //Logger.Log("Path Refreshed " + SelectedPath, MessageTypeEnum.Information);
            try
            {
                var activeDir = Path.GetDirectoryName(SolidWorksEnvironment.Application.ActiveModel.FilePath);
                SetDirectory(activeDir);
                Logger.Log("Path Refreshed with Active Dir " + activeDir, MessageTypeEnum.Information);
            }
            catch(System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
        private void btnPrev_Click(object sender, RoutedEventArgs e) => DirectoryIndex--;
        private void btnNext_Click(object sender, RoutedEventArgs e) => DirectoryIndex++;
    }
}
