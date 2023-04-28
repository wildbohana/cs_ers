using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Proksi loguje sve događaje u txt fajl

namespace Proxy
{
    public class Proksi : IProksi
    {
        public List<Merenje> DobaviPodatkeId(int id)
        {
            throw new NotImplementedException();
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            throw new NotImplementedException();
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            throw new NotImplementedException();
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            throw new NotImplementedException();
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            throw new NotImplementedException();
        }
    }
}
