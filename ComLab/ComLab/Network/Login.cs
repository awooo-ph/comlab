using ProtoBuf;

namespace ComLab.Network
{
    [ProtoContract]
    class Login:Packet<Login>
    {
        [ProtoMember(1)]
        public string Username { get; set; }
        [ProtoMember(2)]
        public string Password { get; set; }
    }

    [ProtoContract]
    class LoginResult : Packet<LoginResult>
    {
        [ProtoMember(1)]
        public string Message { get; set; }
        [ProtoMember(2)]
        public bool Success { get; set; }
        [ProtoMember(3)]
        public Student Student { get; set; }


    }

    [ProtoContract]
    class ClassInfo
    {
        [ProtoMember(1)]
        public string ClassName { get; set; }
        [ProtoMember(2)]
        public string Schedule { get; set; }
        [ProtoMember(3)]
        public string Instructor { get; set; }
    }
}