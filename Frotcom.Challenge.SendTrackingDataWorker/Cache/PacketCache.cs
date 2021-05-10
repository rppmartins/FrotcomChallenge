using Frotcom.Challenge.Data.Models;
using Frotcom.Challenge.SendTrackingDataWorker.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Frotcom.Challenge.SendTrackingDataWorker.Cache
{
    public class PacketCache : ICache<Packet>
    {
        private static int totalPackets = 0;
        private static int portugalPackets = 0;

        private static Dictionary<int, List<Packet>> cache = new Dictionary<int, List<Packet>>();
        private object _lock = new object();

        private const int cacheLimit = 100;

        /// <summary>
        /// Function returns null if it does not reach cacheLimit
        /// </summary>
        /// <param name="key"></param>
        /// <param name="packet"></param>
        /// <returns></returns>
        public List<Packet> Add(int key, Packet packet)
        {
            try
            {
                Interlocked.Increment(ref portugalPackets);

                lock (_lock)
                {
                    if (!cache.TryGetValue(key, out List<Packet> packets))
                    {
                        List<Packet> vehiclesPackets = new List<Packet>();
                        vehiclesPackets.Add(packet);

                        cache.Add(key, vehiclesPackets);
                    }
                    else
                    {
                        packets.Add(packet);
                        cache[key] = packets;
                    }

                    if (packets?.Count == cacheLimit)
                    {
                        cache.Remove(key);
                        return packets;
                    }

                    return null;
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void IncrementTotal()
        {
            Interlocked.Increment(ref totalPackets);
        }

        public void GetEntityValues(object timerState)
        {
            Console.WriteLine($"{DateTimeHelper.GetDay()} {DateTimeHelper.GetTime()}: Total: {totalPackets}, In Portugal: {portugalPackets}");
        }
    }
}
