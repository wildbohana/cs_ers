using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Device
{
    public class Uredjaj : IUredjaj
    {
        // Polja
        private int idUredjaja;
        private static int brojInstanci = 0;    // TODO multiton pattern ?

        // Propertiji
        public int IdUredjaja { get => idUredjaja; set => idUredjaja = value; }
        public static int BrojInstanci { get => brojInstanci; set => brojInstanci = value; }

        // Konstruktor i destruktor
        public Uredjaj() 
        {
            idUredjaja = ++BrojInstanci;
        }

        ~Uredjaj()
        {
            --BrojInstanci;
        }

        // Metode
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
    }
}
