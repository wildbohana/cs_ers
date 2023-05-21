using Common.Interfejsi;
using Common.Klase;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace CommonTest
{
    [TestFixture]
    public class LogTest
    {
        private Mock<ILog> logerMok;
        private Log loger;

        [SetUp]
        public void SetUp()
        {
            loger = new Log("tekst");
            logerMok = new Mock<ILog>();
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
            Exception ex = Assert.Throws<ArgumentException>(() => { loger.LogProksi(dt, dogadjaj); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Datum i Dogadjaj."));
        }

        // Server
        [Test]
        [TestCase(null, null)]
        public void LogServer_NemaObaArgumenta_BacaException(DateTime dt, string dogadjaj)
        {
            //Assert.Throws<ArgumentException>(() => { l.Object.LogServer(dt, dogadjaj); });
            Exception ex = Assert.Throws<ArgumentException>(() => { loger.LogServer(dt, dogadjaj); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Datum i Dogadjaj."));
        }

        // Uređaj
        [Test]
        [TestCase(null)]
        public void LogUredjaj_NemaObaArgumenta_BacaException(Merenje m)
        {
            //Assert.Throws<ArgumentException>(() => { l.Object.LogUredjaj(m); });
            Exception ex = Assert.Throws<ArgumentException>(() => { loger.LogUredjaj(m); });
            Assert.That(ex.Message, Is.EqualTo("Za upis loga je neophodno uneti Merenje."));
        }
        #endregion

        #region UPIS LOGA POZVAN
        [Test]
        [Ignore("Not ready for primetime")]
        public void LogProksi_ImaArgumente_PozivaMetodu()
        {
            DateTime dt = DateTime.Now;
            string dogadjaj = "test";

            // TODO fix
            logerMok.Setup(x => x.LogProksi(dt, dogadjaj));
            logerMok.Verify(x => x.LogProksi(dt, dogadjaj), Times.Once());
        }

        [Test]
        [Ignore("Not ready for primetime")]
        public void LogServer_ImaArgumente_PozivaMetodu()
        {
            DateTime dt = DateTime.Now;
            string dogadjaj = "test";

            // TODO fix
            loger.LogServer(dt, dogadjaj);
            logerMok.Verify(x => x.LogServer(dt, dogadjaj), Times.Once());
        }

        [Test]
        [Ignore("Not ready for primetime")]
        public void LogUredjaj_ImaArgumente_PozivaMetodu()
        {
            Merenje m = new Merenje(1, 1, 1, 1, DateTime.Now);

            // TODO fix
            loger.LogUredjaj(m);
            logerMok.Verify(x => x.LogUredjaj(m), Times.Once());
        }
        #endregion
    }
}
