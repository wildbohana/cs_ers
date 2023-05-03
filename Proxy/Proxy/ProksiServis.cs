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

        static List<Merenje> lokalnaKopija = new List<Merenje>();

        DateTime poslednjeAzuriranjeLokalno = DateTime.MinValue;
        DateTime poslednjiPristupPodacima = DateTime.MinValue;  // ne valja, već znam lol

        public List<Merenje> DobaviPodatkeId(int id)
        {
            string query = "";

            throw new NotImplementedException();
        }

        public Merenje DobaviPoslednjiPodatakId(int id)
        {
            string query = "";

            throw new NotImplementedException();
        }

        public List<Merenje> DobaviPoslednjiPodatakSvi()
        {
            string query = "";

            throw new NotImplementedException();
        }

        public List<Merenje> DobaviSveAnalogne()
        {
            string query = "";

            throw new NotImplementedException();
        }

        public List<Merenje> DobaviSveDigitalne()
        {
            string query = "";

            throw new NotImplementedException();
        }
    }
}
