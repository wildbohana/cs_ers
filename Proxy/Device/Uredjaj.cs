using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device
{
    public class Uredjaj : IUredjaj
    {
        private int idUredjaja;
        public int IdUredjaja { get => idUredjaja; set => idUredjaja = value; }
        private int vremeIzmedjuSlanja;

        public Uredjaj() 
        {
            // ID će biti dovoljno nasumičan da bude jedinstven (pretpostavićemo!)
            Int64 broj = DateTime.Now.ToFileTime();
            idUredjaja = int.Parse((broj % 123456789).ToString());

            // Učitavanje vremena (u minutama) iz konfiguracije
            string temp = ConfigurationManager.AppSettings["vremeSlanje"];
            vremeIzmedjuSlanja = int.Parse(temp);
        }

        // Izmeri i pošalji podatak na svakih 5 minuta
        public void RadUredjaja(IServer kanal)
        {
            while (true)
            {
                Merenje m = Izmeri();
                PosaljiMerenja(kanal, m);

                //Thread.Sleep(TimeSpan.FromMinutes(vremeIzmedjuSlanja));
                Thread.Sleep(500);
            }
        }

        // Metode
        public Merenje Izmeri()
        {
            // Timestamp - sadašnji trenutak
            DateTime vreme = DateTime.Now;

            // ID merenja - zasnovan na DateTime.Now, "skraćen" na int podatak
            long temp = vreme.ToFileTime() % 1000000000;
            int id = int.Parse(temp.ToString());

            // Vrsta i vrednost - na osnovu slučajnosti
            Random rand = new Random();
            VrstaMerenja vrsta = (rand.NextDouble() > 0.5) ? VrstaMerenja.ANALOGNO_MERENJE : VrstaMerenja.DIGITALNO_MERENJE;

            int vrednost;
            if (vrsta == VrstaMerenja.ANALOGNO_MERENJE)
                vrednost = rand.Next(0, 100000);
            else
                vrednost = (rand.NextDouble() > 0.5) ? 1 : 0;

            return new Merenje(id, vrsta, vrednost, vreme, idUredjaja);
        }

        public void PosaljiMerenja(IServer kanal, Merenje m)
        {
            try
            {
                if (kanal.Upis(m))
                    Console.WriteLine(DateTime.Now.ToString() + "\tUspešno slanje merenja u bazu podataka.");
                else
                    Console.WriteLine(DateTime.Now.ToString() + "\tNeuspešno slanje merenja u bazu podataka.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }
    }
}
