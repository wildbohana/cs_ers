using Common;
using Common.Interfejsi;
using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Server loguje sve događaje u txt fajl

namespace Service
{
    public class Server : IServer
    {
        // Device šalje merenje serveru
        public bool UpisUBazu(Merenje m, int idUredjaja)
        {
            // Upis u bazu
            // Log

            return false;
        }

        public Merenje CitanjeIzBaze()
        {
            throw new NotImplementedException();
        }

    }
}
