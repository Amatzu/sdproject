using QuickGraph;

namespace SystemAnalyzer.Graphs
{
	/// <summary>
	/// Представляет поток анализируемой системы.
	/// </summary>
	public sealed class Flow : IEdge<Stock>
	{
		public string Name { get; }
		public Stock Source { get; }
		public Stock Target { get; }

		public Flow(string name, Stock source, Stock target)
		{
			Name = name;
			Source = source;
			Target = target;
		}

		public override string ToString()
	    {
	        return string.Concat(Name, ": ", Source, " -> ", Target);
	    }

	    public override bool Equals(object that)
		{
		    if (that is Flow flow)
		    {
		        return Name.Equals(flow.Name) &&
		               Source == flow.Source &&
		               Target == flow.Target;
		    }

		    return false;
		}

	    public override int GetHashCode()
	    {
	        unchecked
	        {
	            var hashCode = (Name != null ? Name.GetHashCode() : 0);
	            hashCode = (hashCode * 397) ^ (Source != null ? Source.GetHashCode() : 0);
	            hashCode = (hashCode * 397) ^ (Target != null ? Target.GetHashCode() : 0);
	            return hashCode;
	        }
	    }
	}
}
