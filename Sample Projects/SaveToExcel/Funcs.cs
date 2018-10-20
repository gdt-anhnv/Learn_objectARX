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

namespace SaveToExcel
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

		static public AcadApp.Document GetActiveDoc()
		{
			return AcadApp.Application.DocumentManager.MdiActiveDocument;
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

		static public List<ObjectId> PickEnts()
		{
			using (AcadApp.Application.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().GetSelection();
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return new List<ObjectId>();
					}

					tr.Commit();
					return prmpt_ret.Value.GetObjectIds().ToList();
				}
			}
		}
	}
}
