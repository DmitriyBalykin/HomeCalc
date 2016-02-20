using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeCalc.Presentation.Models
{
    public class AggregationInterval
    {
        private static List<string> names = new List<string> { "День", "Тиждень", "Місяць", "Квартал", "Рік" };
        public string DisplayName { get; set; }
        public AggregationIntervalValue Value { get; set; }

        public static string ToString(AggregationIntervalValue value)
        {
            return names[(int)value];
        }
        public static AggregationIntervalValue GetValue(string name)
        {
            return (AggregationIntervalValue)names.IndexOf(name);
        }
        public static IEnumerable<AggregationInterval> GetList()
        {
            return names.Select(n => new AggregationInterval { DisplayName = n, Value = AggregationInterval.GetValue(n) });
        }
    }
    public enum AggregationIntervalValue
    { 
        Day = 0,
        Week = 1,
        Month = 2,
        Quarter = 3,
        Year = 4
    }
}
