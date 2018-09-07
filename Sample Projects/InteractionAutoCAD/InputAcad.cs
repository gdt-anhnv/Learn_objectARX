using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.EditorInput;
using AcadGeo = Autodesk.AutoCAD.Geometry;
using AcadApp = Autodesk.AutoCAD.ApplicationServices.Application;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;

namespace MyNamespace
{
	class InputAcad
	{
		public static bool PickPoint(ref AcadGeo.Point3d picked_pnt, string mess)
		{
			AcadApp.DocumentManager.MdiActiveDocument.Window.Focus();
			using (AcadApp.DocumentManager.MdiActiveDocument.LockDocument())
			{
				using (Transaction tr = AcadFuncs.GetActiveDoc().TransactionManager.StartTransaction())
				{
					PromptPointOptions prmpt_pnt = new PromptPointOptions(mess);
					PromptPointResult prmpt_ret = AcadFuncs.GetEditor().GetPoint(prmpt_pnt);
					if (PromptStatus.Cancel == prmpt_ret.Status)
					{
						tr.Abort();
						tr.Dispose();
						return false;
					}

					picked_pnt = prmpt_ret.Value;
					tr.Commit();
				}
			}


			return true;
		}
	}
}
