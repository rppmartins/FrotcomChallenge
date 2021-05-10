using Frotcom.Challenge.Data.Models;
using Frotcom.Challenge.SendTrackingDataWorker.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frotcom.Challenge.SendTrackingDataWorker.ExternalService
{
    public class ConsoleService : IService<Packet>
    {
        public Task Post(int id, List<Packet> info)
        {
            return Task.Run(() =>
            {
                Console.WriteLine($"{DateTimeHelper.GetDay()} {DateTimeHelper.GetTime()}: Vehicle {id} sent {info.Count} packets in Portugal");
            });
        }
    }
}
