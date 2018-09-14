using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

namespace DBEvent
{
	public class AcadFuncs
	{
		static public Database GetActiveDb()
		{
			return AcadApp.Application.DocumentManager.MdiActiveDocument.Database;
		}

		static public Editor GetEditor()
		{
			return AcadApp.Application.DocumentManager.MdiActiveDocument.Editor;
		}

		static BlockTable GetBlkTbl(Transaction tr)
		{
			return tr.GetObject(GetActiveDb().BlockTableId, OpenMode.ForRead) as BlockTable;
		}

		static BlockTableRecord GetModelSpace(Transaction tr)
		{
			return tr.GetObject(GetBlkTbl(tr)[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
		}

		//public - protected - private
		static public void AddNewEnt(Entity ent)
		{
			Database db = GetActiveDb();

			using (Transaction tr = db.TransactionManager.StartTransaction())
			{
				BlockTableRecord model_space = GetModelSpace(tr);
				if (null == model_space)
					return;

				model_space.UpgradeOpen();
				model_space.AppendEntity(ent);

				tr.AddNewlyCreatedDBObject(ent, true);

				tr.Commit();
			}
		}
	}

	public class SubClass : AcadFuncs
	{
		static void Function()
		{
			AddNewEnt(null);
		}
	}
}
