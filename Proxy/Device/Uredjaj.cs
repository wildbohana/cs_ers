using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Device
{
    public class Uredjaj : IUredjaj
    {
        private int idUredjaja;
        public int IdUredjaja 
        {
            get
            {
                return idUredjaja;
            }
            set
            {
                if (value > 0) idUredjaja = value;
                else throw new ArgumentOutOfRangeException("ID uredjaja mora biti pozitivan broj!");
            }
        }

        public Uredjaj()
        {
            // ID će biti dovoljno nasumičan da bude jedinstven (pretpostavićemo!)
            Int64 broj = DateTime.Now.ToFileTime();
            idUredjaja = int.Parse((broj % 123456789).ToString());
        }

        // Izmeri i pošalji podatak na svakih 5 minuta
        [ExcludeFromCodeCoverage]
        public void RadUredjaja(IServer kanal)
        {
            Merenje m = null;
            while (true)
            {
                try
                {
                    m = Izmeri();
                }
                catch (ArgumentException ae)
                {
                    Console.WriteLine(ae.Message);
                }

                Console.WriteLine("\nPodaci o novom merenju:");
                if (m != null)
                    Console.WriteLine("\t" + m.ToString());
                
                try
                {
                    PosaljiMerenja(kanal, m);
                }
                catch (ArgumentNullException ae)
                {
                    Console.WriteLine(ae.Message);
                }

                Thread.Sleep(CitanjeVremenaIzKonfiguracijeMerenje());
            }
        }

        #region METODE
        [ExcludeFromCodeCoverage]
        private static TimeSpan CitanjeVremenaIzKonfiguracijeMerenje()
        {
            int sati = int.Parse(ConfigurationManager.AppSettings["slanjeSati"]);
            int minute = int.Parse(ConfigurationManager.AppSettings["slanjeMinute"]);
            int sekunde = int.Parse(ConfigurationManager.AppSettings["slanjeSekunde"]);

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }
        
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

            Merenje m = null;
            try
            {
                m = new Merenje(id, vrsta, vrednost, vreme, idUredjaja);
            }
            catch (ArgumentException ae)
            {
                Console.WriteLine(ae.Message);
            }

            return m;
        }

        public void PosaljiMerenja(IServer kanal, Merenje m)
        {
            if (m == null)
                throw new ArgumentNullException();

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
        #endregion
    }
}
