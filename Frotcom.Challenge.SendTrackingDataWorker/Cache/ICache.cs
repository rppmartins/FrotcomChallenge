using Frotcom.Challenge.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Frotcom.Challenge.SendTrackingDataWorker.Cache
{
    public interface ICache<T>
    {
        void IncrementTotal();
        List<Packet> Add(int key, T info);
        void GetEntityValues(object timerState);
    }
}
