using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Klase
{
    [DataContract]
    public class Merenje
    {
        // Polja
        private long idMerenja;
        private VrstaMerenja vrstaMerenja;
        private int vrednost;
        private DateTime vremeMerenja;

        // Propertiji
        [DataMember]
        public long IdMerenja { get => idMerenja; set => idMerenja = value; }
        [DataMember]
        public VrstaMerenja VrstaMerenja { get => vrstaMerenja; set => vrstaMerenja = value; }
        [DataMember]
        public int Vrednost { get => vrednost; set => vrednost = value; }
        [DataMember]
        public DateTime VremeMerenja { get => vremeMerenja; set => vremeMerenja = value; }

        // Konstruktor
        public Merenje(long idMerenja, VrstaMerenja vrstaMerenja, int vrednost, DateTime vremeMerenja)
        {
            this.idMerenja = idMerenja;
            this.vrstaMerenja = vrstaMerenja;
            this.vrednost = vrednost;
            this.vremeMerenja = vremeMerenja;
        }

        // Ispis
        public override string ToString()
        {
            return $"ID: {idMerenja}, Izmerena vrednost: {vrednost} ({vrstaMerenja}), vreme: {vremeMerenja}";
        }
    }
}
