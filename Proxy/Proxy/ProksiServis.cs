using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// TODO: na svakih 5 minuta obriši podatke starije od 24h
// TODO: učitaj vremena iz konfiguracije (već imaš vrednosti u app.config)

// TODO: debaguj ponovno slanje iste komande (čitanje iz lokalne kopije) jer tada krešuje

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        public static IServer kanal = null;
        static readonly Proksi p = new Proksi();

        // idMerenja, (Merenje, VremePoslednjeIzmene)
        static Dictionary<int, Tuple<Merenje, DateTime>> lokalnaKopija = new Dictionary<int, Tuple<Merenje, DateTime>>();

        // Timestampovi za sve kriterujume pretrage
        DateTime poslednjeAzuriranjeLokalnoIdSvi = DateTime.MinValue;
        DateTime poslednjeAzuriranjeLokalnoIdPoslednji = DateTime.MinValue;
        DateTime poslednjeAzuriranjeLokalnoSviPoslednji = DateTime.MinValue;
        DateTime poslednjeAzuriranjeLokalnoAnalogni = DateTime.MinValue;
        DateTime poslednjeAzuriranjeLokalnoDigitalni = DateTime.MinValue;

        // Metoda za proveru poslednjeg ažuriranja podataka u bazi
        // Vreme poslednjeg dodavanja == vreme poslednjeg ažuriranja (jer se merenja ne mogu modifikovati)
        private DateTime PoslednjeDodavanjeUBazu()
        {
            string query = "select * from Podaci where vreme=(select max(vreme) from Podaci)";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0].VremeMerenja;
            else
                return DateTime.MinValue;       // Baza podataka je skroz prazna u ovom slučaju
        }

        public List<Merenje> DobaviPodatkeId(int id)
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalnoIdSvi)
            {
                string query = "select * from Podaci where idUredjaja=" + id.ToString();
                poslednjeAzuriranjeLokalnoIdSvi = DateTime.Now;
                
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                try
                {
                    rezultat = kanal.Citanje("Svi podaci za traženi ID uređaja", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                foreach (Merenje m in rezultat)
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            // Dobavi podatke iz lokalne kopije
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.IdUredjaja == id)
                        rezultat.Add(m.Item1);

                foreach (Merenje m in rezultat)
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }

            return rezultat;
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalnoIdPoslednji)
            {
                string query = "select * from Podaci where vreme=(select max(vreme) from Podaci where idUredjaja=" + id.ToString() + ")";
                poslednjeAzuriranjeLokalnoIdPoslednji = DateTime.Now;

                List<Merenje> rezultat = new List<Merenje>();
                rezultat = kanal.Citanje("Poslednji uneti podatak za traženi ID uređaja", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                foreach (Merenje m in rezultat)
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);

                return rezultat[0];
            }
            // Dobavi podatke iz lokalne kopije
            else
            {
                DateTime poslednjeVreme = DateTime.MinValue;
                Merenje rezultat = null;

                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                {
                    if (m.Item1.IdUredjaja == id && m.Item1.VremeMerenja > poslednjeVreme)
                    {
                        rezultat = m.Item1;
                        poslednjeVreme = m.Item1.VremeMerenja;
                    }
                }

                lokalnaKopija[rezultat.IdMerenja] = new Tuple<Merenje, DateTime>(rezultat, DateTime.Now);
                return rezultat;
            }
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();
            List<Merenje> rezultat = new List<Merenje>();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalnoSviPoslednji)
            {
                string query = "select * from (select * from Podaci order by vreme desc) group by idUredjaja";
                poslednjeAzuriranjeLokalnoSviPoslednji = DateTime.Now;

                rezultat = kanal.Citanje("Poslednji uneti podatak za sve uređaje", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                foreach (Merenje m in rezultat)
                   lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            // Dobavi podatke iz lokalne kopije
            else
            {
                DateTime poslednjeVreme = DateTime.MinValue;
                rezultat = null;

                // Izdvajanje indeksa iz kolekcije
                List<int> indeksi = new List<int>();
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (!indeksi.Contains(m.Item1.IdUredjaja))
                        indeksi.Add(m.Item1.IdUredjaja);

                Merenje mtemp = null;
                DateTime vtemp;

                // Za svaki indeks tražimo podatak sa najnovijim vremenom
                foreach (int i in indeksi)
                {
                    vtemp = DateTime.MinValue;

                    foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    {
                        if (m.Item1.IdUredjaja == i && m.Item1.VremeMerenja > vtemp)
                        {
                            mtemp = m.Item1;
                            vtemp = m.Item1.VremeMerenja;
                        }
                    }                        

                    rezultat.Add(mtemp);
                    lokalnaKopija[i] = new Tuple<Merenje, DateTime>(lokalnaKopija[i].Item1, DateTime.Now);
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalnoAnalogni)
            {
                string query = "select * from Podaci where vrstaMerenja=0";

                poslednjeAzuriranjeLokalnoAnalogni = DateTime.Now;

                rezultat = kanal.Citanje("Svi analogni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                foreach (Merenje m in rezultat)
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            // Dobavi podatke iz lokalne kopije
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                {
                    if (m.Item1.VrstaMerenja == VrstaMerenja.ANALOGNO_MERENJE)
                    {
                        rezultat.Add(m.Item1);
                        lokalnaKopija[m.Item1.IdMerenja] = new Tuple<Merenje, DateTime>(m.Item1, DateTime.Now);
                    }
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalnoDigitalni)
            {
                string query = "select * from Podaci where vrstaMerenja=1";

                poslednjeAzuriranjeLokalnoDigitalni = DateTime.Now;

                rezultat = kanal.Citanje("Svi digitalni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                foreach (Merenje m in rezultat)
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            // Dobavi podatke iz lokalne kopije
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                {
                    if (m.Item1.VrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
                    {
                        rezultat.Add(m.Item1);
                        lokalnaKopija[m.Item1.IdMerenja] = new Tuple<Merenje, DateTime>(m.Item1, DateTime.Now);
                    }
                }
            }

            return rezultat;
        }
    }
}
