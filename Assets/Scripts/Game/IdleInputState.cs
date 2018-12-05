using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.Game
{
    public class IdleInputState : BaseInputState
    {
        public override void OnEnabled()
        {
            // highlight active squad units
            var map = ServiceLocator.Instance.GetService<IHexMap>();

            map.DeselectAllTiles();

            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            foreach (var unit in gm.ActiveSquad.Members)
            {
                map.GetTile(unit.GetComponent<HexMovementComp>().Position).SetColor(gm.ActiveSquad.Config.Color);

                //foreach (var mod in unit.GetModules<MeshModule>().Where(m => m.Name == "WobblyCircle"))
                //{
                //    mod.Enabled = true;
                //}
            }
        }

        public override void OnDisabled()
        {
            //var gm = ServiceLocator.Instance.GetService<IGameManager>();
            //foreach (var unit in gm.ActiveSquad.Members)
            //{
            //    foreach (var mod in unit.GetModules<MeshModule>().Where(m => m.Name == "WobblyCircle"))
            //    {
            //        mod.Enabled = false;
            //    }
            //}
        }

        public override IInputState OnPickGameEntity(Entity entity)
        {
            var gm = ServiceLocator.Instance.GetService<IGameManager>();
            if (gm.IsPartOfActiveSquad(entity))
                return new EntitySelectedInputState(entity);

            return null;
        }

        public override IInputState OnPickHexTile(HexTile tile)
        {
            if (tile.HasEntityOnTop)
            {
                return OnPickGameEntity(tile.Entity);
            }

            ServiceLocator.Instance.GetService<IMessageBus>().Publish(new FocusCameraMessage(this, tile.GetTop()));
            return null;
        }
    }
}
