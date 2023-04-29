using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class ServerServis : IServer
    {
        static readonly Server s = new Server();

        // Device šalje merenje serveru, koji to merenje upisuje u bazu podataka
        public bool Upis(Merenje m, int idUredjaja)
        {
            s.Loger.LogServer(DateTime.Now, $"Uređaj {idUredjaja} je poslao merenje. Upis u bazu podataka je započet.");

            // Upis u bazu
            // Log o uspešnosti, return true/false

            return false;
        }

        // Proksi traži da se učitaju podaci iz baze
        // Izmeni, ovo je samo za sad
        // Verovatno će se morati dodati Query kao argument
        public Merenje Citanje(string kriterijum)
        {
            s.Loger.LogServer(DateTime.Now, $"Proksi je zatražio podatke po krijerijumu: {kriterijum}. Čitanje iz baze podataka je započeto.");

            // Čitanje iz baze
            // Log o uspešnosti, return tražene vrednosti

            return null;
        }
    }
}
