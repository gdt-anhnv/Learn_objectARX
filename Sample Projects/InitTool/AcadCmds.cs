using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(InitTool.AcadFuncs))]
namespace InitTool
{
    public class AcadFuncs
    {
		[CommandMethod("MyCommand")]
		public void MyCommandFunc()
		{
			MessageBox.Show("My Comamnd is called!");
			int a = 12;
			a++;
			MessageBox.Show(a.ToString());
		}

		public void MyCommandFunc2()
		{
			MessageBox.Show("My second command");
		}

		[CommandMethod("MyCommand2")]
		public void MyCommandFunc3()
		{
			MessageBox.Show("My 2nd Comamnd is called!");
		}

	}
}
