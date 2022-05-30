using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using static SW_Utils.CommonResources;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for EditorTemplate.xaml
    /// </summary>
    public partial class EditorTemplate : PropertyEditor
    {
        private int oldIdx = -1;

        private List<PathData> SourceList = new List<PathData>();

        private ObjectTypeEnum _TemplateType = ObjectTypeEnum.None;
        public ObjectTypeEnum TemplateType
        {
            get => _TemplateType;
            set
            {
                _TemplateType = value;
                cmbValue.ItemsSource = null;
                switch (value)
                {
                    case ObjectTypeEnum.Project:
                        break;
                    case ObjectTypeEnum.Folder:
                        break;
                    case ObjectTypeEnum.Part:
                        SourceList = GetPathDataList(AddInSettings.DIR_TEMPL_PART, AddInSettings.EXT_PART_T, AddInSettings.EXT_PART);
                        break;
                    case ObjectTypeEnum.Assembly:
                        SourceList = GetPathDataList(AddInSettings.DIR_TEMPL_ASSM, AddInSettings.EXT_ASSM_T, AddInSettings.EXT_ASSM);
                        break;
                    case ObjectTypeEnum.Drawing:
                        SourceList = GetPathDataList(AddInSettings.DIR_TEMPL_DRAW, AddInSettings.EXT_DRAW_T, AddInSettings.EXT_DRAW);
                        break;
                    case ObjectTypeEnum.None:
                        break;
                    default:
                        break;
                }

                cmbValue.ItemsSource = SourceList;

                if (cmbValue.Items.Count > 0)
                    cmbValue.SelectedIndex = 0;
            }
        }

        public string Value
        {
            get
            {
                if (cmbValue.SelectedIndex > -1)
                {
                    return ((PathData)cmbValue.SelectedItem).Txt_Path;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                var correspondingIdx = -1;

                for (int i = 0; i < cmbValue.Items.Count; i++)
                {
                    var nextData = (PathData)cmbValue.Items[i];
                    if (nextData.Txt_Path == value)
                    {
                        correspondingIdx = i;
                        break;
                    }
                }

                if (correspondingIdx < 0)
                {
                    SourceList.Insert(0, new PathData(value));
                    correspondingIdx = 0;
                    oldIdx = -1;    //To raise ControlChanged in SelectionChanged
                }

                cmbValue.Items.Refresh();

                cmbValue.SelectedIndex = correspondingIdx;
            }
        }

        public EditorTemplate()
        {
            InitializeComponent();
        }

        private List<PathData> GetPathDataList(string folderPath, string ext_1, string ext_2)
        {
            var dataList = new List<PathData>();
            try
            {
                var pathList = new List<string>(Directory.GetFiles(folderPath, "*" + ext_1));
                pathList.AddRange(new List<string>(Directory.GetFiles(folderPath, "*" + ext_2)));

                for (int i = 0; i < pathList.Count; i++)
                {
                    var nextData = new PathData(pathList[i]);
                    if (!nextData.Txt_Name.StartsWith("~"))
                        dataList.Add(nextData);
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                throw;
            }
            return dataList;
        }

        private void cmbValue_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if(cmbValue.SelectedIndex!=oldIdx)
            {
                RaiseControlChanged(this);
                oldIdx = cmbValue.SelectedIndex;
            }
        }
    }


}
