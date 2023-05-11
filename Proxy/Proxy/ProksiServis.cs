using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

// TODO: na svakih 5 minuta obriši podatke starije od 24h
// TODO: učitaj vremena iz konfiguracije (već imaš vrednosti u app.config)

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        public static IServer kanal = null;
        static readonly Proksi p = new Proksi();

        // idMerenja, (Merenje, VremePoslednjeIzmene)
        static Dictionary<int, Tuple<Merenje, DateTime>> lokalnaKopija = new Dictionary<int, Tuple<Merenje, DateTime>>();

        #region BAZA POSLEDNJI
        // Metoda za proveru poslednjeg ažuriranja podataka u bazi
        // Vreme poslednjeg dodavanja == vreme poslednjeg ažuriranja (jer se merenja ne mogu modifikovati)
        private Merenje PoslednjeDodavanjeUBazu()
        {
            string query = "select * from Podaci where vreme=(select max(vreme) from Podaci)";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0];
            else
                return null;      // Baza podataka je skroz prazna u ovom slučaju
        }
        #endregion

        #region LOKALNO POSLEDNJI
        // Metoda za proveru poslednjeg ažuriranja lokalno za traženi ID
        private Merenje PoslednjeDodavanjeLokalnoZaTrazeniId(int id)
        {
            Merenje m = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (Tuple<Merenje, DateTime> torka in lokalnaKopija.Values)
                {
                    if (torka.Item1.IdUredjaja == id)
                    {
                        if (vtemp < torka.Item1.VremeMerenja)
                        {
                            m = torka.Item1;
                            vtemp = m.VremeMerenja;
                        }
                    }
                }
            }
            return m;
        }

        // Metoda za proveru poslednjeg ažuriranja lokalno za traženu vrstu merenja
        private Merenje PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja vr)
        {
            Merenje m = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (Tuple<Merenje, DateTime> torka in lokalnaKopija.Values)
                {
                    if (torka.Item1.VrstaMerenja == vr)
                    {
                        if (vtemp < torka.Item1.VremeMerenja)
                        {
                            m = torka.Item1;
                            vtemp = m.VremeMerenja;
                        }
                    }
                }
            }
            return m;
        }

        // Metoda za proveru poslednjeg ažuriranja lokalno - generalno
        private Merenje PoslednjeDodavanjeLokalnoZaSve()
        {
            Merenje m = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (Tuple<Merenje, DateTime> torka in lokalnaKopija.Values)
                {                    
                    if (vtemp < torka.Item1.VremeMerenja)
                    {
                        m = torka.Item1;
                        vtemp = m.VremeMerenja;
                    }                    
                }
            }
            return m;
        }
        #endregion

        /*
        Prilikom klijentskog zahteva proxy prvo proverava da li tražana podatke ima lokalno.
        Ukoliko ima, šalje serveru zahtev u kom traži informaciju o tome kada su za dati kriterijum poslednji put sačuvani novi podaci.
        Na osnovu datog odgovora od servera, proxy zaključuje da li je potrebno da podatke ponovo povlači sa servera ili su njegove lokalne kopije up to date.
        */

        public List<Merenje> DobaviPodatkeId(int id)
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            Merenje lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();
                
                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.IdUredjaja == id)
                    {
                        if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.VremeMerenja)
                            azurno = false;
                        else
                            azurno = true;
                    }
                }
                else
                {
                    return null;    // Ako je baza prazna, nema poente da vršimo bilo kakvu pretragu
                }
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.IdUredjaja == id)
                        rezultat.Add(m.Item1);

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where idUredjaja=" + id.ToString();

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
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }

            return rezultat;
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            //Merenje rezultat = null;
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            Merenje lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.IdUredjaja == id)
                    {
                        if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.VremeMerenja)
                            azurno = false;
                        else
                            azurno = true;
                    }
                }
                else
                {
                    return null;    // Ako je baza prazna, nema poente da vršimo bilo kakvu pretragu
                }
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

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

                // Ažuriraj poslednje vreme pristupa lokalnoj kopiji
                if (rezultat != null) 
                    lokalnaKopija[rezultat.IdMerenja] = new Tuple<Merenje, DateTime>(rezultat, DateTime.Now);

                return rezultat;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vreme=(select max(vreme) from Podaci where idUredjaja=" + id.ToString() + ")";

                List<Merenje> rezultat = new List<Merenje>();
                rezultat = kanal.Citanje("Poslednji uneti podatak za traženi ID uređaja", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);

                return rezultat[0];
            }
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            Merenje lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaSve();
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {   
                    if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.VremeMerenja)
                        azurno = false;
                    else
                        azurno = true;                    
                }
                else
                {
                    return null;    // Ako je baza prazna, nema poente da vršimo bilo kakvu pretragu
                }
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                DateTime poslednjeVreme = DateTime.MinValue;

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
                }

                // Ažuriranje vremena poslednjeg pristupa lokalnim podacima
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(lokalnaKopija[m.IdMerenja].Item1, DateTime.Now);
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from (select * from Podaci order by vreme desc) group by idUredjaja";

                rezultat = kanal.Citanje("Poslednji uneti podatak za sve uređaje", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            Merenje lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.ANALOGNO_MERENJE);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VrstaMerenja == VrstaMerenja.ANALOGNO_MERENJE)
                    {
                        if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.VremeMerenja)
                            azurno = false;
                        else
                            azurno = true;
                    }
                }
                else
                {
                    return null;    // Ako je baza prazna, nema poente da vršimo bilo kakvu pretragu
                }
            }

            if (azurno)
            {
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.VrstaMerenja == VrstaMerenja.ANALOGNO_MERENJE)    
                        rezultat.Add(m.Item1);
                
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vrstaMerenja=0";

                rezultat = kanal.Citanje("Svi analogni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            Merenje lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.DIGITALNO_MERENJE);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
                    {
                        if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.VremeMerenja)
                            azurno = false;
                        else
                            azurno = true;
                    }
                }
                else
                {
                    return null;    // Ako je baza prazna, nema poente da vršimo bilo kakvu pretragu
                }
            }

            if (azurno)
            {
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.VrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
                        rezultat.Add(m.Item1);

                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vrstaMerenja=1";

                rezultat = kanal.Citanje("Svi digitalni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
            }

            return rezultat;
        }
    }
}
