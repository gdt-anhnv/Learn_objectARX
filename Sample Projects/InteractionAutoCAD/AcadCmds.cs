using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Acad = Autodesk.AutoCAD;

[assembly: CommandClass(typeof(MyNamespace.AcadCmds))]
namespace MyNamespace
{
	public class AcadCmds
    {
		[CommandMethod("PickPoint")]
		public void PickPoint()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptPointOptions prmpt_pnt = new PromptPointOptions("Chọn điểm");
					PromptPointResult prmpt_ret = AcadFuncs.GetEditor().GetPoint(prmpt_pnt);
					if(PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					Point3d picked_pnt = prmpt_ret.Value;
					tr.Commit();
				}
			}
		}

		[CommandMethod("PickPoint2")]
		public void PickPoint2()
		{
			Point3d pnt = new Point3d();
			if (!InputAcad.PickPoint(ref pnt, "Chọn điểm"))
				return;

			//Do something here with picked point
		}

		[CommandMethod("PickEnts")]
		public void PickEnts()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().GetSelection();
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					ObjectId[] ss = prmpt_ret.Value.GetObjectIds();

					foreach(ObjectId ent_id in ss)
					{
						DBObject obj = tr.GetObject(ent_id, OpenMode.ForRead);
						if (null == obj)
							continue;
						if (obj is Line)
							MessageBox.Show("Selected a line!");
						else
							AcadFuncs.GetEditor().SetImpliedSelection(ss);
					}

					tr.Commit();
				}
			}
		}

		private ObjectId sel_obj_id = ObjectId.Null;

		[CommandMethod("PickSingleEnt")]
		public void PickSingleEnt()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptEntityResult prmpt_ret = AcadFuncs.GetEditor().GetEntity("Chọn một đối tượng line");
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					ObjectId obj_id = prmpt_ret.ObjectId;
					sel_obj_id = prmpt_ret.ObjectId;
					DBObject obj = tr.GetObject(obj_id, OpenMode.ForRead);
					if (obj is Line)
						MessageBox.Show("Selected a line!");
					else
						MessageBox.Show("This entity isn't a line!");

					tr.Commit();
				}
			}
		}

		[CommandMethod("EntsInsideWindow")]
		public void EntsInsideWindow()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().
						SelectCrossingWindow(new Point3d(0.0, 0.0, 0.0), new Point3d(10.0, 10.0, 0.0));
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					ObjectId[] ss = prmpt_ret.Value.GetObjectIds();
					foreach (ObjectId ent_id in ss)
					{
						DBObject obj = tr.GetObject(ent_id, OpenMode.ForRead);
						if (null == obj)
							continue;
						if (obj is Line)
							MessageBox.Show("Selected a line!");
					}

					tr.Commit();
				}
			}
		}

		private const string SEL_ENT = "SelEnts";
		private const string ENTS_INSIDE_WIN = "EntsInsideWindow";

		[CommandMethod("SelectionOptions")]
		public void SelectionOptions()
		{
			PromptKeywordOptions keyword = new PromptKeywordOptions("Chọn loại selection:");
			keyword.Keywords.Add(SEL_ENT);
			keyword.Keywords.Add(ENTS_INSIDE_WIN);

			PromptResult prompt_ret = AcadFuncs.GetEditor().GetKeywords(keyword);

			if (PromptStatus.OK == prompt_ret.Status)
			{
				if (SEL_ENT == prompt_ret.StringResult)
					PickEnts();
				else if (ENTS_INSIDE_WIN == prompt_ret.StringResult)
					EntsInsideWindow();
			}
		}

		[CommandMethod("FilterEnts")]
		public void FilterEnts()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					TypedValue[] type_var = new TypedValue[2];
					type_var.SetValue(new TypedValue((int)DxfCode.Start, "circle,line"), 0);
					type_var.SetValue(new TypedValue((int)DxfCode.Color, 1), 1);
					/*
					 * https://knowledge.autodesk.com/search-result/caas/CloudHelp/cloudhelp/2017/ENU/AutoCAD-NET/files/GUID-125398A5-184C-4114-9212-A2FF28FC1F1D-htm.html
					 * */

					SelectionFilter sel_filter = new SelectionFilter(type_var);

					PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().GetSelection(sel_filter);
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					ObjectId[] ss = prmpt_ret.Value.GetObjectIds();
					foreach (ObjectId ent_id in ss)
					{
						DBObject obj = tr.GetObject(ent_id, OpenMode.ForRead);
						if (null == obj)
							continue;
						if (obj is Line)
							MessageBox.Show("Selected a line!");
					}

					tr.Commit();
				}
			}
		}

		[CommandMethod("FilterEntsWildCard")]
		public void FilterEntsWildCard()
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					TypedValue[] type_var = new TypedValue[2];
					type_var.SetValue(new TypedValue((int)DxfCode.Start, "text"), 0);
					type_var.SetValue(new TypedValue((int)DxfCode.Text, "*abc"), 1);
					SelectionFilter sel_filter = new SelectionFilter(type_var);

					PromptSelectionResult prmpt_ret = AcadFuncs.GetEditor().GetSelection(sel_filter);
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					ObjectId[] ss = prmpt_ret.Value.GetObjectIds();
					foreach (ObjectId ent_id in ss)
					{
						DBObject obj = tr.GetObject(ent_id, OpenMode.ForRead);
						if (null == obj)
							continue;
						if (obj is Line)
							MessageBox.Show("Selected a line!");
					}

					tr.Commit();
				}
			}
		}

		[CommandMethod("DrawLineByCode")]
		public void DrawLineByCode()
		{
			AcadFuncs.GetEditor().Command("_LINE");

			AcadFuncs.GetEditor().Command("_LINE", "10, 10, 0", "11, 0, 0", "");
		}
	}
}
