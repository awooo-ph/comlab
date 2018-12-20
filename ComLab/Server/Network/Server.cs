using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using ComLab.Models;
using ComLab.ViewModels;
using NetworkCommsDotNet;
using NetworkCommsDotNet.Connections;
using NetworkCommsDotNet.DPSBase;
using NetworkCommsDotNet.Tools;

namespace ComLab.Network
{
    partial class Server
    {
        public static ObservableCollection<Terminal> Clients { get; } = new ObservableCollection<Terminal>(Terminal.GetAll());
        private static object _terminalsLock = new object();
        private static Task _pinger;

        private Server()
        {
            var serializer = DPSManager.GetDataSerializer<ProtobufSerializer>();

            NetworkComms.DefaultSendReceiveOptions.DataProcessors.Add(
                DPSManager.GetDataProcessor<RijndaelPSKEncrypter>());

            NetworkComms.DefaultSendReceiveOptions = new SendReceiveOptions(serializer,
                NetworkComms.DefaultSendReceiveOptions.DataProcessors, NetworkComms.DefaultSendReceiveOptions.Options);

            RijndaelPSKEncrypter.AddPasswordToOptions(NetworkComms.DefaultSendReceiveOptions.Options, Utility.PSK);

            NetworkComms.DisableLogging();

            NetworkComms.AppendGlobalIncomingPacketHandler<LogonInfo>(LogonInfo.Header, LogonInfoHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<Login>(Login.Header, LoginHandler);
            NetworkComms.AppendGlobalIncomingPacketHandler<Pong>(Pong.Header, PongHandler);

            PeerDiscovery.EnableDiscoverable(PeerDiscovery.DiscoveryMethod.UDPBroadcast);
        }

        private void PongHandler(PacketHeader packetheader, Connection connection, Pong pong)
        {
            var terminal = Clients.FirstOrDefault(x => x.IP == ((IPEndPoint) connection.ConnectionInfo.RemoteEndPoint).Address.ToString());
            if (terminal == null) return;
            terminal.LastHeartbeat = DateTime.Now;
            if (terminal.Status == TerminalStatus.Offline)
                terminal.Status = TerminalStatus.Online;
        }

       
        private async void LogonInfoHandler(PacketHeader packetheader, Connection connection, LogonInfo info)
        {
            var classInfo = MainViewModel.GetCurrentClass();
            var clientInfo = new ClientInfo();
            lock (_terminalsLock)
            {
                var terminal = Clients.FirstOrDefault(x => x.IP == info.IP);
                if (terminal == null)
                {
                    terminal = new Terminal
                    {
                        IP = info.IP,
                        Name = info.ComputerName
                    };
                    terminal.Save();
                    Core.Context?.Post(d => { Clients.Add(terminal); }, null);
                }

                clientInfo.ComputerName = terminal.Name;
                terminal.LogonEndPoint = new IPEndPoint(IPAddress.Parse(info.IP), info.Port);
                terminal.Status = TerminalStatus.Locked;
                terminal.LastHeartbeat = DateTime.Now;
            }

            await clientInfo.Send(connection.ConnectionInfo.RemoteEndPoint);
            await classInfo.Send(connection.ConnectionInfo.RemoteEndPoint);
        }
        
        private static Server _instance;
        public static Server Instance => _instance ?? (_instance = new Server());

        private bool _started;
        private async void _Start()
        {
            if (_started) return;
            _started = true;
            while (true)
            {
                try
                {
                    Connection.StartListening(ConnectionType.UDP, new IPEndPoint(IPAddress.Any, 0), true);
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(111);
                }    
            }
            
        }

        public static void Start()
        {
            Instance._Start();
            _pinger = Task.Factory.StartNew(PingClients);
        }

        public static void Stop()
        {
            _pinger?.Dispose();
        }
        private static async void PingClients()
        {
            while (true)
            {
                foreach (var terminal in Clients)
                {
                    if (!terminal.HasEndPoint) continue;
                    if ((DateTime.Now - terminal.LastHeartbeat).TotalSeconds < 7)
                    {
                        if (terminal.Status == TerminalStatus.Offline)
                            terminal.Status = TerminalStatus.Online;
                    }
                    else
                    {
                        if (terminal.Status != TerminalStatus.Offline)
                            terminal.Status = TerminalStatus.Offline;
                    }

                    terminal.PingId++;
                    var ping = new Ping
                    {
                        Id = terminal.PingId
                    };
                    await ping.Send(terminal.LogonEndPoint);
                    await ping.Send(terminal.IpEndPoint);
                }

                await Task.Delay(7777);
            }
        }

        public static async void Broadcast<T>(Packet<T> packet) where T:Packet<T>
        {
            foreach (var terminal in Clients)
            {
                if (!terminal.HasEndPoint || !terminal.Enabled) continue;
                await packet.Send(terminal.LogonEndPoint);
                await packet.Send(terminal.IpEndPoint);
            }
        }
    }
}
