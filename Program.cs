using System;
using System.IO;
using SystemAnalyzer.Graphs;
using SystemAnalyzer.Graphs.Analyzing;
using SystemAnalyzer.Graphs.Parsing;
using SystemAnalyzer.Graphs.Patterns;

namespace SystemAnalyzer
{
	internal class Program
	{
	    private static readonly string PATH = AppDomain.CurrentDomain.BaseDirectory + "patterns.txt";

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
#endif
		    {
                Console.WriteLine("Парсинг графа...");
				var parser = new GraphParser(filepath, shouldValidate);
				var graph = parser.CreateGraph("[GLOBAL]");
                Console.WriteLine("Граф считан.");

		        Process(graph);
			}
#if !DEBUG
			catch(Exception e)
			{
                Console.WriteLine(e.Message);
			    Console.WriteLine(e.StackTrace);
				Exit(ErrorCode.Fail);
			}
#endif
			Exit(ErrorCode.Success);
		}

	    internal static void Process(Graph graph)
	    {
	        int iteration = 1;
	        bool foundPatterns;
	        PatternMap patterns = null;
            if (File.Exists(PATH)) File.Delete(PATH);
	        do
	        {
	            Console.WriteLine("\nИтерация №" + iteration);

	            if (graph.VertexCount < 3)
	            {
                    Console.WriteLine("Граф имеет слишком мало вершин для поиска паттернов.");
	                break;
	            }

	            Console.WriteLine("Ищем паттерны...");
	            var analyzer = new GraphAnalyzer(graph, patterns);
	            foundPatterns = analyzer.FindPatterns(out patterns);

	            if (foundPatterns)
	            {
	                Console.WriteLine("Найдено экземпляров паттернов: " + patterns.TotalInstances);
                    Console.WriteLine("Делаем подстановку...");
	                graph.ReplacePatterns(patterns);
                    Export(patterns, iteration);
	                iteration++;
	            }
	            else
	            {
	                Console.WriteLine("Паттернов не обнаружено.");
	            }
	        } while (foundPatterns);

            Console.WriteLine("Поиск завершён.");
	    }

	    private static void Export(PatternMap patterns, int iteration)
	    {
	        using (var writer = new StreamWriter(PATH, true))
	        {
	            writer.WriteLine("--- Итерация " + iteration + " ---");

	            for (int n = patterns.MaxPatternSize; n > 2; n--)
	            {
	                foreach (var pattern in patterns[n])
	                {
                        if (pattern.Instances.Count == 0) continue;
	                    writer.WriteLine("Паттерн: " + pattern.Name);

	                    int index = 1;
	                    foreach (var instance in pattern.Instances)
	                    {
	                        string vertices = string.Join(", ", instance.Vertices);
                            writer.WriteLine(index + ") " + vertices);
	                        index++;
	                    }

                        writer.WriteLine();
	                }
	            }
	        }
	    }

		private static void Exit(ErrorCode code)
		{
			Console.WriteLine("\nНажмите любую клавишу для выхода.");
			Console.ReadKey();
			Environment.Exit((int)code);
		}
	}
}
