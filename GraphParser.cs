using System.IO;
using System.Linq;
using QuickGraph;
using System.Xml.Schema;
using System.Xml.Linq;
using Graph = QuickGraph.BidirectionalGraph<string, QuickGraph.IEdge<string>>;

namespace sdproject
{
	class GraphParser
	{
		const string NAMESPACE = @"http://docs.oasis-open.org/xmile/ns/XMILE/v1.0",
					 SCHEMA_URI = @"http://docs.oasis-open.org/xmile/xmile/v1.0/cos01/schemas/xmile.xsd";

		private XDocument xml;

		public GraphParser(string filepath)
		{
			var file = new FileInfo(filepath);
			if (!file.Exists) throw new FileNotFoundException("File not found");
			if (file.Extension != ".xmile") throw new FileFormatException("Input must be an xmile file");

			xml = XDocument.Load(filepath);

			var schemas = new XmlSchemaSet();
			schemas.Add(NAMESPACE, SCHEMA_URI);
			xml.Validate(schemas, (sender, e) => {
				if (e.Exception != null) throw e.Exception;
			});
		}

		public Graph CreateGraph()
		{
			string prefix = "{" + NAMESPACE + "}";

			XElement root = xml.Root.Element(prefix + "model").Element(prefix + "variables");
			var xmlStocks = root.Elements(prefix + "stock");

			//Выборка названий стоков
			var stocks = xmlStocks.Select(stock => stock.Attribute("name").Value);
			
			/*
			 * Для выборки потоков (т.е. рёбер графа) используем реляционную модель данных.
			 * Возьмём таблицы INFLOWS и OUTFLOWS с колонками STOCK_ID, FLOW_ID и соединим их по FLOW_ID.
			 */
			var inflows = xmlStocks.SelectMany(stock =>
				from e in stock.Elements(prefix + "inflow")
				select new {
					StockID = stock.Attribute("name").Value,
					FlowID = e.Value
				});

			var outflows = xmlStocks.SelectMany(stock =>
				from e in stock.Elements(prefix + "outflow")
				select new {
					StockID = stock.Attribute("name").Value,
					FlowID = e.Value
				});

			//JOIN таблиц INFLOWS и OUTFLOWS по STOCK_ID
			var flows = from inflow in inflows
						join outflow in outflows on inflow.FlowID equals outflow.FlowID
						select new Edge<string>(source: outflow.StockID, target: inflow.StockID);

			var graph = new Graph(allowParallelEdges: true);
			graph.AddVertexRange(stocks);
			graph.AddEdgeRange(flows);
			return graph;
		}
	}
}
