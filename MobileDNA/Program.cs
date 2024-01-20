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
            reader.set_year(Console.ReadLine());
            Console.WriteLine();
            Console.WriteLine("Enter the search query: ");
            reader.query = Console.ReadLine();
            Console.WriteLine("Break\n\n\n");
            //reader.write_html_csv();
            reader.write_access_csv();
            Console.ReadKey();
        }
    }
}
