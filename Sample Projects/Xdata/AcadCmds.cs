using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(DrawLine.AcadCmds))]
namespace DrawLine
{
	public class AcadCmds
	{
		private static string XDATA_APP = "MyRegApp";

		[CommandMethod("AddXdata")]
		public void AddXdata()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				RegAppTable reg_app = tr.GetObject(AcadFuncs.GetActiveDb().RegAppTableId, OpenMode.ForRead) as RegAppTable;
				if (null == reg_app)
				{
					tr.Dispose();
					return;
				}

				if (!reg_app.Has(XDATA_APP))
				{
					RegAppTableRecord reg_app_rcd = new RegAppTableRecord();
					reg_app_rcd.Name = XDATA_APP;

					reg_app.UpgradeOpen();
					reg_app.Add(reg_app_rcd);
					tr.AddNewlyCreatedDBObject(reg_app_rcd, true);
				}

				PromptEntityResult prmpt_ent_ret = AcadFuncs.GetEditor().GetEntity("Chọn 1 entity:");
				Entity ent = tr.GetObject(prmpt_ent_ret.ObjectId, OpenMode.ForRead) as Entity;
				if(null == ent)
				{
					tr.Dispose();
					return;
				}

				ent.UpgradeOpen();
				ResultBuffer buffer = new ResultBuffer();
				buffer.Add(new TypedValue((int)DxfCode.ExtendedDataRegAppName, XDATA_APP));
				buffer.Add(new TypedValue((int)DxfCode.ExtendedDataReal, 100.0));
				ent.XData = buffer;

				tr.Commit();
			}
		}
	}
}
