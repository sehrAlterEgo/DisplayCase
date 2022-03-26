using ProtoBuf;

namespace ShowcaseBlock
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class ShowcaseBlockSettings
    {
        [ProtoMember(1)]
        public ushort rotationY;
    
        [ProtoMember(2)]
        public ushort rotationX;
    }
}
