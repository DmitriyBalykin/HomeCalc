using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Core.Utilities
{
    public class Cache<T>
    {
        private List<T> cache { get; set; }
        private bool isActual;
        public void SetActual(bool value)
        {
            isActual = value;
        }
        public bool IsActual()
        {
            return isActual && cache.Count > 0;
        }

        public void SetCache(List<T> list)
        {
            cache = list;
        }

        public List<T> GetCache()
        {
            return cache;
        }
    }
}
