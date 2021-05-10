using Frotcom.Challenge.Data.Models;
using Frotcom.Challenge.Queue;
using Frotcom.Challenge.SendTrackingDataWorker.Cache;

namespace Frotcom.Challenge.SendTrackingDataWorker
{
    public class QueueProcessorFactory : IQueueProcessorFactory
    {
        private readonly ICache<Packet> cache;
        private readonly IService<Packet> service;

        public QueueProcessorFactory(ICache<Packet> cache, IService<Packet> service)
        {
            this.cache = cache;
            this.service = service;
        }

        public IQueueProcessor Create()
        {
            return new QueueProcessor(service, cache);
        }
    }
}
