using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using AcadApp = Autodesk.AutoCAD.ApplicationServices;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.DatabaseServices;

[assembly: CommandClass(typeof(NewLayer.NewLayer))]
namespace NewLayer
{
    public class NewLayer
    {
		[CommandMethod("CreateNewLayer")]
		public void CreateLayer()
		{
			Database db = AcadApp.Application.DocumentManager.MdiActiveDocument.Database;

			using (Transaction tr = db.TransactionManager.StartTransaction())
			{
				LayerTable layer_tbl = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;
				if (null == layer_tbl)
					return;

				LayerTableRecord my_layer = new LayerTableRecord();
				my_layer.Name = "My_Layer";

				layer_tbl.UpgradeOpen();
				layer_tbl.Add(my_layer);

				tr.AddNewlyCreatedDBObject(my_layer, true);

				tr.Commit();
			}
		}


	}
}
