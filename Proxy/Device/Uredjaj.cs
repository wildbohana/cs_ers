using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Oy, this is useful
// DateTime formatiran = DateTime.ParseExact(ss, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);

namespace Device
{
    public class Uredjaj : IUredjaj
    {
        // Polja
        private int idUredjaja;
        private static int brojInstanci = 0;

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
            // ID merenja - na osnovu trenutnog vremena
            long id = DateTime.Now.ToFileTime();

            // Vrsta - na osnovu slučajnosti
            Random rand = new Random();
            VrstaMerenja vrsta = (rand.NextDouble() > 0.5) ? VrstaMerenja.ANALOGNO_MERENJE : VrstaMerenja.DIGITALNO_MERENJE;

            // Vrednost - takođe
            int vrednost;
            if (vrsta == VrstaMerenja.DIGITALNO_MERENJE)
                vrednost = rand.Next(0, 100000);
            else
                vrednost = (rand.NextDouble() > 0.5) ? 1 : 0;

            // Timestamp - sadašnji trenutak
            DateTime vreme = DateTime.Now;

            return new Merenje(id, vrsta, vrednost, vreme);
        }

        // Merenja se šalju serveru
        public void PosaljiMerenja(IServer kanal, Merenje m)
        {
            // try-catch myb ?
            if (kanal.UpisUBazu(m, idUredjaja))
                Console.WriteLine(DateTime.Now.ToString() + "\tUspešno slanje merenja u bazu podataka.");
            else
                Console.WriteLine(DateTime.Now.ToString() + "\tNeuspešno slanje merenja u bazu podataka.");
        }
    }
}
