using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Common.Klase;
using Common.Interfejsi;
using Proxy;

namespace ProxyTest
{
    [TestFixture]
    public class ProxyServisTest
    {
        private Mock<IProksi> kanal;
        private ProksiServis ps;

        [SetUp]
        public void SetUp()
        {
            kanal = new Mock<IProksi>();
            ps = new ProksiServis();

            ps.Loger.ImeFajla = "../../../Logovi/proxyTestLog.txt";
        }

        #region PRAZNA BAZA
        [Test]
        [TestCase(100)]
        public void TraziSveId_PraznaBaza_VracaNull(int trazeni)
        {
            List<Merenje> merenja = kanal.Object.DobaviPodatkeId(trazeni);
            Assert.AreEqual(merenja, null);
        }

        [Test]
        [TestCase(100)]
        public void TraziPoslednjiId_PraznaBaza_VracaNull(int trazeni)
        {
            List<Merenje> merenja = kanal.Object.DobaviPoslednjiPodatakId(trazeni);
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TraziPoslednjiSvi_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = kanal.Object.DobaviPoslednjiPodatakSvi();
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TrazniAnalogne_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = kanal.Object.DobaviSveAnalogne();
            Assert.AreEqual(merenja, null);
        }

        [Test]
        public void TraziDigitalne_PraznaBaza_VracaNull()
        {
            List<Merenje> merenja = kanal.Object.DobaviSveDigitalne();
            Assert.AreEqual(merenja, null);
        }
        #endregion

        #region LOKALNO DOBAVI SVE ZA ID
        [Test]
        [TestCase(300)]
        public void TestSviZaId_NemaTrazeni(int trazeni)
        {
            // Arrange
            MerenjeProksi mp = new MerenjeProksi
            {
                // Bitan je IdUredjaja !!!
                Merenje = new Merenje(1, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            ps.LokalnaKopija.Add(mp.Merenje.IdMerenja, mp);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoIdSvi(trazeni);

            // Assert
            Assert.IsEmpty(merenja);
        }

        [Test]
        [TestCase(400)]
        public void TestSviZaId_ImaTrazeni(int trazeni)
        {
            // Arrange
            MerenjeProksi mp = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            ps.LokalnaKopija.Add(mp.Merenje.IdMerenja, mp);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoIdSvi(trazeni);

            // Assert
            Assert.AreEqual(merenja[0], mp.Merenje);
        }

        [Test]
        [TestCase(400)]
        public void TestSviZaId_ImaTrazene_Oba(int trazeni)
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoIdSvi(trazeni);

            // Assert
            Assert.AreEqual(2, merenja.Count());
        }

        [Test]
        [TestCase(400)]
        public void TestSviZaId_ImaTrazene_Jedan(int trazeni)
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoIdSvi(trazeni);

            // Assert
            Assert.AreEqual(1, merenja.Count());
        }
        #endregion

        #region LOKALNO DOBAVI POSLEDNJI ZA ID
        [Test]
        [TestCase(300)]
        public void TestPoslednjiZaId_NemaTrazeni_VracaNull(int trazeni)
        {
            // Arrange
            MerenjeProksi mp = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            ps.LokalnaKopija.Add(mp.Merenje.IdMerenja, mp);

            // Act
            Merenje mrnj = ps.DobaviLokalnoIdPoslednji(trazeni);

            // Assert
            Assert.IsNull(mrnj);
        }

        [Test]
        [TestCase(400)]
        public void TestPoslednjiZaId_ImaTrazeni_SamoJedan(int trazeni)
        {
            // Arrange
            MerenjeProksi mp = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            ps.LokalnaKopija.Add(mp.Merenje.IdMerenja, mp);

            // Act
            Merenje mrnj = ps.DobaviLokalnoIdPoslednji(trazeni);

            // Assert
            Assert.AreEqual(mrnj, mp.Merenje);
        }

        [Test]
        [TestCase(400)]
        public void TestPoslednjiZaId_ImaTrazeni_VracaMladji(int trazeni)
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                // Ovaj je stariji
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                // Ovaj je noviji
                Merenje = new Merenje(3, 400, 1, 1, DateTime.Now - TimeSpan.FromHours(1)),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            Merenje mrnj = ps.DobaviLokalnoIdPoslednji(trazeni);

