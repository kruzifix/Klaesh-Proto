using Klaesh.Core.Message;

namespace Klaesh.Hex
{
    public class HexMapInitializedMessage : MessageBase<IHexMap>
    {
        public HexMapInitializedMessage(object sender, IHexMap content)
            : base(sender, content)
        { }
    }
}
