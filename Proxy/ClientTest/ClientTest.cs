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

namespace ClientTest
{
    [TestFixture]
    public class ClientTest
    {
        private Mock<IProksi> kanal;
        private Klijent klijent;

        [SetUp]
        public void SetUp()
        {
            klijent = new Klijent();
            kanal = new Mock<IProksi>();
        }

        #region OPCIJA 1
        [Test]
        [TestCase(200)]
        public void Opcija1Test_DobriArgumenti(int trazeni)
        {
            Assert.DoesNotThrow(() => { klijent.Opcija1(kanal.Object, trazeni); });
        }

        [Test]
        [TestCase(-1)]
        public void Opcija1Test_NeispravanFormatZaID_VracaException(int trazeni)
        {
            Exception ex = Assert.Throws<FormatException>(() => { klijent.Opcija1(kanal.Object, trazeni); });
            Assert.That(ex.Message, Is.EqualTo("Neispravno unet ID!"));
        }
        #endregion

        #region OPCIJA 2
        [Test]
        [TestCase(200)]
        public void Opcija2Test_DobriArgumenti(int trazeni)
        {
            Assert.DoesNotThrow(() => { klijent.Opcija2(kanal.Object, trazeni); });
        }

        [Test]
        [TestCase(-1)]
        public void Opcija2Test_NeispravanFormatZaID_VracaException(int trazeni)
        {
            Exception ex = Assert.Throws<FormatException>(() => { klijent.Opcija2(kanal.Object, trazeni); });
            Assert.That(ex.Message, Is.EqualTo("Neispravno unet ID!"));
        }
        #endregion

        #region OPCIJA 3
        [Test]
        public void Opcija3Test_DobriArgumenti()
        {
            Assert.DoesNotThrow(() => { klijent.Opcija3(kanal.Object); });
        }
        #endregion

        #region OPCIJA 4
        [Test]
        public void Opcija4Test_DobriArgumenti()
        {   
            Assert.DoesNotThrow(() => { klijent.Opcija4(kanal.Object); });
        }
        #endregion

        #region OPCIJA 5
        [Test]
        public void Opcija5Test_DobriArgumenti()
        {
            Assert.DoesNotThrow(() => { klijent.Opcija5(kanal.Object); });
        }
        #endregion
    }
}
