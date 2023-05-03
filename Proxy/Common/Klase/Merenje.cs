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
        private int idUredjaja;
        private int idMerenja;
        private VrstaMerenja vrstaMerenja;
        private int vrednost;
        private DateTime vremeMerenja;

        // Sa obzirom da nigde nemam opciju za menjanje vremena, onda se vremeMerenja nikada neće ni menjati
        // Da sam imala opciju izmene već postojećih merenja, ovo bih obavezno ažurirala automatski svaki put

        // Propertiji
        [DataMember]
        public int IdUredjaja { get => idUredjaja; set => idUredjaja = value; }
        [DataMember]
        public int IdMerenja { get => idMerenja; set => idMerenja = value; }
        [DataMember]
        public VrstaMerenja VrstaMerenja { get => vrstaMerenja; set => vrstaMerenja = value; }
        [DataMember]
        public int Vrednost { get => vrednost; set => vrednost = value; }
        [DataMember]
        public DateTime VremeMerenja { get => vremeMerenja; set => vremeMerenja = value; }

        // Konstruktor
        public Merenje(int idMerenja, VrstaMerenja vrstaMerenja, int vrednost, DateTime vremeMerenja, int idUredjaja = -1)
        {
            this.idUredjaja = idUredjaja;
            this.idMerenja = idMerenja;
            this.vremeMerenja = vremeMerenja;
            this.vrstaMerenja = vrstaMerenja;

            if (vrstaMerenja == VrstaMerenja.DIGITALNO_MERENJE)
            {
                if (!(vrednost == 1 || vrednost == 0))
                    throw new ArgumentOutOfRangeException("Digitalno merenje prima vrednosti 0 ili 1!");
                else
                    this.vrednost = vrednost;                    
            }
            else
            {
                this.vrednost = vrednost;
            }
        }

        // Ispis
        public override string ToString()
        {
            return $"ID: {idMerenja}, Izmerena vrednost: {vrednost} ({vrstaMerenja}), vreme: {vremeMerenja}, uređaj: {idUredjaja}";
        }
    }
}
