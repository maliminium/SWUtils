using AngelSix.SolidDna;
using Dna;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for PickerDirectory.xaml
    /// </summary>
    public partial class PickerDirectory : PropertyEditor
    {
        public static System.Windows.Media.Brush BR_SELECTED = System.Windows.Media.Brushes.CadetBlue;
        public static System.Windows.Media.Brush BR_NOT_SELECTED = System.Windows.Media.Brushes.Transparent;

        public event CommonResources.UIControlChanged SubPathRequested;
        public event CommonResources.UIControlChanged Selected;

        public bool IsActive 
        { 
            get => cmbValue.IsEnabled; 
            set => cmbValue.IsEnabled = value;
        }

        public bool IsSelected
        {
            get => Background == BR_SELECTED;
            set
            {
                if (IsSelected != value)
                {
                    Background = value ? BR_SELECTED : BR_NOT_SELECTED;
                    if (value)
                        RaiseSelected();
                }
            }
        }

        public string Title { get => txtTitle.Text; set => txtTitle.Text = value; }
        private FileSystemWatcher Watcher = null;
        private string TargetDirectory = string.Empty;

        public PickerDirectory()
        {
            SetTheControls();
        }

        private void RaiseSubPathRequested() => SubPathRequested?.Invoke(this);
        private void RaiseSelected() => Selected?.Invoke(this);

        /// <summary>
        /// sets control defaults
        /// </summary>
        private void SetTheControls()
        {
            InitializeComponent();
            IsActive = false;
        }

        public void SetDirectories(List<PathData> directories)
        {
            TurnOffWatcher();

            var directoryList = new List<PathData>();

            for (int i = 0; i < directories.Count; i++)
            {
                if (Directory.Exists(directories[i].Txt_Path))
                {
                    directoryList.Add(directories[i]);
                }
            }

            RefreshComboBox(directoryList, 0);

            TargetDirectory = string.Empty;
            
            IsActive = true;
            
        }

        /// <summary>
        /// Refreshes control content and FileSystemWatcher
        /// </summary>
        /// <param name="directory">source directory</param>
        /// <param name="selectFirst">if true, selects first item after refill (default: false)</param>
        public void RefreshTargetDirectory(string directory, bool selectFirst = false)
        {
            if (!directory.IsNullOrEmpty() && Directory.Exists(directory))
            {
                RefreshWatcher(directory);

                var directoryList = PathData.GetNumeratedPaths(directory);
                var finalIdx = selectFirst ? 0 : directoryList.Count - 1;

                RefreshComboBox(directoryList, finalIdx);

                TargetDirectory = directory;
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }
        }

        /// <summary>
        /// Populates the ComboBox with a single directory and turns off the Watcher
        /// </summary>
        /// <param name="directory"></param>
        public void ForceSelectDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                TurnOffWatcher();

                var directoryList = new List<PathData>();
                directoryList.Add(new PathData(directory));

                RefreshComboBox(directoryList, 0);

                TargetDirectory = string.Empty;
                IsActive = true;
            }
            else
            {
                IsActive = false;
            }
        }

        /// <summary>
        /// Selects the appropriate item in the combobox according to the path input
        /// </summary>
        /// <param name="modelPath"></param>
        public void SetSelectedDirectory(string modelPath)
        {
            var correspondingIdx = -1;


            //TODO: .../15 yerine .../1'i seciyor
            for (int i = 0; i < cmbValue.Items.Count; i++)
            {
                var nextPath = (cmbValue.Items[i] as PathData).Txt_Path;
                if (modelPath.Equals(nextPath))
                {
                    correspondingIdx = i;
                    break;
                }
            }
            if (correspondingIdx<0)
            {
                for (int i = 0; i < cmbValue.Items.Count; i++)
                {
                    var nextPath = (cmbValue.Items[i] as PathData).Txt_Path;
                    if (modelPath.StartsWith(nextPath))
                    {
                        correspondingIdx = i;
                        break;
                    }
                }
            }

            //if modelPath is not found then select nothing
            if (correspondingIdx > -1)
                cmbValue.SelectedIndex = correspondingIdx;
        }

        

        public string GetSelectedPath() => cmbValue.SelectedItem == null ? null : (cmbValue.SelectedItem as PathData).Txt_Path;

        private void RefreshWatcher(string directory)
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Changed -= Folder_Changed;
                Watcher.Created -= Folder_Changed;
                Watcher.Deleted -= Folder_Changed;
                Watcher.Renamed -= Folder_Changed;
            }

            if (Directory.Exists(directory))
            {
                Watcher = new FileSystemWatcher(directory);

                Watcher.NotifyFilter = NotifyFilters.LastAccess
                                     | NotifyFilters.LastWrite
                                     | NotifyFilters.FileName
                                     | NotifyFilters.DirectoryName;

                Watcher.Changed += Folder_Changed;
                Watcher.Created += Folder_Changed;
                Watcher.Deleted += Folder_Changed;
                Watcher.Renamed += Folder_Changed;

                Watcher.EnableRaisingEvents = true;
            }
        }

        private void TurnOffWatcher()
        {
            if (Watcher != null)
            {
                Watcher.EnableRaisingEvents = false;
                Watcher.Changed -= Folder_Changed;
                Watcher.Created -= Folder_Changed;
                Watcher.Deleted -= Folder_Changed;
                Watcher.Renamed -= Folder_Changed;
                Watcher = null;
            }
        }

        private void RefreshComboBox(List<PathData> directoryList, int finalIdx)
        {
            if (Dispatcher.CheckAccess())
            {
                cmbValue.ItemsSource = directoryList;
                cmbValue.SelectedIndex = finalIdx;
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    cmbValue.ItemsSource = directoryList;
                    cmbValue.SelectedIndex = finalIdx;
                });
            }
        }

        private void RefreshComboBox(List<PathData> directoryList)
        {
            if (Dispatcher.CheckAccess())
            {
                var selectedPath = GetSelectedPath();
                cmbValue.ItemsSource = directoryList;
                if (!selectedPath.IsNullOrEmpty())
                    SetSelectedDirectory(selectedPath);
            }
            else
            {
                Dispatcher.Invoke(() =>
                {
                    var selectedPath = GetSelectedPath();
                    cmbValue.ItemsSource = directoryList;
                    if (!selectedPath.IsNullOrEmpty())
                        SetSelectedDirectory(selectedPath);
                });
            }
        }

        private void Folder_Changed(object sender, FileSystemEventArgs e)
        {
            var directoryList = PathData.GetNumeratedPaths(TargetDirectory);//IsProject ? PathData.GetSortedProjectPaths(TargetDirectory) : PathData.GetSortedPaths(TargetDirectory);
            RefreshComboBox(directoryList);
        }

        private void cmbValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ToolTip = GetSelectedPath();
            RaiseControlChanged(this);
            //IsSelected = true;

            //btnNew.Visibility = cmbValue.SelectedIndex > -1 ? Visibility.Hidden : Visibility.Visible;
        }

        private void cmbValue_PreviewMouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (cmbValue.SelectedItem != null)
            {
                //SolidWorksEnvironment.Application.UnsafeObject.SetCurrentWorkingDirectory(((PathData)cmbValue.SelectedItem).Txt_Path);  //Aslinda select event duzgun calistigi surece buna gerek yok
                //RaiseSelected();
                IsSelected = true;
                SolidWorksEnvironment.Application.UnsafeObject.Command((int)swCommand_e.swFileOpen, null);
            }

            //if (cmbValue.SelectedItem != null)
            //    CommonResources.OpenFolder(((PathData)cmbValue.SelectedItem).Txt_Path);
        }

        private void btnSub_Click(object sender, RoutedEventArgs e)
        {
            RaiseSubPathRequested();
        }

        private void txtTitle_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //IsSelected = !IsSelected;
            IsSelected = true;
        }

        private void cmbValue_DropDownClosed(object sender, System.EventArgs e)
        {
            IsSelected = true;
        }

        private void btnOpenFolder_Click(object sender, RoutedEventArgs e)
        {
            if (cmbValue.SelectedItem != null)
            {
                var dirPath = ((PathData)cmbValue.SelectedItem).Txt_Path;
                if (Directory.Exists(dirPath))
                    CommonResources.OpenFolder(dirPath);
            }
                
        }
    }
}
