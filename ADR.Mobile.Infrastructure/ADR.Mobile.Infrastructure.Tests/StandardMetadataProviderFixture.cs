using System;
using ADR.Mobile.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Tests
{
    [TestClass]
    public class given_a_metadata_provider
    {
        [TestMethod]
        public void when_getting_metadata_then_returns_type_name()
        {
            var provider = new StandardMetadataProvider();
            var expected = typeof(given_a_metadata_provider).Name;

            var metadata = provider.GetMetadata(this);

            Assert.IsTrue(metadata.Values.Contains(expected));
            Assert.IsTrue(metadata.Keys.Contains(StandardMetadata.TypeName));

        }

        [TestMethod]
        public void when_getting_metadata_then_returns_type_fullname()
        {
            var provider = new StandardMetadataProvider();
            var expected = typeof(given_a_metadata_provider).FullName;

            var metadata = provider.GetMetadata(this);

            Assert.IsTrue(metadata.Values.Contains(expected));
            Assert.IsTrue(metadata.Keys.Contains(StandardMetadata.FullName));
        }

        [TestMethod]
        public void when_getting_metadata_then_returns_assembly_name()
        {
            var provider = new StandardMetadataProvider();
            var expected = typeof(given_a_metadata_provider).Assembly.GetName().Name;

            var metadata = provider.GetMetadata(this);

            Assert.IsTrue(metadata.Values.Contains(expected));
            Assert.IsTrue(metadata.Keys.Contains(StandardMetadata.AssemblyName));
        }

        [TestMethod]
        public void when_getting_metadata_then_returns_namespace()
        {
            var provider = new StandardMetadataProvider();
            var expected = typeof(given_a_metadata_provider).Namespace;

            var metadata = provider.GetMetadata(this);

            Assert.IsTrue(metadata.Values.Contains(expected));
            Assert.IsTrue(metadata.Keys.Contains(StandardMetadata.Namespace));
        }

        private class FakeEvent : IEvent
        {
            public Guid SourceId { get; set; }
        }
    }
}
