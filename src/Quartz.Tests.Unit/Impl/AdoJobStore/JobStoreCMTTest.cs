using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using FakeItEasy;

using NUnit.Framework;

using Quartz.Impl.AdoJobStore;
using Quartz.Util;

namespace Quartz.Tests.Unit.Impl.AdoJobStore
{
    [TestFixture]
    public class JobStoreCMTTest
    {
        private TestJobStoreCMT jobStore;
        private IDbConnectionManager connectionManager;

        [SetUp]
        public void SetUp()
        {
            jobStore = new TestJobStoreCMT();
            connectionManager = A.Fake<IDbConnectionManager>();
            jobStore.ConnectionManager = connectionManager;
        }

        private class TestJobStoreCMT : JobStoreCMT
        {
            public async Task ExecuteGetNonManagedConnection(CancellationToken cancellationToken)
            {
                await GetNonManagedTXConnection(cancellationToken);
            }
        }

        [Test]
        public async Task ShouldNotAutomaticallyOpenConnection()
        {
            var mock = A.Fake<DbConnection>();
            A.CallTo(() => connectionManager.GetConnection(A<string>.Ignored)).Returns(mock);

            await jobStore.ExecuteGetNonManagedConnection(CancellationToken.None);

            A.CallTo(() => mock.Open()).MustNotHaveHappened();
        }

        [Test]
        public async Task ShouldOpenConnectionIfRequested()
        {
            jobStore.OpenConnection = true;
            var mock = A.Fake<DbConnection>();
            A.CallTo(() => connectionManager.GetConnection(A<string>.Ignored)).Returns(mock);

            await jobStore.ExecuteGetNonManagedConnection(CancellationToken.None);

            A.CallTo(() => mock.OpenAsync(CancellationToken.None)).MustHaveHappened();
        }
    }
}
