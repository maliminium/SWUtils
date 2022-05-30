using SolidWorks.Interop.swconst;
using System.IO;

namespace SW_Utils
{
    public class ReleaseOpItem
    {
        private int _RevNo = 0;
        public int RevNo 
        {
            get => _RevNo;
            set
            {
                _RevNo = value;
                if (!IsStandartPart)
                    UpdateName();
            }
        }

        private string _TargetDir = string.Empty; 
        public string TargetDir 
        {
            get => _TargetDir;
            set
            {
                _TargetDir = value;
                UpdateName();
            }
        }

        public bool IsStandartPart => OriginalName.StartsWith(AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_FILE_NUMERATION);

        public swDocumentTypes_e DocType { get; set; }

        public string OriginalPath { get; private set; }
        public string OriginalDir => Path.GetDirectoryName(OriginalPath);
        public string OriginalName => Path.GetFileNameWithoutExtension(OriginalPath);
        public string OriginalExt => Path.GetExtension(OriginalPath);

        
        public string TargetName { get; private set; }

        public string Ext_PaG { get; set; }
        public string Ext_Sas { get; set; }
        public string Ext_DXF { get; set; }

        public string DirAnnex_PaG { get; set; }
        public string DirAnnex_Sas { get; set; }
        public string DirAnnex_DXF { get; set; }

        public string TargetPath_PaG { get; set; }
        public string TargetPath_Sas { get; set; }
        public string TargetPath_DXF { get; set; }

        public ReleaseOpItem()
        {

        }

        public ReleaseOpItem(string originalPath, string targetDir)
        {
            //Original Path
            OriginalPath = StringChecker.GetCaseSensitivePath(originalPath);            

            //Doc Type
            var originalExt_upper = OriginalExt.ToUpper();
            switch (originalExt_upper)
            {
                case AddInSettings.EXT_ASSM:
                    DocType = swDocumentTypes_e.swDocASSEMBLY;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_STEP;
                    Ext_PaG = AddInSettings.EXT_ASSM;
                    Ext_Sas = AddInSettings.EXT_ALT_SOLID;
                    break;
                case AddInSettings.EXT_PART:
                    DocType = swDocumentTypes_e.swDocPART;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_STEP;
                    DirAnnex_DXF = Path.DirectorySeparatorChar + AddInSettings.FOLDER_CUT_DXF;
                    Ext_PaG = AddInSettings.EXT_PART;
                    Ext_Sas = AddInSettings.EXT_ALT_SOLID;
                    Ext_DXF = AddInSettings.EXT_ALT_SHEET;
                    break;
                case AddInSettings.EXT_DRAW:
                    DocType = swDocumentTypes_e.swDocDRAWING;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_PDF;
                    Ext_PaG = AddInSettings.EXT_DRAW;
                    Ext_Sas = AddInSettings.EXT_ALT_DRAW;
                    break;
                case AddInSettings.EXT_ASSM_T:
                    DocType = swDocumentTypes_e.swDocASSEMBLY;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_STEP;
                    Ext_PaG = AddInSettings.EXT_ASSM_T;
                    Ext_Sas = AddInSettings.EXT_ALT_SOLID;
                    break;
                case AddInSettings.EXT_PART_T:
                    DocType = swDocumentTypes_e.swDocPART;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_SOLID_STEP;
                    DirAnnex_DXF = Path.DirectorySeparatorChar + AddInSettings.FOLDER_CUT_DXF;
                    Ext_PaG = AddInSettings.EXT_PART_T;
                    Ext_Sas = AddInSettings.EXT_ALT_SOLID;
                    Ext_DXF = AddInSettings.EXT_ALT_SHEET;
                    break;
                case AddInSettings.EXT_DRAW_T:
                    DocType = swDocumentTypes_e.swDocDRAWING;
                    DirAnnex_PaG = Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_SW;
                    DirAnnex_Sas = Path.DirectorySeparatorChar + AddInSettings.FOLDER_DRAW_PDF;
                    Ext_PaG = AddInSettings.EXT_ASSM_T;
                    Ext_Sas = AddInSettings.EXT_ALT_SOLID;
                    break;
                default:
                    DocType = swDocumentTypes_e.swDocNONE;
                    break;
            }

            //TargetDir
            TargetDir = targetDir.Trim(Path.DirectorySeparatorChar); //indirectly update Name and TargetPaths 
        }

        private void UpdateName()
        {
            //TODO: Name #-#.#.#.Description ise Description ucuyor!

            var targetAffix = CommonResources.GetFullPrefixFromPath(TargetDir);
            if (IsStandartPart)
            {
                TargetName = OriginalName + AddInSettings.SEP_FILE_NUMERATION + targetAffix;
            }
            else
            {
                TargetName = targetAffix + AddInSettings.SEP_TEXT;

                var mainBlocks = OriginalName.Split(AddInSettings.SEP_FILE_NUMERATION.ToCharArray());
                if (mainBlocks.Length > 1)
                {
                    //TODO: Degisiklik
                    var numerationBlocks = CommonResources.GetNumberingFromName(mainBlocks[1], AddInSettings.SEP_FILE_NUMERATION);
                    if (numerationBlocks.Count > 2)
                        numerationBlocks.RemoveRange(0, 2);

                    TargetName += string.Join(AddInSettings.SEP_FILE_NUMERATION, numerationBlocks) + AddInSettings.SEP_FILE_NUMERATION + AddInSettings.REV_NO_PFX + RevNo.ToString();
                }
                else
                {
                    TargetName += OriginalName;
                }
            }

            TargetPath_PaG = TargetDir + DirAnnex_PaG + Path.DirectorySeparatorChar + TargetName + Ext_PaG;

            if (!IsStandartPart)
            {
                TargetPath_Sas = TargetDir + DirAnnex_Sas + Path.DirectorySeparatorChar + TargetName + Ext_Sas;

                //TODO: SheetMetal'i nasil detect edecen?
                if (DocType == swDocumentTypes_e.swDocPART)
                    TargetPath_DXF = TargetDir + DirAnnex_DXF + Path.DirectorySeparatorChar + TargetName + Ext_DXF;
            }   
        }
    }
}
