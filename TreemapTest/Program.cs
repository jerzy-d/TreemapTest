using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreemapTest
{
    class Program
    {
        static void Main(string[] args)
        {
            double[] sizes = new double[] { 6, 6, 4, 3, 2, 2, 1 };
            var tm = new Treemap(sizes, 0, 0, 6, 4);
            tm.Squarify();
        }
    }
}
