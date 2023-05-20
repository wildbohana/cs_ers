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
    public interface IProksi
    {
        [OperationContract]
        List<Merenje> DobaviPodatkeId(int id);
        [OperationContract]
        List<Merenje> DobaviPoslednjiPodatakId(int id);
        [OperationContract]
        List<Merenje> DobaviPoslednjiPodatakSvi();
        [OperationContract]
        List<Merenje> DobaviSveAnalogne();
        [OperationContract]
        List<Merenje> DobaviSveDigitalne();
    }
}
