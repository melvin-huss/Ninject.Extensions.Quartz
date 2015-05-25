using System;
using NUnit.Framework;
using Ninject.Modules;
using Quartz;
using Quartz.Spi;

namespace Ninject.Extensions.Quartz.Tests
{
    [TestFixture]
    public class JobTests
    {
        private IKernel _kernel;
        private static bool _hasRun;

        [SetUp]
        public void Setup()
        {
            _kernel = new StandardKernel();
        }

        [Test]
        public void TestKernelCanInstantiateScheduler()
        {
            var scheduler = _kernel.Get<IScheduler>();
            Assert.IsNotNull(scheduler);
        }

        [Test]
        public void TestHarnessCanCreateJob()
        {
            var scheduler = _kernel.Get<IScheduler>();

            IJobDetail testJob = JobBuilder
                                    .Create<TestJob>()
                                    .Build();

            ITrigger runOnce = TriggerBuilder.Create().WithSimpleSchedule(builder => builder.WithRepeatCount(0)).Build();
            
            scheduler.ScheduleJob(testJob, runOnce);

            scheduler.Start();

            System.Threading.Thread.Sleep(TimeSpan.FromSeconds(5.0));

            Assert.IsTrue(_hasRun);
        }

        public class TestJob : IJob
        {
            public void Execute(IJobExecutionContext context)
            {
                _hasRun = true;
            }
        }

    }
}
