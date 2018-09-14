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

[assembly: CommandClass(typeof(DBEvent.AcadCmds))]
namespace DBEvent
{
	public class AcadCmds
	{
		private ObjectIdCollection appended_ids = new ObjectIdCollection();
		[CommandMethod("GetNewlyCreatedEnt")]
		public void GetNewlyCreatedEnt()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				AcadFuncs.GetActiveDb().ObjectAppended += new ObjectEventHandler(AppendEvent);
				appended_ids.Clear();

				AcadFuncs.GetEditor().Command("_LINE");

				AcadFuncs.GetEditor().Command("_LINE", "10, 10, 0", "11, 0, 0", "");

				AcadFuncs.GetActiveDb().ObjectAppended -= new ObjectEventHandler(AppendEvent);

				tr.Commit();
			}
		}

		private void AppendEvent(object sender, ObjectEventArgs arg)
		{
			appended_ids.Add(arg.DBObject.ObjectId);
		}

		[CommandMethod("ChangeColorLine")]
		public void ChangeColorLine()
		{
			using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				for (int i = 0; i < appended_ids.Count; i++)
				{
					Entity ent = tr.GetObject(appended_ids[i], OpenMode.ForRead) as Entity;
					if (ent is Line)
					{
						ent.UpgradeOpen();
						ent.ColorIndex = 1;
						ent.DowngradeOpen();
					}
				}

				tr.Commit();
			}
		}
	}
}
