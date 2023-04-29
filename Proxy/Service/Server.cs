using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class Server
    {
        private static int brojInstanci = 0;    // TODO singleton pattern ?
        private Log loger;
        // Dodati potrebna polja

        public static int BrojInstanci { get => brojInstanci; set => brojInstanci = value; }
        public Log Loger { get => loger; set => loger = value; }

        public Server()
        {
            if (++BrojInstanci > 1)
                throw new Exception("Ne može se pokrenuti više od jedne instance Servera!");

            loger = new Log("../../../serverLog.txt");
        }

        ~Server()
        {
            --brojInstanci;
            loger.UpisPriGasenju(DateTime.Now);
        }
    }
}
