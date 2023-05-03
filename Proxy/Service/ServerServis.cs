using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;

namespace Service
{
    public class ServerServis : IServer
    {
        static readonly Server s = new Server();

        public bool Upis(Merenje m)
        {
            s.Loger.LogServer(DateTime.Now, $"Uređaj {m.IdUredjaja} je poslao merenje {m.IdMerenja}. Upis u bazu podataka je započet.");

            using (IDbConnection veza = new SQLiteConnection(LoadConnectionString()))
            {
                int dodatih = veza.Execute("insert into Podaci (idMerenja, idUredjaja, vrstaMerenja, vrednost, vreme) " +
                    "values (@IdMerenja, @IdUredjaja, @VrstaMerenja, @Vrednost, @VremeMerenja)", m);

                // Log o uspešnosi, vraćanje bool vrendosti uređaju
                if (dodatih > 0)
                {
                    s.Loger.LogServer(DateTime.Now, $"Podatak {m.IdMerenja} je uspešno upisan u bazu podataka.");
                    return true;
                }
                else
                {
                    s.Loger.LogServer(DateTime.Now, $"GREŠKA! Podatak {m.IdMerenja} nije upisan u bazu podataka.");
                    return false;
                }
            }
        }

        // TODO
        public List<Merenje> Citanje(string kriterijum, string query)
        {
            s.Loger.LogServer(DateTime.Now, $"Proksi je zatražio podatke po krijerijumu: {kriterijum}. Čitanje iz baze podataka je započeto.");

            // Čitanje iz baze
            // Log o uspešnosti, return tražene vrednosti

            using (IDbConnection veza = new SQLiteConnection(LoadConnectionString()))
            {
                var output = veza.Query<Merenje>("select * from Podaci", new DynamicParameters());
                return output.ToList();
            }
        }

        // Za SQLite
        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
