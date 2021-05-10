using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Frotcom.Challenge.SendTrackingDataWorker
{
    public interface IService<T>
    {
        Task Post(int id, List<T> info);
    }
}
