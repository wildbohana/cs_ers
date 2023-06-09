﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        static void Main(string[] args)
        {
            Console.Title = "PROKSI - Server";

            // ServiceHost mi instancira Loger kao deo ServerServisa
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
