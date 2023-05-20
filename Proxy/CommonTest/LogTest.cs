using Common.Klase;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTest
{
    [TestFixture]
    public class LogTest
    {
        Mock<Log> l;

        [SetUp]
        public void SetUp()
        {
            l = new Mock<Log>("C:\\Users\\wildbohana\\Desktop\\Proxy\\CommonTest\\testLoger.txt");
        }

        #region KONSTRUKTOR
        [Test]
        [TestCase("../../testLoger.txt")]
        public void Konstruktor_DobriParametri(string fajl)
        {
            Log log = new Log(fajl);
            Assert.AreEqual(fajl, log.ImeFajla);
        }

        [Test]
        [TestCase(null)]
        public void Konstruktor_BezImena_BacaException(string fajl)
        {
            Assert.Throws<ArgumentNullException>(() => { Log log = new Log(fajl); });
        }

        [Test]
        [TestCase("   ")]
        public void Konstruktor_BezKaraktera_BacaException(string fajl)
        {
            ArgumentException ex = Assert.Throws<ArgumentException>(() => { Log log = new Log(fajl); });
            Assert.That(ex.Message, Is.EqualTo("Naziv fajla mora sadržati karaktere!"));
        }
        #endregion

        #region UPIS LOGA NEUSPEŠAN
        // Proksi
        [Test]
        [TestCase(null, null)]
        public void LogProksi_NemaObaArgumenta_BacaException(DateTime dt, string dogadjaj)
        {
            //Assert.Throws<ArgumentException>(() => { l.Object.LogProksi(dt, dogadjaj); });
            Exception ex = Assert.Throws<ArgumentException>(() => { l.Object.LogProksi(dt, dogadjaj); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Datum i Dogadjaj."));
        }

        // Server
        [Test]
        [TestCase(null, null)]
        public void LogServer_NemaObaArgumenta_BacaException(DateTime dt, string dogadjaj)
        {
            //Assert.Throws<ArgumentException>(() => { l.Object.LogServer(dt, dogadjaj); });
            Exception ex = Assert.Throws<ArgumentException>(() => { l.Object.LogServer(dt, dogadjaj); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Datum i Dogadjaj."));
        }

        // Uređaj
        [Test]
        [TestCase(null)]
        public void LogUredjaj_NemaObaArgumenta_BacaException(Merenje m)
        {
            //Assert.Throws<ArgumentException>(() => { l.Object.LogUredjaj(m); });
            Exception ex = Assert.Throws<ArgumentException>(() => { l.Object.LogUredjaj(m); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Merenje."));
        }
        #endregion

        #region UPIS LOGA USPEŠAN
        [Test]
        [Ignore("Not ready for primetime")]
        public void LogProksi_ImaArgumente_PozivaMetodu()
        {
            DateTime dt = DateTime.Now;
            string dogadjaj = "test";

            // TODO fix

            //l.Setup(mock => mock.UpisUFajl(l.Object.ImeFajla));
            //l.Verify(mok => mok.UpisUFajl(l.Object.ImeFajla), Times.Once);
        }
        #endregion
    }
}
