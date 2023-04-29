using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// NE ZABORAVI - PROKSI ŠALJE ZAHTEV SERVERU I NJENOJ BAZI; DAKLE PROKSI PIŠE QUERY!!!
// Lokalna kopija podataka je In-Memory???

namespace Proxy
{
    public class Proksi
    {
        private static int brojInstanci = 0;    // TODO singleton pattern
        private Log loger;
        // Dodati polja po potrebi

        public static int BrojInstanci { get => brojInstanci; set => brojInstanci = value; }
        public Log Loger { get => loger; set => loger = value; }

        public Proksi()
        {
            ++brojInstanci;
            loger = new Log("../../../proxyLog.txt");
        }

        ~Proksi()
        {
            loger.UpisPriGasenju(DateTime.Now);
        }
    }
}
