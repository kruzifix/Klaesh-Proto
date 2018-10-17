using Klaesh.Core.Message;

public class HexMapInitializedMessage : GenericMessage<HexMap>
{
    public HexMapInitializedMessage(object sender, HexMap content)
        : base(sender, content)
    { }
}
