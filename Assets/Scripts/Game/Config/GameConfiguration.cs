using System.Collections.Generic;
using System.Linq;

namespace Klaesh.Game.Config
{
    public interface IGameConfiguration
    {
        IHexMapConfiguration Map { get; }
        IList<ISquadConfiguration> Squads { get; }
    }

    public class GameConfiguration : IGameConfiguration
    {
        public HexMapConfiguration MapConfig { get; set; }
        public List<SquadConfiguration> SquadsConfig { get; set; }

        public IHexMapConfiguration Map => MapConfig;
        public IList<ISquadConfiguration> Squads => SquadsConfig.ToList<ISquadConfiguration>();
    }
}
