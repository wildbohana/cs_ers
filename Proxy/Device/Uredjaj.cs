using Common;
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
        // Polje
        private int idUredjaja;

        // Properti
        public int IdUredjaja { get => idUredjaja; set => idUredjaja = value; }

        // Konstruktor
        public Uredjaj() { }

        // Metode
        public Merenje Izmeri()
        {
            // ID merenja - na osnovu trenutnog vremena
            long id = DateTime.Now.ToFileTime();

            // Vrsta na osnovu slučajnosti
            Random rand = new Random();
            double prob = rand.NextDouble();
            VrstaMerenja vrsta = (prob > 0.5) ? VrstaMerenja.ANALOGNO_MERENJE : VrstaMerenja.DIGITALNO_MERENJE;

            // Vrednost - takođe
            int vrednost = rand.Next(0, 100000);

            // Timestamp - sadašnji trenutak
            DateTime vreme = DateTime.Now;

            return new Merenje(id, vrsta, vrednost, vreme);
        }

        // TODO ostalo
    }
}
