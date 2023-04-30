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
        static Server s = new Server();        
        
        public bool Upis(Merenje m, int idUredjaja)
        {
            s.Loger.LogServer(DateTime.Now, $"Uređaj {idUredjaja} je poslao merenje. Upis u bazu podataka je započet.");

            // Upis u bazu
            // Log o uspešnosti, return true/false

            return false;
        }

        public List<Merenje> Citanje(string kriterijum, string query)
        {
            s.Loger.LogServer(DateTime.Now, $"Proksi je zatražio podatke po krijerijumu: {kriterijum}. Čitanje iz baze podataka je započeto.");

            // Čitanje iz baze
            // Log o uspešnosti, return tražene vrednosti

            return null;
        }
    }
}
