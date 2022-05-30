using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
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
using System.Windows.Shapes;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for CheckUI.xaml
    /// </summary>
    public partial class UI_Check : UserControl
    {
        Model model = null;
        private PackAndGo swPackAndGo = null;
        ObservableCollection<ReleaseOpItem> opItems = new ObservableCollection<ReleaseOpItem>();

        public UI_Check()
        {
            InitializeComponent();
            //dtgPackAndGoItems.ItemsSource = opItems;
            if (SolidWorksEnvironment.Application != null)
                SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
        }
        private void Application_ActiveModelInformationChanged(Model obj)
        {
            model = obj;
        }

        private void btnRelease_Click(object sender, RoutedEventArgs e)
        {
            if (model != null)
            {
                //get doc list
                var swPackAndGo = model.UnsafeObject.Extension.GetPackAndGo();

                swPackAndGo.FlattenToSingleFolder = true;
                swPackAndGo.IncludeDrawings = true;
                swPackAndGo.IncludeSuppressed = true;
                swPackAndGo.IncludeToolboxComponents = true;

                swPackAndGo.GetDocumentNames(out object docNames_obj);
                var docNames = (string[])docNames_obj;

                var targetDir = CommonResources.GetNextRelaseDirectory(model.FilePath);

                var items = new ObservableCollection<ReleaseOpItem>();
                for (int i = 0; i < docNames.Length; i++)
                {
                    items.Add(new ReleaseOpItem(docNames[i], targetDir));
                }

                //if (!targetDir.IsNullOrEmpty())
                //{
                //    var action_PaG = new PackAndGoAction(model, targetDir, items);
                //    trvActions.Items.Add(action_PaG);

                //    trvActions.Items.Add(new SaveAsAction(action_PaG));

                //    //swPackAndGo = null;
                //    //model.Dispose();

                //    //for (int i = 0; i < items.Count; i++)
                //    //{
                //    //    items[i].TargetPath_PaG = items[i].OriginalPath;
                //    //}

                //    //trvActions.Items.Add(new SaveAsAction(items));
                //}


                //update rev & save & commit
                //new ctrl -> output list (PackAndGoItem[] (including revNo or TargetName)


                //pack and go
                //PackAndGoAction(model, packAndGo, targetDir) -> output list

                //open & clear & stamp source SVN rev & Save & saveAs (DXF included) & Close
                //SaveAsAction(sourceList, targetList)
                //trvActions.Items.Add(new SaveAsAction());
            }
        }

        private void edTargetDir_ControlChanged(object UIcontrol)
        {

        }
    }
}
