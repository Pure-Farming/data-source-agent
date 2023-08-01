using Hangfire;

namespace pfDataSource
{
    public class DIJobActivator : JobActivator
	{
        private IServiceProvider _container;

        public DIJobActivator(IServiceProvider container) => _container = container;

        public override object ActivateJob(Type type) => _container.GetService(type);
    }
}

