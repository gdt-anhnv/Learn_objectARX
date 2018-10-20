using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace TestRealDWG
{
	class AppService : HostApplicationServices
	{
		public AppService()
		{
			RuntimeSystem.Initialize(this, 1033);
		}

		private string SearchPath(string fn)
		{
			// check if the file is already with full path
			if (System.IO.File.Exists(fn))
				return fn;

			// check application folder
			string app_path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			if (File.Exists(app_path + "\\" + fn))
				return app_path + "\\" + fn;

			// we can also check the Acad path
			string acad = Environment.GetEnvironmentVariable("ACAD");
			if (File.Exists(acad + "\\" + fn))
				return acad + "\\" + fn;

			// search folders in %PATH%
			string[] paths = Environment.GetEnvironmentVariable("Path").Split(new char[] { ';' });
			foreach (string path in paths)
			{
				// some folders end with \\, some don't
				string validated_path = Path.GetFullPath(path.TrimEnd('\\') + "\\" + fn);
				if (File.Exists(validated_path))
					return validated_path;
			}

			// check the Fonts folders
			string systemFonts = Environment.GetEnvironmentVariable("SystemRoot") + "\\Fonts\\";
			if (File.Exists(systemFonts + fn))
				return systemFonts + fn;

			// if we installed fonts in our own folder  
			string rdwgFonts = app_path + "\\Fonts\\";
			if (File.Exists(rdwgFonts + fn))
				return rdwgFonts + fn;

			return "";
		}

		public override string FindFile(
			string fileName,
			Database database,
			FindFileHint hint)
		{
			// add extension if not already part of the file name
			if (!fileName.Contains("."))
			{
				string extension;
				switch (hint)
				{
					case FindFileHint.CompiledShapeFile:
						extension = ".shx";
						break;
					case FindFileHint.TrueTypeFontFile:
						extension = ".ttf";
						break;
					case FindFileHint.PatternFile:
						extension = ".pat";
						break;
					case FindFileHint.ArxApplication:
						extension = ".dbx";
						break;
					case FindFileHint.FontMapFile:
						extension = ".fmp";
						break;
					case FindFileHint.XRefDrawing:
						extension = ".dwg";
						break;
					// Fall through. These could have
					// various extensions
					case FindFileHint.FontFile:
					case FindFileHint.EmbeddedImageFile:
					default:
						extension = "";
						break;
				}

				fileName += extension;
			}

			// add it to the unresolved items if it could not be resolved
			return SearchPath(fileName);
		}

		public override string GetPassword(string dwgName, PasswordOptions options)
		{
			return base.GetPassword(dwgName, options);
		}

		public override string GetRemoteFile(System.Uri url, bool ignoreCache)
		{
			return base.GetRemoteFile(url, ignoreCache);
		}

		public override System.Uri GetUrl(string localFile)
		{
			return base.GetUrl(localFile);
		}

		public override bool IsUrl(string filePath)
		{
			return base.IsUrl(filePath);
		}

		public override void LoadApplication(
			string appName,
			ApplicationLoadReasons why,
			bool printIt,
			bool asCmd)
		{
			base.LoadApplication(appName, why, printIt, asCmd);
		}

		public override void PutRemoteFile(
			System.Uri url,
			string localFile)
		{
			base.PutRemoteFile(url, localFile);
		}

		public override string AlternateFontName
		{
			get
			{
				// we return our own Alternate Font, which is installed as part of our application
				return "txt.shx";
			}
		}

		public override string CompanyName
		{
			get
			{
				return base.CompanyName;
			}
		}

		public override string FontMapFileName
		{
			get
			{
				return base.FontMapFileName;
			}
		}

		public override string LocalRootFolder
		{
			get
			{
				return base.LocalRootFolder;
			}
		}

		public override ModelerFlavor ModelerFlavor
		{
			get
			{
				ModelerFlavor flavour = base.ModelerFlavor;
				return flavour;
			}
		}

		public override string Product
		{
			get
			{
				return base.Product;
			}
		}

		public override string Program
		{
			get
			{
				return base.Program;
			}
		}

		public override string MachineRegistryProductRootKey
		{
			get
			{
				// this should be the same as the value of ODBXHOSTAPPREGROOT in our msi file
				return base.MachineRegistryProductRootKey;
			}
		}

		public override string UserRegistryProductRootKey
		{
			get
			{
				// this should be the same as the value of ODBXHOSTAPPREGROOT in our msi file
				return base.UserRegistryProductRootKey;
			}
		}

		public override string RoamableRootFolder
		{
			get
			{
				return base.RoamableRootFolder;
			}
		}
	}

	static class Commands
	{
		[STAThread]
		static public void Main()
		{
			new AppService();

			Database db = new Database(true, true);
			//db.ReadDwgFile(@"D:\Test.dwg", System.IO.FileShare.ReadWrite, true, "");
			HostApplicationServices.WorkingDatabase = db;
			using (db)
			{
				try
				{
					var tm = db.TransactionManager;
					using (Transaction tr = db.TransactionManager.StartTransaction())
					{
						BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
						BlockTableRecord btr = new BlockTableRecord();
						btr.Name = "abc";

						bt.Add(btr);
						tr.AddNewlyCreatedDBObject(btr, true);
						tr.Commit();
					}

					db.SaveAs(@"D:\Test.dwg", DwgVersion.Newest);
				}
				catch (DataAdapterSourceFilesException)
				{
				}
			}
		}
	}
}