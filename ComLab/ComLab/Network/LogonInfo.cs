using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class LogonInfo:Packet<LogonInfo>
    {
        [ProtoMember(1)]
        public string IP { get; set; }
        [ProtoMember(2)]
        public int Port { get; set; }
        [ProtoMember(3)] public string ComputerName { get; set; }
    }
}
