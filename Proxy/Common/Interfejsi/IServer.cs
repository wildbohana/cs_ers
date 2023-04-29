using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfejsi
{
    [ServiceContract]
    public interface IServer
    {
        // TODO izmeniti, ovo su samo placeholderi za sad
        [OperationContract]
        bool Upis(Merenje m, int idUredjaja);

        // Jedno ili više (List)
        [OperationContract]
        Merenje Citanje(string kriterijum);
    }
}
