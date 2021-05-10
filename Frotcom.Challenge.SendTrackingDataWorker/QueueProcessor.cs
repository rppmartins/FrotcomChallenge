using Frotcom.Challenge.Data.Models;
using Frotcom.Challenge.Queue;
using Frotcom.Challenge.Reverse.Geocoding;
using Frotcom.Challenge.SendTrackingDataWorker.Cache;
using Frotcom.Challenge.SendTrackingDataWorker.Helpers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Frotcom.Challenge.SendTrackingDataWorker
{
    public class QueueProcessor : IQueueProcessor
    {
        private ReverseGeocoding reverseGeocoding = new ReverseGeocoding();

        private IService<Packet> service;
        private ICache<Packet> cache;

        public QueueProcessor(IService<Packet> service, ICache<Packet> cache)
        {
            this.service = service;
            this.cache = cache;
        }

        public Task Process(IEnumerable<Packet> packets, CancellationToken ct)
        {
            return Task.Run(() =>
            {
                foreach (Packet packet in packets)
                {
                    if (ct.IsCancellationRequested)
                    {
                        return;
                    }

                    ConsumePacket(packet, ct);
                }
                
            }, ct);
        }

        private async void ConsumePacket(Packet packet, CancellationToken ct)
        {
            try
            {
                cache.IncrementTotal();

                //receive only portuguese data
                Task<Country> packetCountry = reverseGeocoding.GetCountry(packet.Latitude, packet.Longitude);

                if (await packetCountry == Country.Portugal)
                {
                    List<Packet> vehiclePackets = cache.Add(packet.VehicleId, packet);

                    if (vehiclePackets != null)
                    {
                        await service.Post(packet.VehicleId, vehiclePackets);
                    }
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Something went wrong");
            }
        }
    }
}
