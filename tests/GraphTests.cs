using NUnit.Framework;
using System;
using System.IO;

namespace sdproject
{
	internal class GraphTests
	{
		[Test, TestCase("teacup", 2, 1), TestCase("Borneo", 19, 23)]
		public void GraphParsingTest(string filename, int expectedVertices, int expectedEdges)
		{
			string filepath = AppDomain.CurrentDomain.BaseDirectory +
			                  @"..\..\Templates\" + filename + ".xmile";
			var parser = new GraphParser(filepath, false);
			var graph  = parser.CreateGraph("DEFAULT");

			Assert.AreEqual(graph.VertexCount, expectedVertices);
			Assert.AreEqual(graph.EdgeCount, expectedEdges);
		}

		[Test]
		public void GraphParserExceptionsTest()
		{
			Assert.Throws<ArgumentException>(() => new GraphParser(@"/\|*:?"));

			Assert.Throws<FileNotFoundException>(() => new GraphParser(@"Non-existant file!"));

			string filepath = AppDomain.CurrentDomain.BaseDirectory + @"..\..\UnitTests.cs";
			Assert.Throws<ArgumentException>(() => new GraphParser(filepath));
		}
	}
}
