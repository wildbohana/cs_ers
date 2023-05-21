using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfejsi
{
    public interface ILog
    {
        void LogUredjaj(Merenje m);
        void LogProksi(DateTime dt, string dogadjaj);
        void LogServer(DateTime dt, string dogadjaj);

        void UpisUFajl(string tekst);
        void UpisPriGasenju();
    }
}
