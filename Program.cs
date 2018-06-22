using System;
using System.Linq;
using System.Xml;
using SystemAnalyzer.Graphs;
using SystemAnalyzer.Graphs.Patterns;
using SystemAnalyzer.Matrices;

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
#endif
		    {
                Console.WriteLine("Парсинг графа...");
				var parser = new GraphParser(filepath, shouldValidate);
				var graph = parser.CreateGraph("[GLOBAL]");
                Console.WriteLine("Граф считан.");

                graph = new Graph(false, new Stock("[GLOBAL]"));
		        var stocks = Enumerable.Range(0, 'O' - 'A' + 1)
		                               .Select(c => new Stock(((char) (c + 'A')).ToString()))
		                               .ToArray();
		        graph.AddVertexRange(stocks);
		        void makeFlow(char a, char b)
		        {
		            var name = a.ToString() + b.ToString();
		            var flow = new Flow(name, stocks[a - 'A'], stocks[b - 'A']);
		            graph.AddEdge(flow);
		        }

		        makeFlow('A', 'B');
		        makeFlow('B', 'C');
		        makeFlow('B', 'J');
		        makeFlow('C', 'A');
		        makeFlow('C', 'E');
		        makeFlow('D', 'C');
		        makeFlow('D', 'H');
		        makeFlow('D', 'I');
		        makeFlow('I', 'H');
		        makeFlow('J', 'H');
		        makeFlow('L', 'N');
		        makeFlow('N', 'M');
		        makeFlow('A', 'O');
                makeFlow('B', 'O');
		        makeFlow('D', 'E');
		        makeFlow('E', 'G');
		        makeFlow('G', 'D');
		        makeFlow('D', 'F');
		        makeFlow('E', 'F');
		        makeFlow('G', 'F');
		        makeFlow('I', 'J');
		        makeFlow('J', 'L');
		        makeFlow('L', 'I');
		        makeFlow('I', 'K');
		        makeFlow('J', 'K');
		        makeFlow('L', 'K');

		        int iteration = 1;
			    bool foundPatterns;
		        PatternMap patterns = null;
			    do
			    {
                    Console.WriteLine("\nИтерация №" + iteration);
			        Console.WriteLine("Анализ матрицы смежности...");
			        var analyzer = new GraphAnalyzer(graph, patterns);

			        Console.WriteLine("Проверка потенциальных паттернов на изоморфизм...");
			        foundPatterns = analyzer.FindPatterns(out patterns);

			        if (foundPatterns)
			        {
                        Console.WriteLine("Найдено экземпляров паттернов: " + patterns.TotalInstances);
			            graph.ReplacePatterns(patterns);
			            iteration++;
			        }
			        else
			        {
			            Console.WriteLine("Паттернов не обнаружено.");
			        }
			    } while (foundPatterns);
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

	    private static void Export(PatternMap patterns)
	    {
	        //todo
	    }

		private static void Exit(ErrorCode code)
		{
			Console.WriteLine("\nНажмите любую клавишу для выхода.");
			Console.ReadKey();
			Environment.Exit((int)code);
		}
	}
}
