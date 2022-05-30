using AngelSix.SolidDna;
using System;
using System.IO;
using System.Xml.Serialization;

namespace SW_Utils
{
    public class AddInSettings
    {
        public const string SETTINGS_LOC_FILE_NAME = @"\SettingsLocation.txt";
        public const string SETTINGS_FILE_NAME = @"\AddInSettings.xml";
        public const string DFLT_SETTINGS_FILE_DIR = @"D:\mekanik\Resources\MAM_AddInSettings";

        //Bunlar hep Uppercase olmali (PackAndGoItem kullaniyor)
        public const string EXT_PART = ".SLDPRT";
        public const string EXT_ASSM = ".SLDASM";
        public const string EXT_DRAW = ".SLDDRW";

        public const string EXT_PART_T = ".PRTDOT";
        public const string EXT_ASSM_T = ".ASMDOT";
        public const string EXT_DRAW_T = ".DRWDOT";

        public const string EXT_ALT_SOLID = ".STEP";
        public const string EXT_ALT_DRAW = ".PDF";
        public const string EXT_ALT_SHEET = ".DXF";

        private const string DFLT_PROJ_LOC = @"D:\mekanik";
        private const string DFLT_TOOLBOX_LOC = @"D:\mekanik\Resources\MAM_Toolbox";
        private const string DFLT_PRINT_PROJ_LOC = @"G:\My Drive\1.PRIVATE\1.3DP_OWN";  //@"D:\GoogleDrive\1.PRIVATE\1.3DP_OWN";

        private const string DFLT_DIR_TEMPL_PART = @"D:\mekanik\Resources\MAM_Templates\MAM_Parts";
        private const string DFLT_DIR_TEMPL_ASSM = @"D:\mekanik\Resources\MAM_Templates\MAM_Assemblies";
        private const string DFLT_DIR_TEMPL_DRAW = @"D:\mekanik\Resources\MAM_Templates\MAM_Drawings";
                

        //1 karakteri gecmeleri dert yaratir!
        private const string DFLT_SEP_FOLDER_NUMERATION = ".";
        private const string DFLT_SEP_FILE_NUMERATION = "-";
        private const string DFLT_SEP_TEXT = "_";
        //private const string DFLT_SEP_PROJECTFOLDER_NUMERATION = "P";
        //private const string DFLT_SEP_DRAFTFOLDER_NUMERATION = "D";

        //private const string DFLT_SEP_NUMBER = ".";
        //private const string DFLT_SEP_PROJECT = "-";

        private const string DFLT_FOLDER_DRAFT = @"DRAFT";
        private const string DFLT_FOLDER_RELEASE = @"RELEASE";
        
        private const string DFLT_MATERIAL = @"6061-O (Aluminyum) [d:\mekanik\resources\mam_materialdb\malzeme_bama.sldmat]";
        private const int DFLT_SEQ_START = 1;
        private const string DFLT_TOOLBOX_PROJ_STR = "S";
        private const string DFLT_REV_NO_PFX = "r";

        private const string DFLT_FOLDER_SOLIDS = @"KATI_MODELLER_SW";
        private const string DFLT_FOLDER_SOLIDS_STEP = @"KATI_MODELLER_STEP";
        private const string DFLT_FOLDER_DRAWINGS = @"TEKNIK_RESIMLER_SW";
        private const string DFLT_FOLDER_DRAWINGS_PDF = @"TEKNIK_RESIMLER_PDF";
        private const string DFLT_FOLDER_CUTTING_PATHS = @"KESIM_DOSYALARI_DXF";

        private const string DFLT_CREATED_INTO_SFX = "CI";
        private const string DFLT_CREATED_FROM_SFX = "CF";

        //public const string DFLT_EMPTY_PREFIX = "X";

        //[XmlIgnore]
        private static AddInSettings Instance = new AddInSettings();
        public string Dir_Projects { get; set; }
        public string Dir_Templates_Part { get; set; }
        public string Dir_Templates_Assm { get; set; }
        public string Dir_Templates_Draw { get; set; }
        public string DFLT_Material { get; set; }
        public int Seq_Start { get; set; }

