using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab2RPKS.Model
{
    public class Pair<T, K>
    {
        public Pair() { }
        public Pair(T first, K second)
        {
            First = first;
            Second = second;
        }
        public T First { get; set; }
        public K Second { get; set; }
    }
}
