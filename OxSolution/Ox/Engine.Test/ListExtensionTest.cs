using System.Collections.Generic;
using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    [TestFixture]
    public class ListExtensionTest
    {
        [Test]
        public void TransferRange_Test()
        {
            IList<int> list = new List<int>(new int[] { 1, 2, 3, 4, 5 });
            IList<int> range = new List<int>(new int[] { 6, 7 });
            list.TransferRange(range);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(0, range.Count);
        }

        [Test]
        public void AddRange_Test()
        {
            IList<int> list = new List<int>(new int[] { 1, 2, 3, 4, 5 });
            IList<int> range = new List<int>(new int[] { 6, 7 });
            list.AddRange(range);
            Assert.AreEqual(7, list.Count);
            Assert.AreEqual(2, range.Count);
        }

        [Test]
        public void RemoveUnstable_Test()
        {
            List<int> list = new List<int>(new int[] { 1, 2, 2, 3});
            Assert.IsTrue(list.RemoveUnstable(2));
            Assert.AreEqual(3, list.Count);
            Assert.IsTrue(list.RemoveUnstable(2));
            Assert.AreEqual(2, list.Count);
            list.ForEach(x => Assert.AreNotEqual(2, x));
            Assert.IsFalse(list.RemoveUnstable(2));
            Assert.AreEqual(2, list.Count);
        }

        [Test]
        public void RemoveUnstableAtBegin_Test()
        {
            List<int> list = new List<int>(new int[] { 1, 2, 3} );
            list.RemoveAtUnstable(0);
            Assert.AreEqual(2, list.Count);
            list.ForEach(x => Assert.AreNotEqual(1, x));
        }

        [Test]
        public void RemoveUnstableAtMiddle_Test()
        {
            List<int> list = new List<int>(new int[] { 1, 2, 3});
            list.RemoveAtUnstable(1);
            Assert.AreEqual(2, list.Count);
            list.ForEach(x => Assert.AreNotEqual(2, x));
        }

        [Test]
        public void RemoveUnstableAtEnd_Test()
        {
            List<int> list = new List<int>(new int[] { 1, 2, 3 });
            list.RemoveAtUnstable(list.Count - 1);
            Assert.AreEqual(2, list.Count);
            list.ForEach(x => Assert.AreNotEqual(3, x));
        }
    }
}
