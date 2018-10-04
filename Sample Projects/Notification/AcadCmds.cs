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

namespace Notification
{
	public class AcadCmds
	{
		AcadDb.Polyline curr_pl = null;
		[CommandMethod("CreateNotification")]
		public void CreateNotification()
		{
			using (AcadDb.Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
			{
				AcadDb.Polyline pl = new AcadDb.Polyline();
				pl.AddVertexAt(0, new AcadGeo.Point2d(0.0, 0.0), 0.0, 0.0, 0.0);
				pl.AddVertexAt(1, new AcadGeo.Point2d(10.0, 0.0), 0.0, 0.0, 0.0);

				AcadDb.BlockTableRecord model_space = AcadFuncs.GetModelSpace(tr);

				model_space.UpgradeOpen();
				model_space.AppendEntity(pl);
				tr.AddNewlyCreatedDBObject(pl, true);

				pl.ObjectClosed += new AcadDb.ObjectClosedEventHandler(ClosedHandler);
				//pl.Modified += new EventHandler(ModifiedHandler);
				curr_pl = pl;

				tr.Commit();
			}
		}

		//open -> opened -> modified -> (1) tr.Dispose() -> closed   -> (2) closed    (

		public void ClosedHandler(object sender, AcadDb.ObjectClosedEventArgs args)
		{
			using (var tr = AcadFuncs.GetActiveDb().TransactionManager.StartOpenCloseTransaction())
			{
				AcadDb.BlockTableRecord model_space = AcadFuncs.GetModelSpace(tr);
				model_space.UpgradeOpen();

				AcadDb.DBText text = new AcadDb.DBText();
				text.Position = curr_pl.StartPoint;
				text.TextString = curr_pl.Length.ToString();

				model_space.AppendEntity(text);
				tr.AddNewlyCreatedDBObject(text, true);
				tr.Commit();
			}
		}

		//public void ModifiedHandler(object sender, EventArgs args)
		//{
		//	using (var tr = AcadFuncs.GetActiveDb().TransactionManager.StartOpenCloseTransaction())
		//	{
		//		AcadDb.BlockTableRecord model_space = AcadFuncs.GetModelSpace(tr);
		//		model_space.UpgradeOpen();

		//		AcadDb.DBText text = new AcadDb.DBText();
		//		text.Position = curr_pl.StartPoint;
		//		text.TextString = curr_pl.Length.ToString();

		//		model_space.AppendEntity(text);
		//		tr.AddNewlyCreatedDBObject(text, true);
		//		tr.Commit();
		//	}
		//}

	}
}
