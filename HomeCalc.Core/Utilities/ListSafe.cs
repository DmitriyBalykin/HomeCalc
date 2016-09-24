using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Utilities
{
    public class ListSafe<T> : List<T>
    {
        private object monitor = new object();
        public new void Add(T item)
        {
            lock (monitor)
            {
                base.Add(item);
            }
        }
        public new void AddRange(IEnumerable<T> range)
        {
            lock (monitor)
            {
                base.AddRange(range);
            }
        }
    }
}
