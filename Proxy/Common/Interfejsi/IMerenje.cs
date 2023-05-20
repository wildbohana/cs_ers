using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfejsi
{
    public interface IMerenje
    {
        int IdUredjaja { get; set; }
        int IdMerenja { get; set; }
        VrstaMerenja VrstaMerenja { get; set; }
        int Vrednost { get; set; }
        DateTime VremeMerenja { get; set; }
    }
}
