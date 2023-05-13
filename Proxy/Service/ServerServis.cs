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

            using (IDbConnection veza = new SQLiteConnection(UcitajStringZaBazu()))
            {
                int dodatih = veza.Execute("insert into Podaci (idMerenja, idUredjaja, vrstaMerenja, vrednost, vreme) " +
                    "values (@IdMerenja, @IdUredjaja, @VrstaMerenja, @Vrednost, @VremeMerenja)", m);

                // Log o uspešnosi, vraćanje bool vrednosti uređaju
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

        public List<Merenje> Citanje(string kriterijum, string query)
        {
            s.Loger.LogServer(DateTime.Now, $"Proksi je zatražio podatke po krijerijumu: <{kriterijum}>. Čitanje iz baze podataka je započeto.");

            using (IDbConnection veza = new SQLiteConnection(UcitajStringZaBazu()))
            {
                IEnumerable<Merenje> output = null;

                try
                {
                    output = veza.Query<Merenje>(query, new DynamicParameters());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                if (output.Count() > 0)
                {
                    s.Loger.LogServer(DateTime.Now, "Podaci su uspešno dobavljeni iz baze podataka.");
                    return output.ToList();
                }
                else
                {
                    s.Loger.LogServer(DateTime.Now, "Podaci nisu pronađeni u bazi podataka. Možda taj tip podataka još uvek nije upisan u bazu.");
                    return null;
                }
            }
        }

        // Za SQLite
        private static string UcitajStringZaBazu(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
