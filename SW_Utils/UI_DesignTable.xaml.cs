using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Excel = Microsoft.Office.Interop.Excel;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for DesignTableUI.xaml
    /// </summary>
    public partial class UI_DesignTable : UserControl
    {
        public UI_DesignTable()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
            }
            catch
            {

            }
            
        }

        private void Application_ActiveModelInformationChanged(Model obj)
        {
            try
            {
                var model = SolidWorksEnvironment.Application.ActiveModel;
                if (model!=null)
                {
                    var swModel = model.UnsafeObject;
                    if (swModel.Extension.HasDesignTable())
                    {
                        btnUpdateAndOpen.IsEnabled = true;
                        txtParams.Text = string.Empty;
                    }
                    else
                    {
                        btnUpdateAndOpen.IsEnabled = false;
                        txtParams.Text = "Design table not found";
                    }
                }
            }
            catch
            {

            }
        }

        private void btnUpdateAndOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = SolidWorksEnvironment.Application.ActiveModel;
                var swModel = model.UnsafeObject;
                var swDesTable = (DesignTable)swModel.GetDesignTable();

                //ModelConfiguration conf = model.ActiveConfiguration;
                //conf.UnsafeObject.

                //var excelSheet = (Excel.Worksheet)swDesTable.EditTable2((bool)cbOpenInNewWindow.IsChecked);
                var excelSheet = (Excel.Worksheet)swDesTable.EditTable2(false);
                var nTotCol = swDesTable.GetTotalColumnCount();
                List<string> colList = new List<string>();
                for (int i = 0; i < nTotCol; i++)
                {
                    colList.Add(swDesTable.GetEntryText(0, i + 1));
                }

                List<string> featList = new List<string>();
                List<string> dimList = new List<string>();
                model.Features(delegate (ModelFeature modelFeature, int featureDepth)
                {
                    if (modelFeature.FeatureName.StartsWith("cnf"))
                        featList.Add("$STATE@" + modelFeature.FeatureName);

                    if (modelFeature.IsSketch)
                    {
                        var swFeature = modelFeature.UnsafeObject;
                        var swDispDim = (DisplayDimension)swFeature.GetFirstDisplayDimension();
                        while (swDispDim != null)
                        {
                            var swDim = swDispDim.GetDimension2(0);

                            if (swDim.Name.StartsWith("cnf"))
                            {
                                var dimName = swDim.Name + "@" + modelFeature.FeatureName;

                                if (!dimList.Exists(s => s == dimName))
                                    dimList.Add(dimName);
                            }

                            swDispDim = (DisplayDimension)swFeature.GetNextDisplayDimension(swDispDim);
                        }
                    }
                });

                List<string> addList = new List<string>();
                addList.AddRange(featList.Except(colList));
                addList.AddRange(dimList.Except(colList));

                //Logger.Log("EXISTING COLUMNS");
                //for (int i = 0; i < colList.Count; i++)
                //{
                //    Logger.Log(colList[i]);
                //}

                //Logger.Log("MARKED FEATURES");
                //for (int i = 0; i < featList.Count; i++)
                //{
                //    Logger.Log(featList[i]);
                //}

                //Logger.Log("MARKED DIMENSIONS");
                //for (int i = 0; i < dimList.Count; i++)
                //{
                //    Logger.Log(dimList[i]);
                //}

                //Logger.Log("COLUMNS TO ADD");
                //for (int i = 0; i < addList.Count; i++)
                //{
                //    Logger.Log(addList[i]);
                //}

                Logger.Log("COLUMNS ADDED");
                for (int i = 0; i < addList.Count; i++)
                {
                    excelSheet.Cells[2, nTotCol+2] = addList[i];
                    Logger.Log(addList[i]);
                }
                Logger.Log("\r");

                swDesTable.UpdateTable((int)swDesignTableUpdateOptions_e.swUpdateDesignTableAll, true);

                swDesTable.EditTable2((bool)cbOpenInNewWindow.IsChecked);
                //int row = 2;
                //int col = nTotCol + 2;
                //string newData = addList[0];

                //excelSheet.Cells[row, col] = newData;
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var model = SolidWorksEnvironment.Application.ActiveModel;
                var swModel = model.UnsafeObject;

                
                //var parameters = new List<string>();
                //var values = new List<string>();


                //conf.UnsafeObject.GetParameters(out parameters, out values);

                var configurations = (string[])swModel.GetConfigurationNames();
                for (int i = 0; i < configurations.Length; i++)
                {
                    var conf = (Configuration)swModel.GetConfigurationByName(configurations[i]);
                    var parameters = new object();
                    var values = new object();
                    conf.GetParameters(out parameters, out values);

                    var parList = (string[])parameters;
                    var valList = (string[])values;

                    Logger.Log(conf.Name, MessageTypeEnum.Normal);
                    for (int j = 0; j < parList.Length; j++)
                    {
                        Logger.Log($"\tP: {parList[j]}  -  V: {valList[j]}", MessageTypeEnum.Normal);
                    }
                    Logger.Log("\r");

                }

                //model.UnsafeObject.ConfigurationManager.GetConfigurationParams("name", out parameters, out values);

                //var parList = (string[])parameters;
                //var valList = (string[])values;

                //for (int i = 0; i < parList.Length; i++)
                //{
                //    rtbOut.AppendText($"P: {parList[i]}  -  V: {valList[i]} \n");
                //}

                //var conf = (Configuration)swModel.GetConfigurationByName("Default");
            }
            catch (System.Exception ex)
            {
                Logger.LogException(ex);
            }
        }
    }
}
