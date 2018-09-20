using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tut02
{
	class Program
	{
		static void Main(string[] args)
		{
			List<Shape> shapes = new List<Shape>();
			shapes.Add(new Rectangle(10, 20));
			shapes.Add(new Square(40));
			shapes.Add(new Circle(new Point(0.0, 0.0, 0.0), 10));

			double total_area = 0.0;
			foreach(Shape s in shapes)
			{
				total_area += s.CalArea();
			}

			Console.WriteLine(total_area);
			Console.ReadLine();
		}

		abstract class Shape
		{
			protected int index;
			private int test;
			abstract public double CalArea();
		}

		struct Point
		{
			public double x;
			public double y;
			public double z;

			public Point(double _x, double _y, double _z)
			{
				x = _x;
				y = _y;
				z = _z;
			}

			public Point(Point p)
			{
				x = p.x;
				y = p.y;
				z = p.z;
			}
		}

		class Circle : Shape
		{
			Point center;
			double radius;

			public Circle(Point p, double rad) : base()
			{
				center = p;
				radius = rad;
			}

			public override double CalArea()
			{
				return 3.14 * radius * radius;
			}
		}

		class Rectangle : Shape
		{
			double width;
			double height;

			public Rectangle(double _w, double _h) : base()
			{
				width = _w;
				height = _h;
			}

			public override double CalArea()
			{
				return width * height;
			}
		}

		class Square : Shape
		{
			double edge;

			public Square(double _e) : base()
			{
				edge = _e;
			}

			public override double CalArea()
			{
				return edge * edge;
			}
		}
	}
}
