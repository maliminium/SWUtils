using AngelSix.SolidDna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace SW_Utils
{
    /// <summary>
    /// Interaction logic for MainAddInUI.xaml
    /// </summary>
    public partial class MainAddInUI : UserControl
    {
        public MainAddInUI()
        {
            InitializeComponent();
        }

        private const string COPY_SFX = "_2";
        private void CopyDir(string mainDir, string copyDir)
        {
            Directory.CreateDirectory(copyDir);

            var subFiles = Directory.GetFiles(mainDir);
            for (int i = 0; i < subFiles.Length; i++)
            {
                var nextName = subFiles[i];
                nextName = nextName.Replace(mainDir, copyDir);
                var idx = nextName.LastIndexOf(".");
                nextName = nextName.Insert(idx, COPY_SFX);

                File.Copy(subFiles[i], nextName);
            }


            var subDirs = Directory.GetDirectories(mainDir);
            for (int i = 0; i < subDirs.Length; i++)
            {
                var nextName = subDirs[i];
                nextName = nextName.Replace(mainDir, copyDir);
                nextName += COPY_SFX;

                CopyDir(subDirs[i], nextName);
            }
        }


        string[] RandomText = {
            "SVN",
            "ile ilgili tüm",
            "şlemler için ",
            "bir",
            "Kullanıcı Adı",
            "ve",
            "Parola",
            "gerekir.  Bu bilgiler",
            "Bilgi İşlem",
            "Sorumlusu",
            "tarafından",
            "belirlenir.",
            "Ayrıca Repository",
            "leri görebilmek",
            "değiştirebilmek için farklı",
            " yetki",
            " seviyeleri tanımlanmıştır.",
            " Her çalışan",
            "yalnızca",
            "yetki seviyesinin",
            "verdiği işlemleri ",
            "gerçekleştirebilmektedir.",
            "ERİŞİM",
            "başlığı",
            "altında",
            "açıklanan",
            "tüm işlemler",
            "için",
            "Tortoise",
            "SVN isimli",
            "program",
            "bilgisayarımızda "
        };


        private List<List<int>> GetItems_10(List<int> firstItem, int maxLength)
        {
            var items = new List<List<int>>();
            items.Add(firstItem);

            if (firstItem.Count < maxLength)
            {
                var seed = firstItem.Last();
                var random = new Random(seed);
                var nextNo = random.Next(-2, 5);

                if (nextNo > 0)
                {
                    for (int i = 1; i < nextNo; i++)
                    {
                        var nextItem = new List<int>(firstItem);
                        nextItem.Add(i);
                        items.AddRange(GetItems_10(nextItem, maxLength));
                        //items.Add(i);
                    }
                }
            }
            return items;
        }



        private List<string> GetItems_36(string firstItem)
        {
            var items = new List<string>();
            items.Add(firstItem);

            if (firstItem.Length > 5)
                return items;
            else
            {
                var seed = (int)CommonResources.BaseToLong(firstItem.Substring(firstItem.Length - 2));
                var random = new Random(seed);
                var nextNo = random.Next(-4, 4);

                if (nextNo>0)
                {  
                    for (int i = 0; i < nextNo; i++)
                    {
                        var nextItem = firstItem + CommonResources.LongToBase(i);
                        items.AddRange(GetItems_36(nextItem));
                    }
                }

                return items;
            }
        }

        private void CreateDocs(string targetDir, List<string> nameList)
        {
            for (int i = 0; i < nameList.Count; i++)
            {
                var newName = targetDir + "\\" + nameList[i];

                Directory.CreateDirectory(newName);

                using (System.IO.StreamWriter file = new System.IO.StreamWriter(newName + ".txt"))
                {
                    file.Close();
                }
            }
        }

        private List<Feature> GetFeatures(object feature_obj)
        {
            var results = new List<Feature>();
            if(feature_obj!=null)
            {
                var feature = (Feature)feature_obj;

                if (feature.GetTypeName2() == "Reference")
                {
                    results.Add(feature);

                    var comp_obj = feature.GetSpecificFeature2();
                    if (comp_obj != null)
                    {
                        var comp = (IComponent2)comp_obj;
                        results.AddRange(GetFeatures(comp.FirstFeature()));
                    }

                }
                results.AddRange(GetFeatures(feature.GetNextFeature()));
            }
            
            return results;
        }


        private void btnDummy_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                ////DIMENSION SEARCH
                var model = SolidWorksEnvironment.Application.ActiveModel;
                model.Features(delegate (ModelFeature feat, int idx)
                {
                    if (feat.FeatureName.StartsWith("cnf"))
                        Logger.Log(idx.ToString() + "\t" + feat.FeatureType.ToString() + "\t\t" + feat.FeatureTypeName + "\t\t" + feat.FeatureName);

                    if (feat.IsSketch)
                    {
                        //rtbOut.AppendText("\nDIMENSIONS\r");

                        var f = (Feature)feat.UnsafeObject;

                        var dispDim = (DisplayDimension)f.GetFirstDisplayDimension();

                        while (dispDim != null)
                        {
                            var dim = dispDim.GetDimension2(0);

                            Logger.Log("\t\t" + feat.FeatureName + " - " + dim.FullName + " : " + dim.Value.ToString());
                            dispDim = (DisplayDimension)f.GetNextDisplayDimension(dispDim);
                        }
                    }
                });


                //UI_Properties.RecreateDefaultProps(SolidWorksEnvironment.Application.ActiveModel);

                ////TextBox border color
                //var txt = new TextBox();
                //Logger.Log(btnDummy.BorderBrush.ToString());

                ////substring large index
                //var text = "ab";
                //Logger.Log(text.Substring(3));

                ////Empty List Join
                //var list = new List<int>();
                //Logger.Log(string.Join("-", list), MessageTypeEnum.Greeting);
                //list.Add(1);
                //Logger.Log(string.Join("-", list), MessageTypeEnum.Warning);
                //list.Add(2);
                //Logger.Log(string.Join("-", list), MessageTypeEnum.Information);

                ////Rtb satir atlatma Dir
                //Logger.Log("Line1 n\nLine2 n", MessageTypeEnum.Information);
                //Logger.Log("Line1 r\rLine2 r", MessageTypeEnum.Greeting);
                //Logger.Log("Line1 nr\n\rLine2 nr", MessageTypeEnum.Warning);
                //Logger.Log("Line1 rn\r\nLine2 rn", MessageTypeEnum.Error);

                ////App Dir
                //Logger.Log(SolidWorksEnvironment.Application.AssemblyPath());


                ////Get name without extension from name denemesi
                //Logger.Log(Path.GetExtension("1-10-1-1_fileName.eXe"));




                ////Get Assembly Components (sirali) denemesi

                //Model model = SolidWorksEnvironment.Application.ActiveModel;
                //var items = GetFeatures(model.UnsafeObject.FirstFeature());
                //for (int i = 0; i < items.Count; i++)
                //{
                //    Logger.Log(items[i].Name + " - " + items[i].GetTypeName2());
                //    Logger.Log(model.AsAssembly().GetComponentByName(items[i].Name).GetPathName(), MessageTypeEnum.Greeting);
                //}
                ////TODO: Insert model name into the beginning



                ////model.AsAssembly().GetComponents(true);

                ////var items = new List<Feature>();
                ////var firstFeat = (Feature)model.UnsafeObject.FirstFeature();
                ////items.AddRange(GetFeatures(firstFeat));
                ////var nextFeat_obj = firstFeat.GetNextFeature();
                ////while (nextFeat_obj != null)
                ////{
                ////    var nextFeat = (Feature)nextFeat_obj;
                ////    items.AddRange(GetFeatures(nextFeat));
                ////    nextFeat_obj = nextFeat.GetNextFeature();
                ////}

                ////CompStrings.Clear();
                ////model.Components(ComponentAction);
                //////for (int i = 0; i < components.Count; i++)
                //////{
                //////    Logger.Log(components[i].AsModel.FilePath);
                //////}
                ////for (int i = 0; i < CompStrings.Count; i++)
                ////{
                ////    Logger.Log(Path.GetFileName(CompStrings[i]));
                ////}

                ////var pag = model.UnsafeObject.Extension.GetPackAndGo();
                ////var pag_obj = new object();
                ////pag.GetDocumentNames(out pag_obj);
                ////var pag_arr = (string[])pag_obj;

                ////for (int i = 0; i < pag_arr.Length; i++)
                ////{
                ////    Logger.Log(Path.GetFileName(pag_arr[i]), MessageTypeEnum.Greeting);
                ////}

                ////items.Add((Feature)model.UnsafeObject.FirstFeature());







                ////Open File Dialog Denemesi
                //SolidWorksEnvironment.Application.UnsafeObject.SetCurrentWorkingDirectory(@"D:\mekanik\3.PQ\2.RELEASE");
                //SolidWorksEnvironment.Application.UnsafeObject.Command((int)swCommand_e.swFileOpen, null);
                ////Logger.Log(SolidWorksEnvironment.Application.UnsafeObject.GetCurrentWorkingDirectory());





                ////GetDirectoryName DENEMESI
                //Logger.Log("GetDirectoryName - File: " + Path.GetDirectoryName(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik\ADT-0218-p10_fig1.jpg"));
                //Logger.Log("GetDirectoryName - Folder: " + Path.GetDirectoryName(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik"));





                ////FilePath ile DirectoryPath ayirdetme denemesi
                //Logger.Log("FileExist - File: " + File.Exists(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik\ADT-0218-p10_fig1.jpg").ToString());
                //Logger.Log("FileExist - Folder: " + File.Exists(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik").ToString());

                //Logger.Log("GetFileName - File: " + Path.GetFileName(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik\ADT-0218-p10_fig1.jpg").ToString());
                //Logger.Log("GetFileName - Folder: " + Path.GetFileName(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik").ToString());

                //Logger.Log("GetExtension - File: {" + Path.GetExtension(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik\ADT-0218-p10_fig1.jpg") + "}");
                //Logger.Log("GetExtension - Folder: {" + Path.GetExtension(@"D:\mekanik\2.KACST_SCI\1.DRAFT\1.DerinDelik") + "}");

                ////SORTING DENEMESI
                //var list = CommonResources.GetSortedSubPaths(AddInSettings.PATH_PROJECTS, false);
                //for (int i = 0; i < list.Length; i++)
                //{
                //    Logger.Log(list[i]);
                //}





                ////KLASOR VE DOSYA SIRALAMA DENEMESI

                //var projCtLimit = Convert.ToInt32(edProjCt.Value) + 1;
                //var projDigits = Convert.ToInt32(edProjDigits.Value);
                //var projSep = edProjSep.Value;

                //var sepBtw = edSepBtw.Value;

                //var docCtLimit = Convert.ToInt32(edDocCt.Value) + 1;
                //var docDigits = Convert.ToInt32(edDocDigits.Value);
                //var docSep = edDocSep.Value;

                //var targetDir = @"D:\PROJECTS\SW_Utils\test";
                //Directory.CreateDirectory(targetDir);

                //System.IO.DirectoryInfo di = new DirectoryInfo(targetDir);

                //foreach (FileInfo file in di.GetFiles())
                //{
                //    file.Delete();
                //}
                //foreach (DirectoryInfo dir in di.GetDirectories())
                //{
                //    dir.Delete(true);
                //}

                //var projList = new List<string>();
                //for (int i = 0; i < projCtLimit; i++)
                //{
                //    var nextBase = new List<int>();
                //    nextBase.Add(i);

                //    var nextList = GetItems_10(nextBase, projDigits);
                //    for (int j = 0; j < nextList.Count; j++)
                //    {
                //        var nextName = string.Join(projSep, nextList[j]);
                //        projList.Add(nextName);
                //    }
                //}

                //for (int i = 0; i < projList.Count; i++)
                //{
                //    var nameList = new List<string>();
                //    for (int j = 1; j < docCtLimit; j++)
                //    {
                //        var nextBase = new List<int>();
                //        nextBase.Add(j);

                //        var nextList = GetItems_10(nextBase, docDigits);
                //        for (int k = 0; k < nextList.Count; k++)
                //        {
                //            var nextName = projList[i] + sepBtw + string.Join(docSep, nextList[k]);

                //            var rnd_i = new Random((i + 1) * j * (k + 1));

                //            if (rnd_i.Next() % 2 == 1)
                //            {
                //                nextName += edSuffixSep.Value + RandomText[rnd_i.Next(RandomText.Length)];
                //            }
                //            nameList.Add(nextName);
                //        }
                //    }
                //    CreateDocs(targetDir, nameList);
                //}

                //__________

                ////2 HANELI 36LIK KLASOR SIRALAMA DENEMESI

                //var targetDir = @"D:\PROJECTS\SW_Utils\test";
                //Directory.CreateDirectory(targetDir);

                //var nameList = new List<string>();
                //for (int i = 0; i < 676; i++)
                //{
                //    //Directory.CreateDirectory(targetDir + "\\" + CommonResources.LongToBase(i));
                //    nameList.AddRange(GetItems(CommonResources.LongToBase(i)));
                //}

                //for (int i = 0; i < nameList.Count; i++)
                //{
                //    var numList = new List<string>();
                //    for (int j = 0; j < nameList[i].Length; j = j + 2)
                //    {
                //        numList.Add(CommonResources.BaseToLong(nameList[i].Substring(j, 2)).ToString());
                //    }


                //    var newName = targetDir + "\\" + nameList[i] + "-" + string.Join(".", numList);

                //    Directory.CreateDirectory(newName);

                //    using (System.IO.StreamWriter file = new System.IO.StreamWriter(newName + ".txt"))
                //    {
                //        file.Close();
                //    }
                //}


                ////__________



                ////36LIK SISTEM SAYI DOKUMU
                ////Logger.Log(CommonResources.LongToBase(9223372036854775806));
                ////Logger.Log(CommonResources.BaseToLong("ZZZ").ToString());
                ////var limit = 1679615;
                ////var step = limit / 10;
                ////var counter = 0;
                ////var acc = 0;

                //using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"D:\PROJECTS\SW_Utils\test.txt"))
                //{
                //    for (int i = 0; i < 1679617; i++)
                //    {
                //        var s = CommonResources.LongToBase(i);
                //        var n = CommonResources.BaseToLong(s);



                //        //if (i != n)
                //        //    Logger.Log($"wrong conversion i:{i} s:{s} n:{n}", MessageTypeEnum.Error);

                //        //if(s.Length>4)
                //        //    Logger.Log($"long string i:{i} s:{s} n:{n}", MessageTypeEnum.Warning);

                //        file.WriteLine($"i:{i}\ts:{s}\tn:{n}");

                //        //if (counter > step)
                //        //{
                //        //    counter = 0;
                //        //    acc++;
                //        //    Logger.Log($"%{acc}.........");
                //        //}
                //        //else
                //        //    counter++;
                //    }
                //    file.Close();
                //}
                ////_________






                ////GetDirectoryName SON SLASH TEPKISI
                //Logger.Log(Path.GetFileName(@"\RAKOR\Rakorpart.SLDPRT"));
                //Logger.Log(Path.GetDirectoryName(@"C:\Users\mehmet.ciftci\Google Drive\3D\RAKOR"));
                //Logger.Log(Path.GetDirectoryName(@"C:\Users\mehmet.ciftci\Google Drive\3D\RAKOR\"));
                ////_________



                ////ISIM KIYASI
                //var model = SolidWorksEnvironment.Application.ActiveModel.UnsafeObject;
                //var assem = (AssemblyDoc)model;
                //var components = (object[])assem.GetComponents(false);

                //for (int i = 0; i < components.Length; i++)
                //{
                //    var nextModel = (IComponent2)components[i];
                //    Logger.Log(nextModel.GetPathName(), MessageTypeEnum.Greeting);
                //}

                //var swPackAndGo = model.Extension.GetPackAndGo();
                ////swPackAndGo.FlattenToSingleFolder = true;
                ////swPackAndGo.IncludeDrawings = true;
                ////swPackAndGo.IncludeSuppressed = true;
                ////swPackAndGo.IncludeToolboxComponents = true;
                ////edTargetDir.Value = targetDir;


                //swPackAndGo.GetDocumentNames(out object originalNames_obj);
                //var originalNames = (string[])originalNames_obj;
                //for (int i = 0; i < originalNames.Length; i++)
                //{
                //    originalNames[i] = StringChecker.GetCaseSensitivePath(originalNames[i]);
                //    Logger.Log(originalNames[i], MessageTypeEnum.Error);
                //}

                //swPackAndGo.GetDocumentSaveToNames(out object targetNames_obj, out object docStatuses_obj);
                //var targetNames = (string[])targetNames_obj;
                //for (int i = 0; i < targetNames.Length; i++)
                //{
                //    Logger.Log(targetNames[i], MessageTypeEnum.Information);
                //}

                ////_________



                //Logger.Log(CommonResources.GetNextRelaseDirectory(@"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy\0-1.12.1.2.SLDPRT"));

                //Logger.Log(@"D:\mekanik\0.Miscellaneous\2.RELEASE\5.Deneysel\KATI_MODELLER_SW\S-1.1.6.flans_celik-0-2.5.SLDPRT".Length.ToString());


                //var list = new List<int>();
                //var msg = "*" + string.Join(".", list) + "*";
                //Logger.Log(msg);

                //list.Add(1);
                //msg = "*" + string.Join(".", list) + "*";
                //Logger.Log(msg);

                //list.Add(1);
                //msg = "*" + string.Join(".", list) + "*";
                //Logger.Log(msg);

                //COPYING & REPLACEMENT
                //var err = 0;
                //var war = 0;

                //var model = SolidWorksEnvironment.Application.UnsafeObject.OpenDoc6(
                //    @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy_2\1.1_rev_2.sldasm",
                //    (int)swDocumentTypes_e.swDocASSEMBLY,
                //    (int)swOpenDocOptions_e.swOpenDocOptions_Silent,
                //    string.Empty,
                //    ref err,
                //    ref war
                //    );

                //Logger.Log("Opened");

                //var assem = (AssemblyDoc)model;

                //var components = (object[])assem.GetComponents(false);

                //for (int i = 0; i < components.Length; i++)
                //{
                //    var nextModel = (IComponent2)components[i];

                //    var originalPath = nextModel.GetPathName();
                //    var replacementPath = originalPath.Replace(
                //        @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy", 
                //        @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy_2");
                //    var idx = replacementPath.LastIndexOf(".");
                //    replacementPath = replacementPath.Insert(idx, COPY_SFX);

                //    var sData = default(SelectData);
                //    nextModel.Select4(false, sData, false);
                //    assem.ReplaceComponents2(replacementPath, string.Empty, true, (int)swReplaceComponentsConfiguration_e.swReplaceComponentsConfiguration_MatchName, true);

                //    Logger.Log(originalPath, MessageTypeEnum.Error);
                //    Logger.Log(replacementPath, MessageTypeEnum.Greeting);
                //}

                //model.Save3((int)swSaveAsOptions_e.swSaveAsOptions_Silent, ref err, ref war);

                //model.Close();
                //var title = model.GetTitle();

                //var docType = (swDocumentTypes_e)model.GetType();

                //SolidWorksEnvironment.Application.UnsafeObject.CloseDoc(title);
                //Logger.Log($"{title} - {docType} - Closed");

                //CopyDir(@"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy", @"D:\mekanik\0.Miscellaneous\1.DRAFT\12.Deneme\1.Dummy" + COPY_SFX);



                //PACK & GO
                //var oldDir = @"D:\mekanik\0.Miscellaneous\1.DRAFT\1.Deneme".ToLower();
                //var newDir = @"C:\Users\mehmet.ciftci\Desktop\Trial";

                //var swPackAndGo = SolidWorksEnvironment.Application.ActiveModel.UnsafeObject.Extension.GetPackAndGo();

                ////swPackAndGo.FlattenToSingleFolder = false;
                ////Logger.Log(swPackAndGo.FlattenToSingleFolder.ToString());
                ////swPackAndGo.SetSaveToName(true, newDir);
                ////Logger.Log(swPackAndGo.FlattenToSingleFolder.ToString());


                //swPackAndGo.AddSuffix = "_rev";

                //swPackAndGo.GetDocumentSaveToNames(out object pathNames_obj, out object docStatuses_obj);
                //var pathNames = (string[])pathNames_obj;
                //var docStatuses = (swPackAndGoDocumentStatus_e[])docStatuses_obj;

                //Logger.Log("Get Names");
                //for (int i = 0; i < pathNames.Length; i++)
                //{
                //    Logger.Log(pathNames[i] + " - " + docStatuses[i].ToString());
                //}



                //for (int i = 0; i < pathNames.Length; i++)
                //{
                //    pathNames[i] = pathNames[i].Replace(oldDir, newDir);
                //    pathNames[i] = pathNames[i].Replace(@"\1.1\", @"\1.1\1.T.1\");
                //}


                //Logger.Log("Set Names");
                //for (int i = 0; i < pathNames.Length; i++)
                //{
                //    Logger.Log(pathNames[i]);
                //}

                //swPackAndGo.SetDocumentSaveToNames(pathNames);

                //swPackAndGo.GetDocumentSaveToNames(out object pathNames_obj2, out object docStatuses_obj2);
                //var pathNames2 = (string[])pathNames_obj2;
                //var docStatuses2 = (swPackAndGoDocumentStatus_e[])docStatuses_obj2;

                //Logger.Log("Get Names");
                //for (int i = 0; i < pathNames2.Length; i++)
                //{
                //    Logger.Log(pathNames2[i] + " - " + docStatuses2[i].ToString());
                //}

                //var result = SolidWorksEnvironment.Application.ActiveModel.UnsafeObject.Extension.SavePackAndGo(swPackAndGo);
                //Logger.Log(result.ToString());


                //OTHERS
                //Logger.Log(Path.GetDirectoryName(@"D:\mekanik\1.YGDA"));
                //Logger.Log(Path.GetExtension(@"D:\mekanik\1.YGDA\1.DRAFT\8.Akyurt\1.YGDA_8.pdf"));

                //PrintLockers(@"D:\mekanik\P_0");
                //Logger.Log("done");
                //var pList = FileUtil.WhoIsLocking(@"D:\mekanik\P_0\1.DRAFT\6.SW_WPF\solidworks-api\SolidDna\AngelSix.SolidDna\AngelSix.SolidDna.sln");
                //for (int i = 0; i < pList.Count; i++)
                //{
                //    Logger.Log(pList[i].ToString());
                //}
                //Logger.Log("done");
                //var cmb = new ComboBox();
                //var list = new List<string>() { "a", "b" };
                //cmb.ItemsSource = list;
                //cmb.SelectedIndex = 0;
                //cmb.SelectedIndex = -1;
                //Logger.Log(cmb.SelectedIndex.ToString());

                //var sc = new StringChecker();
                //Logger.Log(sc.IsDirectoryValid(@"D:").ToString());
                //Logger.Log(sc.IsDirectoryValid(@"D:\").ToString());
                //Logger.Log(sc.IsDirectoryValid(@"D:\mekanik\P_0\1.DRAFT\12.Deneme").ToString());


                //var drawDoc = (DrawingDoc)SolidWorksEnvironment.Application.ActiveModel.UnsafeObject;
                //drawDoc.GenerateViewPaletteViews(@"D:\mekanik\P_0\1.DRAFT\12.Deneme\5.SLDPRT");

                //var paletteViewNames = (string[])drawDoc.GetDrawingPaletteViewNames();

                ////var ip = (double[])drawDoc.GetInsertionPoint();
                ////Logger.Log("X :" + ip[0].ToString());
                ////Logger.Log("Y :" + ip[1].ToString());
                ////Logger.Log("Z :" + ip[2].ToString());

                //var sheet = (Sheet)drawDoc.GetCurrentSheet();
                ////m cinsinden kagit boyutlari
                //double H = 0;
                //double W = 0;
                //sheet.GetSize(ref W, ref H);
                ////Logger.Log("H :" + H.ToString());
                ////Logger.Log("W :" + W.ToString());

                //drawDoc.DropDrawingViewFromPalette2(paletteViewNames[0], W / 2, H / 2, 0);

                //var assemb = (AssemblyDoc)SolidWorksEnvironment.Application.ActiveModel.UnsafeObject;
                //string[] compNames = new string[1];
                ////double[] compXforms = new double[16];
                //string[] compCoordSysNames = new string[1];
                //compNames[0] = @"D:\mekanik\P_0\1.DRAFT\12.Deneme\5.SLDPRT";
                //assemb.AddComponents3(compNames, null, compCoordSysNames);

                //var newModel = (Model)SolidWorksEnvironment.Application.UnsafeObject.NewDocument(@"D:\mekanik\Resources\MAM_Templates\MAM_Parts\Head.PRTDOT", 0, 0, 0);
                //SolidWorksEnvironment.Application.UnsafeObject.NewDocument(@"D:\mekanik\Resources\MAM_Templates\MAM_Parts\Part_General.SLDPRT", 0, 0, 0);




                //AddInSettings.Reset();
                //AddInSettings.Save();

                //Logger.Log(System.IO.Directory.Exists(@"D:\mekanik\Resources\MAM_AddInSettings").ToString());
                //Logger.Log(System.IO.Directory.Exists(@"D:\mekanik\Resources\MAM_AddInSetting\").ToString());

                //var model = SolidWorksEnvironment.Application.ActiveModel;
                //var parameters = new object();
                //var values = new object();
                //model.UnsafeObject.ConfigurationManager.GetConfigurationParams("name", out parameters, out values);
                //var parList = (string[])parameters;
                //var valList = (string[])values;
                //for (int i = 0; i < parList.Length; i++)
                //{
                //    rtbOut.AppendText($"P: {parList[i]}  -  V: {valList[i]} \n");
                //}

                //ModelDoc2 swModel = default(ModelDoc2);
                //DesignTable swDesTable = default(DesignTable);
                //int nTotRow = 0;
                //int nTotCol = 0;
                //string sRowStr = string.Empty;
                //int i = 0;
                //int j = 0;
                //bool bRet = false;
                //swModel = (ModelDoc2)SolidWorksEnvironment.Application.ActiveModel.UnsafeObject;
                //swDesTable = (DesignTable)swModel.GetDesignTable();
                //bRet = swDesTable.Attach();
                //nTotRow = swDesTable.GetTotalRowCount();
                //nTotCol = swDesTable.GetTotalColumnCount();
                //rtbOut.AppendText("File = " + swModel.GetPathName());
                //rtbOut.AppendText("\r  Title        = " + swDesTable.GetTitle());
                //rtbOut.AppendText("\r  Row          = " + swDesTable.GetRowCount());
                //rtbOut.AppendText("\r  Col          = " + swDesTable.GetColumnCount());
                //rtbOut.AppendText("\r  TotRow       = " + nTotRow);
                //rtbOut.AppendText("\r  TotCol       = " + nTotCol);
                //rtbOut.AppendText("\r  VisRow       = " + swDesTable.GetVisibleRowCount());
                //rtbOut.AppendText("\r  VisCol       = " + swDesTable.GetVisibleColumnCount());
                //rtbOut.AppendText("\r");
                //for (i = 0; i <= nTotRow; i++)
                //{
                //    sRowStr = "|";
                //    for (j = 0; j <= nTotCol; j++)
                //    {
                //        sRowStr = sRowStr + swDesTable.GetEntryText(i, j) + "|";
                //    }
                //    rtbOut.AppendText(sRowStr + "\r");
                //}
                //swDesTable.Detach();

                //var model = SolidWorksEnvironment.Application.ActiveModel;
                //model.Features(delegate (ModelFeature feat, int idx)
                //{
                //    if (feat.FeatureName.StartsWith("cnf"))
                //        Logger.Log(idx.ToString() + "\t" + feat.FeatureType.ToString() + "\t\t" + feat.FeatureTypeName + "\t\t" + feat.FeatureName);

                //    if (feat.IsSketch)
                //    {
                //        //rtbOut.AppendText("\nDIMENSIONS\r");

                //        var f = (Feature)feat.UnsafeObject;

                //        var dispDim = (DisplayDimension)f.GetFirstDisplayDimension();

                //        while (dispDim != null)
                //        {
                //            var dim = dispDim.GetDimension2(0);

                //            Logger.Log("\t\t" + feat.FeatureName + " - " + dim.FullName + " : " + dim.Value.ToString());
                //            dispDim = (DisplayDimension)f.GetNextDisplayDimension(dispDim);
                //        }
                //    }
                //});
            }
            catch (System.Exception ex)
            {
                Logger.Log(ex.Message);
            }
            finally
            {
                Logger.Log("Done", MessageTypeEnum.Greeting);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            //try
            //{
            //    AddInSettings.Load();
            //}
            //catch
            //{
            //}

            AddInSettings.Load();

            //Logger.Log(AddInSettings.Instance.STR[4]);
        }
    }
}