        public string Dir_Toolbox { get; set; }
        public string Dir_PrintProj { get; set; }
                
        public string Sep_Folder_Num { get; set; }
        public string Sep_File_Num { get; set; }
        public string Sep_Text { get; set; }
        //public string Sep_Proj_Num { get; set; }

        //public string Sep_Number { get; set; }
        //public string Sep_Project { get; set; }

        public string Folder_Draft { get; set; }
        public string Folder_Release { get; set; }
        public string Toolbox_Proj_Str { get; set; }

        public string RevNo_Prefix { get; set; }

        public string Folder_Solids { get; set; }
        public string Folder_Solids_Step { get; set; }
        public string Folder_Drawings { get; set; }
        public string Folder_Drawings_Pdf { get; set; }
        public string Folder_Cutting_Paths { get; set; }

        public string Created_From_SFX { get; set; }
        public string Created_Into_SFX { get; set; }

        //public string EmptyPrefix { get; set; }

        public AddInSettings()
        {
            Dir_Projects = DFLT_PROJ_LOC;
            Dir_Toolbox = DFLT_TOOLBOX_LOC;
            Dir_PrintProj = DFLT_PRINT_PROJ_LOC;

            Dir_Templates_Part = DFLT_DIR_TEMPL_PART;
            Dir_Templates_Assm = DFLT_DIR_TEMPL_ASSM;
            Dir_Templates_Draw = DFLT_DIR_TEMPL_DRAW;

            Sep_Folder_Num = DFLT_SEP_FOLDER_NUMERATION;
            Sep_File_Num = DFLT_SEP_FILE_NUMERATION;
            Sep_Text = DFLT_SEP_TEXT;
            //Sep_Proj_Num = DFLT_SEP_PROJECTFOLDER_NUMERATION;

            //Sep_Number = DFLT_SEP_NUMBER;
            //Sep_Project = DFLT_SEP_PROJECT;

            Folder_Draft = DFLT_FOLDER_DRAFT;
            Folder_Release = DFLT_FOLDER_RELEASE;

            DFLT_Material = DFLT_MATERIAL;
            Seq_Start = DFLT_SEQ_START;
            Toolbox_Proj_Str = DFLT_TOOLBOX_PROJ_STR;
            RevNo_Prefix = DFLT_REV_NO_PFX;

            Folder_Solids = DFLT_FOLDER_SOLIDS;
            Folder_Solids_Step = DFLT_FOLDER_SOLIDS_STEP;
            Folder_Drawings = DFLT_FOLDER_DRAWINGS;
            Folder_Drawings_Pdf = DFLT_FOLDER_DRAWINGS_PDF;
            Folder_Cutting_Paths = DFLT_FOLDER_CUTTING_PATHS;

            //EmptyPrefix = DFLT_EMPTY_PREFIX;

            Created_From_SFX = DFLT_CREATED_FROM_SFX;
            Created_Into_SFX = DFLT_CREATED_INTO_SFX;
        }


        public static string PATH_PROJECTS => Instance.Dir_Projects;
        public static string PATH_TOOLBOX => Instance.Dir_Toolbox;
        public static string PATH_PRINT_PROJ => Instance.Dir_PrintProj;

        public static string DIR_TEMPL_PART => Instance.Dir_Templates_Part;
        public static string DIR_TEMPL_ASSM => Instance.Dir_Templates_Assm;
        public static string DIR_TEMPL_DRAW => Instance.Dir_Templates_Draw;

        public static string SEP_FOLDER_NUMERATION => Instance.Sep_Folder_Num;
        public static string SEP_FILE_NUMERATION => Instance.Sep_File_Num;
        public static string SEP_TEXT => Instance.Sep_Text;
        //public static string SEP_PROJECTFOLDER_NUMERATION => Instance.Sep_Proj_Num;

        //TODO: Bunlari kaldir
        //public static string SEP_NUMBER => "-";
        //public static string SEP_PROJECT => "-";

