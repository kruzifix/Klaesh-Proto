using Klaesh.Core.Message;

public class HexTileSelectedMessage : GenericMessage<HexTile>
{
    public HexTileSelectedMessage(object sender, HexTile tile)
        : base(sender, tile)
    { }
}
