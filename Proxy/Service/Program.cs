using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "PROKSI - Server";

            // ServiceHost mi instancira Server kao deo ServerServisa
            using (ServiceHost host = new ServiceHost(typeof(ServerServis)))
            {
                host.Open();

                Console.WriteLine("Server je pokrenut.");
                Console.WriteLine("Adresa servera: " + host.BaseAddresses.FirstOrDefault());

                Console.WriteLine("Pritisnite bilo koji taster za zaustavljanje servera.");
                Console.ReadKey();
                host.Close();
            }                       
        }
    }
}
