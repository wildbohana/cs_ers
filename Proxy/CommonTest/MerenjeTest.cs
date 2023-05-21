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
    public class MerenjeTest
    {
        private Mock<Merenje> m;

        [SetUp]
        public void SetUp()
        {
            m = new Mock<Merenje>();
        }

        #region ISPRAVNI PARAMETRI
        [Test]
        [TestCase(1000, 0, 2222, 5000)]
        [TestCase(2000, 0, 4545, 6000)]
        [TestCase(3000, 1, 1, 7000)]
        [TestCase(3000, 1, 0, 8000)]
        public void MerenjeKonstruktor_DobriParametri(int idMerenja, VrstaMerenja vrsta, int vrednost, int idUredjaja)
        {
            DateTime dt = DateTime.Now;
            Merenje m = new Merenje(idMerenja, vrsta, vrednost, dt, idUredjaja);

            Assert.AreEqual(m.IdMerenja, idMerenja);
            Assert.AreEqual(m.VrstaMerenja, vrsta);
            Assert.AreEqual(m.VremeMerenja, dt);
            Assert.AreEqual(m.IdUredjaja, idUredjaja);
            Assert.AreEqual(m.Vrednost, vrednost);
        }
        #endregion

        #region NEISPRAVNI PARAMETRI
        [Test]
        [TestCase(1111, 1, 2222, 3333)]
        [TestCase(3434, 1, 4545, 6666)]
        public void MerenjeKonstruktor_LosiParametri_ZaVrstuIVrednost(int idMerenja, VrstaMerenja vrsta, int vrednost, int idUredjaja)
        {
            DateTime dt = DateTime.Now;            
            Assert.Throws<ArgumentOutOfRangeException>(() => { Merenje m = new Merenje(idMerenja, vrsta, vrednost, dt, idUredjaja); });
        }

        [Test]
        [TestCase(1111, 1, 2222, 3333)]
        public void MerenjeKonstruktor_LosiParametri_ZaDatum(int idMerenja, VrstaMerenja vrsta, int vrednost, int idUredjaja)
        {
            DateTime dt = DateTime.Now + TimeSpan.FromDays(1);
            Assert.Throws<ArgumentException>(() => { Merenje m = new Merenje(idMerenja, vrsta, vrednost, dt, idUredjaja); });
        }

        [Test]
        [TestCase(-1000, 1, 1, 2000)]
        [TestCase(1000, 1, 1, -2000)]
        public void MerenjeKonstruktor_LosiParametri(int idMerenja, VrstaMerenja vrsta, int vrednost, int idUredjaja)
        {
            DateTime dt = DateTime.Now;
            Assert.Throws<ArgumentException>(() => { Merenje m = new Merenje(idMerenja, vrsta, vrednost, dt, idUredjaja); });
        }
        #endregion
    }
}
