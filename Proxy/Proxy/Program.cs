using Common.Interfejsi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

// Proksi ima i host i client deo !!!
// Host ka klijentu, client ka serveru

namespace Proxy
{
    public class Program
    {
        static void Main(string[] args)
        {
			Console.Title = "Proksi projekat - Proksi server";

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