        public static string FOLDER_DRAFT => Instance.Folder_Draft;
        public static string FOLDER_RELEASE => Instance.Folder_Release;

        public static Material DFT_MATERIAL => CommonResources.GetMaterialFromText(Instance.DFLT_Material);
        public static int SEQUENCE_START => Instance.Seq_Start;
        public static string TOOLBOX_PROJ_STR => Instance.Toolbox_Proj_Str;
        public static string REV_NO_PFX => Instance.RevNo_Prefix;

        public static string FOLDER_SOLID_SW => Instance.Folder_Solids;
        public static string FOLDER_SOLID_STEP => Instance.Folder_Solids_Step;
        public static string FOLDER_DRAW_SW => Instance.Folder_Drawings;
        public static string FOLDER_DRAW_PDF => Instance.Folder_Drawings_Pdf;
        public static string FOLDER_CUT_DXF => Instance.Folder_Cutting_Paths;

        //public static string EMPTY_PREFIX => Instance.EmptyPrefix;

        public static string CREATED_FROM_SFX => Instance.Created_From_SFX;
        public static string CREATED_INTO_SFX => Instance.Created_Into_SFX;

        public static string TOOLBOX_PFX_START => TOOLBOX_PROJ_STR + SEP_FILE_NUMERATION;

        public static void Reset()
        {
            Instance = new AddInSettings();
        }

        public static void Save()
        {
            try
            {
                //default TXT dir
                //var settingsLocationFilePath = Environment.CurrentDirectory + SETTINGS_LOC_FILE_NAME;
                var settingsLocationFilePath = SolidWorksEnvironment.Application.AssemblyPath() + SETTINGS_LOC_FILE_NAME;

                //if there exists no file, default one is saved (TXT)
                if (!File.Exists(settingsLocationFilePath))
                {
                    TextWriter txtWriter = new StreamWriter(settingsLocationFilePath);

                    var defaultSettingsFilePath = 
                        Directory.Exists(DFLT_SETTINGS_FILE_DIR) ?
                        DFLT_SETTINGS_FILE_DIR + SETTINGS_FILE_NAME : 
                        Environment.CurrentDirectory + SETTINGS_FILE_NAME;
                    txtWriter.WriteLine(defaultSettingsFilePath);
                    //txtWriter.WriteLine(Environment.CurrentDirectory + SETTINGS_FILE_NAME);
                    txtWriter.Close();
                    Logger.Log("New settings location file is created at " + settingsLocationFilePath, MessageTypeEnum.Greeting);
                }

                //settings file path is taken
                TextReader txtReader = new StreamReader(settingsLocationFilePath);
                var settingsFilePath = txtReader.ReadLine();
                txtReader.Close();

                //setings are saved (created or overwritten XML)
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AddInSettings));
                TextWriter xmlWriter = new StreamWriter(settingsFilePath);
                xmlSerializer.Serialize(xmlWriter, Instance);
                xmlWriter.Close();

                Logger.Log("Settings are saved to " + settingsFilePath, MessageTypeEnum.Greeting);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        public static void Load()
        {
            try
            {
                //default TXT dir
                //var settingsLocationFilePath = Environment.CurrentDirectory + SETTINGS_LOC_FILE_NAME;
                var settingsLocationFilePath = SolidWorksEnvironment.Application.AssemblyPath() + SETTINGS_LOC_FILE_NAME;

                //if there exists no file, default one is saved (TXT) Caution: if there exists an XML in the default location, it will be overwritten with a default one
                if (!File.Exists(settingsLocationFilePath))
                {
                    Save();
                }

                //settings file path is taken
                TextReader txtReader = new StreamReader(settingsLocationFilePath);
                var settingsFilePath = txtReader.ReadLine();
                txtReader.Close();

                //Instance is updated from the settings file path
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(AddInSettings));
                using(Stream xmlStream = new FileStream(settingsFilePath, FileMode.Open))
                {
                    Instance = (AddInSettings)xmlSerializer.Deserialize(xmlStream);
                }

                //Logger.Log("Settings are loaded from " + settingsFilePath);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

    }
}
