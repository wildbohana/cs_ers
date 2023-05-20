using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        private IServer kanal;
        public IServer Kanal { get => kanal; set => kanal = value; }

        #region SPAJANJE SA SERVEROM
        private IServer KonekcijaServer()
        {
            try
            {
                string adresa = "net.tcp://localhost:8001/Server";
                ChannelFactory<IServer> cf = new ChannelFactory<IServer>(new NetTcpBinding(), new EndpointAddress(adresa));
                IServer kanal = cf.CreateChannel();

                Console.WriteLine("Uspešno spajanje Proksija na Server sa bazom podataka.");
                return kanal;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return null;
        }
        #endregion

        // Polja
        private Log loger;

        private DateTime poslednjeAzuriranjeAnalogno = DateTime.MinValue;
        private DateTime poslednjeAzuriranjeDigitalno = DateTime.MinValue;

        private Dictionary<int, MerenjeProksi> lokalnaKopija;       // int - idMerenja
        private Merenje bazaPoslednji;
        private MerenjeProksi lokalnoPoslednji;

        public Dictionary<int, MerenjeProksi> LokalnaKopija { get => lokalnaKopija; set => lokalnaKopija = value; }
        public Merenje BazaPoslednji { get => bazaPoslednji; set => bazaPoslednji = value; }
        public MerenjeProksi LokalnoPoslednji { get => lokalnoPoslednji; set => lokalnoPoslednji = value; }

        public ProksiServis()
        {
            loger = new Log("../../../Logovi/proxyLog.txt");
            lokalnaKopija = new Dictionary<int, MerenjeProksi>();
            kanal = KonekcijaServer();
            
            Task.Factory.StartNew(() => ProveraStarihVremena());
        }

        ~ProksiServis()
        {
            loger.UpisPriGasenju();
        }

        #region BAZA POSLEDNJI
        // Metoda za proveru poslednjeg ažuriranja podataka u bazi
        // Vreme poslednjeg dodavanja == vreme poslednjeg ažuriranja (jer se merenja ne mogu modifikovati)
        [ExcludeFromCodeCoverage]
        private Merenje PoslednjeDodavanjeUBazu()
        {
            string query = "select * from Podaci where vreme=(select max(vreme) from Podaci)";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0];
            else
                return null;      // Baza podataka je skroz prazna u ovom slučaju
        }

        [ExcludeFromCodeCoverage]
        private Merenje PoslednjeDodavanjeUBazuZaTrazeniId(int id)
        {
            // select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where idUredjaja=29021823))
            string query = "select * from Podaci where vreme=(select max(vreme) from (select * from Podaci where idUredjaja=" + id + "))";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            if (rezultat.Count > 0)
                return rezultat[0];
            else
                return null;      // Baza podataka nema te podatke u ovom slučaju
        }

        [ExcludeFromCodeCoverage]
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
                return null;      // Baza podataka nema te podatke u ovom slučaju
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
        [ExcludeFromCodeCoverage]
        private void ProveraStarihVremena()
        {
            while (true)
            {
                Task.Delay(CitanjeVremenaIzKonfiguracijeProvera()).Wait();
                loger.LogProksi(DateTime.Now, "Provera da li postoje merenja kojima nije pristupano duže vreme u lokalnoj kopiji.");
                ObrisiStaraMerenja();
            }
        }
        
        private void ObrisiStaraMerenja()
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
                            loger.LogProksi(DateTime.Now, e.Message);
                        }

                        cnt++;
                    }
                }
            }
            loger.LogProksi(DateTime.Now, $"Iz lokalne kopije je obrisano {cnt} zastarelih merenja.");
        }

        [ExcludeFromCodeCoverage]
        private TimeSpan CitanjeVremenaIzKonfiguracijeProvera()
        {
            int sati = int.Parse(ConfigurationManager.AppSettings["proveraSati"]);
            int minute = int.Parse(ConfigurationManager.AppSettings["proveraMinute"]);
            int sekunde = int.Parse(ConfigurationManager.AppSettings["proveraSekunde"]);

            TimeSpan vreme = TimeSpan.FromHours(sati) + TimeSpan.FromMinutes(minute) + TimeSpan.FromSeconds(sekunde);
            return vreme;
        }

        [ExcludeFromCodeCoverage]
        private TimeSpan CitanjeVremenaIzKonfiguracijeBrisanje()
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
            lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazeniId(id);

            if (bazaPoslednji == null)
            {
                return null;
            }

            if (lokalnaKopija != null)
            {
                if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
                    azurno = false;
                else
                    azurno = true;
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                loger.LogProksi(DateTime.Now, $"Kriterijum pretrage - svi podaci za traženi ID uređaja [{id}].");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                    if (mp.Merenje.IdUredjaja == id)
                        rezultat.Add(mp.Merenje);

                // Ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                loger.LogProksi(DateTime.Now, $"Kriterijum pretrage - svi podaci za traženi ID uređaja [{id}].");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                string query = "select * from Podaci where idUredjaja=" + id.ToString();
                try
                {
                    rezultat = kanal.Citanje($"Svi podaci za traženi ID uređaja [{id}].", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                // Smeštanje podataka u lokalnu kopiju, ažuriranje vremena poslednjeg pristupa tim podacima
                if (rezultat.Count > 0)
                    foreach (Merenje m in rezultat)
                        lokalnaKopija[m.IdMerenja] = new MerenjeProksi { Merenje = m, PoslednjeAzuriranje = DateTime.Now, PoslednjiPristup = DateTime.Now };
            }

            return rezultat;
        }

        public List<Merenje> DobaviPoslednjiPodatakId(int id)
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazeniId(id);
            bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazeniId(id);

            if (bazaPoslednji == null)
            {
                return null;
            }

            if (lokalnoPoslednji != null)
            {
                if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
                    azurno = false;
                else
                    azurno = true;   
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                loger.LogProksi(DateTime.Now, $"Kriterijum pretrage - poslednji podatak za traženi ID uređaja [{id}].");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

                DateTime poslednjeVreme = DateTime.MinValue;
                Merenje poslednje = null;

                foreach (MerenjeProksi mp in lokalnaKopija.Values)
                {
                    if (mp.Merenje.IdUredjaja == id && mp.Merenje.VremeMerenja > poslednjeVreme)
                    {
                        poslednje = mp.Merenje;
                        poslednjeVreme = mp.Merenje.VremeMerenja;
                    }
                }

                // Ažuriraj poslednje vreme pristupa lokalnoj kopiji
                if (poslednje != null) 
                    lokalnaKopija[poslednje.IdMerenja].PoslednjiPristup = DateTime.Now;
            }
            else
            {
                // Dobavi merenja iz baze
                loger.LogProksi(DateTime.Now, $"Kriterijum pretrage - poslednji podatak za traženi ID uređaja [{id}].");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                string query = "select * from Podaci where vreme=(select max(vreme) from Podaci where idUredjaja=" + id + ")";
                try
                {
                    rezultat = kanal.Citanje($"Poslednji uneti podatak za traženi ID uređaja [{id}].", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaSve();
            bazaPoslednji = PoslednjeDodavanjeUBazu();

            if (bazaPoslednji == null)
            {
                return null;
            }

            if (lokalnoPoslednji != null)
            {
                if (bazaPoslednji.VremeMerenja > lokalnoPoslednji.Merenje.VremeMerenja)
                    azurno = false;
                else
                    azurno = true;
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji uneti podatak za sve uređaje.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

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
                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - poslednji uneti podatak za sve uređaje.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera.");

                string query = "select * from (select * from Podaci order by vreme desc) group by idUredjaja";
                try
                {
                    rezultat = kanal.Citanje("Poslednji uneti podatak za sve uređaje", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            bool azurno = false;

            // Provera ažurnosti podataka u lokalnoj kopiji
            lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.ANALOGNO_MERENJE);
            bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazenuVrstu(VrstaMerenja.ANALOGNO_MERENJE);

            if (bazaPoslednji == null)
            {
                return null;
            }

            if (lokalnoPoslednji != null)
            {
                if (bazaPoslednji.VremeMerenja > poslednjeAzuriranjeAnalogno)
                    azurno = false;
                else
                    azurno = true;
            }

            if (azurno)
            {
                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva analogna merenja.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

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
                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva analogna merenja.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera.");

                string query = "select * from Podaci where vrstaMerenja=0";
                try
                {
                    rezultat = kanal.Citanje("Svi analogni signali", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }                
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
            lokalnoPoslednji = PoslednjeDodavanjeLokalnoZaTrazenuVrstu(VrstaMerenja.DIGITALNO_MERENJE);
            bazaPoslednji = PoslednjeDodavanjeUBazuZaTrazenuVrstu(VrstaMerenja.DIGITALNO_MERENJE);

            if (bazaPoslednji == null)
            {
                return null;
            }

            if (lokalnoPoslednji != null)
            {
                if (bazaPoslednji.VremeMerenja > poslednjeAzuriranjeDigitalno)
                    azurno = false;
                else
                    azurno = true;
            }

            if (azurno)
            {
                // Dobavi merenja lokalno
                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva digitalna merenja.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka iz lokalne kopije.");

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

                loger.LogProksi(DateTime.Now, "Kriterijum pretrage - sva digitalna merenja.");
                loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera.");

                string query = "select * from Podaci where vrstaMerenja=1";
                try
                {
                    rezultat = kanal.Citanje("Svi digitalni signali", query);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
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
