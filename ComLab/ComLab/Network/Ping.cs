using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class Ping:Packet<Ping>
    {
        [ProtoMember(1)] public DateTime DateTime { get; set; } = DateTime.Now;
        [ProtoMember(2)] public long Id { get; set; }
    }

    [ProtoContract]
    class Pong:Packet<Pong>
    {
        [ProtoMember(1)] public DateTime DateTime { get; set; } = DateTime.Now;
        [ProtoMember(2)] public long PingId { get; set; }
    }
}
