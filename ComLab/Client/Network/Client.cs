using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;

namespace ComLab.Network
{
    class Client
    {
        private IPEndPoint _ServerEndPoint;

        private Client()
        {
            NetworkComms.DisableLogging();
            
            NetworkComms.IgnoreUnknownPacketTypes = true;
            var serializer = DPSManager.GetDataSerializer<NetworkCommsDotNet.DPSBase.ProtobufSerializer>();
  //          NetworkComms.DefaultSendReceiveOptions.DataProcessors.Add(
//                DPSManager.GetDataProcessor<RijndaelPSKEncrypter>());
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            //RijndaelPSKEncrypter.AddPasswordToOptions(NetworkComms.DefaultSendReceiveOptions.Options, "awooo");

            //NetworkComms.AppendGlobalIncomingPacketHandler<ServerInfo>(ServerInfo.Header, ServerInfoReceived);
            
            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

            PeerDiscovery.OnPeerDiscovered += OnPeerDiscovered;


            //NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("PartialFileData", IncomingPartialFileData);

            //NetworkComms.AppendGlobalIncomingPacketHandler<SendInfo>("PartialFileDataInfo",
            //    IncomingPartialFileDataInfo);

            //NetworkComms.AppendGlobalConnectionCloseHandler(OnConnectionClose);

            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

        }

        private void _Start()
        {
            Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
        }

        public static void Start()
        {
            Instance._Start();
            if(Instance._ServerEndPoint == null)
                PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
        }

        //private async Task _FindServer()
        //{
        //    while (true)
        //        try
        //        {
        //            var start = DateTime.Now;
        //            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
        //            while ((DateTime.Now - start).TotalSeconds < 7)
        //            {
        //                if (Server != null)
        //                    break;
        //                await TaskEx.Delay(TimeSpan.FromSeconds(1));
        //            }
        //            return;
        //        }
        //        catch (Exception e)
        //        {
        //            await TaskEx.Delay(100);
        //        }


        //}

        public async Task<LoginResult> Login(string username, string password)
        {
            var pwd = new PasswordHash(password);
            var login = new Login
            {
                Username = username,
                Password = pwd.ToArray()
            };
            LoginResult result = null;

            NetworkComms.AppendGlobalIncomingPacketHandler<LoginResult>(LoginResult.Header, (h, c, r)=>result=r);

            await login.Send(_ServerEndPoint);
            for (var i = 0; i < 74; i++)
            {
                if (result != null) return result;
               await Task.Delay(7);
            }

            return result;
        }

        private async void OnPeerDiscovered(ShortGuid peeridentifier, Dictionary<ConnectionType, List<EndPoint>> endPoints)
        {
            var info = new LogonInfo();
            info.ComputerName = Environment.MachineName;

            var eps = endPoints[ConnectionType.UDP];
            var localEPs = Connection.AllExistingLocalListenEndPoints();

            foreach (var value in eps)
            {
                var ip = value as IPEndPoint;
                if (ip?.AddressFamily != AddressFamily.InterNetwork)
                    continue;

                foreach (var localEP in localEPs[ConnectionType.UDP])
                {
                    var lEp = (IPEndPoint)localEP;

                    if (lEp.AddressFamily != AddressFamily.InterNetwork) continue;

                    if (!ip.Address.IsInSameSubnet(lEp.Address))
                        continue;
                    info.IP = lEp.Address.ToString();
                    info.Port = lEp.Port;
                    //if (localEPs.ContainsKey(ConnectionType.TCP))
                    //{
                    //    var tcp = localEPs[ConnectionType.TCP]
                    //        .FirstOrDefault(x => ip.Address.AddressFamily == AddressFamily.InterNetwork && ip.Address.IsInSameSubnet(((IPEndPoint)x).Address)) as IPEndPoint;
                    //    info.DataPort = tcp?.Port ?? 0;
                    //}
                    _ServerEndPoint = ip;
                    await info.Send(ip);
                    break;
                }

            }
        }

        /// <summary>
        /// If a connection is closed we clean-up any incomplete ReceivedFiles
        /// </summary>
        /// <param name="conn">The closed connection</param>
        //private void OnConnectionClose(Connection conn)
        //{
        //    ReceivedFile[] filesToRemove = null;

        //    lock (syncRoot)
        //    {
        //        //Remove any associated data from the caches
        //        incomingDataCache.Remove(conn.ConnectionInfo);
        //        incomingDataInfoCache.Remove(conn.ConnectionInfo);

        //        //Remove any non completed files
        //        if (receivedFilesDict.ContainsKey(conn.ConnectionInfo))
        //        {
        //            filesToRemove = (from current in receivedFilesDict[conn.ConnectionInfo] where !current.Value.IsCompleted select current.Value).ToArray();
        //            receivedFilesDict[conn.ConnectionInfo] = (from current in receivedFilesDict[conn.ConnectionInfo] where current.Value.IsCompleted select current).ToDictionary(entry => entry.Key, entry => entry.Value);
        //        }
        //    }

        //    awooo.Context.Post(d =>
        //    {
        //        lock (syncRoot)
        //        {
        //            if (filesToRemove != null)
        //            {
        //                foreach (ReceivedFile file in filesToRemove)
        //                {
        //                    receivedFiles.Remove(file);
        //                    file.Close();
        //                }
        //            }
        //        }
        //    }, null);

        //}


        private static Client _instance;
        public static Client Instance => _instance ?? (_instance = new Client());

    }
}