            // Assert
            Assert.AreEqual(mrnj, mp2.Merenje);
        }
        #endregion

        #region LOKALNO DOBAVI NAJMLADJI ZA SVE
        [Test]
        public void TestPoslednjiZaSve_ImaViseSaIstimId_VracaMladje()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                // Ovaj je stariji za ID 400
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji za ID 400
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                // Ovaj je najnoviji za ID 500
                Merenje = new Merenje(3, 500, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoSvePoslednje();

            // Assert
            Assert.IsTrue(!merenja.Contains(mp1.Merenje));
            Assert.IsTrue(merenja.Contains(mp2.Merenje));
            Assert.IsTrue(merenja.Contains(mp3.Merenje));
        }

        [Test]
        public void TestPoslednjiZaSve_ImaVise_VracaSve()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                // Ovaj je stariji
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji
                Merenje = new Merenje(2, 500, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                // Ovaj je noviji
                Merenje = new Merenje(3, 600, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoSvePoslednje();

            // Assert
            Assert.IsTrue(merenja.Contains(mp1.Merenje));
            Assert.IsTrue(merenja.Contains(mp2.Merenje));
            Assert.IsTrue(merenja.Contains(mp3.Merenje));
        }

        #endregion

        #region LOKALNO DOBAVI ANALOGNE
        [Test]
        public void TestAnalogni_VracaSve()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 0, 1000, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 0, 2000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 0, 3000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoAnalogne();

            // Assert
            Assert.IsTrue(merenja.Contains(mp1.Merenje));
            Assert.IsTrue(merenja.Contains(mp2.Merenje));
            Assert.IsTrue(merenja.Contains(mp3.Merenje));
        }

