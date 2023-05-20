using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfejsi
{
    public interface IUredjaj
    {
        void RadUredjaja(IServer kanal);
        Merenje Izmeri();
        void PosaljiMerenja(IServer kanal, Merenje m);
    }
}
