using AngelSix.SolidDna;
using Dna;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System.Collections.ObjectModel;
using System.IO;

namespace SW_Utils
{
    public class PackAndGoItem
    {
        public swDocumentTypes_e DocType { get; set; }

        public string OriginalPath { get; set; }
        public string OriginalName { get; set; }
        public string OriginalExt { get; set; }

        public string TargetPath { get; set; }
        public string TargetName { get; set; }
        public string TargetExt { get; set; }

        public PackAndGoItem()
        {
            DocType = swDocumentTypes_e.swDocNONE;
        }

        public PackAndGoItem(string originalPath, string targetDir, string targetExt = null)
        {
            //Original Path
            OriginalPath = StringChecker.GetCaseSensitivePath(originalPath);

            //Original Name
            OriginalName = Path.GetFileNameWithoutExtension(OriginalPath);

            //Original Extension
            OriginalExt = Path.GetExtension(OriginalPath);

            

            //Doc Type
            var originalExt_upper = OriginalExt.ToUpper();
            switch (originalExt_upper)
            {
                case AddInSettings.EXT_ASSM:
                    DocType = swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case AddInSettings.EXT_PART:
                    DocType = swDocumentTypes_e.swDocPART;
                    break;
                case AddInSettings.EXT_DRAW:
                    DocType = swDocumentTypes_e.swDocDRAWING;
                    break;
                case AddInSettings.EXT_ASSM_T:
                    DocType = swDocumentTypes_e.swDocASSEMBLY;
                    break;
                case AddInSettings.EXT_PART_T:
                    DocType = swDocumentTypes_e.swDocPART;
                    break;
                case AddInSettings.EXT_DRAW_T:
                    DocType = swDocumentTypes_e.swDocDRAWING;
                    break;
                default:
                    DocType = swDocumentTypes_e.swDocNONE;
                    break;
            }

            //TODO: Burayi komple degistir
            //TODO: Revision no'yu nasil ekleyecegiz?
            //Target Name
            var targetAffix = CommonResources.GetFullPrefixFromPath(targetDir);
            if (OriginalName.StartsWith(AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_FILE_NUMERATION))
            {
                TargetName = OriginalName + AddInSettings.SEP_TEXT + targetAffix;
            }
            else
            {
                TargetName = targetAffix + AddInSettings.SEP_FILE_NUMERATION;

                var mainBlocks = OriginalName.Split(AddInSettings.SEP_FILE_NUMERATION.ToCharArray());
                if (mainBlocks.Length > 1)
                {
                    //TODO: Degisiklik lazim
                    var numerationBlocks = CommonResources.GetNumberingFromName(mainBlocks[1], AddInSettings.SEP_FILE_NUMERATION);
                    if (numerationBlocks.Count > 2)
                        numerationBlocks.RemoveRange(0, 2);

                    TargetName += string.Join(AddInSettings.SEP_FILE_NUMERATION, numerationBlocks);
                }
                else
                {
                    TargetName += OriginalName;
                }
            }

            //Target Extension
            TargetExt = targetExt != null ? targetExt : originalExt_upper;

            //Target Path
            TargetPath = targetDir;
            switch (TargetExt)
            {
                case AddInSettings.EXT_ASSM:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    break;
                case AddInSettings.EXT_PART:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    break;
                case AddInSettings.EXT_DRAW:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_SW;
                    break;
                case AddInSettings.EXT_ALT_SOLID:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_STEP;
                    break;
                case AddInSettings.EXT_ALT_DRAW:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_PDF;
                    break;
                case AddInSettings.EXT_ALT_SHEET:
                    TargetPath += Path.DirectorySeparatorChar + AddInSettings.FOLDER_CUT_DXF;
                    break;
                default:
                    break;
            }
            TargetPath += Path.DirectorySeparatorChar + TargetName + TargetExt;
        }

        //public PackAndGoItem(string originalPath, string targetPath)
        //{
        //    //Correct originalpath

        //    //Get Original name
        //    //Get docType
        //    //Construct PackAndGoTargetPath
        //    //Construct PackAndGoTargetName
        //    //Construct SaveAsTargetPath
        //    //Construct SaveAsTargetName




        //    OriginalPath = StringChecker.GetCaseSensitivePath(originalPath);

        //    OriginalName = Path.GetFileName(originalPath);





        //    //TODO: .GetDocumentSaveToNames() file isimlerini lowercase yaptigi icin boyle bir kisim var. Belki de Assembly'den Component isimleri alinarak duzelme yapilmali
        //    var toolboxPrefix_Upper = Path.DirectorySeparatorChar + AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_PROJECT;
        //    var toolboxPrefix_Lower = toolboxPrefix_Upper.ToLower();
        //    targetPath = targetPath.Replace(toolboxPrefix_Lower, toolboxPrefix_Upper);

