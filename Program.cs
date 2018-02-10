using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdproject
{
	class Program
	{
		const int ERRORCODE_OK = 0,
			ERRORCODE_FAIL = 1,
			ERRORCODE_WRONG_ARGS = 2;

		static void Main(string[] args)
		{
			//if(args.Length != 1)
			//{
			//	Console.WriteLine("Программе требуется один аргумент командной строки.");
			//	exit(ERRORCODE_WRONG_ARGS);
			//}
			//string filepath = args[0];

			string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Templates\Borneo.xmile";

			try
			{
				var parser = new GraphParser(filepath);
				var graph = parser.CreateGraph("DEFAULT_STOCK");
				var analyzer = new GraphAnalyzer(graph);

				throw new NotImplementedException("NYI");
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				if (e.InnerException != null) Console.WriteLine(e.InnerException.Message);
				exit(ERRORCODE_FAIL);
			}

			exit(ERRORCODE_OK);
		}

		private static void exit(int errorcode)
		{
			Console.WriteLine("\nНажмите любую клавишу для выхода.");
			Console.ReadKey();
			Environment.Exit(errorcode);
		}
	}
}
