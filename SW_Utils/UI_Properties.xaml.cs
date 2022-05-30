using AngelSix.SolidDna;
using Dna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for UI_Properties.xaml
    /// </summary>
    /// 
    public partial class UI_Properties : UserControl
    {
        Model model = null;


        //TODO: Bunlar AddInSettings'e aktarilmali!!! Part, Assembly, Drawing icin ayri ayri KeyValuePair listler halinde
        public const string CP_Design = "Tasarim";
        public const string CP_PartNo = "ParcaNo";
        public const string CP_Material = "Malzeme";
        public const string CP_Coating = "Kaplama";
        public const string CP_Paint = "Boya";
        public const string CP_Quantity = "Adet";
        public const string CP_Description = "Aciklama";

        public const string CP_STLConjugate = "STLConj";


        public const string CP_DftMaterial = "VarsayilanMalzeme";

        public const string DFT_Designer = "C. M. Ali ÇİFTÇİ";
        //public const string DFT_Material = "-";
        public const string DFT_Quantity = "1";
        public const string DFT_Value = "-";

        public const string STR_True = "T";
        public const string STR_False = "F";

        public UI_Properties()
        {
            InitializeComponent();
            if (SolidWorksEnvironment.Application != null)
            {
                SolidWorksEnvironment.Application.ActiveModelInformationChanged += Application_ActiveModelInformationChanged;
                SolidWorksEnvironment.Application.ActiveFileSaved += Application_ActiveFileSaved;
            }
        }

        private void RefreshControl()
        {
            try
            {
                ThreadHelpers.RunOnUIThread(() =>
                {
                    if (model != null && model.UnsafeObject != null)
                    {
                        cmbConf.ItemsSource = null;
                        cmbConf.Items.Clear();
                        cmbConf.ItemsSource = (string[])model.UnsafeObject.GetConfigurationNames();

                        dtgPropConf.ItemsSource = null;
                        dtgPropConf.Items.Clear();
                        dtgPropConf.Items.Refresh();

                        model.CustomProperties(delegate (List<CustomProperty> pList)
                        {
                            List<DummyProp> sourceList = DummyProp.GetDummySet(model, null, pList);
                            if (sourceList != null)
                                dtgPropGen.ItemsSource = sourceList;
                            //dtgPropGen.ItemsSource = DummyProp.GetDummySet(model, null, pList);
                            //dtgPropGen.ItemsSource = pList;
                        });

                        if(cmbConf.Items.Count>0)
                        {
                            cmbConf.SelectedIndex = 0;
                        }

                        cbHasSTLConjugate.IsChecked = model.GetCustomProperty(CP_STLConjugate) == STR_True ? true : false;
                    }
                    else
                    {
                        dtgPropGen.ItemsSource = null;
                        dtgPropConf.ItemsSource = null;
                        cmbConf.ItemsSource = null;
                        cmbConf.Items.Clear();
                        cbHasSTLConjugate.IsChecked = false;
                    }

                    //TODO: bu refreshler, null'a cekmeler gereksiz olabilir, ya da SelectConfiguration'da da gerekli olabilir
                    dtgPropGen.Items.Refresh();
                    dtgPropConf.Items.Refresh();
                    cmbConf.Items.Refresh();
                });
                
            }
            catch (Exception ex) 
            {
                Logger.LogException(ex);
            }
        }

        bool isSaveEventActive = true;

        private void Application_ActiveFileSaved(string arg1, Model arg2)
        {
            try
            {
                if (arg2.IsPart && isSaveEventActive)
                {
                    bool hasSTLConj = arg2.GetCustomProperty(CP_STLConjugate) == STR_True ? true : false;
                    if (hasSTLConj)
                    {
                        //SldWorks sldWorks = SolidWorksEnvironment.Application.UnsafeObject;

                        //var model = arg2.UnsafeObject;

                        var originalExt = Path.GetExtension(arg1);
                        var targetExt = ".STL";
                        var targetPath = arg1.Replace(originalExt, targetExt);

                        isSaveEventActive = false;

                        var errSave = 0;
                        var warSave = 0;
                        //var result = model.Extension.SaveAs(
                        //    targetPath,
                        //    (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                        //    (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                        //    null,
                        //    ref errSave,
                        //    ref warSave
                        //    );
                        var result = arg2.UnsafeObject.Extension.SaveAs(
                            targetPath,
                            (int)swSaveAsVersion_e.swSaveAsCurrentVersion,
                            (int)swSaveAsOptions_e.swSaveAsOptions_Silent,
                            null,
                            ref errSave,
                            ref warSave
                            );

                        isSaveEventActive = true;

                        var title = Path.GetFileName(targetPath);//model.GetTitle();
                        //sldWorks.CloseDoc(title);
                        SolidWorksEnvironment.Application.UnsafeObject.CloseDoc(title);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        private void SelectConfiguration(int newIndex)
        {
            try
            {
                //if (cmbConf.Items.Count > 0 && newIndex > -1)
                //{
                //    var confName = (string)cmbConf.Items[newIndex];
                //    if (!confName.IsNullOrEmpty())
                //    {
                //        ThreadHelpers.RunOnUIThread(() =>
                //        {
                //            //var model = SolidWorksEnvironment.Application.ActiveModel;

                //            model.CustomProperties(delegate (List<CustomProperty> pList)
                //            {
                //                //TODO: Buradan RCW error geliyor. Belki pList yerine propertyleri gondermekle cozulur
                //                List<DummyProp> sourceList = DummyProp.GetDummySet(model, confName, pList);
                //                if (sourceList != null)
                //                    dtgPropConf.ItemsSource = sourceList;
                //                //dtgPropConf.ItemsSource = DummyProp.GetDummySet(model, confName, pList);
                //                //dtgPropConf.ItemsSource = pList;
                //            }, confName);
                //        });
                //    }
                //}
                Logger.Log("Function is disabled", MessageTypeEnum.Warning);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }


        private void RecreateDefaultProps()
        {
            //var model = SolidWorksEnvironment.Application.ActiveModel;

            model.SetCustomProperty(CP_Design, DFT_Designer);
            model.SetCustomProperty(CP_STLConjugate, STR_True);

            if (cmbConf.Items.Count > 0)
            {
                var FirstConfName = (string)cmbConf.Items[0];
                model.SetCustomProperty(CP_PartNo, DFT_Value, FirstConfName);
                model.SetCustomProperty(CP_Material, AddInSettings.DFT_MATERIAL.Name, FirstConfName);
                model.SetCustomProperty(CP_Coating, DFT_Value, FirstConfName);
                model.SetCustomProperty(CP_Paint, DFT_Value, FirstConfName);
                model.SetCustomProperty(CP_Quantity, DFT_Quantity, FirstConfName);
                model.SetCustomProperty(CP_Description, DFT_Value, FirstConfName);
            }
                        
            model.UnsafeObject.SetSaveFlag();

            RefreshControl();
            Logger.Log("Default Properties are recreated", MessageTypeEnum.Information);
        }

        public static bool RecreateDefaultProps(Model model)
        {
            if (model != null && model.UnsafeObject != null)
            {
                var confNames = (string[])model.UnsafeObject.GetConfigurationNames();

                model.SetCustomProperty(CP_Design, DFT_Designer);
                model.SetCustomProperty(CP_STLConjugate, STR_True);

                for (int i = 0; i < confNames.Length; i++)
                {
                    model.SetCustomProperty(CP_PartNo, DFT_Value, confNames[i]);
                    model.SetCustomProperty(CP_Material, AddInSettings.DFT_MATERIAL.Name, confNames[i]);
                    model.SetCustomProperty(CP_Coating, DFT_Value, confNames[i]);
                    model.SetCustomProperty(CP_Paint, DFT_Value, confNames[i]);
                    model.SetCustomProperty(CP_Quantity, DFT_Quantity, confNames[i]);
                    model.SetCustomProperty(CP_Description, DFT_Value, confNames[i]);
                    if (model.ModelType==ModelType.Assembly)
                        model.SetCustomProperty(CP_DftMaterial, DFT_Value, confNames[i]);
                }

                model.UnsafeObject.SetSaveFlag();

                Logger.Log("Default Properties are recreated", MessageTypeEnum.Information);

                return true;
            }
            else
            {
                return false;
            }  
        }

        public static bool SetCustomPropForAllConf(Model model, string propName, string propValue)
        {
            if(model != null && model.UnsafeObject != null)
            {
                var confNames = (string[])model.UnsafeObject.GetConfigurationNames();
                for (int i = 0; i < confNames.Length; i++)
                {
                    model.SetCustomProperty(propName, propValue, confNames[i]);
                }
                model.UnsafeObject.SetSaveFlag();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void Application_ActiveModelInformationChanged(Model obj)
        {
            model = obj;
            RefreshControl();
        }

        private void btnDefault_Click(object sender, RoutedEventArgs e) => RecreateDefaultProps();

        private void cmbConf_SelectionChanged(object sender, SelectionChangedEventArgs e) => SelectConfiguration(cmbConf.SelectedIndex);

        private void cbHasSTLConjugate_Checked(object sender, RoutedEventArgs e)
        {
            //var model = SolidWorksEnvironment.Application.ActiveModel;
            if(model!=null)
            {
                model.SetCustomProperty(CP_STLConjugate, STR_True);
                model.UnsafeObject.SetSaveFlag();
            }
            
        }

        private void cbHasSTLConjugate_Unchecked(object sender, RoutedEventArgs e)
        {
            //var model = SolidWorksEnvironment.Application.ActiveModel;
            if (model != null)
            {
                model.SetCustomProperty(CP_STLConjugate, STR_False);
                model.UnsafeObject.SetSaveFlag();
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            model = null;
        }
    }

    //TODO: Sacmalik! CustomProperty direkt bind olabilmeliydi.
    public class DummyProp
    {
        //private Model AssignedModel = null;
        //private string _Value = string.Empty;
        //public string Value 
        //{ 
        //    get
        //    {
        //        return _Value;
        //    }
        //    set
        //    {
        //        if(value!=_Value)
        //        {
        //            _Value = value;
        //            if(AssignedModel!=null)
        //            {
        //                if (ConfName.IsNullOrEmpty())
        //                    AssignedModel.SetCustomProperty(Name, _Value);
        //                else
        //                    AssignedModel.SetCustomProperty(Name, _Value, ConfName);
        //                AssignedModel.UnsafeObject.SetSaveFlag();
        //            }
        //        }
        //    }
                
        //}

        public string Value { get; private set; }
        public string Name { get; private set; }
        public string ResolvedValue { get; private set; }
        public string ConfName { get; private set; }



        public DummyProp()
        {
        }

        public DummyProp(Model model, CustomProperty customProperty, string confName)
        {
            Name = customProperty.Name;
            Value = customProperty.Value;
            ResolvedValue = customProperty.ResolvedValue;
            if (!confName.IsNullOrEmpty())
                ConfName = confName;
            //AssignedModel = model;
        }

        public static List<DummyProp> GetDummySet(Model model, string confName, List<CustomProperty> list)
        {
            try
            {
                List<DummyProp> dummyList = new List<DummyProp>();
                for (int i = 0; i < list.Count; i++)
                {
                    dummyList.Add(new DummyProp(model, list[i], confName));
                }
                return dummyList;
            }
            catch(Exception ex)
            {
                Logger.LogException(ex);
                Logger.Log("ConfName: " + confName, MessageTypeEnum.Information);
                //return new List<DummyProp>();
                return null;
            }
        }
    }
}