        [Test]
        public void TestAnalogni_VracaSamoAnalogne()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 0, 1000, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 0, 2000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);       // Analogni
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);       // Analogni
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);       // Digitalni

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoAnalogne();

            // Assert
            Assert.IsTrue(merenja.Contains(mp1.Merenje));
            Assert.IsTrue(merenja.Contains(mp2.Merenje));
            Assert.IsTrue(!merenja.Contains(mp3.Merenje));
        }

        [Test]
        public void TestAnalogni_VracaPraznuListu()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 1, 0, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 0, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoAnalogne();

            // Assert
            Assert.IsEmpty(merenja);
        }
        #endregion

        #region LOKALNO DOBAVI DIGITALNE
        [Test]
        public void TestDigitalni_VracaSve()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 0, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 1, 0, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoDigitalni();

            // Assert
            Assert.IsTrue(merenja.Contains(mp1.Merenje));
            Assert.IsTrue(merenja.Contains(mp2.Merenje));
            Assert.IsTrue(merenja.Contains(mp3.Merenje));
        }

        [Test]
        public void TestDigitalni_VracaSamoDigitalne()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 0, 1000, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 0, 2000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);       // Analogni
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);       // Analogni
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);       // Digitalni

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoDigitalni();

            // Assert
            Assert.IsTrue(!merenja.Contains(mp1.Merenje));
            Assert.IsTrue(!merenja.Contains(mp2.Merenje));
            Assert.IsTrue(merenja.Contains(mp3.Merenje));
        }

        [Test]
        public void TestDigitalni_VracaPraznuListu()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 300, 0, 0, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 0, 2000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 500, 0, 3000, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            List<Merenje> merenja = ps.DobaviLokalnoDigitalni();

            // Assert
            Assert.IsEmpty(merenja);
        }
        #endregion

        #region LOKALNO POSLEDNJI
        // Za sve
        [Test]
        public void TestLokalnoPoslednjiSvi_ImaVise_VracaJednog()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 400, 1, 1, DateTime.Now - TimeSpan.FromHours(1)),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaSve();

            // Assert
            Assert.AreEqual(mrnj.Merenje, mp2.Merenje);
        }

        [Test]
        public void TestLokalnoPoslednjiSvi_PraznaLK_VracaNull()
        {
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaSve();
            Assert.IsNull(mrnj);
        }

        // Za trazeni ID
        [Test]
        [TestCase(400)]
        public void TestLokalnoPoslednjiId_ImaVise_VracaJednog(int trazeni)
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 400, 1, 1, DateTime.Now - TimeSpan.FromHours(1)),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);

            // Act
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaTrazeniId(trazeni);

            // Assert
            Assert.AreEqual(mrnj.Merenje, mp2.Merenje);
        }

        [Test]
        [TestCase(300)]
        public void TestLokalnoPoslednjiId_NemaTrazeni_VracaNull(int trazeni)
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);

            // Act
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaTrazeniId(trazeni);

            // Assert
            Assert.IsNull(mrnj);
        }

        [Test]
        [TestCase(400)]
        public void TestLokalnoPoslednjiId_PraznaLK_VracaNull(int trazeni)
        {
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaTrazeniId(trazeni);
            Assert.IsNull(mrnj);
        }

        // Za traženu vrstu
        [Test]
        [TestCase(VrstaMerenja.DIGITALNO_MERENJE)]
        [TestCase(VrstaMerenja.ANALOGNO_MERENJE)]
        public void TestLokalnoPoslednjiVrsta_ImaVise_VracaJednog(VrstaMerenja vr)
        {
            // Arrange
            DateTime testVreme = DateTime.Now;

            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                // Ovaj je najnoviji digitalni
                Merenje = new Merenje(2, 500, 1, 0, testVreme),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp3 = new MerenjeProksi
            {
                Merenje = new Merenje(3, 600, 0, 1000, DateTime.Now - TimeSpan.FromHours(1)),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp4 = new MerenjeProksi
            {
                // Ovaj je najnoviji analogni
                Merenje = new Merenje(4, 700, 0, 3000, testVreme),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);
            ps.LokalnaKopija.Add(mp3.Merenje.IdMerenja, mp3);
            ps.LokalnaKopija.Add(mp4.Merenje.IdMerenja, mp4);

            // Act
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaTrazenuVrstu(vr);

            // Assert
            Assert.AreEqual(mrnj.Merenje.VrstaMerenja, vr);
            Assert.AreEqual(mrnj.Merenje.VremeMerenja, testVreme);
        }

        [Test]
        [TestCase(VrstaMerenja.DIGITALNO_MERENJE)]
        [TestCase(VrstaMerenja.ANALOGNO_MERENJE)]
        public void TestLokalnoPoslednjiVrsta_PraznaLK_VracaNull(VrstaMerenja vr)
        {
            MerenjeProksi mrnj = ps.PoslednjeDodavanjeLokalnoZaTrazenuVrstu(vr);
            Assert.IsNull(mrnj);
        }
        #endregion

        #region OBRISI STARE PODATKE
        [Test]
        public void BrisanjePodatka_ManjeOd24_NeceObrisati()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now - TimeSpan.FromHours(23)
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);

            // Act
            ps.ObrisiStaraMerenja();

            // Assert
            Assert.IsTrue(ps.LokalnaKopija.ContainsKey(mp1.Merenje.IdMerenja));
            Assert.IsTrue(ps.LokalnaKopija.ContainsKey(mp2.Merenje.IdMerenja));
        }

        [Test]
        public void BrisanjePodatka_ViseOd24_Obrisace()
        {
            // Arrange
            MerenjeProksi mp1 = new MerenjeProksi
            {
                Merenje = new Merenje(1, 400, 1, 1, DateTime.MinValue),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now
            };
            MerenjeProksi mp2 = new MerenjeProksi
            {
                Merenje = new Merenje(2, 400, 1, 1, DateTime.Now),
                PoslednjeAzuriranje = DateTime.Now,
                PoslednjiPristup = DateTime.Now - TimeSpan.FromHours(25)
            };

            ps.LokalnaKopija.Add(mp1.Merenje.IdMerenja, mp1);
            ps.LokalnaKopija.Add(mp2.Merenje.IdMerenja, mp2);

            // Act
            ps.ObrisiStaraMerenja();

            // Assert
            Assert.IsTrue(ps.LokalnaKopija.ContainsKey(mp1.Merenje.IdMerenja));
            Assert.IsTrue(!ps.LokalnaKopija.ContainsKey(mp2.Merenje.IdMerenja));
        }
        #endregion
    }
}
