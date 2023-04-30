using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Klase
{
    public class Log
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
        public void LogUredjaj(Merenje m, long id)
        {
            string s = $"UREDJAJ: {id}, MERENJE {m}";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region PROKSI
        public void LogProksi(DateTime dt, string dogadjaj)
        {
            string s = $"{dt}\t{dogadjaj}\n";
            UpisUFajl(s);
        }
        #endregion

        #region SERVER
        public void LogServer(DateTime dt, string dogadjaj)
        {
            string s = $"{dt}\t{dogadjaj}\n";
            UpisUFajl(s);
        }
        #endregion

        #region OSTALO
        public void OcistiLog(string fajl)
        {
            if (File.Exists(fajl))
                File.Delete(fajl);
        }

        public void UpisUFajl(string tekst)
        {
            using (StreamWriter sw = File.AppendText(imeFajla))
            {
                sw.Write(tekst);
            }
        }

        public void UpisPriGasenju(DateTime dt)
        {
            string s = "\n" + dt.ToString() + " Gašenje aplikacije...\n\n";
            s += "..............................................................\n";
            UpisUFajl(s);
        }
        #endregion
    }
}
