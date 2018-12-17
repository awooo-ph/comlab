using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;

namespace ComLab.Network
{
    class Server
    {
        private List<LogonInfo> _logonInfos = new List<LogonInfo>();

        private Server()
        {
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            NetworkComms.DisableLogging();

            NetworkComms.AppendGlobalIncomingPacketHandler<LogonInfo>(LogonInfo.Header, LogonInfoHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<Login>(Login.Header, LoginHandler);

            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
        }

        private async void LoginHandler(PacketHeader packetheader, Connection connection, Login incomingobject)
        {
            var result = new LoginResult
            {
                Message = "awooo",
                Success = true
            };
            await result.Send(connection.ConnectionInfo.RemoteEndPoint as IPEndPoint);
        }

        private void LogonInfoHandler(PacketHeader packetheader, Connection connection, LogonInfo info)
        {
            _logonInfos.Add(info);
        }

        private static Server _instance;
        public static Server Instance => _instance ?? (_instance = new Server());

        private bool _started = false;
        private void _Start()
        {
            if (_started) return;
            _started = true;

        }

        public static void Start()
        {
            Instance._Start();
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
        }
    }
}
