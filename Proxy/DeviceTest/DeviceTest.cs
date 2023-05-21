using Castle.Components.DictionaryAdapter.Xml;
using Common.Interfejsi;
using Common.Klase;
using Device;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DeviceTest
{
    [TestFixture]
    public class DeviceTest
    {
        private Uredjaj uredjaj;
        private Mock<IUredjaj> u;
        private Mock<IServer> kanal;

        [SetUp]
        public void SetUp()
        {
            uredjaj = new Uredjaj();
            u = new Mock<IUredjaj>();
            kanal = new Mock<IServer>();
        }

        #region KONSTRUKTOR
        [Test]
        public void KonstruktorTest()
        {
            Uredjaj uredjaj = new Uredjaj();
            Assert.IsNotNull(uredjaj.IdUredjaja);
        }

        [Test]
        public void KonstruktorTest_NasumicnostZaJedan()
        {
            Uredjaj uredjaj = new Uredjaj();
            Assert.AreEqual(uredjaj.IdUredjaja, int.Parse((DateTime.Now.ToFileTime() % 123456789).ToString()));
        }

        [Test]
        public void KonstruktorTest_NasumicnostZaDva()
        {
            Uredjaj uredjaj1 = new Uredjaj();
            Thread.Sleep(1500);
            Uredjaj uredjaj2 = new Uredjaj();

            Assert.AreNotEqual(uredjaj1.IdUredjaja, uredjaj2.IdUredjaja);
        }
        #endregion

        #region PROPERTIJI
        [Test]
        [TestCase(123)]
        [TestCase(2000)]
        public void KonstruktorTest_GetMetoda(int id)
        {
            Uredjaj uredjaj = new Uredjaj() { IdUredjaja = id };
            Assert.AreEqual(uredjaj.IdUredjaja, id);
        }

        [Test]
        [TestCase(-123)]
        [TestCase(-2000)]
        public void KonstruktorTest_SetMetoda(int id)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => uredjaj.IdUredjaja = id);
        }
        #endregion

        #region METODE
        [Test]
        public void IzmeriTest()
        {
            Merenje m = uredjaj.Izmeri();
            Assert.IsNotNull(m);
        }

        [Test]
        public void PosaljiMerenjeTest_NullMerenje_BacaException()
        {   
            Assert.Throws<ArgumentException>(() => uredjaj.PosaljiMerenja(kanal.Object, null));
        }
        #endregion
    }
}
