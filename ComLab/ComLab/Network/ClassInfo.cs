using System;
using System.Collections.Generic;
using System.Text;
using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class ClassInfo:Packet<ClassInfo>
    {
        [ProtoMember(1)] public string ClassName { get; set; }
        [ProtoMember(2)] public string Schedule { get; set; }
        [ProtoMember(3)] public string Instructor { get; set; }
        [ProtoMember(4)] public string WelcomMessage { get; set; }
        [ProtoMember(5)] public bool ClassStarted { get; set; }
        [ProtoMember(6)] public bool HasInstructor { get; set; }
    }

    [ProtoContract]
    class ClientInfo : Packet<ClientInfo>
    {
        [ProtoMember(1)]
        public string ComputerName { get; set; }
    }
}
