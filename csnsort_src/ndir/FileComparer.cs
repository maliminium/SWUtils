using System;
using System.Collections;
using System.IO;

namespace ns
{
	
	// sample class to demostrate ordering full path files
	public class FileLogicalComparer
	{
		public ArrayList files = null;
		
		public FileLogicalComparer()
		{}		
		
		public void AddFile(string file)
		{
			if(file == null) return;
			if(files == null) files = new ArrayList();
			files.Add(new DictionaryEntry(Path.GetFileName(file), file));
		}
		
		// convenience method
		public void AddFiles(string[] f)
		{
			if(f == null) return;
			for(int i = 0; i < f.Length; i++)
				AddFile(f[i]);
		}

		public ArrayList GetSorted()
		{
			files.Sort(new DictionaryEntryComparer(new
				ns.NumericComparer()));
			return files;
		}

		public static string[] Sort(string[] files)
		{
			if(files == null) return null;
			FileLogicalComparer fc = new FileLogicalComparer();
			fc.AddFiles(files);
			ArrayList list = fc.GetSorted();
			if(list == null) return files;
			for(int i = 0; i < list.Count; i++)
			{
				files[i] = (string)((DictionaryEntry)list[i]).Value;
			}
			return files;
		}
		
	}//EOC
	
	public class DictionaryEntryComparer : IComparer
	{
		private IComparer nc = null;

		public DictionaryEntryComparer(IComparer nc)
		{
			if(nc == null) throw new Exception("null IComparer");
			this.nc = nc;
		}

		public int Compare(object x, object y)
		{
			if((x is DictionaryEntry) && (y is DictionaryEntry))
			{
				return nc.Compare(((DictionaryEntry)x).Key,
					((DictionaryEntry)y).Key);
			}
			return -1;
		}
	}//EOC
	
}