using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy
{
    public class ProksiServis : IProksi
    {
        static readonly Proksi p = new Proksi();

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
