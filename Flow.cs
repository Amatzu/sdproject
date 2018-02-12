using QuickGraph;

namespace sdproject
{
	internal class Flow : IEdge<string>
	{
		private readonly string name;
		private readonly string source;
		private readonly string target;

		public string Name => name;
		public string Source => source;
		public string Target => target;

		public Flow(string name, string source, string target)
		{
			this.name = name;
			this.source = source;
			this.target = target;
		}

		public override string ToString() => string.Concat(name, ": ", source, " -> ", target);

		public override int GetHashCode()
		{
			return name.GetHashCode() + (source.GetHashCode() ^ target.GetHashCode());
		} 
		
		public override bool Equals(object obj)
		{
			var flow = obj as Flow;
			return name == flow.name && 
			       source == flow.source && 
			       target == flow.target;
		}
	}
}
