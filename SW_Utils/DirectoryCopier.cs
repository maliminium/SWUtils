using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SW_Utils
{
    public class DirectoryCopier
    {
        //private const string COPY_SFX = "_2";
        private const string TOOLBOX_PATH= @"D:\mekanik\Resources\MAM_Toolbox";

        public Dictionary<string, string> LookUpTable = new Dictionary<string, string>();

        public DirectoryCopier()
        {
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\2_TRANSISTOR\FF300R12ME4.sldprt", @"D:\mekanik\Resources\MAM_Toolbox\2.TRANSISTOR\S-2.1.FF300R12ME4.sldprt");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\KovanliBilyaliStoper.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\S-3.1.KovanliBilyaliStoper.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\PercinSomun_5.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\S-3.2.PercinSomun_5.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\PercinSomun_kap.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\S-3.3.PercinSomun_kap.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\SaplamaVida_Cu.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\S-3.4.SaplamaVida_Cu.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\KAUCUK_TAKOZ\TypeE.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\1.KAUCUK_TAKOZ\S-3.1.1.TypeE.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\DEMONTE_68x13.SLDASM", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.1.DEMONTE_68x13.SLDASM");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\DEMONTE_68x13_DISI.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.2.DEMONTE_68x13_DISI.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\DEMONTE_68x13_ERKEK.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.3.DEMONTE_68x13_ERKEK.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\KELEBEK-100 Govde.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.4.KELEBEK-100 Govde.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\KELEBEK-100 Kanat.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.5.KELEBEK-100 Kanat.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\KELEBEK-100.SLDASM", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.6.KELEBEK-100.SLDASM");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\PVC-100 Govde.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.7.PVC-100 Govde.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\PVC-100 Kanat.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.8.PVC-100 Kanat.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\3_MEKANIK_HIRDAVAT\MENTESE\PVC-100.SLDASM", @"D:\mekanik\Resources\MAM_Toolbox\3.MEKANIK_HIRDAVAT\2.MENTESE\S-3.2.9.PVC-100.SLDASM");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\4_RESISTOR\FPA250.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\4.RESISTOR\S-4.1.FPA250.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\5_SIGMA\GenisKoseBaglanti.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\5.SIGMA\S-5.1.GenisKoseBaglanti.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\6_PANO_APARATLARI\RayAdaptoru.SLDASM", @"D:\mekanik\Resources\MAM_Toolbox\6.PANO_APARATLARI\S-6.1.RayAdaptoru.SLDASM");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\6_PANO_APARATLARI\RayAdaptoru_mandal.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\6.PANO_APARATLARI\S-6.2.RayAdaptoru_mandal.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\6_PANO_APARATLARI\RayAdaptoru_taban.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\6.PANO_APARATLARI\S-6.3.RayAdaptoru_taban.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\7_KONTAKTOR\AF400-GAF460.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\7.KONTAKTOR\S-7.1.AF400-GAF460.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\8_KAPASITOR\E66.R11-434W20.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\8.KAPASITOR\S-8.1.E66.R11-434W20.SLDPRT");
            LookUpTable.Add(@"D:\mekanik\Resources\MAM_Toolbox\9_IZOLATOR\Findik.SLDPRT", @"D:\mekanik\Resources\MAM_Toolbox\9.IZOLATOR\S-9.1.Findik.SLDPRT");
        }



        //private void CopyDir(string mainDir, string copyDir)
        //{
        //    Directory.CreateDirectory(copyDir);

        //    var subFiles = Directory.GetFiles(mainDir);
        //    for (int i = 0; i < subFiles.Length; i++)
        //    {
        //        var nextName = subFiles[i];
        //        nextName = nextName.Replace(mainDir, copyDir);
        //        var idx = nextName.LastIndexOf("\\");
        //        nextName = nextName.Insert(idx + 1, GetFullPrefixFromPath(nextName) + AddInSettings.SEP_NUMBER);

        //        File.Copy(subFiles[i], nextName);
        //    }


        //    var subDirs = Directory.GetDirectories(mainDir);
        //    for (int i = 0; i < subDirs.Length; i++)
        //    {
        //        var nextName = subDirs[i];
        //        nextName = nextName.Replace(mainDir, copyDir);
        //        nextName += COPY_SFX;

        //        CopyDir(subDirs[i], nextName);
        //    }
        //}

        //public static List<int> GetNumberingFromName(string name)
        //{
        //    var numberSeperator = AddInSettings.SEP_NUMBER;

        //    var numberBlocks = name.Split(numberSeperator.ToCharArray());

        //    var numberList = new List<int>();

        //    for (int i = 0; i < numberBlocks.Length; i++)
        //    {
        //        if (int.TryParse(numberBlocks[i], out int n))
        //            numberList.Add(n);
        //        else
        //            break;
        //    }

        //    return numberList;

        //}

        //public static string GetPrefixFromName(string name)
        //{
        //    var numberSeperator = AddInSettings.SEP_NUMBER;
        //    return string.Join(numberSeperator, GetNumberingFromName(name));
        //}

        //public static string GetFullPrefixFromPath(string path)
        //{
        //    var numberSeperator = AddInSettings.SEP_NUMBER;
        //    var projectSeperator = AddInSettings.SEP_WORD1;

        //    var relativePath = path.Replace(TOOLBOX_PATH, string.Empty);
        //    relativePath = relativePath.Trim('\\');
        //    var subFolders = relativePath.Split('\\');

        //    var projectPrefix = GetPrefixFromName(subFolders[0]);

        //    var subPrefixes = new List<string>();
        //    for (int i = 1; i < subFolders.Length; i++)
        //    {
        //        var nextSuffix = GetPrefixFromName(subFolders[i]);
        //        if (!nextSuffix.IsNullOrEmpty())
        //            subPrefixes.Add(nextSuffix);
        //    }

        //    return "S" + projectSeperator + string.Join(numberSeperator, subPrefixes);
        //}

    }
}
