using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tut05
{
	class Program
	{
		static void Main(string[] args)
		{
		}
	}

	class TestGeneric
	{
		static public T CalculatePlus<T>(T t1, T t2)
		{
			dynamic dt1 = t1;
			dynamic dt2 = t2;
			return dt1 + dt2;
		}

		static public bool EqualVal<T>(T t1, T t2) where T : class
		{
			return t1 == t2;
		}
	}
}
