using DotSpatial.Positioning;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace GpsService.Model
{
    public struct LatestGpsData
    {
        public DateTime time;
        public Position3D position;
        public ushort satellites;
        public Speed speed5sec;
    }

    [ServiceContract]
    interface IGpsServiceContract
    {
        [OperationContract]
        bool IsConnected();

        [OperationContract]
        LatestGpsData GetLatest();

        [OperationContract]
        DateTime GpsTime();

        [OperationContract]
        Position GpsPosition();

        [OperationContract]
        Distance Elevation();

        [OperationContract]
        Speed Speed5Sec();

        [OperationContract]
        ushort Satellites();
    }
}
