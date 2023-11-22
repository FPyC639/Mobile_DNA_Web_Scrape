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
            Console.Write("Enter the Year: ");
            reader.year = Console.ReadLine();
            var title_ls = reader.read_title();
            title_ls.ForEach(x => Console.WriteLine(x));
            Console.WriteLine("Break\n\n\n");
            reader.two_deep_extractor();
            Console.ReadKey();
        }
    }
}
