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
        [OperationContract]
        bool Upis(Merenje m);

        [OperationContract]
        List<Merenje> Citanje(string kriterijum, string query);
    }
}
