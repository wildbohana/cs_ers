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
using System.Diagnostics.CodeAnalysis;

namespace Service
{
    public class ServerServis : IServer
    {
        private Log loger;

        [ExcludeFromCodeCoverage]
        public ServerServis()
        {
            loger = new Log("../../../Logovi/serverLog.txt");
        }

        [ExcludeFromCodeCoverage]
        ~ServerServis()
        {
            loger.UpisPriGasenju();
        }

        public bool Upis(Merenje m)
        {
            loger.LogServer(DateTime.Now, $"Uređaj {m.IdUredjaja} je poslao merenje {m.IdMerenja}. Upis u bazu podataka je započet.");

            using (IDbConnection veza = new SQLiteConnection(UcitajStringZaBazu()))
            {
                int dodatih = veza.Execute("insert into Podaci (idMerenja, idUredjaja, vrstaMerenja, vrednost, vreme) " +
                    "values (@IdMerenja, @IdUredjaja, @VrstaMerenja, @Vrednost, @VremeMerenja)", m);

                // Log o uspešnosi, vraćanje bool vrednosti uređaju
                if (dodatih > 0)
                {
                    loger.LogServer(DateTime.Now, $"Podatak {m.IdMerenja} je uspešno upisan u bazu podataka.");
                    return true;
                }
                else
                {
                    loger.LogServer(DateTime.Now, $"GREŠKA! Podatak {m.IdMerenja} nije upisan u bazu podataka.");
                    return false;
                }
            }
        }

        public List<Merenje> Citanje(string kriterijum, string query)
        {
            loger.LogServer(DateTime.Now, $"Proksi je zatražio podatke po krijerijumu: <{kriterijum}>. Čitanje iz baze podataka je započeto.");

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
                    loger.LogServer(DateTime.Now, "Podaci su uspešno dobavljeni iz baze podataka.");
                    return output.ToList();
                }
                else
                {
                    loger.LogServer(DateTime.Now, "Podaci nisu pronađeni u bazi podataka. Možda taj tip podataka još uvek nije upisan u bazu.");
                    return new List<Merenje>();
                }
            }
        }

        // Za SQLite
        private string UcitajStringZaBazu(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
