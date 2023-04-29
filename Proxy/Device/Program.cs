using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device
{
    public class Program
    {
        static void Main(string[] args)
        {
            // Otvaranje kanala
            ChannelFactory<IServer> cf = new ChannelFactory<IServer>("Server");
            IServer kanal;

            try
            {
                kanal = cf.CreateChannel();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + "\n");
                Console.WriteLine("Uređaj se gasi. Pokušajte ponovo da ga pokrenete malo kasnije.");
                Console.ReadKey();
                return;
            }

            // Instanciranje uređaja
            Uredjaj u = new Uredjaj();

            // TODO: Da li jedno merenje šalje, ili šalje više merenja na svakih 5 minuta?

            while (true)
            {
                // Izmeri i pošalji podatak
                Merenje m = u.Izmeri();
                //Console.WriteLine(m);
                u.PosaljiMerenja(kanal, m);

                // Na svakih 5 minuta
                Thread.Sleep(TimeSpan.FromMinutes(5));
                //Thread.Sleep(500);
            }
        }
    }
}
