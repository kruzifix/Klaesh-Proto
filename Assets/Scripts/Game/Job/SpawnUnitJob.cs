using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Klaesh.Core;
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

            Completed();
        }
    }
}
