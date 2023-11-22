using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDNA
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var reader = new Process_HTML();
            reader.year = Console.ReadLine();
            reader.read_html();
            Console.ReadKey();
        }
    }
}
