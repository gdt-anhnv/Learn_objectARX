using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TryCatch
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				FuncA(7);
				FuncB(0);
			}
			catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			finally
			{
				Console.ReadLine();
			}
		}

		static void FuncA(int a)
		{
			if (0 == a)
				throw new System.Exception("FuncA: Divide by zero!");

			int ret = 15 / a;
			Console.WriteLine(ret);
		}

		static void FuncB(int a)
		{
			if (0 == a)
				throw new System.Exception("FuncB: Divide by zero!");

			int ret = 15 / a;
			Console.WriteLine(ret);
		}
	}
}
