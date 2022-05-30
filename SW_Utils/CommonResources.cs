using AngelSix.SolidDna;
using Dna;
using ns;
using SolidWorks.Interop.swdimxpert;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;
using System.Windows.Data;

namespace SW_Utils
{
    public static class CommonResources
    {
        public const string SEP_ENUM_MASK = ", ";
        public const string TEMP_FILE_START = "~$";

        public delegate void UIControlChanged(object UIcontrol);

        /// <summary>
        /// Opens the given folder in Windows Explorer
        /// </summary>
        /// <param name="path"></param>
        public static void OpenFolder(string path)
        {
            try
            {
                var startInfo = new ProcessStartInfo
                {
                    Arguments = path,
                    FileName = "explorer.exe"
                };
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
            }
        }

        //TODO: Burayi AddInSettings'le koordine etmek lazim
        /// <summary>
        /// Formats the given filename as a copied version
        /// </summary>
        /// <param name="originalFileName"></param>
        /// <returns></returns>
        public static string GetCopyFileName(string originalFileName)
        {
            return originalFileName + "-Copy";
        }



        //DONE
        /// <summary>
        /// returns numerically ascending ordered file/directory paths in a given directory path
        /// </summary>
        /// <param name="path">path of the directory</param>
        /// <param name="isFile">true for files, false for directories</param>
        /// <param name="ext">optional extension filter in the form of ".EXT" (default is null meaning all files)</param>
        /// <returns></returns>
        public static string[] GetSortedSubPaths(string path, bool isFile, string ext = null)
        {
            try
            {
                string[] subPaths;
                if (isFile)
                    subPaths = ext.IsNullOrEmpty() ? Directory.GetFiles(path) : Directory.GetFiles(path, "*" + ext);
                else
                    subPaths = Directory.GetDirectories(path);

                //filter out temporary files
                var pathList = new List<string>();
                for (int i = 0; i < subPaths.Length; i++)
                {
                    if (!Path.GetFileName(subPaths[i]).StartsWith(TEMP_FILE_START))
                        pathList.Add(subPaths[i]);
                }
                subPaths = new string[pathList.Count];
                for (int i = 0; i < subPaths.Length; i++)
                {
                    subPaths[i] = pathList[i];
                }

                Array.Sort(subPaths, new NumericComparer());
                return subPaths;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        //DONE
        /// <summary>
        /// Returns only numerically prefixed ascending ordered file/folder list. If isFile=true and fullyPrefixed (see <see cref="GetFullPrefixFromPath(string)"/>) files are found, this list will be returned.
        /// </summary>
        /// <param name="path">path of the directory</param>
        /// <param name="isFile">true for files, false for directories</param>
        /// <param name="ext">optional extension filter in the form of ".EXT" (default is null meaning all files)</param>
        /// <returns></returns>
        public static List<string> GetNumeratedSubPaths(string path, bool isFile, string parentPrefix=null, string ext = null)
        {
            path = path.Replace('/', '\\');
            path = path.Trim('\\');

            if(File.Exists(path))
            {
                Logger.Log("No sub path for a file name", MessageTypeEnum.Error);
                return null;
            }

            List<string> numeratedPaths = new List<string>();

            if (Directory.Exists(path))
            {
                var allPaths = GetSortedSubPaths(path, isFile, ext);


                if (isFile)
                {
                    for (int i = 0; i < allPaths.Length; i++)
                    {
                        var nextPrefix = GetPrefixFromName(Path.GetFileName(allPaths[i]));
                        if (!nextPrefix.IsNullOrEmpty())
                        {
                            if(parentPrefix.IsNullOrEmpty())
                            {
                                numeratedPaths.Add(allPaths[i]);
                            }
                            else
                            {
                                if(nextPrefix.StartsWith(parentPrefix))
                                {
                                    var parentNumbering = GetNumberingFromName(parentPrefix, AddInSettings.SEP_FILE_NUMERATION);
                                    var nextNumebering = GetNumberingFromName(nextPrefix, AddInSettings.SEP_FILE_NUMERATION);
                                    if (nextNumebering.Count - parentNumbering.Count == 1)
                                        numeratedPaths.Add(allPaths[i]);
                                }
                            }
                        }
                            
                    }
                }
                else
                {
                    for (int i = 0; i < allPaths.Length; i++)
                    {
                        if (GetFolderNumberFromName(Path.GetFileName(allPaths[i])) > -1)
                            numeratedPaths.Add(allPaths[i]);
                    }
                }

                
            }

            return numeratedPaths;
        }

        //DONE - belki direkt nameden olur
        /// <summary>
        /// First trims the name string with <see cref="AddInSettings.SEP_TEXT"/> and removes the TOOLBOX_PROJ_STR prefix. Then tries to split the text with the given seperator. If any of the blocks is not a number, returns empty list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="numberSeperator"></param>
        /// <returns></returns>
        public static List<int> GetNumberingFromName(string name, string numberSeperator)
        {
            if (name.IsNullOrEmpty())
                return null;

            //remove extension
            var extDotIdx = name.LastIndexOf(".");
            if (extDotIdx > -1)
                name = name.Remove(extDotIdx);

            //remove text
            name = name.Split(AddInSettings.SEP_TEXT.ToCharArray()).First();

            var numberBlocks = name.Split(numberSeperator.ToCharArray());

            var numberList = new List<int>();

            if(numberBlocks.Length>0)
            {
                var startIdx = numberBlocks.First() == AddInSettings.TOOLBOX_PROJ_STR ? 1 : 0;
                for (int i = startIdx; i < numberBlocks.Length; i++)
                {
                    if (int.TryParse(numberBlocks[i], out int n))
                        numberList.Add(n);
                }
            }

            return numberList;
        }

        //DONE
        /// <summary>
        /// returns <see cref="AddInSettings.SEP_FOLDER_NUMERATION"/> seperated numbering in the beginning of a text
        /// </summary>
        /// <param name="name">folder name without extension</param>
        /// <returns></returns>
        public static int GetFolderNumberFromName(string name)
        {
            var nameBlocks = name.Split(AddInSettings.SEP_FOLDER_NUMERATION.ToCharArray());

            if (nameBlocks.Length > 0)
            {
                if (int.TryParse(nameBlocks[0], out int n))
                    return n;
            }
            
            return -1;
        }

        //TODO: Extension ayiklamiyor (sanki artik ayikliyor?)
        /// <summary>
        /// returns the prefix in the name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string GetPrefixFromName(string name)
        {
            if (!name.IsNullOrEmpty())
            {
                //remove any extension like suffix
                name = Path.GetFileNameWithoutExtension(name);

                //if it is a simple numerated name(like a folder)
                var folderNumber = GetFolderNumberFromName(name);
                if (folderNumber > -1)
                    return folderNumber.ToString();

                //if it is a fullPrefixed name (like a file)
                //toolbox file
                var tbxPfx = AddInSettings.TOOLBOX_PFX_START;
                if (name.StartsWith(tbxPfx))
                    return tbxPfx + string.Join(AddInSettings.SEP_FILE_NUMERATION, GetNumberingFromName(name.Substring(tbxPfx.Length), AddInSettings.SEP_FILE_NUMERATION));
                else//normal file
                    return string.Join(AddInSettings.SEP_FILE_NUMERATION, GetNumberingFromName(name, AddInSettings.SEP_FILE_NUMERATION));


                ////if it is a fullPrefixed name (like a file)
                //return string.Join(AddInSettings.SEP_FILE_NUMERATION, GetNumberingFromName(name, AddInSettings.SEP_FILE_NUMERATION));
                //var numList = GetNumberingFromName(name, AddInSettings.SEP_FILE_NUMERATION);
                //if (numList.Count > 0)
                //    return name.Split(AddInSettings.SEP_TEXT.ToCharArray()).First();
            }

            return string.Empty;
            //Bunu acinca IsNullOrEmpty() checkleri sasiyor
            //return AddInSettings.EMPTY_PREFIX;
        }

        /// <summary>
        /// Returns full prefix from a given (DO NOT CALL WITH FILE PATH!)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static  string GetFullPrefixFromPath(string path)
        {
            path = path.Replace('/', '\\');
            path = path.Trim('\\');

            //if it is a filePath & the file exists (not operational for virtual filePaths)
            if (File.Exists(path))
            {
                var fileName = Path.GetFileNameWithoutExtension(path);
                return GetPrefixFromName(fileName);
            }

            //if it is a folderPath
            var relativePath = string.Empty;
            var fullPrefix = string.Empty;

            if (path.StartsWith(AddInSettings.PATH_PROJECTS))
            {
                relativePath = path.Replace(AddInSettings.PATH_PROJECTS, string.Empty);
                
            }
            else if (path.StartsWith(AddInSettings.PATH_PRINT_PROJ))
            {
                relativePath = path.Replace(AddInSettings.PATH_PRINT_PROJ, string.Empty);

            }
            else if (path.StartsWith(AddInSettings.PATH_TOOLBOX))
            {
                relativePath = path.Replace(AddInSettings.PATH_TOOLBOX, string.Empty);
                fullPrefix = AddInSettings.TOOLBOX_PROJ_STR;
            }
            else
            {
                //TODO: proje klasoru disindaki folderlar icin bu mantik yeterli mi?
                return AddInSettings.SEQUENCE_START.ToString();
            }

            var subFolderNames = relativePath.Trim('\\').Split('\\');

            if (fullPrefix.IsNullOrEmpty())
                fullPrefix = GetPrefixFromName(subFolderNames.First());

            if(subFolderNames.Length>1)
            {
                for (int i = 1; i < subFolderNames.Length; i++)
                {
                    fullPrefix += AddInSettings.SEP_FILE_NUMERATION + GetPrefixFromName(subFolderNames[i]);
                }
            }

            return fullPrefix;
        }

        //DONE
        /// <summary>
        /// Returns the appropriate prefix for the next file/folder in a directory. Returns correct prefix even if the path does not exist
        /// </summary>
        /// <param name="dirPath">full path of the container directory</param>
        /// <param name="isFile">true for files, false for folders</param>
        /// <param name="ext">optional extension filter in the form of ".EXT" (default is null meaning all files)</param>
        /// <returns>appropriate prefix</returns>
        public static string GetNextChildPrefix(string dirPath, bool isFile, string ext = null, string parentPrefix = null)
        {
            try
            {
                var subPaths = new List<string>();
                var numList = new List<int>();

                //TODO: Daha iyi cozumler olabilir ama bu simdilik calisiyor. Bunun aslinda dirPath dosya ismiyse onu parent olarak kullanmasi gerekiyor sanki.
                var possibleToolboxPrefix = dirPath.StartsWith(AddInSettings.PATH_TOOLBOX) ? AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_FILE_NUMERATION : string.Empty;

                if (Directory.Exists(dirPath))
                {
                    subPaths = GetNumeratedSubPaths(dirPath, isFile, parentPrefix, ext);
                }

                if (subPaths.Count > 0)
                {
                    var lastFileName = Path.GetFileName(subPaths.Last());

                    if (isFile)
                    {
                        numList = GetNumberingFromName(lastFileName, AddInSettings.SEP_FILE_NUMERATION);
                    }
                    else
                    {
                        numList.Add(GetFolderNumberFromName(lastFileName));
                    }
                    numList[numList.Count - 1] += 1;
                }
                else
                {
                    if (isFile)
                    {
                        var dirPrefix = GetFullPrefixFromPath(dirPath);
                        numList = GetNumberingFromName(dirPrefix, AddInSettings.SEP_FILE_NUMERATION);
                    }
                    numList.Add(AddInSettings.SEQUENCE_START);
                }

                var nextPrefix = string.Join(AddInSettings.SEP_FILE_NUMERATION, numList);

                if (isFile)
                    nextPrefix = possibleToolboxPrefix + nextPrefix;

                return nextPrefix;

            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return string.Empty;
            }
            
        }

        //DONE
        /// <summary>
        /// Returns the appropriate prefix for the next file/folder in the container directory of the given path
        /// </summary>
        /// <param name="path">full path of the peer file/folder</param>
        /// <param name="isFile">true for files, false for folders</param>
        /// <param name="ext">optional extension filter in the form of ".EXT" (default is null meaning all files)</param>
        /// <returns>appropriate prefix</returns>
        public static string GetNextPeerPrefix(string path, bool isFile, string ext = null)
        {
            var dirPath = Path.GetDirectoryName(path);
            return GetNextChildPrefix(dirPath, isFile, ext);
        }

        /// <summary>
        /// Artik yanlis olabilir!
        /// takes the FileNameWithoutExtension and returns the string after the first occurance of the <see cref="AddInSettings.SEP_TEXT"/>, including <see cref="AddInSettings.SEP_TEXT"/> (for D:\\Folder\1-1-1_Text.SLDPRT => _Text.SLDPRT)/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string GetNameBlock(string path)
        {
            var nameWithOutExtension = Path.GetFileNameWithoutExtension(path);
            var pfx = GetPrefixFromName(nameWithOutExtension);

            if (pfx == nameWithOutExtension)
                return string.Empty;
            else if (pfx == string.Empty)
                return nameWithOutExtension;
            else
                return nameWithOutExtension.Substring(pfx.Length + 1);

            ////var nameWithExtension = Path.GetFileName(path);
            ////var sepIdx = nameWithExtension.IndexOf(AddInSettings.SEP_TEXT);
            ////return nameWithExtension.Substring(sepIdx + 1);

            //var nameWithOutExtension = Path.GetFileNameWithoutExtension(path);
            //var sepIdx = nameWithOutExtension.IndexOf(AddInSettings.SEP_TEXT);
            //var pfx = GetPrefixFromName(nameWithOutExtension);

            ////TODO: GetPrefixFromName'in ext dâhil dondurmesi cok sakat
            ////eger prefixten ibaretse
            //if (pfx == nameWithOutExtension)
            //    return Path.GetExtension(path);
            //else
            //    return AddInSettings.SEP_TEXT + Path.GetFileName(path).Substring(sepIdx + 1);
        }


        public static string GetNextRelaseDirectory(string draftPath)
        {
            try
            {
                var draftDir = Path.GetDirectoryName(draftPath);
                var cleanedDir = draftDir.Replace(AddInSettings.PATH_PROJECTS, string.Empty);
                cleanedDir = cleanedDir.Trim(Path.DirectorySeparatorChar);
                var folderBlocks = cleanedDir.Split(Path.DirectorySeparatorChar);
                var releaseDir = AddInSettings.PATH_PROJECTS + Path.DirectorySeparatorChar + folderBlocks.First() + Path.DirectorySeparatorChar + AddInSettings.FOLDER_RELEASE;
                return releaseDir + Path.DirectorySeparatorChar + GetNextChildPrefix(releaseDir, false);
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        public static List<string> GetEnumMaskList(int enumValue, Type enumType)
        {
            var resultSet = new List<string>();
            foreach (int item in Enum.GetValues(enumType))
            {
                if ((item & enumValue) == item)
                    resultSet.Add(Enum.GetName(enumType, item));
            }
            return resultSet;
        }

        public static string GetEnumMaskString(int enumValue, Type enumType)
        {
            return string.Join(SEP_ENUM_MASK, GetEnumMaskList(enumValue, enumType));
        }


        //private static readonly char[] BaseChars =
        // "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        //private static readonly char[] BaseChars =
        // "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly char[] BaseChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();

        private static readonly Dictionary<char, int> CharValues = BaseChars
                   .Select((c, i) => new { Char = c, Index = i })
                   .ToDictionary(c => c.Char, c => c.Index);

        public static string LongToBase(long value)
        {
            long targetBase = BaseChars.Length;
            // Determine exact number of characters to use.
            char[] buffer = new char[Math.Max(
                       (int)Math.Ceiling(Math.Log(value + 1, targetBase)), 1)];

            var i = buffer.Length;
            do
            {
                buffer[--i] = BaseChars[value % targetBase];
                value = value / targetBase;
            }
            while (value > 0);

            var result = new string(buffer, i, buffer.Length - i);

            if (result.Length < 2)
                result = "A" + result;

            return result;
        }

        public static long BaseToLong(string number)
        {
            char[] chrs = number.ToCharArray();
            int m = chrs.Length - 1;
            int n = BaseChars.Length, x;
            long result = 0;
            for (int i = 0; i < chrs.Length; i++)
            {
                x = CharValues[chrs[i]];
                result += x * (long)Math.Pow(n, m--);
            }
            return result;
        }

        /// <summary>
        /// Checks whether the path starts with <see cref="AddInSettings.PATH_TOOLBOX"/>
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsToolboxComponentPath(string path)
        {
            //TODO: acaba daha dettay kontrol gerekir mi
            return path.ToLower().StartsWith(AddInSettings.PATH_TOOLBOX.ToLower());
            //Path.GetFileName(path).StartsWith(AddInSettings.TOOLBOX_PROJ_STR + AddInSettings.SEP_FILE_NUMERATION)
        }




        ///// <summary>
        ///// Returns the numbering array (idx=0 : Leftmost) in the prefix of the file/folder name
        ///// </summary>
        ///// <param name="path">full path of the file/folder</param>
        ///// <param name="numberSepString">optional number seperator string (default is <see cref="AddInSettings.SEP_NUMBER"/>)</param>
        ///// <returns>numbering array (idx=0 : Leftmost) </returns>
        //public static List<int> GetNumberingFromPath(string path, string numberSepString = null)
        //{
        //    var numberSeperator = numberSepString.IsNullOrEmpty() ? AddInSettings.SEP_NUMBER : numberSepString;

        //    var cleanedText = Path.GetFileName(path);

        //    return GetNumberingFromName(cleanedText);

        //    //var numberBlocks = cleanedText.Split(numberSeperator.ToCharArray());

        //    //var numberList = new List<int>();

        //    //for (int i = 0; i < numberBlocks.Length; i++)
        //    //{
        //    //    if (int.TryParse(numberBlocks[i], out int n))
        //    //        numberList.Add(n);
        //    //    else
        //    //        break;
        //    //}

        //    //return numberList;

        //}



        ///// <summary>
        ///// Returns the numbering prefix of the file/folder name of the given path
        ///// </summary>
        ///// <param name="path"></param>
        ///// <param name="numberSepString"></param>
        ///// <returns></returns>
        //public static string GetLastPrefixFromPath(string path, string numberSepString = null)
        //{
        //    var numberSeperator = numberSepString.IsNullOrEmpty() ? AddInSettings.SEP_NUMBER : numberSepString;
        //    return string.Join(numberSeperator, GetNumberingFromPath(path, numberSeperator));
        //}





        /// <summary>
        /// returns Material from string representation
        /// </summary>
        /// <param name="materialText"></param>
        /// <returns></returns>
        public static Material GetMaterialFromText(string materialText)
        {
            //TODO: Based on AngelSix' Material.ToString() (Not that safe)
            try
            {
                var dbStart = materialText.LastIndexOf('[') + 1;
                var dbLength = materialText.LastIndexOf(']') - dbStart;

                var clStart = materialText.LastIndexOf('(') + 1;
                var clLength = materialText.LastIndexOf(')') - clStart;

                var nmStart = 0;
                var nmLength = clStart - 2;

                var material = new Material()
                {
                    Database = materialText.Substring(dbStart, dbLength),
                    Classification = materialText.Substring(clStart, clLength),
                    Name = materialText.Substring(nmStart, nmLength)
                };

                if (SolidWorksEnvironment.Application != null)
                {
                    var allMaterials = SolidWorksEnvironment.Application.GetMaterials(material.Database);
                    var filteredMaterials = allMaterials.Where(x => x.Classification == material.Classification && x.Name == material.Name);
                    //var filteredMaterials = from x in allMaterials where x.Classification == material.Classification && x.Name == material.Name select x;//allMaterials.Where(x => x.Classification == material.Classification && x.Name == material.Name);
                    material = filteredMaterials.First();
                    //material = SolidWorksEnvironment.Application.GetMaterials(material.Database).Where(
                    //    x => x.Classification == material.Classification && x.Name == material.Name).First();
                }


                return material;
            }
            catch (Exception ex)
            {
                Logger.LogException(ex);
                return null;
            }
        }

        /// <summary>
        /// Type of possible saveble document object in <see cref="SW_Utils"/>
        /// </summary>
        
    }

    public enum ObjectTypeEnum
    {
        Project,
        Folder,
        Part,
        Assembly,
        Drawing,
        None
    }

    public enum MessageTypeEnum
    {
        Normal,
        Warning,
        Error,
        Greeting,
        Exception,
        Information,
        None
    }

    public enum DocOpsTypeEnum
    {
        Project,
        Toolbox,
        Print
    }

    public enum InsertationDirectionEnum
    {
        ModelIntoControl,
        ControlIntoModel
    }
}
