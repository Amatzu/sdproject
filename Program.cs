using System;
using SystemAnalyzer.Graphs;

namespace SystemAnalyzer
{
    //TODO: refactoring
	internal class Program
	{
		private enum ErrorCode
		{
			Success = 0,
			Fail = 1,
			ArgumentsError = 2
		}

		private static void Main(string[] args)
		{
#if DEBUG
			string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\Templates\Borneo.xmile";
#else
			if (args.Length != 1)
			{
				Console.WriteLine("Программе требуется один аргумент командной строки.");
				Exit(ErrorCode.ArgumentsError);
			}
			string filepath = args[0];
#endif
			Console.WriteLine("Выполнить валидацию XML? (Y/N)");
			ConsoleKey key = Console.ReadKey(true).Key;
			bool shouldValidate = key == ConsoleKey.Y;
#if !DEBUG
			try
			{
#endif
                Console.WriteLine("Парсинг графа...");
				var parser = new GraphParser(filepath, shouldValidate);
				var graph = parser.CreateGraph("[GLOBAL]");

			    Console.WriteLine("Анализ матрицы смежности...");
			    var analyzer = new GraphAnalyzer(graph);

		        Console.WriteLine("Проверка потенциальных паттернов на изоморфизм...");
                analyzer.FindPatterns();
#if !DEBUG
            }
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
			    Console.WriteLine(e.StackTrace);
				Exit(ErrorCode.Fail);
			}
#endif

			Exit(ErrorCode.Success);
		}

		private static void Exit(ErrorCode code)
		{
			Console.WriteLine("\nНажмите любую клавишу для выхода.");
			Console.ReadKey();
			Environment.Exit((int)code);
		}
	}
}
