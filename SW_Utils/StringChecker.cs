using Dna;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SW_Utils
{
    public class StringChecker
    {
        /// <summary>
        /// Default Invalid Characters (Forbidden to be anywhere)
        /// </summary>
        public const string DFLT_INVALID_CHARS = @"#%&{}\<>*?/$!':@+`|="" ";

        /// <summary>
        /// Default Trimming Characters (Forbidden to be at beginning or end)
        /// </summary>
        public const string DFLT_TRIM_CHARS = @" .-_";

        /// <summary>
        /// Turkish characters and their proper replacements.
        /// </summary>
        public List<KeyValuePair<char, char>> TurkishCharacters = new List<KeyValuePair<char, char>>()
        {
            new KeyValuePair<char, char>('ç', 'c'),
            new KeyValuePair<char, char>('ğ', 'g'),
            new KeyValuePair<char, char>('ı', 'i'),
            new KeyValuePair<char, char>('ö', 'o'),
            new KeyValuePair<char, char>('ş', 's'),
            new KeyValuePair<char, char>('ü', 'u'),
            new KeyValuePair<char, char>('Ç', 'C'),
            new KeyValuePair<char, char>('Ğ', 'G'),
            new KeyValuePair<char, char>('İ', 'I'),
            new KeyValuePair<char, char>('Ö', 'O'),
            new KeyValuePair<char, char>('Ş', 'S'),
            new KeyValuePair<char, char>('Ü', 'U')
        };

        /// <summary>
        /// Reserved Windows System names
        /// </summary>
        public static string[] RESERVED_NAMES = {
            "CON",
            "PRN",
            "AUX",
            "NUL",
            "COM1",
            "COM2",
            "COM3",
            "COM4",
            "COM5",
            "COM6",
            "COM7",
            "COM8",
            "COM9",
            "LPT1",
            "LPT2",
            "LPT3",
            "LPT4",
            "LPT5",
            "LPT6",
            "LPT7",
            "LPT8",
            "LPT9"
        };

        /// <summary>
        /// Character limit allowed.
        /// </summary>
        public int CharLimit { get; set; } = 250;

        /// <summary>
        /// Characters not allowed in anywhere of the string.
        /// </summary>
        public string InvalidChars { get; set; } = DFLT_INVALID_CHARS;

        /// <summary>
        /// Characters not allowed in the beginning and end of the string.
        /// </summary>
        public string TrimChars { get; set; } = DFLT_TRIM_CHARS;

        /// <summary>
        /// Determines whether white space character is allowed in string.
        /// </summary>
        public bool IsSpaceAllowed 
        {
            get { return InvalidChars.Contains(" "); }
            set 
            {
                if(!value)
                {
                    if (!InvalidChars.Contains(" "))
                        InvalidChars += " ";
                }
                else
                {
                    if (InvalidChars.Contains(" "))
                        InvalidChars=InvalidChars.Trim();
                    //InvalidChars.Replace(" ", "");
                }
            }
        }

        /// <summary>
        /// Determines whether Turkish characters are allowed in string.
        /// </summary>
        public bool IsTurkishCharsAllowed { get; set; } = false;

        /// <summary>
        /// Enables the check for Windows reserved file/folder names. If the string will be used as a file or folder name, then this should be true.
        /// </summary>
        public bool IsSystemName { get; set; } = true;

        /// <summary>
        /// Determines whether empty string is allowed.
        /// </summary>
        public bool IsEmptyStringAllowed { get; set; } = true;

        public StringChecker()
        {

        }

        /// <summary>
        /// Cleans the string. (If the result is empty, it will be returned regardless of <see cref="IsEmptyStringAllowed"/> 
        /// (For directory adresses, <see cref="CleanDirectoryString(string)"/> should be used.)/>
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public string CleanString(string inputString)
        {
            //original string is saved
            var originalString = inputString;

            //correction for invalid chars
            foreach (char c in InvalidChars.ToCharArray())
            {
                inputString = inputString.Replace(c.ToString(), "");
            }

            //correction for start and end
            inputString = inputString.Trim(TrimChars.ToCharArray());

            //correction for Turkish chars
            if (!IsTurkishCharsAllowed)
            {
                for (int i = 0; i < TurkishCharacters.Count; i++)
                {
                    inputString = inputString.Replace(TurkishCharacters[i].Key, TurkishCharacters[i].Value);
                }
            }

            //correction for character limit
            if (inputString.Length > CharLimit)
                inputString = inputString.Substring(0, CharLimit);

            //if the clean string is a reserved name, empty string is returned
            if (IsSystemName && IsNameReserved(inputString))
            {
                Logger.Log(originalString + " (" + inputString + ") is a reserved name", MessageTypeEnum.Warning);
                return string.Empty;
            }

            //if the clean string is emty, then it means it's too invalid to clean
            if (!IsEmptyStringAllowed && inputString == string.Empty)
            {
                Logger.Log(originalString + " is a super invalid name", MessageTypeEnum.Warning);
            }           

            //result is returned
            return inputString;
        }

        /// <summary>
        /// Checks whether the string is valid.
        /// (For directory adresses, <see cref="IsDirectoryValid(string)"/> should be used.)/>
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public bool IsStringValid(string inputString)
        {
            //empty check
            if (!IsEmptyStringAllowed && inputString.IsNullOrEmpty())
                return false;

            //character limit check
            if (inputString.Length > CharLimit)
                return false;

            //reserved name check
            if (IsSystemName && IsNameReserved(inputString))
                return false;

            //invalid char check
            if (inputString.IndexOfAny(InvalidChars.ToCharArray()) > -1)
                return false;

            //start/end check
            foreach (char c in TrimChars.ToCharArray())
            {
                if (inputString.StartsWith(c.ToString()))
                    return false;

                if (inputString.EndsWith(c.ToString()))
                    return false;
            }

            //Turkish character check
            if (!IsTurkishCharsAllowed)
            {
                for (int i = 0; i < TurkishCharacters.Count; i++)
                {
                    if (inputString.Contains(TurkishCharacters[i].Key))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Cleans a directory adress. (If something goes wrong, Root directory of the <see cref="AddInSettings.PATH_PROJECTS"/> is returned.)
        /// (For ordinary words like file names, <see cref="CleanString(string)"/> should be used.)/>
        /// </summary>
        /// <param name="dirString"></param>
        /// <returns></returns>
        public string CleanDirectoryString(string dirString)
        {
            //default root is assigned as the root of the Projects Folder Path int the settings
            var defaultRoot = Directory.GetDirectoryRoot(AddInSettings.PATH_PROJECTS);

            //slashes are corrected in the input string and the string is split into folder blocks
            //var folderBlocks = dirString.Trim().Replace("/", "\\").Split('\\');
            var folderBlocks = dirString.Replace("/", "\\").Split('\\');

            //if there is no slash in the input string (means it is a completely meaningles adress) default root is returned
            if (folderBlocks.Length < 1)
                return defaultRoot;

            //Check if the first block is a proper logical drive. If not, it is replaced with the default root
            var driveBlock = folderBlocks[0] + "\\";
            var logicalDrives = Directory.GetLogicalDrives();
            if (!logicalDrives.Contains(driveBlock))
                folderBlocks[0] = defaultRoot;

            //list for the final folder blocks
            List<string> cleanBlocks = new List<string>();
            cleanBlocks.Add(folderBlocks[0]);

            //folder blocks are cleaned and proper ones are added to the final list
            for (int i = 1; i < folderBlocks.Length; i++)
            {
                folderBlocks[i] = CleanString(folderBlocks[i]);
                if(!folderBlocks[i].IsNullOrEmpty())
                {
                    cleanBlocks.Add(folderBlocks[i]);
                }
            }

            //final list is joined and returned
            return string.Join("\\", cleanBlocks);
        }

        /// <summary>
        /// Checks whether the directory is valid.
        /// (For ordinary words like file names, <see cref="IsStringValid(string)"/> should be used.)/>
        /// </summary>
        /// <param name="dirString"></param>
        /// <returns></returns>
        public bool IsDirectoryValid(string dirString)
        {
            //empty check
            if (dirString.IsNullOrEmpty())
                return false;

            //character limit check
            if (dirString.Length > CharLimit)
                return false;

            //adress structure check
            var folderBlocks = dirString.Trim().Replace("/", "\\").Split('\\');
            if (folderBlocks.Length < 1)
                return false;

            //logical drive check
            var logicalDrives = Directory.GetLogicalDrives();
            if (!logicalDrives.Contains(folderBlocks[0] + "\\"))
                return false;

            //folder names check
            for (int i = 1; i < folderBlocks.Length; i++)
            {
                if (!IsStringValid(folderBlocks[i]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Checks whether the string is a reserved name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsNameReserved(string name)
        {
            return RESERVED_NAMES.Contains(name);
        }


        public static string GetCaseSensitivePath(string path)
        {
            var root = Path.GetPathRoot(path);
            try
            {
                foreach (var name in path.Substring(root.Length).Split(Path.DirectorySeparatorChar))
                    root = Directory.GetFileSystemEntries(root, name).First();
            }
            catch (System.Exception e)
            {
                // Log("Path not found: " + path);
                root += path.Substring(root.Length);
            }
            return root;
        }
    }
}
