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
using System.Threading.Tasks;

namespace DeviceTest
{
    [TestFixture]
    public class DeviceTest
    {
        // trebaće IServer kao mock
        private Mock<Uredjaj> u;
        private Mock<IServer> kanal;

        public DeviceTest()
        {
            SetUp();
        }

        public void SetUp()
        {
            u = new Mock<Uredjaj>();
            kanal = new Mock<IServer>();
        }

        [Test]
        [Ignore("Šrodingerova mačka")]
        public void KonstruktorTest_IdJeDovoljnoNasumican()
        {
            Uredjaj uredjaj = new Uredjaj();
            Assert.AreNotEqual(uredjaj.IdUredjaja, int.Parse((DateTime.Now.ToFileTime() % 123456789).ToString()));
        }

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
            Assert.Throws<ArgumentOutOfRangeException>(() => u.Object.IdUredjaja = id);
        }

        [Test]
        [Ignore("Not ready for primetime")]
        public void IzmeriTest()
        {
            // new Merenje(id, vrsta, vrednost, vreme, idUredjaja);
            u.Setup(mock => mock.Izmeri());

            // Test - returns new Merenje();a
        }

        [Test]
        public void PosaljiMerenjeTest_NullMerenje_BacaException()
        {   
            Assert.Throws<ArgumentNullException>(() => u.Object.PosaljiMerenja(kanal.Object, null));
        }

    }
}
