using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;

namespace Klaesh.Game
{
    public static class EntityExtensions
    {
        public static bool CanMove(this Entity entity)
        {
            var moveComp = entity.GetComponent<HexMovementComp>();
            if (moveComp == null)
                return false;
            return moveComp.MovementLeft > 0;
        }

        public static bool IsAttackable(this Entity entity)
        {
            // Has to have a position on the map
            if (entity.GetComponent<HexPosComp>() == null)
                return false;

            // TODO: what about attacking neutral things? like debris, chests, ...
            if (entity.GetModule<SquadMember>() == null)
                return false;

            if (entity.GetComponent<VitalityComp>() == null)
                return false;

            return true;
        }

        public static bool HasUsableWeapon(this Entity entity)
        {
            // Has to have a position on the map
            if (entity.GetComponent<HexPosComp>() == null)
                return false;

            var weapon = entity.GetComponent<WeaponComp>();
            if (weapon == null)
                return false;

            if (weapon.UsesLeft <= 0)
                return false;

            var squad = entity.GetModule<SquadMember>();
            if (squad == null)
                return false;

            return true;
        }

        public static List<Entity> AttackableEntitiesInRange(this Entity entity)
        {
            var squad = entity.GetModule<SquadMember>().Squad;
            var pos = entity.GetComponent<HexPosComp>().Position;
            var weapon = entity.GetComponent<WeaponComp>();

            // TODO: consider height diff!

            var gem = ServiceLocator.Instance.GetService<IEntityManager>();
            return gem.GetEntities(e =>
                e.IsAttackable() &&
                e.GetModule<SquadMember>().Squad != squad &&
                HexFun.Distance(e.GetComponent<HexPosComp>().Position, pos) <= weapon.range
            ).ToList();
        }

        public static bool CanAttack(this Entity entity)
        {
            return entity.HasUsableWeapon() && entity.AttackableEntitiesInRange().Count > 0;
        }
    }
}
