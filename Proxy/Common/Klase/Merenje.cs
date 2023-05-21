using Common.Interfejsi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Klase
{
    [DataContract]
    public class Merenje : IMerenje
    {
        // Polja
        private int idUredjaja;
        private int idMerenja;
        private VrstaMerenja vrstaMerenja;
        private int vrednost;
        private DateTime vremeMerenja;

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

        // Konstruktor - za Device
        public Merenje(int idMerenja, VrstaMerenja vrstaMerenja, int vrednost, DateTime vremeMerenja, int idUredjaja)
        {
            if (idMerenja < 1 || idUredjaja < 1 || vrednost < 0 || vremeMerenja == null || vremeMerenja > DateTime.Now)
                throw new ArgumentException("Morate popuniti sva polja sa ispravnim vrednostima!");

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

        // Konstruktor -  za BazuPodataka
        [ExcludeFromCodeCoverage]
        public Merenje(Int64 idMerenja, Int64 idUredjaja , Int64 vrstaMerenja, Int64 vrednost, DateTime vreme)
        {
            try
            {
                this.idUredjaja = int.Parse(idUredjaja.ToString());
                this.idMerenja = int.Parse(idMerenja.ToString());
                this.vremeMerenja = vreme;
                this.vrstaMerenja = vrstaMerenja == 0 ? VrstaMerenja.ANALOGNO_MERENJE : VrstaMerenja.DIGITALNO_MERENJE;
                this.vrednost = int.Parse(vrednost.ToString());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        // Ispis
        [ExcludeFromCodeCoverage]
        public override string ToString()
        {
            return $"ID: {idMerenja}, Izmerena vrednost: {vrednost} ({vrstaMerenja}), vreme: {vremeMerenja}, uređaj: {idUredjaja}";
        }
    }
}
