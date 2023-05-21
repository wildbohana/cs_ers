﻿using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Common.Interfejsi;
using Service;
using System.Data;
using Common.Klase;

namespace ServiceTest
{
    [TestFixture]
    public class ServiceTest
    {
        private Mock<IServer> kanal;
        private Mock<IDbConnection> bazaMok;
        private ServerServis ss;

        [SetUp]
        public void SetUp()
        {
            kanal = new Mock<IServer>();
            ss = new ServerServis();
            bazaMok = new Mock<IDbConnection>();

            ss.Loger.ImeFajla = "../../../Logovi/serverTestLog.txt";
        }

        [Test]
        [Ignore("Glupa sam, ne ide ovako")]
        public void TestUpis_VracaFalse()
        {
            Merenje mrnj = new Merenje(1, 1, 1, 1, DateTime.Now);
            bool uspesno = ss.Upis(mrnj);

            Assert.IsFalse(uspesno);
        }
    }
}
