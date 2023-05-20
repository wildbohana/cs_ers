using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfejsi
{
    public interface IKlijent
    {
        void RadKlijenta(IProksi kanal);
        int Meni();
        int UnosId();
        void Opcija1(IProksi kanal, int trazeni);
        void Opcija2(IProksi kanal, int trazeni);
        void Opcija3(IProksi kanal);
        void Opcija4(IProksi kanal);
        void Opcija5(IProksi kanal);
    }
}
