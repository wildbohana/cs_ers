using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        // Temporary
        static IServer kanal = null;

        // Permanently
        static readonly Proksi p = new Proksi();

        // idMerenja, (Merenje, VremePoslednjeIzmene)
        static Dictionary<int, Tuple<Merenje, DateTime>> lokalnaKopija = new Dictionary<int, Tuple<Merenje, DateTime>>();
        DateTime poslednjeAzuriranjeLokalno = DateTime.MinValue;

        // Metoda za proveru poslednjeg ažuriranja podataka u bazi
        // Pošto u ovom sistemu ne postoji opcija modifikovanja merenja,
        // vreme poslednjeg dodavanja je isto kao i vreme poslednjeg ažuriranja
        private DateTime PoslednjeDodavanjeUBazu()
        {
            string query = "select * from Podaci where vreme=(select max(vreme) from Podaci)";
            List<Merenje> rezultat = kanal.Citanje("Vreme poslednjeg ažuriranja baze podataka", query);

            return rezultat[0].VremeMerenja;
        }

        public List<Merenje> DobaviPodatkeId(int id)
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalno)
            {
                string query = "select * from Podaci where idUredjaja=" + id.ToString();
                poslednjeAzuriranjeLokalno = DateTime.Now;

                rezultat = kanal.Citanje("Svi podaci za traženi ID uređaja", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // TODO dodaj nove podatke u lokalnu kopiju
                foreach (Merenje m in rezultat)
                {
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
                }
            }
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.IdUredjaja == id)
                        rezultat.Add(m.Item1);
            }

            return rezultat;
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalno)
            {
                string query = "select * from Podaci where vreme=(select max(vreme) from Podaci where idUredjaja=" + id.ToString() + ")";
                poslednjeAzuriranjeLokalno = DateTime.Now;

                List<Merenje> rezultat = new List<Merenje>();
                rezultat = kanal.Citanje("Poslednji uneti podatak za traženi ID uređaja", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // TODO dodaj novi podatak u lokalnu kopiju
                // Znam da ne mora foreach, ali ajde
                foreach (Merenje m in rezultat)
                {
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
                }

                return rezultat[0];
            }
            else
            {
                DateTime poslednjeVreme = DateTime.MinValue;
                Merenje rezultat = null;

                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.IdUredjaja == id && m.Item1.VremeMerenja > poslednjeVreme)
                        rezultat = m.Item1;

                return rezultat;
            }
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();
            List<Merenje> rezultat = new List<Merenje>();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalno)
            {
                string query = "select * from (select * from Podaci order by vreme desc) group by idUredjaja";
                poslednjeAzuriranjeLokalno = DateTime.Now;

                rezultat = kanal.Citanje("Poslednji uneti podatak za sve uređaje", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // TODO dodaj nove podatke u lokalnu kopiju
                foreach (Merenje m in rezultat)
                {
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
                }
            }
            else
            {
                DateTime poslednjeVreme = DateTime.MinValue;
                rezultat = null;

                List<int> indeksi = new List<int>();
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (!indeksi.Contains(m.Item1.IdUredjaja))
                        indeksi.Add(m.Item1.IdUredjaja);

                Merenje mtemp = null;
                DateTime vtemp;

                foreach (int i in indeksi)
                {
                    vtemp = DateTime.MinValue;

                    foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                        if (m.Item1.IdUredjaja == i && m.Item1.VremeMerenja > vtemp)
                            mtemp = m.Item1;

                    rezultat.Add(mtemp);
                }
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalno)
            {
                string query = "select * from Podaci where vrstaMerenja=0";

                poslednjeAzuriranjeLokalno = DateTime.Now;

                rezultat = kanal.Citanje("Svi analogni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // TODO dodaj nove podatke u lokalnu kopiju
                foreach (Merenje m in rezultat)
                {
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
                }
            }
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.VrstaMerenja == VrstaMerenja.ANALOGNO_MERENJE)
                        rezultat.Add(m.Item1);
            }

            return rezultat;
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            List<Merenje> rezultat = new List<Merenje>();
            DateTime poslednjiPrisupBaza = PoslednjeDodavanjeUBazu();

            // Ažuriraj lokalnu kopiju po traženom kriterijumu
            if (poslednjiPrisupBaza > poslednjeAzuriranjeLokalno)
            {
                string query = "select * from Podaci where vrstaMerenja=1";

                poslednjeAzuriranjeLokalno = DateTime.Now;

                rezultat = kanal.Citanje("Svi digitalni signali", query);
                p.Loger.LogProksi(DateTime.Now, "Dobavljanje podataka sa servera");

                // TODO dodaj nove podatke u lokalnu kopiju
                foreach (Merenje m in rezultat)
                {
                    lokalnaKopija[m.IdMerenja] = new Tuple<Merenje, DateTime>(m, DateTime.Now);
                }
            }
            else
            {
                foreach (Tuple<Merenje, DateTime> m in lokalnaKopija.Values)
                    if (m.Item1.VrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
                        rezultat.Add(m.Item1);
            }

            return rezultat;
        }
    }
}
