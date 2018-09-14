using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(DrawLine.AcadCmds))]
namespace DrawLine
{
	public class AcadCmds
    {
		private static string DICT_NAME = "MY_DICT";
		private static string XREC_NAME = "Test";

		private DBDictionary GetDict(Transaction tr)
		{
			DBDictionary dict = null;
			DBDictionary db_dict = tr.GetObject(AcadFuncs.GetActiveDb().NamedObjectsDictionaryId, OpenMode.ForRead)
				as DBDictionary;

			if (null == db_dict)
			{
				tr.Dispose();
				return null;
			}

			if (!db_dict.Contains(DICT_NAME))
			{
				dict = new DBDictionary();
				dict.TreatElementsAsHard = true;
				db_dict.UpgradeOpen();
				db_dict.SetAt(DICT_NAME, dict);
				tr.AddNewlyCreatedDBObject(dict, true);
				db_dict.DowngradeOpen();
			}
			else
				dict = tr.GetObject((ObjectId)db_dict[DICT_NAME], OpenMode.ForRead) as DBDictionary;

			return dict;
		}

		private ObjectId PickSingleEnt()
		{
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptEntityResult prmpt_ret = AcadFuncs.GetEditor().GetEntity("Chọn một đối tượng line");
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return ObjectId.Null;
					}

					ObjectId obj_id = prmpt_ret.ObjectId;
					tr.Commit();

					return obj_id;
				}
			}
		}

		private ObjectId GetBlkTblRcd(Transaction tr)
		{
			BlockTable blk_tbl = AcadFuncs.GetBlkTbl(tr);
			BlockTableRecord blk_ref = new BlockTableRecord();
			blk_ref.Name = "TestBlkRef";
			blk_tbl.UpgradeOpen();
			blk_tbl.Add(blk_ref);
			tr.AddNewlyCreatedDBObject(blk_ref, true);
			return blk_ref.Id;
		}

		private static ResultBuffer SoftPointerForId(ObjectId id)
		{
			var rb = new ResultBuffer();
			var gc = (int)DxfCode.SoftPointerId;
			//var gc = (int)DxfCode.HardPointerId;
			rb.Add(new TypedValue(gc, id));
			return rb;
		}

		private void AddXRecord(Transaction tr, DBDictionary dict, string key, ObjectId id)
		{
			Xrecord xr = null;
			if (dict.Contains(key))
			{
				// Update the existing lock object
				xr = tr.GetObject((ObjectId)dict[key], OpenMode.ForWrite) as Xrecord;
				xr.Data = SoftPointerForId(id);
			}
			else
			{
				// Create a new lock object
				xr = new Xrecord();
				xr.XlateReferences = true;
				xr.Data = SoftPointerForId(id);

				dict.UpgradeOpen();
				dict.SetAt(key, xr);
				tr.AddNewlyCreatedDBObject(xr, true);
			}
		}

		[CommandMethod("SoftPointer")]
		public void SoftPointer()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				DBDictionary dict = GetDict(tr);
				if (null == dict)
					return;

				ObjectId sel_ent_id = GetBlkTblRcd(tr); // PickSingleEnt();
				if (ObjectId.Null == sel_ent_id)
					return;

				AddXRecord(tr, dict, XREC_NAME, sel_ent_id);
				tr.Commit();
			}
		}

		[CommandMethod("HighlightSoftPointer")]
		public void HighlightSoftPointer()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				DBDictionary dict = GetDict(tr);
				if (null == dict)
					return;

				Xrecord xrec = tr.GetObject(dict.GetAt(XREC_NAME), OpenMode.ForRead) as Xrecord;
				if (null == xrec)
					return;

				ObjectId obj_id = (ObjectId)xrec.Data.AsArray()[0].Value;

				Entity ent = tr.GetObject(obj_id, OpenMode.ForRead) as Entity;
				if (null == ent)
					return;

				if(ent is Line)
				{
					Line line = ent as Line;
					line.UpgradeOpen();
					line.ColorIndex = 1;
				}

				tr.Commit();
			}
		}
	}
}
