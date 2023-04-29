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
        // Polja
        private string imeFajla;

        // Propertiji
        public string ImeFajla { get => imeFajla; set => imeFajla = value; }

        // Konstruktor
        public Log(string imeFajla)
        {
            if (imeFajla == null)
                throw new ArgumentNullException("Morate uneti ime fajla.");
            else
                this.imeFajla = imeFajla;
        }

        #region UREĐAJ
        // Za uređaj
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
        public void UpisUFajl(string tekst)
        {
            using (StreamWriter sw = File.AppendText(imeFajla))
            {
                sw.Write(tekst);
            }
        }

        public void UpisPriGasenju(DateTime dt)
        {
            if (dt > DateTime.Now)
            {
                throw new ArgumentException("Datum je pogrešan");
            }

            string s = "\n" + dt.ToString() + " Gašenje aplikacije...\n\n";
            s += "..............................................................\n";
            UpisUFajl(s);
        }
        #endregion
    }
}
