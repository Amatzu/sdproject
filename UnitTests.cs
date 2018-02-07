using NUnit.Framework;
using System;
using System.IO;

namespace sdproject
{
	class UnitTests
	{
		[Test]
		[TestCase("teacup", 1, 0)]
		[TestCase("Borneo", 18, 8)]
		public void GraphParsingTest(string filename, int expectedVertices, int expectedEdges)
		{
			var parser = new GraphParser(AppDomain.CurrentDomain.BaseDirectory + @"..\..\Templates\" + filename + ".xmile");
			var graph = parser.CreateGraph();

			Assert.AreEqual(graph.VertexCount, expectedVertices);
			Assert.AreEqual(graph.EdgeCount, expectedEdges);
		}

		[Test]
		public void GraphParserExceptionsTest()
		{
			Assert.Throws<ArgumentException>(() => new GraphParser(@"/\|*:?"));

			Assert.Throws<FileNotFoundException>(() => new GraphParser(@"Non-existant file!"));

			Assert.Throws<FileFormatException>(() => new GraphParser(AppDomain.CurrentDomain.BaseDirectory + @"..\..\UnitTests.cs"));
		}
	}
}
