using Common.Klase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    [ServiceContract]
    public interface IProksi
    {
        // TODO izmeni, dopuni...

        //[OperationContract]
        //void PrihvatiZahtevOdKlijenta();
        //[OperationContract]
        //void DobaviPodatkeOdServera();
        //[OperationContract]
        //void ObrisiStarePodatke();

        [OperationContract]
        List<Merenje> DobaviPodatkeId(int id);
        [OperationContract]
        Merenje DobaviPoslednjiPodatakId(int id);
        [OperationContract]
        List<Merenje> DobaviPoslednjiPodatakSvi();
        [OperationContract]
        List<Merenje> DobaviSveAnalogne();
        [OperationContract]
        List<Merenje> DobaviSveDigitalne();
    }
}
