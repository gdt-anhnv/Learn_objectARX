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

using Excel = Microsoft.Office.Interop.Excel;

[assembly: CommandClass(typeof(SaveToExcel.AcadCmds))]
namespace SaveToExcel
{
	public class AcadCmds
	{
		[CommandMethod("SaveToExcel")]
		public void SaveToExcel()
		{
			Excel.Application excel = new Excel.Application();
			Excel.Workbook workbook = excel.Workbooks.Add(Type.Missing);
			Excel.Worksheet worksheet = null;

			try
			{
				worksheet = workbook.ActiveSheet;

				worksheet.Name = "ExportedFromDatGrid";

				int row_index = 1;
				int col_index = 1;

				List<DataExcel> data_excel = GetData();

				foreach (var de in data_excel)
				{
					worksheet.Cells[row_index, col_index] = de.start_point.X;
					worksheet.Cells[row_index, col_index + 1] = de.start_point.Y;
					worksheet.Cells[row_index, col_index + 2] = de.start_point.Z;
					worksheet.Cells[row_index, col_index + 4] = de.end_point.X;
					worksheet.Cells[row_index, col_index + 5] = de.end_point.Y;
					worksheet.Cells[row_index, col_index + 6] = de.end_point.Z;

					worksheet.Cells[row_index, col_index + 9] = "= SQRT((" +
						worksheet.Cells[row_index, col_index].Address + "-" + worksheet.Cells[row_index, col_index + 4].Address + ")*(" +
						worksheet.Cells[row_index, col_index].Address + "-" + worksheet.Cells[row_index, col_index + 4].Address + ")+(" +
						worksheet.Cells[row_index, col_index + 1].Address + "-" + worksheet.Cells[row_index, col_index + 5].Address + ")*(" +
						worksheet.Cells[row_index, col_index + 1].Address + "-" + worksheet.Cells[row_index, col_index + 5].Address + "))";
					row_index++;
				}

				//Getting the location and file name of the excel to save from user. 
				System.Windows.Forms.SaveFileDialog saveDialog = new System.Windows.Forms.SaveFileDialog();
				saveDialog.Filter = "Excel files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
				saveDialog.FilterIndex = 2;

				int max_row = worksheet.UsedRange.Rows.Count - 1;
				int max_col = worksheet.UsedRange.Columns.Count - 1;

				if (saveDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
				{
					workbook.SaveAs(saveDialog.FileName);
					MessageBox.Show("Export Successful");
				}
			}
			catch (System.Exception ex)
			{
				MessageBox.Show(ex.Message);
			}
			finally
			{
				excel.Quit();
				workbook = null;
				excel = null;
			}
		}

		[CommandMethod("TTL")]
		public void Test()
		{
			var acDoc = AcadApp.Application.DocumentManager.MdiActiveDocument;
			var acDb = acDoc.Database;
			var ed = acDoc.Editor;

			// build a selection filter to get only measurable entities
			var filter = new SelectionFilter(new[]
			{
				new TypedValue(-4, "<OR"),
					new TypedValue(0, "ARC,CIRCLE,ELLIPSE,LINE,LWPOLYLINE,SPLINE"),
					new TypedValue(-4, "<AND"),
					new TypedValue(0, "POLYLINE"), // polylines 2d or 3d
                        new TypedValue(-4, "<NOT"), // but not meshes
                            new TypedValue(-4, "&"),
							new TypedValue(70, 112),
						new TypedValue(-4, "NOT>"),
					new TypedValue(-4, "AND>"),
				new TypedValue(-4, "OR>")

				});

			var selection = ed.GetSelection(filter);
			if (selection.Status != PromptStatus.OK)
				return;

			var watch = System.Diagnostics.Stopwatch.StartNew();
			using (var tr = acDb.TransactionManager.StartTransaction())
			{
				// use Linq queries to get lengths by type in a dictionary
				var lengths = selection.Value
					.Cast<SelectedObject>()
					.Select(selObj => (Curve)tr.GetObject(selObj.ObjectId, OpenMode.ForRead))
					.ToLookup(curve => curve.GetType().Name,                         // <- key selector
							  curve => curve.GetDistanceAtParameter(curve.EndParam)) // <- element selector
					.ToDictionary(group => group.Key,    // <- key selector
								  group => group.Sum()); // <- element selector

				// print results
				foreach (var entry in lengths)
				{
					ed.WriteMessage($"\n{entry.Key,-12} = {entry.Value,9:0.00}");
				}
				ed.WriteMessage($"\nTotal Length = {lengths.Values.Sum(),9:0.00}");

				tr.Commit();
			}
			AcadApp.Application.DisplayTextScreen = false;
			watch.Stop();
			AcadFuncs.GetEditor().WriteMessage(watch.ElapsedMilliseconds.ToString());
		}

		public List<DataExcel> GetData()
		{
			List<ObjectId> ids = AcadFuncs.PickEnts();
			List<DataExcel> data_excel = new List<DataExcel>();
			using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
			{
				foreach (var id in ids)
				{
					Line line = tr.GetObject(id, OpenMode.ForRead) as Line;
					if (null == line)
						continue;

					data_excel.Add(new DataExcel(line.StartPoint, line.EndPoint));
				}
			}

			return data_excel;
		}

	}

	public class DataExcel
	{
		public AcadGeo.Point3d start_point;
		public AcadGeo.Point3d end_point;

		public DataExcel(AcadGeo.Point3d sp, AcadGeo.Point3d ep)
		{
			start_point = sp;
			end_point = ep;
		}
	}
}
