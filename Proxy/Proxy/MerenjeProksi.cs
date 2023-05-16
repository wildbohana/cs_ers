using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class MerenjeProksi
    {
        public Merenje Merenje { get; set; }
        public DateTime PoslednjeAzuriranje { get; set; }
        public DateTime PoslednjiPristup { get; set; }

        public MerenjeProksi() { }
    }
}
