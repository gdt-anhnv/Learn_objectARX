using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tut01
{
	class Program
	{
		static void Main(string[] args)
		{
			Student s1 = new Student();
			Student s2 = new Student(16, "Nguyen Van Anh");

			s2.AddMark("Math", 8.6, 2);
			s2.AddMark("Physics", 6.6, 1);
			double avarage = s2.CalculateAvarage();

			MyEnum pos = s2.PositionInClass();
			Console.ReadLine();
		}
	}

	enum MyEnum
	{
		kFirst = 0,
		kFirstAndAhalf,
		kSecond,
		kThird
	}

	class Student
	{
		private int age;					//class member
		private string name;				//class member
		private List<Mark> marks;

		public MyEnum PositionInClass()
		{
			return MyEnum.kFirst;
		}

		public Student()
		{
			age = 10;
			name = "";
			marks = new List<Mark>();
		}

		public Student(int _a, string _n)			// int _a, string _n: parameter/argument cua function
		{
			age = _a;
			name = _n;
			marks = new List<Mark>();
		}

		public void AddMark(string sub, double value, int fac = 1)
		{
			marks.Add(new Mark(sub, value, fac));
			return;
		}

		public double CalculateAvarage()
		{
			int total = 0;
			double val = 0.0;
			foreach(Mark tmp in marks)
			{
				total += tmp.Factor;
				val += tmp.Factor * tmp.MarkSubject;
			}

			return val / total;
		}
	}

	class Mark
	{
		private string subject;
		private double mark_subject;
		public double MarkSubject
		{
			get { return mark_subject; }
			set { mark_subject = value;}
		}
		private int factor;
		public int Factor
		{
			get { return factor; }
		}

		public Mark(string sub, double val, int fac = 1)
		{
			subject = sub;
			mark_subject = val;
			factor = fac;
		}
	}
}
