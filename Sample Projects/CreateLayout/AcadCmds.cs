using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadDb = Autodesk.AutoCAD.DatabaseServices;
using AcadEd = Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

namespace CreateLayout
{
	public class AcadCmds
    {
		[CommandMethod("CreateLayout")]
		public void CreateLayout()
		{
			using (AcadDb.Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				AcadDb.LayoutManager layout_man = AcadDb.LayoutManager.Current;
				AcadDb.ObjectId layout_id = layout_man.CreateLayout("My_Layout");
				layout_man.SetCurrentLayoutId(layout_id);

				AcadDb.Layout layout = tr.GetObject(layout_id, AcadDb.OpenMode.ForRead) as AcadDb.Layout;
				if (null == layout)
					return;

				AcadDb.BlockTableRecord blk_tbl_rcd = tr.GetObject(layout.BlockTableRecordId, AcadDb.OpenMode.ForRead)
					as AcadDb.BlockTableRecord;
				if (null == blk_tbl_rcd)
					return;

				AcadDb.ObjectIdCollection vp_ids = layout.GetViewports();
				AcadDb.Viewport vp = null;

				foreach(AcadDb.ObjectId vp_id in vp_ids)
				{
					AcadDb.Viewport vp2 = tr.GetObject(vp_id, AcadDb.OpenMode.ForWrite) as AcadDb.Viewport;
					if (null != vp2 && 2 == vp2.Number)
					{
						vp = vp2;
						break;
					}
				}

				if (null == vp)
				{
					vp = new AcadDb.Viewport();
					blk_tbl_rcd.UpgradeOpen();
					blk_tbl_rcd.AppendEntity(vp);
					tr.AddNewlyCreatedDBObject(vp, true);
					vp.On = true;
					vp.GridOn = true;
				}

				vp.ViewCenter = new AcadGeo.Point2d(0.0, 0.0);
				double scale = 0;
				{
					AcadEd.PromptDoubleOptions prmpt_pnt = new AcadEd.PromptDoubleOptions("Nhập scale:");
					AcadEd.PromptDoubleResult prmpt_ret = AcadFuncs.GetEditor().GetDouble(prmpt_pnt);
					if (AcadEd.PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return;
					}

					scale = prmpt_ret.Value;
				}
				vp.CustomScale = scale;
				vp.Locked = true;

				tr.Commit();
			}
		}
	}
}
