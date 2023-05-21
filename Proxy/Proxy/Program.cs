using Common.Interfejsi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        static void Main(string[] args)
        {
            Console.Title = "PROKSI - Proxy";

            using (ServiceHost host = new ServiceHost(typeof(ProksiServis)))
            {
                host.Open();

                Console.WriteLine("Proksi je pokrenut.");
                Console.WriteLine("Adresa proksija: " + host.BaseAddresses.FirstOrDefault());

                Console.WriteLine("Pritisnite bilo koji taster za zaustavljanje proksija.");
                Console.ReadKey();
                host.Close();
            }
        }
    }
}
