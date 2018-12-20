using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class InstructorLogin : Packet<InstructorLogin>
    {
        [ProtoMember(1)]
        public string Fullname { get; set; }
    }

    [ProtoContract]
    class InstructorLogout : Packet<InstructorLogout>
    {
        [ProtoMember(1)]
        public bool Lock { get; set; } = true;
    }
}