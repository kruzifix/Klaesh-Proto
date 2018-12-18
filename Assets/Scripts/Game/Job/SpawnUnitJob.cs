using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.GameEntity;
using Klaesh.Hex;
using Newtonsoft.Json;

namespace Klaesh.Game.Job
{
    public class SpawnUnitJob : AbstractJob
    {
        [JsonProperty("position")]
        public HexOffsetCoord Position { get; set; }

        [JsonProperty("entity")]
        public string EntityId { get; set; }

        public override void StartJob()
        {
            var em = ServiceLocator.Instance.GetService<IEntityManager>();
            var gm = ServiceLocator.Instance.GetService<IGameManager>();

            gm.ActiveSquad.AddMember(em, Position, EntityId);

            var bus = ServiceLocator.Instance.GetService<IMessageBus>();
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            bus.Publish(new FocusCameraMessage(this, map.GetTile(Position).GetTop()));

            Completed();
        }
    }
}
