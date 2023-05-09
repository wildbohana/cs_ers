using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class Proksi
    {
        private static int brojInstanci = 0;
        private Log loger;

        public static int BrojInstanci { get => brojInstanci; set => brojInstanci = value; }
        public Log Loger { get => loger; set => loger = value; }

        public Proksi()
        {
            if (++brojInstanci > 1)
                throw new Exception("Ne može se pokrenuti više od jedne instance Proksija!");

            loger = new Log("../../../Logovi/proxyLog.txt");
        }

        ~Proksi()
        {
            --brojInstanci;
            loger.UpisPriGasenju(DateTime.Now);
        }
    }
}
