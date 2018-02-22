using System;

namespace SystemAnalyzer
{
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
				Exit(ErrorCode.Fail);
			}

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
