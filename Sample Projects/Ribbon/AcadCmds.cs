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
using AcadWindows = Autodesk.Windows;
using System.Windows.Media.Imaging;
using System.Drawing;
using System.IO;

namespace RibbonArx
{
	public class AcadCmds
    {
		public static PaletteSet ps = null;

		[CommandMethod("CreateRibbon")]
		public void CreateRibbon()
		{
			AcadWindows.RibbonControl ribbon = AcadWindows.ComponentManager.Ribbon;
			AcadWindows.RibbonTab tab = new AcadWindows.RibbonTab();
			tab.Name = "MyTab";
			tab.Title = "My Tab";
			tab.Id = "MyTabId";
			ribbon.Tabs.Add(tab);

			AcadWindows.RibbonPanelSource panelSrc = new AcadWindows.RibbonPanelSource();
			panelSrc.Name = "MyPanel";
			panelSrc.Title = "My Panel";
			panelSrc.Id = "MyPanelId";

			AcadWindows.RibbonCheckBox cb = new Autodesk.Windows.RibbonCheckBox();
			cb.Text = "Testing";
			cb.IsChecked = true;
			cb.Size = AcadWindows.RibbonItemSize.Large;

			AcadWindows.RibbonButton button1 = new AcadWindows.RibbonButton();
			button1.Name = "MyButton";
			button1.Text = "My Button";
			button1.Id = "MyButtonId";

			AcadWindows.RibbonButton button2 = new AcadWindows.RibbonButton();
			button2.Name = "MyOtherButton";
			button2.Text = "My Other Button";
			button2.Id = "MyOtherButtonId";
			button2.Size = AcadWindows.RibbonItemSize.Large;
			button2.ShowText = true;
			button2.LargeImage = GetBitmap(new Bitmap(@"C:\Users\nguye\OneDrive\Desktop\Untitled.png"), 64, 64);
			button2.CommandHandler = new MyButtonCmd();

			AcadWindows.RibbonButton button3 = new AcadWindows.RibbonButton();
			button3.Name = "MyOtherButton";
			button3.Text = "My Other Button";
			button3.Id = "MyOtherButtonId";
			button3.Size = AcadWindows.RibbonItemSize.Large;
			button3.ShowText = true;
			button3.Orientation = System.Windows.Controls.Orientation.Vertical;
			button3.LargeImage = GetBitmap(new Bitmap(@"C:\Users\nguye\OneDrive\Desktop\Untitled.png"), 64, 64);
			button3.CommandHandler = new MyButtonCmd();

			panelSrc.Items.Add(button1);
			panelSrc.Items.Add(button2);
			panelSrc.Items.Add(button3);
			panelSrc.Items.Add(cb);

			AcadWindows.RibbonPanel panel = new AcadWindows.RibbonPanel();
			panel.Source = panelSrc;
			panel.ResizeStyle = AcadWindows.RibbonResizeStyles.NeverChangeImageSize;
			tab.Panels.Add(panel);

			if (ps == null)
			{
				ps = new PaletteSet("My Palette 1",
				new Guid("229E43DB-E76F-48F9-849A-CC8D726DF257"));
				ps.SetLocation(new System.Drawing.Point(312, 763));
				ps.SetSize(new System.Drawing.Size(909, 40));
				/*For the first time we 'll enable on Bottom*/
				ps.DockEnabled = DockSides.Right;

				UserControl user_control = new UserControl();

				Button btn = new Button();
				btn.Width = 100;
				btn.Height = 100;
				btn.Text = "abc";
				user_control.Controls.Add(btn);
				ps.Add("1", user_control);
			}

			ps.Visible = true;

			/*Add Handler*/

			ps.PaletteSetMoved += ps_PaletteSetMoved;
		}

		void ps_PaletteSetMoved(object sender, PaletteSetMoveEventArgs e)
		{
			PaletteSet pt = sender as PaletteSet;
			/*Remove Handler*/
			pt.PaletteSetMoved -= ps_PaletteSetMoved;
			pt.DockEnabled = DockSides.Bottom | DockSides.Left | DockSides.Top | DockSides.Right;
		}

		BitmapImage GetBitmap(Bitmap bitmap, int height, int width)
		{
			MemoryStream stream = new MemoryStream();
			bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);

			BitmapImage bmp = new BitmapImage();
			bmp.BeginInit();
			bmp.StreamSource = new MemoryStream(stream.ToArray());
			bmp.DecodePixelHeight = height;
			bmp.DecodePixelWidth = width;
			bmp.EndInit();
			return bmp;
		}
	}

	public class MyButtonCmd : System.Windows.Input.ICommand
	{
		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return true;
		}

		public void Execute(object parameter)
		{
			MessageBox.Show("AbcXYZ");
		}
	}
}
