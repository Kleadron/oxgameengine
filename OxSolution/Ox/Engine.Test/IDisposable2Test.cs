using System;
using NUnit.Framework;
using Ox.Engine.Utility;

namespace Ox.Engine.Test
{
    internal sealed class DisposableObject : IDisposable
    {
        public bool Disposed
        {
            get { return disposed; }
        }

        public void Dispose()
        {
            disposed = true;
        }

        private bool disposed;
    }

    [TestFixture]
    public class IDisposable2Test
    {
        [SetUp]
        public void SetUp()
        {
            disposable = new Disposable2();
        }

        [Test]
        public void AddGarbage_Dispose_TestDisposed()
        {
            DisposableObject[] disposables =
            {
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject()
            };
            disposables.ForEach(x => disposable.AddGarbage(x));
            disposable.Dispose();
            disposables.ForEach(x => Assert.IsTrue(x.Disposed));
        }

        [Test]
        public void AddGarbage_RemoveGarbage_Dispose_TestDisposed()
        {
            DisposableObject[] objects =
            {
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject(),
                new DisposableObject()
            };
            objects.ForEach(x => disposable.AddGarbage(x));
            objects.ForEach(x => disposable.RemoveGarbage(x));
            disposable.Dispose();
            objects.ForEach(x => Assert.IsFalse(x.Disposed));
        }

        private IDisposable2 disposable;
    }
}
