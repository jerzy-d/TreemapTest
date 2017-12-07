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
            double[] sizes = new double[] { 60, 60, 40, 30, 20, 20, 10 };
            var tm = new Treemap(sizes, 0, 0, 60, 40);
            tm.Squarify();
        }
    }
}
