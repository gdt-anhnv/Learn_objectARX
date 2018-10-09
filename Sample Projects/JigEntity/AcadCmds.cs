using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;

using MgdAcApplication = Autodesk.AutoCAD.ApplicationServices.Application;
using MgdAcDocument = Autodesk.AutoCAD.ApplicationServices.Document;
using AcWindowsNS = Autodesk.AutoCAD.Windows;

namespace JigEntity
{
	public class TextJig : EntityJig, IDisposable
	{
		private Point3d position;
		//private Point3d center;
		private Point3d p1;
		private Point3d p2;

		public TextJig(DBText entity) : base(entity)
		{
			position = new Point3d(0.0, 0.0, 0.0);
			p1 = new Point3d(0.0, 0.0, 0.0);
			p2 = new Point3d(0.0, 0.0, 0.0);
			AcadFuncs.GetEditor().Rollover += new RolloverEventHandler(RollOverHandler);
		}

		private void RollOverHandler(object sender, RolloverEventArgs args)
		{
			foreach (var id in args.Picked.GetObjectIds())
			{
				using (Transaction tr = AcadFuncs.GetActiveDb().TransactionManager.StartTransaction())
				{
					Line line = tr.GetObject(id, OpenMode.ForRead) as Line;
					if (null != line)
					{
						//center = line.Center;
						p1 = line.StartPoint;
						p2 = line.EndPoint;
						break;
					}
				}
			}
		}

		protected override SamplerStatus Sampler(JigPrompts prompts)
		{
			var pnt_input = prompts.AcquirePoint();
			if (PromptStatus.Cancel == pnt_input.Status)
				return SamplerStatus.Cancel;

			DBText text = Entity as DBText;
			if (null == text)
				return SamplerStatus.NoChange;

			Point3d pnt = pnt_input.Value;
			position = pnt;

			if (position.IsEqualTo(text.Position))
				return SamplerStatus.NoChange;

			return SamplerStatus.OK;
		}

		protected override bool Update()
		{
			(Entity as DBText).Position = position;
			//Vector3d vec = new Vector3d(position.X - center.X, position.Y - center.Y, 0.0);
			Vector3d vec = new Vector3d(p2.X - p1.X, p2.Y - p1.Y, 0.0);
			(Entity as DBText).Rotation = Math.Abs(vec.GetAngleTo(Vector3d.XAxis));
			return true;
		}

		public void Dispose()
		{
			AcadFuncs.GetEditor().Rollover -= new RolloverEventHandler(RollOverHandler);
		}
	}

	public class AcadCmds
    {
		[CommandMethod("JigEntity")]
		public void CreateLayout()
		{
			try
			{
				DBText text = new DBText();
				text.TextString = "abc";
				TextJig jig = new TextJig(text);
				if(PromptStatus.OK == AcadFuncs.GetEditor().Drag(jig).Status)
				{
					AcadFuncs.AddNewEnt(text);
				}
			}
			catch
			{
				return;
			}
		}
	}
}
