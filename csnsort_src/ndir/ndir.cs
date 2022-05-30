using System;
using System.Collections;
using System.IO;

using ns;

	public class NDIR
	{
		public static void Main(string[] args)
		{
			if((args == null) || (args.Length <= 0))
			{
				ListDir.List(null);
			}
			else foreach(string dir in args)
				ListDir.List(dir);
			
		}

	}

	public class ListDir
	{
		public static void List(string dir)
		{
			if((dir == null) || !Directory.Exists(dir)) dir = ".";
			Console.WriteLine("DIR " + dir);
			string[] files = FileLogicalComparer.Sort(Directory.GetFiles(dir));
			for(int i = 0; i < files.Length; i++)
			{
				Console.WriteLine(files[i]);
			}
			
		}

	}	