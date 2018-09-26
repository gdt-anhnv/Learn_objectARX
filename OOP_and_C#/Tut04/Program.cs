using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tut04
{
	class Program
	{
		static void Main(string[] args)
		{
			UseTuple();
		}

		static void UseList()
		{
			List<string> strs = new List<string>();
			strs.Add("abc");
			strs.Add("def");
			strs.Add("xyz");

			strs.RemoveAt(1);
			strs.Insert(1, "zyx");

			foreach(var str in strs)
			{
				Console.WriteLine(str);
			}
		}

		static void UseQueue()
		{
			Queue<string> strs = new Queue<string>();
			strs.Enqueue("abc");
			strs.Enqueue("def");
			strs.Enqueue("xyz");

			foreach(var str in strs)
			{
				Console.WriteLine(str);
			}

			strs.Dequeue();
		}

		static void UseStack()
		{
			Stack<string> strs = new Stack<string>();
			strs.Push("abc");
			strs.Push("xyz");
			strs.Push("dfx");

			foreach(var str in strs)
			{
				Console.WriteLine(str);
			}

			strs.Pop();
		}

		static void UseTuple()
		{
			Tuple<string, int> pairs = new Tuple<string, int>("abc", 12);
			Tuple<string, int> pairs2 = new Tuple<string, int>("abc", 10);
			bool cmp = pairs.Equals(pairs2);

			List<Tuple<string, int>> items = new List<Tuple<string, int>>();
			items.Add(new Tuple<string, int>("abc", 12));
			items.Add(new Tuple<string, int>("abc", 10));
			items.Add(new Tuple<string, int>("dxy", 120));

			var search_result = items.FindAll(x => "abc" == x.Item1);
			var search_result2 = items.FindAll(x => 12 == x.Item2);
		}

		static void UseDictionary()
		{
			Dictionary<string, int> dic = new Dictionary<string, int>();
			dic.Add("abc", 12);
			dic.Add("dxy", 14);

			Console.Write(dic["abc"]);
		}
	}
}
