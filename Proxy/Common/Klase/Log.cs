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
        private StreamWriter streamWriter;

        // Propertiji
        public string ImeFajla { get => imeFajla; set => imeFajla = value; }
        public StreamWriter StreamWriter { get => streamWriter; set => streamWriter = value; }

        // Konstruktor
        public Log(string imeFajla)
        {
            if (imeFajla == null)
                throw new ArgumentNullException("Morate uneti ime fajla.");
            else if (!imeFajla.Contains(@"\"))
                throw new ArgumentException("Pogrešan format stringa.");
            else
                this.imeFajla = imeFajla;
        }

        #region UREĐAJ
        // Za uređaj
        public void UpisUredjaj(Merenje m, long id)
        {
            string s = $"UREDJAJ: {id}, MERENJE {m}";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region PROKSI
        // TODO koriguj
        public void UpisProksiZahtevKlijent()
        {
            string s = DateTime.Now.ToString() + "\tProksi je primio zahtev od klijenta.";
            s += "\n";
            UpisUFajl(s);
        }

        public void UpisProksiOdgovorKlijent()
        {
            string s = DateTime.Now.ToString() + "\tProksi je poslao odgovor klijentu.";
            s += "\n";
            UpisUFajl(s);
        }

        public void UpisProksiZahtevServer()
        {
            string s = DateTime.Now.ToString() + "\tProksi je poslao zahtev serveru.";
            s += "\n";
            UpisUFajl(s);
        }

        public void UpisProksiOdgovorServer()
        {
            string s = DateTime.Now.ToString() + "\tProksi je primio odgovor od servera.";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region GREŠKA
        public void UpisGreska()
        {
            string s = DateTime.Now.ToString() + "\tDošlo je do greške.";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region BAZA
        public void CitanjeIzBaze(DateTime dt)
        {
            string s = dt.ToString() + "\tCitanje podataka iz baze (proxy).";
            s += "\n";
            UpisUFajl(s);
        }

        public void UpisUBazu(DateTime dt)
        {
            string s = dt.ToString() + "\tUpis podataka u bazu (device).";
            s += "\n";
            UpisUFajl(s);
        }
        #endregion

        #region OSTALO
        public void UpisUFajl(string tekst)
        {
            using (StreamWriter = new StreamWriter(imeFajla, true))
            {
                StreamWriter.WriteLine(tekst);
            }
        }

        public void UpisPriGasenju(DateTime dt)
        {
            if (dt > DateTime.Now)
            {
                throw new ArgumentException("Datum je pogresan");
            }

            string s = "\n" + dt.ToString() + " Gašenje aplikacije...\n\n";
            s += "..............................................................\n";
            UpisUFajl(s);
        }
        #endregion
    }
}
