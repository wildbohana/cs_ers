using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Client;
using Common;
using Common.Interfejsi;
using Common.Klase;

// AKO JE BAZA PRAZNA, VRAĆA NULL
// AKO NE NAĐE PODATKE, VRAĆA PRAZNU LISTU (.COUNT == 0)

namespace ClientTest
{
    [TestFixture]
    public class ClientTest
    {
        // KLijentu je potreban IProksi - to kao Mock
        private Mock<IProksi> proksiServisMock;
        private Klijent klijent;

        public ClientTest()
        {
            SetUp();
        }

        // Izvršava se na početku svakog testa
        public void SetUp()
        {
            klijent = new Klijent();
            proksiServisMock = new Mock<IProksi>();
        }

        #region OPCIJA 1
        [Test]
        public void Opcija1Test_PrazanProksi_VracaPraznuListu()
        {
            proksiServisMock.Setup(x => x.DobaviPodatkeId(123)).Returns(new List<Merenje>());
            Assert.DoesNotThrow(() => { klijent.Opcija1(proksiServisMock.Object, 123); });
        }

        [Test]
        public void Opcija1Test_NeispravanFormatZaID_VracaException()
        {
            //Assert.Throws<FormatException>(() => { klijent.Opcija1(proksiServisMock.Object, -1); });

            Exception ex = Assert.Throws<FormatException>(() => { klijent.Opcija1(proksiServisMock.Object, -1); });
            Assert.That(ex.Message, Is.EqualTo("Neispravno unet ID!"));
        }

        //[Test]
        //public void Opcija1Test_PraznaBaza_VracaNull()
        //{
        //    // Arrange + Act
        //    List<Merenje> merenja = proksiServisMock.Object.DobaviPodatkeId(123);

        //    // Assert
        //    Assert.AreEqual(merenja, null);
        //}

        // TODO 
        [Test]
        public void Opcija1Test_NemaTrazene_VracaPraznuListu()
        {
            //moram dodati da u bazi posotji bar jedan element !!!

            Assert.Pass();
        }

        // TODO
        [Test]
        public void Opcija1Test_ImaTrazeni_VracaDobarRezultat()
        {
            //moram dodati da u bazi posotji bar jedan element !!!

            //Assert.AreEqual(merenja, new List<Merenje>());
            Assert.Pass();
        }
        #endregion

        #region OPCIJA 2

        [Test]
        public void Opcija2Test_PrazanProksi_VracaPraznuListu()
        {
            proksiServisMock.Setup(x => x.DobaviPoslednjiPodatakId(123)).Returns(new List<Merenje>());
            Assert.DoesNotThrow(() => { klijent.Opcija2(proksiServisMock.Object, 123); });
        }

        [Test]
        public void Opcija2Test_NeispravanFormatZaID_VracaException()
        {
            //Assert.Throws<FormatException>(() => { klijent.Opcija2(proksiServisMock.Object, -1); });

            Exception ex = Assert.Throws<FormatException>(() => { klijent.Opcija2(proksiServisMock.Object, -1); });
            Assert.That(ex.Message, Is.EqualTo("Neispravno unet ID!"));
        }
        #endregion

        #region OPCIJA 3
        [Test]
        public void Opcija3Test_PrazanProksi_VracaPraznuListu()
        {
            proksiServisMock.Setup(x => x.DobaviPoslednjiPodatakSvi()).Returns(new List<Merenje>());
            Assert.DoesNotThrow(() => { klijent.Opcija3(proksiServisMock.Object); });
        }
        #endregion

        #region OPCIJA 4
        [Test]
        public void Opcija4Test_PrazanProksi_VracaPraznuListu()
        {
            proksiServisMock.Setup(x => x.DobaviSveAnalogne()).Returns(new List<Merenje>());
            Assert.DoesNotThrow(() => { klijent.Opcija4(proksiServisMock.Object); });
        }
        #endregion

        #region OPCIJA 5
        [Test]
        public void Opcija5Test_PrazanProksi_VracaPraznuListu()
        {
            proksiServisMock.Setup(x => x.DobaviSveDigitalne()).Returns(new List<Merenje>());
            Assert.DoesNotThrow(() => { klijent.Opcija5(proksiServisMock.Object); });
        }
        #endregion
    }
}
