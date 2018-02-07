﻿using QuickGraph;

namespace sdproject
{
	internal class Flow : IEdge<string>
	{
		private string name;
		private string source;
		private string target;

		string IEdge<string>.Source => source;
		string IEdge<string>.Target => target;

		public Flow(string name, string source, string target)
		{
			this.name = name;
			this.source = source;
			this.target = target;
		}

		public override string ToString() => string.Concat(name, ": ", source, " -> ", target);
	}
}