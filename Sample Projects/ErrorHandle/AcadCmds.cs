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
		[CommandMethod("DrawLine1")]
		public void DrawLine1()
		{
			Database db = AcadApp.Application.DocumentManager.MdiActiveDocument.Database;
			using (Transaction tr = db.TransactionManager.StartTransaction())
			{
				BlockTable blk_tbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
				if (null == blk_tbl)
					return;

				BlockTableRecord model_space = tr.GetObject(blk_tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
				if (null == model_space)
					return;

				DoSomething(model_space); //error here

				Line line = new Line();

				//bad coding habit

				/*
				 * calculate something here to findout the start point of the line
				*/
				line.StartPoint = new AcadGeo.Point3d(0.0, 0.0, 0.0);

				/*
				 * Calculate something here to findout the end point of the line
				*/
				line.EndPoint = new AcadGeo.Point3d(10.0, 0.0, 0.0);

				model_space.UpgradeOpen();
				model_space.AppendEntity(line);

				tr.AddNewlyCreatedDBObject(line, true);

				tr.Commit();
			}

		}

		[CommandMethod("DrawLine3")]
		public void DrawLine3()
		{
			Database db = AcadApp.Application.DocumentManager.MdiActiveDocument.Database;
			using (Transaction tr = db.TransactionManager.StartTransaction())
			{
				BlockTable blk_tbl = tr.GetObject(db.BlockTableId, OpenMode.ForWrite) as BlockTable;
				if (null == blk_tbl)
					return;

				BlockTableRecord model_space = tr.GetObject(blk_tbl[BlockTableRecord.ModelSpace], OpenMode.ForRead) as BlockTableRecord;
				if (null == model_space)
					return;

				DoSomething(model_space); //error here

				Line line = new Line();

				//good coding habit
				line.StartPoint = FindoutStartPoint();
				line.EndPoint = FindoutEndPoint();

				model_space.UpgradeOpen();
				model_space.AppendEntity(line);

				tr.AddNewlyCreatedDBObject(line, true);

				tr.Commit();
			}

		}

		AcadGeo.Point3d FindoutStartPoint()
		{
			return AcadGeo.Point3d.Origin;
		}

		AcadGeo.Point3d FindoutEndPoint()
		{
			return new AcadGeo.Point3d(1.0, 0.0, 0.0);
		}

		void DoSomething(BlockTableRecord model_space)
		{
			model_space.Close();
		}

		[CommandMethod("DrawLine2")]
		public void DrawLine2()
		{
			try
			{
				AcadFuncs.AddNewEnt(new Line(new AcadGeo.Point3d(0.0, 0.0, 0.0), new AcadGeo.Point3d(1.0, 1.0, 0.0)));

				AcadFuncs.AddNewEnt(new Line(new AcadGeo.Point3d(1.0, 1.0, 0.0), new AcadGeo.Point3d(1.0, 0.0, 0.0)));
			}
			catch(Autodesk.AutoCAD.Runtime.Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
		}
	}
}
