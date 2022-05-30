using AngelSix.SolidDna;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for EditorMaterial.xaml
    /// </summary>
    public partial class EditorMaterial : PropertyEditor
    {
        private Material _Value = null;
        public Material Value
        {
            get => _Value;
            set
            {
                if(_Value!=value)
                {
                    _Value = value;
                    headerItem.Header = value.DisplayName;
                    RaiseControlChanged(this);
                }
            }
        }

        public string Title { get => txtLabel.Text; set => txtLabel.Text = value; }

        public EditorMaterial()
        {
            InitializeComponent();
            if (SolidWorksEnvironment.Application != null)
            {
                //tum malzemelerin listesi cekiliyor
                List<Material> mList = SolidWorksEnvironment.Application.GetMaterials();

                //tum malzeme listesi icindeki farkli db'ler cekiliyor
                var dbList = mList.Select(x => x.Database).Distinct();
                foreach (string dbItem in dbList)
                {
                    //siradaki db icin headerItem'a yeni bir subItem ekleniyor
                    var dbName = GetDBName(dbItem);
                    var nextDBItem = new TreeViewItem() { Header = dbName, Tag=dbItem};
                    headerItem.Items.Add(nextDBItem);

                    //siradaki db'ye ait malzemeler cekiliyor
                    var currentDbList = mList.Where(x => x.Database == dbItem);

                    //siradaki db'ye ait malzemeler icindeki farkli Classification'lar cekiliyor
                    var classificationList = mList.Select(x => x.Classification).Distinct();

                    foreach (string clItem in classificationList)
                    {
                        //siradaki classification'a ait malzemeler cekiliyor
                        var currentMaterialList = currentDbList.Where(x => x.Classification == clItem);

                        //eger siradaki classification'a ait malzeme bulunduysa...
                        if (currentMaterialList.Count() > 0)
                        {
                            //siradaki classification icin nextDBItem'a yeni bir subItem ekleniyor
                            var nextCLItem = new TreeViewItem() { Header = clItem };
                            nextDBItem.Items.Add(nextCLItem);

                            foreach (Material mItem in currentMaterialList)
                            {
                                //siradaki malzeme icin nextCLItem'a yeni bir subItem ekleniyor
                                var nextMtItem = new MaterialItem() { AssignedMaterial = mItem };
                                nextMtItem.Selected += Material_Selected;
                                nextCLItem.Items.Add(nextMtItem);
                            }
                        }
                    }
                }

                //cmbValue.ItemsSource = dbList;//SolidWorksEnvironment.Application.GetMaterials();
            }
        }

        private void Material_Selected(object sender, RoutedEventArgs e)
        {
            Value = ((MaterialItem)sender).AssignedMaterial;
            //headerItem.Header = (string)((TreeViewItem)sender).Header;
        }

        private string GetDBName(string dbText)
        {
            //TODO: burasi aslinda Path.GetFileName()
            var startIdx = dbText.LastIndexOf("\\") + 1;
            var nameLength = dbText.LastIndexOf(".") - startIdx;
            return dbText.Substring(startIdx, nameLength);
        }

        private void headerItem_Expanded(object sender, RoutedEventArgs e)
        {
            if(Value!=null)
            {
                //ilgili db node bulunuyor
                var dbItem = new TreeViewItem();
                for (int i = 0; i < headerItem.Items.Count; i++)
                {
                    dbItem = (TreeViewItem)headerItem.Items[i];
                    if (dbItem.Header.ToString() == GetDBName(Value.Database))
                        break;
                }

                //ilgili classification node bulunuyor
                var clItem = new TreeViewItem();
                for (int i = 0; i < dbItem.Items.Count; i++)
                {
                    clItem = (TreeViewItem)dbItem.Items[i];
                    if (clItem.Header.ToString() == Value.Classification)
                        break;
                }

                //ilgili material node bulunuyor
                var mtItem = new MaterialItem();
                for (int i = 0; i < clItem.Items.Count; i++)
                {
                    mtItem = (MaterialItem)clItem.Items[i];
                    if (mtItem.AssignedMaterial.DisplayName == Value.DisplayName)
                        break;
                }

                mtItem.IsSelected = true;
            }
        }

        private void trvMaterials_LostFocus(object sender, RoutedEventArgs e)
        {
            headerItem.IsExpanded = false;
        }
    }
}
