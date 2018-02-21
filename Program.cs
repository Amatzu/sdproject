using System;

namespace sdproject
{
	internal class Program
	{
		private const int ERRORCODE_OK = 0,
						  ERRORCODE_FAIL = 1,
						  ERRORCODE_WRONG_ARGS = 2;

		private static void Main(string[] args)
		{
#if !DEBUG
			if(args.Length != 1)
			{
				Console.WriteLine("Программе требуется один аргумент командной строки.");
				exit(ERRORCODE_WRONG_ARGS);
			}
			string filepath = args[0];
#else
			string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Templates\Borneo.xmile";
#endif
			Console.WriteLine("Выполнить валидацию XML? (Y/N)");
			ConsoleKey key = Console.ReadKey(true).Key;
			bool shouldValidate = key == ConsoleKey.Y;

			try
			{
				var parser = new GraphParser(filepath, shouldValidate);
				var graph = parser.CreateGraph("[Global]");
				var analyzer = new GraphAnalyzer(graph);

				//TODO: main flow
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
