using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        public static IServer kanal = null;
        static readonly Proksi p = new Proksi();

        // idMerenja, (Merenje, VremePoslednjeIzmene)
        static Dictionary<int, MerenjeProksi> lokalnaKopija = new Dictionary<int, MerenjeProksi>();

        private DateTime poslednjeAzuriranjeAnalogno = DateTime.MinValue;
        private DateTime poslednjeAzuriranjeDigitalno = DateTime.MinValue;
        private DateTime poslednjeAzuriranjeZaSve = DateTime.MinValue;

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

        private Merenje PoslednjeDodavanjeUBazuZaTrazeniId(int id)
        {
            // select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where idUredjaja=29021823))
            string query = "select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where idUredjaja=" + id + "))";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0];
            else
                return null;      // Baza podataka je skroz prazna u ovom slučaju
        }

        private Merenje PoslednjeDodavanjeUBazuZaTrazenuVrstu(VrstaMerenja vr)
        {
            // 0 za analogne, 1 za digitalne
            int vrsta = (vr == VrstaMerenja.ANALOGNO_MERENJE) ? 0 : 1;

            // select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where vrstaMerenja=0)) 
            string query = "select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where vrstaMerenja=" + vrsta + "))";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0];
            else
                return null;      // Baza podataka je skroz prazna u ovom slučaju
        }
        #endregion

        #region LOKALNO POSLEDNJI
        // Metoda za proveru poslednjeg ažuriranja lokalno za traženi ID
        private MerenjeProksi PoslednjeDodavanjeLokalnoZaTrazeniId(int id)
        {
            MerenjeProksi mp = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (MerenjeProksi mtemp in lokalnaKopija.Values)
                {
                    if (mtemp.Merenje.IdUredjaja == id)
                    {
                        if (vtemp < mtemp.Merenje.VremeMerenja)
                        {
                            mp = mtemp;
                            vtemp = mp.Merenje.VremeMerenja;
                        }
                    }
                }
            }
            return mp;
        }

        // Metoda za proveru poslednjeg ažuriranja lokalno za traženu vrstu merenja
        private MerenjeProksi PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja vr)
        {
            MerenjeProksi mp = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (MerenjeProksi mtemp in lokalnaKopija.Values)
                {
                    if (mtemp.Merenje.VrstaMerenja == vr)
                    {
                        if (vtemp < mtemp.Merenje.VremeMerenja)
                        {
                            mp = mtemp;
                            vtemp = mp.Merenje.VremeMerenja;
                        }
                    }
                }
            }
            return mp;
        }

        // Metoda za proveru poslednjeg ažuriranja lokalno - generalno
        private MerenjeProksi PoslednjeDodavanjeLokalnoZaSve()
        {
            MerenjeProksi mp = null;
            DateTime vtemp = DateTime.MinValue;

            if (lokalnaKopija.Count > 0)
            {
                foreach (MerenjeProksi mtemp in lokalnaKopija.Values)
                {                    
                    if (vtemp < mtemp.Merenje.VremeMerenja)
                    {
                        mp = mtemp;
                        vtemp = mp.Merenje.VremeMerenja;
                    }                    
                }
            }
            return mp;
        }
        #endregion

        #region BRISANJE STARIH MERENJA
        public static void ProveraStarihVremena()
        {
            while (true)
            {
                Task.Delay(CitanjeVremenaIzKonfiguracijeProvera()).Wait();
                p.Loger.LogProksi(DateTime.Now, "Provera da li postoje merenja kojima nije pristupano duže vreme u lokalnoj kopiji.");
                ObrisiStaraMerenja();
            }
        }
        
        private static void ObrisiStaraMerenja()
        {
            int cnt = 0;

            // Izvlačenje ID merenja svih onih koja se nalaze u lokalnoj kopiji
            List<int> lista = new List<int>();
            foreach (MerenjeProksi mp in lokalnaKopija.Values)
                if (!lista.Contains(mp.Merenje.IdMerenja))
                    lista.Add(mp.Merenje.IdMerenja);

            if (lokalnaKopija.Count > 0)
            {
                foreach (int i in lista)
                {
                    if (DateTime.Now - lokalnaKopija[i].PoslednjiPristup > CitanjeVremenaIzKonfiguracijeBrisanje())
                    {
                        try
                        {
                            lokalnaKopija.Remove(i);
                        }
                        catch (Exception e)
                        {
                            p.Loger.LogProksi(DateTime.Now, e.Message);
                        }

                        cnt++;
                    }
                }
            }
            p.Loger.LogProksi(DateTime.Now, $"Iz lokalne kopije je obrisano {cnt} zastarelih merenja.");
        }

        private static TimeSpan CitanjeVremenaIzKonfiguracijeProvera()
        {
            int sati = int.Parse(ConfigurationManager.AppSettings["proveraSati"]);
            int minute = int.Parse(ConfigurationManager.AppSettings["proveraMinute"]);
            int sekunde = int.Parse(ConfigurationManager.AppSettings["proveraSekunde"]);

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }

        private static TimeSpan CitanjeVremenaIzKonfiguracijeBrisanje()
        {
            int sati = int.Parse(ConfigurationManager.AppSettings["brisanjeSati"]);
            int minute = int.Parse(ConfigurationManager.AppSettings["brisanjeMinute"]);
            int sekunde = int.Parse(ConfigurationManager.AppSettings["brisanjeSekunde"]);

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }
        #endregion

        public List<Merenje> DobaviPodatkeId(int id)
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            MerenjeProksi lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazeniId(id);
                
                // TODO fix
                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
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
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - svi podaci za traženi ID uređaja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    if (mp.Merenje.IdUredjaja == id)
                        rezultat.Add(mp.Merenje);

                // Ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where idUredjaja=" + id.ToString();

                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - svi podaci za traženi ID uređaja");
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
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new MerenjeProksi { Merenje = m, PoslednjeAzuriranje = poslednjeAzuriranjeZaSve, PoslednjiPristup = poslednjeAzuriranjeZaSve };
            }

            return rezultat;
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            Merenje rezultat = null;
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            MerenjeProksi lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazeniId(id);

                // TODO fix

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
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
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji podatak za traženi ID uređaja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                DateTime poslednjeVreme = DateTime.MinValue;

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                {
                    if (mp.Merenje.IdUredjaja == id && mp.Merenje.VremeMerenja > poslednjeVreme)
                    {
                        rezultat = mp.Merenje;
                        poslednjeVreme = mp.Merenje.VremeMerenja;
                    }
                }

                // Ažuriraj poslednje vreme pristupa lokalnoj kopiji
                if (rezultat != null) lokalnaKopija[rezultat.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vreme=(select max(vreme) from Podaci where idUredjaja=" + id.ToString() + ")";

                List<Merenje> rezultatOdServera = new List<Merenje>();
                rezultatOdServera = kanal.Citanje("Poslednji uneti podatak za traženi ID uređaja", query);
                
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji podatak za traženi ID uređaja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultatOdServera != null)
                {
                    rezultat = rezultatOdServera[0];
                    lokalnaKopija[rezultat.IdMerenja] = new MerenjeProksi 
                    { 
                        Merenje = rezultat, 
                        PoslednjeAzuriranje = DateTime.Now, 
                        PoslednjiPristup = DateTime.Now 
                    };
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            MerenjeProksi lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaSve();
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazu();

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {   
                    if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
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
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji podatak za sve uređaje");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                // Izdvajanje indeksa iz kolekcije
                List<int> indeksi = new List<int>();
                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    if (!indeksi.Contains(mp.Merenje.IdUredjaja))
                        indeksi.Add(mp.Merenje.IdUredjaja);

                Merenje mtemp = null;
                DateTime vtemp;

                // Za svaki indeks tražimo podatak sa najnovijim vremenom
                foreach (int i in indeksi)
                {
                    vtemp = DateTime.MinValue;

                    foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    {
                        if (mp.Merenje.IdUredjaja == i && mp.Merenje.VremeMerenja > vtemp)
                        {
                            mtemp = mp.Merenje;
                            vtemp = mp.Merenje.VremeMerenja;
                        }
                    }

                    rezultat.Add(mtemp);
                }

                // Ažuriranje vremena poslednjeg pristupa lokalnim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from (select * from Podaci order by vreme desc) group by idUredjaja";

                rezultat = kanal.Citanje("Poslednji uneti podatak za sve uređaje", query);
                
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji podatak za sve uređaje");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");
                poslednjeAzuriranjeZaSve = DateTime.Now;

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new MerenjeProksi
                        { 
                            Merenje = m, 
                            PoslednjiPristup = poslednjeAzuriranjeZaSve, 
                            PoslednjeAzuriranje = poslednjeAzuriranjeZaSve 
                        };
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            MerenjeProksi lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.ANALOGNO_MERENJE);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazenuVrstu(VrstaMerenja.ANALOGNO_MERENJE);

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VremeMerenja > poslednjeAzuriranjeAnalogno)
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
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva analogna merenja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    if (mp.Merenje.VrstaMerenja == VrstaMerenja.ANALOGNO_MERENJE)    
                        rezultat.Add(mp.Merenje);
                
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vrstaMerenja=0";

                rezultat = kanal.Citanje("Svi analogni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva analogna merenja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                poslednjeAzuriranjeAnalogno = DateTime.Now;

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new MerenjeProksi 
                        { 
                            Merenje = m, 
                            PoslednjiPristup = DateTime.Now, 
                            PoslednjeAzuriranje = DateTime.Now 
                        };
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            MerenjeProksi lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.DIGITALNO_MERENJE);
            if (lokalnoPoslednji != null)
            {
                Merenje bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazenuVrstu(VrstaMerenja.DIGITALNO_MERENJE);

                // Ako baza nije prazna
                if (bazaPoslednji != null)
                {
                    if (bazaPoslednji.VremeMerenja > poslednjeAzuriranjeDigitalno)
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
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva digitalna merenja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    if (mp.Merenje.VrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
                        rezultat.Add(mp.Merenje);

                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                string query = "select * from Podaci where vrstaMerenja=1";

                rezultat = kanal.Citanje("Svi digitalni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva digitalna merenja");
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                poslednjeAzuriranjeDigitalno = DateTime.Now;

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat != null)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new MerenjeProksi 
                        { 
                            Merenje = m, 
                            PoslednjiPristup = DateTime.Now, 
                            PoslednjeAzuriranje = DateTime.Now 
                        };
            }

            return rezultat;
        }        
    }
}
