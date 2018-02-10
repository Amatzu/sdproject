using QuickGraph;

namespace sdproject
{
	internal class Flow : IEdge<string>
	{
		private string name;
		private string source;
		private string target;

		private int nameHash;
		private int sourceHash;
		private int targetHash;

		string Name => name;
		string IEdge<string>.Source => source;
		string IEdge<string>.Target => target;

		public Flow(string name, string source, string target)
		{
			this.name = name;
			this.source = source;
			this.target = target;

			nameHash = name.GetHashCode();
			sourceHash = source.GetHashCode();
			targetHash = target.GetHashCode();
		}

		public override string ToString() => string.Concat(name, ": ", source, " -> ", target);

		public override int GetHashCode() => nameHash + (sourceHash ^ targetHash);
		
		public override bool Equals(object obj)
		{
			var flow = obj as Flow;
			return name == flow.name 
				&& sourceHash == flow.sourceHash 
				&& targetHash == flow.targetHash;
		}
	}
}
