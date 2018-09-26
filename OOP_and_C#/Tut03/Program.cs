using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tut03
{
	class Program
	{
		static void Main(string[] args)
		{
			Shape s1 = new Rectangle(10, 12);
			Shape s2 = new Square(10);

			Console.WriteLine(s1.Area());
			Console.WriteLine(s2.Area());
		}
	}

	interface Shape
	{
		double Area();
	}

	class Rectangle : Shape
	{
		protected double width;
		protected double height;

		public Rectangle(double _w, double _h)
		{
			width = _w;
			height = _h;
		}

		public double Area()
		{
			return width * height;
		}
	}

	class Square : Rectangle, Shape
	{
		public Square(double _w) : base(_w, _w)
		{
		}

		public new double Area()
		{
			Console.WriteLine("Calculate Area of Square:");
			return base.Area();
		}
	}
}
