using System.IO;
using System.Linq;
using QuickGraph;
using System.Xml.Schema;
using System.Xml.Linq;
using Graph = QuickGraph.UndirectedGraph<string, QuickGraph.IEdge<string>>;

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

			xml = XDocument.Load(file.FullName);

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
			var stocks = from stock in root.Elements(prefix + "stock")
						 select stock.Attribute("name").Value;
			// TODO: fix flow query
			var flows = from flow in root.Elements(prefix + "flow")
						let inflow = flow.Attribute("inflow")?.Value
						let outflow = flow.Attribute("outflow").Value
						select new Edge<string>(outflow, inflow ?? outflow);

			var graph = new Graph(true);
			graph.AddVertexRange(stocks);
			graph.AddEdgeRange(flows);

			return graph;
		}
	}
}
