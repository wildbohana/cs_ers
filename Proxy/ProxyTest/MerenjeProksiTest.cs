using Common.Klase;
using NUnit.Framework;
using Proxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProxyTest
{
    [TestFixture]
    public class MerenjeProksiTest
    {
        [Test]
        public void PropertijiTest()
        {
            Merenje m = new Merenje(1, 1, 1, 1, DateTime.Now);
            DateTime pp = DateTime.Now - TimeSpan.FromHours(1);
            DateTime pa = DateTime.Now - TimeSpan.FromHours(2);

            MerenjeProksi mp = new MerenjeProksi { Merenje = m, PoslednjiPristup = pp, PoslednjeAzuriranje = pa };

            Assert.AreEqual(m, mp.Merenje);
            Assert.AreEqual(pp, mp.PoslednjiPristup);
            Assert.AreEqual(pa, mp.PoslednjeAzuriranje);
        }
    }
}
