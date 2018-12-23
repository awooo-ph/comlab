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

    public enum LockClientReasons
    {
        ClassEnded,
        InstructorLogout,
        StudentLogout
    }

    [ProtoContract]
    class LockClient : Packet<LockClient>
    {
        [ProtoMember(1)]
        public LockClientReasons LockReason { get; set; } = LockClientReasons.ClassEnded;
    }

    [ProtoContract]
    class RestartClient : Packet<RestartClient> { }

    [ProtoContract]
    class ShutdownClient : Packet<ShutdownClient> { }
}