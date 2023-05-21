using Common.Interfejsi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Klase
{
    public class Log : ILog
    {
        private string imeFajla;
        public string ImeFajla { get => imeFajla; set => imeFajla = value; }

        public Log(string imeFajla)
        {
            if (imeFajla == null)
                throw new ArgumentNullException("Morate uneti ime fajla.");
            else if (imeFajla.Trim() == "")
                throw new ArgumentException("Naziv fajla mora sadržati karaktere!");
            else
                this.imeFajla = imeFajla;
        }

        #region UREĐAJ
        public void LogUredjaj(Merenje m)
        {
            if (m == null)
            {
                throw new ArgumentException("Za upis loga je neophodno uneti Merenje.");
            }

            string s = $"UREDJAJ: {m.IdUredjaja}, MERENJE {m}";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region PROKSI
        public void LogProksi(DateTime dt, string dogadjaj)
        {
            if (dt == null || dogadjaj == null)
            {
                throw new ArgumentException("Za upis loga je neophodno uneti Datum i Dogadjaj.");
            }

            string s = $"{dt}\t{dogadjaj}\n";
            UpisUFajl(s);
        }
        #endregion

        #region SERVER
        public void LogServer(DateTime dt, string dogadjaj)
        {
            if (dt == null || dogadjaj == null)
            {
                throw new ArgumentException("Za upis loga je neophodno uneti Datum i Dogadjaj.");
            }

            string s = $"{dt}\t{dogadjaj}\n";
            UpisUFajl(s);
        }
        #endregion

        #region OSTALO
        [ExcludeFromCodeCoverage]
        public void OcistiLog(string fajl)
        {
            if (File.Exists(fajl))
                File.Delete(fajl);
        }

        [ExcludeFromCodeCoverage]
        public void UpisUFajl(string tekst)
        {
            using (StreamWriter sw = File.AppendText(imeFajla))
            {
                sw.Write(tekst);
            }
        }

        [ExcludeFromCodeCoverage]
        public void UpisPriGasenju()
        {
            DateTime dt = DateTime.Now;
            string s = dt.ToString() + "\tGašenje aplikacije...\n";
            s += "..............................................................\n";
            UpisUFajl(s);
        }
        #endregion
    }
}
