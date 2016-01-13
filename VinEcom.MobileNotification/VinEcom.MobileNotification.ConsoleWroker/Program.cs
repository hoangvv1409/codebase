using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VinEcom.MobileNotification.ConsoleWorker;
using VinEcom.MobileNotification.Infrastructure;

namespace VinEcom.MobileNotification.ConsoleWroker
{
    class Program
    {
        private IUnityContainer container;
        private CancellationTokenSource cancellationTokenSource;
        private List<IProcessor> listProcessors;

        static void Main(string[] args)
        {
            Program p = new Program();
            p.Init();
            p.Start();

            while (true)
            {

            }
        }

        public void Init()
        {
            this.cancellationTokenSource = new CancellationTokenSource();
            this.container = Container.CreateContainer();
            this.listProcessors = this.container.ResolveAll<IProcessor>().ToList();
        }

        public void Start()
        {
            this.listProcessors.ForEach(p => p.Start());
        }

        public void Stop()
        {
            this.cancellationTokenSource.Cancel();
            this.listProcessors.ForEach(p => p.Stop());
        }
    }
}
