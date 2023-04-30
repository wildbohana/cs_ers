using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
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

        public Uredjaj() 
        {
            // ID će biti dovoljno nasumičan da bude jedinstven (pretpostavićemo!)
            long broj = DateTime.Now.ToFileTime();
            idUredjaja = int.Parse((broj % 123123123).ToString());
        }

        #region METODE
        public Merenje Izmeri()
        {
            // Timestamp i ID merenja - sadašnji trenutak
            DateTime vreme = DateTime.Now;
            long id = vreme.ToFileTime();

            // Vrsta i vrednost - na osnovu slučajnosti
            Random rand = new Random();
            VrstaMerenja vrsta = (rand.NextDouble() > 0.5) ? VrstaMerenja.ANALOGNO_MERENJE : VrstaMerenja.DIGITALNO_MERENJE;

            int vrednost;
            if (vrsta == VrstaMerenja.ANALOGNO_MERENJE)
                vrednost = rand.Next(0, 100000);
            else
                vrednost = (rand.NextDouble() > 0.5) ? 1 : 0;

            return new Merenje(id, vrsta, vrednost, vreme);
        }

        public void PosaljiMerenja(IServer kanal, Merenje m)
        {
            try
            {
                if (kanal.Upis(m, idUredjaja))
                    Console.WriteLine(DateTime.Now.ToString() + "\tUspešno slanje merenja u bazu podataka.");
                else
                    Console.WriteLine(DateTime.Now.ToString() + "\tNeuspešno slanje merenja u bazu podataka.");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }            
        }

        // Izmeri i pošalji podatak na svakih 5 minuta
        public void RadUredjaja(IServer kanal)
        {
            while (true)
            {
                Merenje m = Izmeri();
                PosaljiMerenja(kanal, m);

                Thread.Sleep(TimeSpan.FromMinutes(5));
                //Thread.Sleep(500);
            }
        }
        #endregion
    }
}
