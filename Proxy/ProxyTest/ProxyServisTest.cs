using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Common.Klase;
using Common.Interfejsi;

namespace ProxyTest
{
    [TestFixture]
    public class ProxyServisTest
    {
        private Mock<IProksi> ps;

        public ProxyServisTest()
        {
            SetUp();
        }

        public void SetUp()
        {
            ps = new Mock<IProksi>();
        }

        #region PRAZNA BAZA
        [Test]
        [TestCase(123)]
        public void TraziSveId_PraznaBaza_VracaNull(int trazeni)
        {
            List<Merenje> merenja = ps.Object.DobaviPodatkeId(trazeni);
            Assert.AreEqual(merenja, null);
        }

        [Test]
        [TestCase(123)]
        public void TraziPoslednjiId_PraznaBaza_VracaNull(int trazeni)
        {
            List<Merenje> merenja = ps.Object.DobaviPoslednjiPodatakId(trazeni);
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TraziPoslednjiSvi_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = ps.Object.DobaviPoslednjiPodatakSvi();
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TrazniAnalogne_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = ps.Object.DobaviSveAnalogne();
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TraziDigitalne_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = ps.Object.DobaviSveDigitalne();
            Assert.AreEqual(merenja, null);
        }
        #endregion

        #region NEMA TRAZENOG
        // TODO
        [Test]
        [TestCase(123)]
        [Ignore("Not ready for primetime")]
        public void Opcija1Test_NemaTrazene_VracaPraznuListu(int trazeni)
        {
            List<Merenje> merenja = ps.Object.DobaviPodatkeId(trazeni);

            // Assert
            //Assert.AreEqual(merenja, new List<Merenje>());
        }

        #endregion

        #region LOKALNO POSLEDNJI

        // TODO: Dodaj u lokalnu kopiju neki podatak, i proveri da li će ti ga vratiti sve tri metode

        #endregion

        #region OBRISI STARE PODATKE
        // TODO: Dodaj u lokalnu kopiju neki podatak, i proveri da li će ti ga obrisati (lokalnaKopija.ContainsKey)
        public void BrisanjePodatka_ViseOd24_Obrisace()
        {

        }

        public void BrisanjePodatka_ManjeOd24_NeceObrisati()
        {

        }

        #endregion


    }
}
