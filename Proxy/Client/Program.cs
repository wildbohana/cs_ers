using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.ServiceModel;
using Common;
using Common.Interfejsi;

namespace Client
{
    public class Program
    {
        [ExcludeFromCodeCoverage]
        static void Main(string[] args)
        {
            Console.Title = "Proxy projekat - Klijent";

            // Otvaranje kanala
            ChannelFactory<IProksi> cf = new ChannelFactory<IProksi>("Proksi");
            IProksi kanal;

            try
            {
                kanal = cf.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n");
                Console.WriteLine("Klijent se gasi. Pokušajte ponovo da ga pokrenete malo kasnije.");
                Console.ReadKey();
                return;
            }

            // Početak rada klijenta
            Klijent k = new Klijent();
            k.RadKlijenta(kanal);

            // Gašenje klijenta
            Console.WriteLine("Klijent je završio sa radom. Pritisni bilo koji taster za izlaz.");
            Console.ReadKey();
        }
    }
}
