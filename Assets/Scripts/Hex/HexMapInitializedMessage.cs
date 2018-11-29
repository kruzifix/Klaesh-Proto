using Klaesh.Core.Message;

namespace Klaesh.Hex
{
    public class HexMapInitializedMessage : GenericMessage<IHexMap>
    {
        public HexMapInitializedMessage(object sender, IHexMap content)
            : base(sender, content)
        { }
    }
}
