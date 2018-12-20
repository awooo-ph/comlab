using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using ComLab.ViewModels;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.Connections.UDP;
using ProtoBuf;

namespace ComLab.Network
{
  [ProtoContract]
    abstract class Packet<T>:ViewModelBase where T : Packet<T>
    {
        public static string Header => typeof(T).FullName;
        
        public override string ToString()
        {
            return Header;
        }

        public Task Send(EndPoint ep)
        {
            return Send(ep as IPEndPoint);
        }

        public async Task Send(IPEndPoint ep)
        {
            if (ep == null) return;
            var sent = false;
            while (!sent)
            {
                try
                {
                    UDPConnection.SendObject(Header, this, ep, NetworkComms.DefaultSendReceiveOptions,
                        ApplicationLayerProtocolStatus.Enabled);
                    sent = true;
                    break;
                }
                catch (Exception)
                {
                    await Task.Delay(100);
                }
            }
        }
    }
}
