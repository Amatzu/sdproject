using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет парсер, составляющий граф на основе системы, описанной в файле XMILE.
	/// </summary>
	public class GraphParser
	{
		private const string NAMESPACE = "http://docs.oasis-open.org/xmile/ns/XMILE/v1.0";
		private static readonly	string SCHEMA_LOCATION = AppDomain.CurrentDomain.BaseDirectory +
		                       							 @"..\..\Templates\schema.xsd";
		private readonly XDocument xml;

		public GraphParser(string filepath, bool validate = true)
		{
			var file = new FileInfo(filepath);
			if (!file.Exists)
				throw new FileNotFoundException("Файл не найден");
			if (file.Extension != ".xmile")
				throw new ArgumentException("Файл должен иметь расширение .xmile");

			xml = XDocument.Load(filepath);

			if (!validate) return;

            //todo: check if schema exists
			using (var reader = new StreamReader(SCHEMA_LOCATION))
			using (var xmlReader = XmlReader.Create(reader))
			{
				var schemas = new XmlSchemaSet();
				var schema = XmlSchema.Read(xmlReader, null);

				schemas.Add(schema);
				xml.Validate(schemas, (sender, e) => {
					if (e.Exception == null) return;
					throw new XmlSchemaValidationException("Ошибка валидации:\n" + e.Message,
						e.Exception);
				});
			}
		}

		public Graph CreateGraph(string defaultStockName)
		{
			const string PREFIX = "{" + NAMESPACE + "}";

			XElement root = xml.Root.Element(PREFIX + "model").Element(PREFIX + "variables");
			var xmlStocks = root.Elements(PREFIX + "stock");

			//Выборка названий стоков
			var stocks = xmlStocks.Select(stock => new Stock(stock.Attribute("name").Value)).ToList();

			// Для выборки потоков (т.е. рёбер графа) используем реляционную модель данных.
			// Возьмём таблицы INFLOWS и OUTFLOWS с колонками STOCK_ID, FLOW_ID и соединим их по FLOW_ID.
			var inflows = xmlStocks.SelectMany(stock =>
				from e in stock.Elements(PREFIX + "inflow")
				select new {
					StockID = stock.Attribute("name").Value,
					FlowID = e.Value
				});

			var outflows = xmlStocks.SelectMany(stock =>
				from e in stock.Elements(PREFIX + "outflow")
				select new {
					StockID = stock.Attribute("name").Value,
					FlowID = e.Value
				});

		    var defaultStock = new Stock(defaultStockName);

			//FULL OUTER JOIN таблиц INFLOWS и OUTFLOWS по STOCK_ID
		    var leftOuter =
		        from outflow in outflows
		        join inflow in inflows on outflow.FlowID equals inflow.FlowID into subflows
		        from subflow in subflows.DefaultIfEmpty()
		        select new Flow(outflow.FlowID,
		                        new Stock(outflow.StockID),
		                        subflow?.StockID != null ? new Stock(subflow.StockID) : defaultStock);

			var rightOuter =
			    from inflow in inflows
				join outflow in outflows on inflow.FlowID equals outflow.FlowID into subflows
				from subflow in subflows.DefaultIfEmpty()
				select new Flow(inflow.FlowID,
				                subflow?.StockID != null ? new Stock(subflow.StockID) : defaultStock,
							    new Stock(inflow.StockID));

			var flows = leftOuter.Union(rightOuter);

			var graph = new Graph(allowParallelEdges: true, defaultStock);
		    graph.AddVertex(defaultStock);
			graph.AddVertexRange(stocks);
			graph.AddEdgeRange(flows);
			return graph;
		}
	}
}