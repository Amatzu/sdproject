using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SystemAnalyzer.Graphs
{
    public class Stock : IComparable
    {
        public readonly string Name;
        public Stock(string name)
        {
            Name = name;
        }

        public override bool Equals(object that)
        {
            if (that is Stock stock)
            {
                return Name.Equals(stock.Name);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Name != null ? Name.GetHashCode() : 0;
        }

        public int CompareTo(object obj)
        {
            var that = obj as Stock;
            return string.Compare(Name, that.Name, StringComparison.Ordinal);
        }
    }
}
