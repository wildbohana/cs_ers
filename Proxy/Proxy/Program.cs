using Common.Interfejsi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// Proxy Host:  localhost://8002/Proksi
// Server Host: localhost://8001/Server

namespace Proxy
{
    public class Program
    {
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
