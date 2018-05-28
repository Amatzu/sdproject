using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAnalyzer.Core
{
    internal struct Edge
    {
        public readonly string Source;
        public readonly string Target;

        private Edge(string source, string target)
        {
            Source = source;
            Target = target;
        }

        public static Edge FromFlow(Flow flow)
        {
            return new Edge(flow.Source, flow.Target);
        }
    }
}
