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
            Console.WriteLine("Break\n\n\n");
            reader.write_html_csv();
            Console.ReadKey();
        }
    }
}