        //    //TODO: File isminin lowercase olmasi sonucu uzantilar baska turlu kiyaslanamaz oluyor
        //    var targetExt_Lower = Path.GetExtension(targetPath);
        //    var targetExt_Upper = targetExt_Lower.ToUpper();
        //    targetPath = targetPath.Replace(targetExt_Lower, targetExt_Upper);

        //    var originalExt_Upper = Path.GetExtension(originalPath).ToUpper();
        //    switch (originalExt_Upper)
        //    {
        //        case AddInSettings.EXT_ASSM:
        //            DocType = swDocumentTypes_e.swDocASSEMBLY;
        //            break;
        //        case AddInSettings.EXT_PART:
        //            DocType = swDocumentTypes_e.swDocPART;
        //            break;
        //        case AddInSettings.EXT_DRAW:
        //            DocType = swDocumentTypes_e.swDocDRAWING;
        //            break;
        //        case AddInSettings.EXT_ASSM_T:
        //            DocType = swDocumentTypes_e.swDocASSEMBLY;  //TODO: Bunlar boyle dogru mu bilmiyorum
        //            break;
        //        case AddInSettings.EXT_PART_T:
        //            DocType = swDocumentTypes_e.swDocPART;
        //            break;
        //        case AddInSettings.EXT_DRAW_T:
        //            DocType = swDocumentTypes_e.swDocDRAWING;
        //            break;
        //        default:
        //            DocType = swDocumentTypes_e.swDocNONE;
        //            break;
        //    }

        //    var targetFolder = string.Empty;
        //    switch (targetExt_Upper)
        //    {
        //        case AddInSettings.EXT_ASSM:
        //            targetFolder = AddInSettings.FOLDER_SOLID_SW;
        //            break;
        //        case AddInSettings.EXT_PART:
        //            targetFolder = AddInSettings.FOLDER_SOLID_SW;
        //            break;
        //        case AddInSettings.EXT_DRAW:
        //            targetFolder = AddInSettings.FOLDER_DRAW_SW;
        //            break;
        //        default:
        //            break;
        //    }


        //    //TODO: Cok cirkin ama kafam daha iyisine basmadi
        //    var targetDir = Path.GetDirectoryName(targetPath);
        //    var targetAffix = CommonResources.GetFullPrefixFromPath(targetDir);
        //    var targetName = Path.GetFileNameWithoutExtension(targetPath);
        //    var targetName_old = targetName; 

        //    if (targetName.StartsWith(AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_PROJECT))
        //    {
        //        targetName = targetName + AddInSettings.SEP_PROJECT + targetAffix;
        //    }
        //    else
        //    {
        //        var mainBlocks = OriginalName.Split(AddInSettings.SEP_PROJECT.ToCharArray());
        //        if (mainBlocks.Length > 1)
        //        {
        //            var numerationBlocks = CommonResources.GetNumberingFromName(mainBlocks[1]);
        //            if(numerationBlocks.Count>2)
        //            {
        //                numerationBlocks.RemoveRange(0, 2);
        //            }

        //            targetName = targetAffix + AddInSettings.SEP_NUMBER + string.Join(AddInSettings.SEP_NUMBER, numerationBlocks);
        //        }
        //    }

        //    targetPath = targetPath.Replace(targetName_old, targetName);

        //    if (!targetFolder.IsNullOrEmpty())
        //    {
        //        var idx = targetPath.LastIndexOf(Path.DirectorySeparatorChar);
        //        targetFolder = Path.DirectorySeparatorChar + targetFolder;
        //        targetPath = targetPath.Insert(idx, targetFolder);
        //    }

        //    TargetPath = targetPath;
        //}

        public static ObservableCollection<PackAndGoItem> GetPackAndGoItems(Model model, string targetDir)
        {
            ObservableCollection<PackAndGoItem> list = new ObservableCollection<PackAndGoItem>();

            var swPackAndGo = model.UnsafeObject.Extension.GetPackAndGo();
            swPackAndGo.GetDocumentNames(out object originalNames_obj);
            var originalNames = (string[])originalNames_obj;

            for (int i = 0; i < originalNames.Length; i++)
                list.Add(new PackAndGoItem(originalNames[i], targetDir));

            return list;
        }

        //public static ObservableCollection<PackAndGoItem> GetPackAndGoItems(PackAndGo packAndGo)
        //{
        //    ObservableCollection<PackAndGoItem> list = new ObservableCollection<PackAndGoItem>();

        //    packAndGo.GetDocumentNames(out object originalNames_obj);
        //    var originalNames = (string[])originalNames_obj;

        //    packAndGo.GetDocumentSaveToNames(out object targetNames_obj, out object docStatuses_obj);
        //    var targetNames = (string[])targetNames_obj;
        //    //var docStatuses = (swPackAndGoDocumentStatus_e[])docStatuses_obj;

        //    for (int i = 0; i < originalNames.Length; i++)
        //    {
        //        list.Add(new PackAndGoItem(originalNames[i], targetNames[i]));
        //    }

        //    return list;
        //}
    }
}
