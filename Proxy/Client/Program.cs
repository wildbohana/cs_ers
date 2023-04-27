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

namespace Client
{
    public class Program
    {
        //[ExcludeFromCodeCoverage]
        static void Main(string[] args)
        {
            //Writer w = new Writer();
            try
            {
                // TODO: dopuni App.config
                // Otvaranje kanala
                ChannelFactory<IProxyService> cf = new ChannelFactory<IProxyService>(new NetTcpBinding(), new EndpointAddress("net.tcp://localhost:4002/IProxyService"));
                IProxyService kanal = cf.CreateChannel();
            }
            catch
            {
                throw new AddressAccessDeniedException("Neuspešna konekcija na server.");
            }
                
            // pocetak slanja w.Start(...)
            //w.Start(kanal);

            Console.WriteLine("Klijent je završio sa radom. Pritisni bilo koji taster za izlaz...");
            Console.ReadLine();
        }
    }
}
