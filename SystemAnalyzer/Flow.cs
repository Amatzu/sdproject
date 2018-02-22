using QuickGraph;

namespace SystemAnalyzer
{
	/// <summary>
	/// Представляет поток анализируемой системы.
	/// </summary>
	internal class Flow : IEdge<string>
	{
		public string Name { get; }
		public string Source { get; }
		public string Target { get; }

		public Flow(string name, string source, string target)
		{
			Name = name;
			Source = source;
			Target = target;
		}

		public override string ToString() => string.Concat(Name, ": ", Source, " -> ", Target);

		public override int GetHashCode() => Name.GetHashCode() + (Source.GetHashCode() ^ Target.GetHashCode());

		public override bool Equals(object obj)
		{
			var flow = obj as Flow;
			return Name == flow.Name &&
			       Source == flow.Source &&
			       Target == flow.Target;
		}
	}
}
