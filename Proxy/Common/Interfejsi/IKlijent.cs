using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Moq, NUnit, NUnit3TestAdapter

namespace Common.Interfejsi
{
    public interface IKlijent
    {
        void RadKlijenta(IProksi kanal);
    }
}
