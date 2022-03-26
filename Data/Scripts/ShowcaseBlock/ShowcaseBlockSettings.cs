using ProtoBuf;

namespace ShowcaseBlock
{
    [ProtoContract(UseProtoMembersOnly = true)]
    public class ShowcaseBlockSettings
    {
        [ProtoMember(1)]
        public ushort rotationX;

        [ProtoMember(2)]
        public ushort rotationY;

        [ProtoMember(3)]
        public ushort rotationZ;
    }
}
