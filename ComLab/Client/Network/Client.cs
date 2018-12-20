using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ComLab.ViewModels;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;

namespace ComLab.Network
{
    class Client:ViewModelBase
    {
        private IPEndPoint _ServerEndPoint;
        private DateTime _LastPing;
        private Client()
        {
            NetworkComms.DisableLogging();
            
            NetworkComms.IgnoreUnknownPacketTypes = true;
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();
            NetworkComms.DefaultSendReceiveOptions.DataProcessors.Add(
                DPSManager.GetDataProcessor<RijndaelPSKEncrypter>());
            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            RijndaelPSKEncrypter.AddPasswordToOptions(NetworkComms.DefaultSendReceiveOptions.Options, Utility.PSK);

            NetworkComms.AppendGlobalIncomingPacketHandler<Ping>(Ping.Header, PingHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<ClassInfo>(ClassInfo.Header, ClassInfoHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<InstructorLogin>(InstructorLogin.Header, InstructorLoginHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<ClientInfo>(ClientInfo.Header, ClientInfoHandler);
            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

            PeerDiscovery.OnPeerDiscovered += OnPeerDiscovered;

            //NetworkComms.AppendGlobalIncomingPacketHandler<byte[]>("PartialFileData", IncomingPartialFileData);

            //NetworkComms.AppendGlobalIncomingPacketHandler<SendInfo>("PartialFileDataInfo",
            //    IncomingPartialFileDataInfo);

            //NetworkComms.AppendGlobalConnectionCloseHandler(OnConnectionClose);

            PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);

        }

        private void ClientInfoHandler(PacketHeader packetheader, Connection connection, ClientInfo incomingobject)
        {
            MainViewModel.Instance.UpdateInfo(incomingobject);
        }

        private void InstructorLoginHandler(PacketHeader packetheader, Connection connection, InstructorLogin incomingobject)
        {
            MainViewModel.Instance.Instructor = incomingobject.Fullname;
            MainViewModel.Instance.PageIndex = MainViewModel.NO_CLASS_INDEX;
        }

        private void ClassInfoHandler(PacketHeader packetheader, Connection connection, ClassInfo incomingobject)
        {
            MainViewModel.Instance.UpdateClassInfo(incomingobject);
        }

        private async void PingHandler(PacketHeader packetheader, Connection connection, Ping incomingobject)
        {
            IsConnected = true;
            _LastPing = DateTime.Now;
            var pong = new Pong
            {
                PingId = incomingobject.Id
            };
            await pong.Send(connection.ConnectionInfo.RemoteEndPoint);
        }

        private async void _Start()
        {
            while (true)
            {
                try
                {
                    Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(777);
                }
            }
        }

        public static void Start()
        {
            Instance._Start();
            Instance._FindServer();
        }

        private bool _IsConnected;

        public bool IsConnected
        {
            get => _IsConnected;
            set
            {
                if (value == _IsConnected) return;
                _IsConnected = value;
                OnPropertyChanged(nameof(IsConnected));
            }
        }

        private async void _FindServer()
        {
            while (true)
                try
                {
                    if (_ServerEndPoint == null || (DateTime.Now - _LastPing).TotalSeconds > 47)
                    {
                        _ServerEndPoint = null;
                        var start = DateTime.Now;
                        PeerDiscovery.DiscoverPeersAsync(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
                        while ((DateTime.Now - start).TotalSeconds < 7)
                        {
                            if (_ServerEndPoint != null) break;
                            await Task.Delay(777);
                        }

                        if (_ServerEndPoint == null)
                            IsConnected = false;
                    }

                    await Task.Delay(777);
                }
                catch (Exception e)
                {
                    await Task.Delay(100);
                }
        }

        public async Task<LoginResult> Login(string username, string password)
        {
            IsConnected = true;
            var login = new Login
            {
                Username = username,
                Password = password
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

            if(MainViewModel.Instance.PageIndex==MainViewModel.CONNECTING_INDEX)
                MainViewModel.Instance.PageIndex = MainViewModel.INSTRUCTOR_INDEX;
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
