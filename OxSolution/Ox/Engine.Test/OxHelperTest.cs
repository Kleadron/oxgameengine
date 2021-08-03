using System;
using NUnit.Framework;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class OxHelperTest
    {
        [Test]
        public void ArgumentNullCheckPass()
        {
            OxHelper.ArgumentNullCheck(obj);
        }

        [Test]
        public void ArgumentNullCheckPass2()
        {
            OxHelper.ArgumentNullCheck(obj, obj);
        }

        [Test]
        public void ArgumentNullCheckPass3()
        {
            OxHelper.ArgumentNullCheck(obj, obj, obj);
        }

        [Test]
        public void ArgumentNullCheckPass4()
        {
            OxHelper.ArgumentNullCheck(obj, obj, obj, obj);
        }

        [Test]
        public void ArgumentNullCheckPass5()
        {
            OxHelper.ArgumentNullCheck(obj, obj, obj, obj, obj);
        }

        [Test, ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullCheckFail()
        {
            OxHelper.ArgumentNullCheck(null);
        }
        
        [TestCase(null, "")]
        [TestCase("", null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullCheckFail2(string str, string str2)
        {
            OxHelper.ArgumentNullCheck(str, str2);
        }

        [TestCase(null, "", "")]
        [TestCase("", null, "")]
        [TestCase("", "", null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullCheckFail3(string str, string str2, string str3)
        {
            OxHelper.ArgumentNullCheck(str, str2, str3);
        }

        [TestCase(null, "", "", "")]
        [TestCase("", null, "", "")]
        [TestCase("", "", null, "")]
        [TestCase("", "", "", null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullCheckFail4(string str, string str2, string str3, string str4)
        {
            OxHelper.ArgumentNullCheck(str, str2, str3, str4);
        }

        [TestCase(null, "", "", "", "")]
        [TestCase("", null, "", "", "")]
        [TestCase("", "", null, "", "")]
        [TestCase("", "", "", null, "")]
        [TestCase("", "", "", "", null)]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ArgumentNullCheckFail5(string str, string str2, string str3, string str4, string str5)
        {
            OxHelper.ArgumentNullCheck(str, str2, str3, str4, str5);
        }

        private readonly object obj = new object();
    }
}
