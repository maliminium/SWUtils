using System.Collections.Generic;
using System.IO;
using System.Windows.Documents;

namespace SW_Utils
{
    /// <summary>
    /// An object that contains both fullPath and fileName of a given path
    /// </summary>
    public class PathData
    {
        private string _Txt_Path = string.Empty;
        public string Txt_Path
        {
            get => _Txt_Path;
            set
            {
                _Txt_Path = value;
                Txt_Name = Path.GetFileName(value);
            }
        }
        public string Txt_Name { get; set; }

        public PathData()
        {

        }

        public PathData(string path)
        {
            Txt_Path = path;
        }

        public PathData(string path, string name)
        {
            Txt_Path = path;
            Txt_Name = name;
        }

        public override string ToString()
        {
            return Txt_Name;
        }

        //public static List<PathData> GetSortedProjectPaths(string directory)
        //{
        //    var allPaths = CommonResources.GetSortedDirectoryPaths(directory);
        //    var selectedPaths = new List<PathData>();
        //    for (int i = 0; i < allPaths.Length; i++)
        //    {
        //        var folderData = new PathData(allPaths[i]);

        //        if (folderData.Txt_Name.StartsWith(AddInSettings.PROJ_SFX))
        //        {
        //            //TODO: Boyle prop degistrmek hos degil
        //            folderData.Txt_Name = folderData.Txt_Name.Substring(AddInSettings.PROJ_SFX.Length);
        //            selectedPaths.Add(folderData);
        //        }
        //    }
        //    return selectedPaths;
        //}

        public static List<PathData> GetNumeratedPaths(string directory)
        {
            var allPaths = CommonResources.GetNumeratedSubPaths(directory, false);
            var selectedPaths = new List<PathData>();
            for (int i = 0; i < allPaths.Count; i++)
            {
                selectedPaths.Add(new PathData(allPaths[i]));
            }
            return selectedPaths;
        }
    }
}
