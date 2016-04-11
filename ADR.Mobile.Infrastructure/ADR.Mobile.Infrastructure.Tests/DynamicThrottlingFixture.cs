using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ADR.Mobile.Infrastructure.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Infrastructure.Tests
{
    public class DynamicThrottlingFixture
    {
        [TestMethod]
        public void starts_low()
        {
            using (var sut = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            {
                Assert.IsTrue(10 == sut.AvailableDegreesOfParallelism);
            }
        }

        [TestMethod]
        public void increases_on_completed_work()
        {
            using (var sut = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            {
                sut.NotifyWorkStarted();
                var startingValue = sut.AvailableDegreesOfParallelism;

                sut.NotifyWorkCompleted();

                Assert.IsTrue(startingValue < sut.AvailableDegreesOfParallelism);
            }
        }

        [TestMethod]
        public void continually_increases_on_completed_work()
        {
            using (var sut = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            {
                for (int i = 0; i < 10; i++)
                {
                    sut.NotifyWorkStarted();
                    var startingValue = sut.AvailableDegreesOfParallelism;
                    sut.NotifyWorkCompleted();
                    Assert.IsTrue(startingValue < sut.AvailableDegreesOfParallelism);
                }
            }
        }

        [TestMethod]
        public void decreases_on_penalize()
        {
            using (var sut = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            {
                IncreaseDegreesOfParallelism(sut);

                sut.NotifyWorkStarted();
                var startingValue = sut.AvailableDegreesOfParallelism;
                sut.Penalize();

                Assert.IsTrue(startingValue > sut.AvailableDegreesOfParallelism);
            }
        }

        [TestMethod]
        public void penalize_decreases_less_than_completed_with_error()
        {
            using (var sut1 = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            using (var sut2 = new DynamicThrottling(100, 10, 3, 5, 1, 8000))
            {
                IncreaseDegreesOfParallelism(sut1);
                IncreaseDegreesOfParallelism(sut2);

                sut1.NotifyWorkStarted();
                sut2.NotifyWorkStarted();

                sut1.Penalize();
                sut2.NotifyWorkCompletedWithError();

                Assert.IsTrue(sut1.AvailableDegreesOfParallelism > sut2.AvailableDegreesOfParallelism);
            }
        }

        private static void IncreaseDegreesOfParallelism(DynamicThrottling sut)
        {
            for (int i = 0; i < 10; i++)
            {
                // increase degrees to avoid being in the minimum boundary
                sut.NotifyWorkStarted();
                sut.NotifyWorkCompleted();
            }
        }
    }
}
