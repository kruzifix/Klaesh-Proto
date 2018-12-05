using System;
using System.Collections;
using System.Linq;
using Klaesh.Core;
using Klaesh.Game;
using Klaesh.Hex;
using UnityEngine;

namespace Klaesh.GameEntity.Component
{
    public class HexMovementComp : MonoBehaviour
    {
        private IHexMap _map;
        private Entity _owner;

        public HexCubeCoord Position { get; private set; }
        public int MovementLeft { get; private set; }

        public int maxDistance;
        public int jumpHeight;

        private void Awake()
        {
            _map = ServiceLocator.Instance.GetService<IHexMap>();
            _owner = GetComponent<Entity>();
        }

        public void SetPosition(IHexCoord position)
        {
            var tile = _map.GetTile(position);

            Position = position.CubeCoord;
            tile.Entity = _owner;
            transform.position = tile.GetTop();
        }

        public void OnNextTurn()
        {
            MovementLeft = maxDistance;
        }

        public bool StartMovingTo(IHexCoord position, Action arrivalCallback)
        {
            if (MovementLeft <= 0)
                return false;

            var reachable = _map.GetReachableTiles(Position, MovementLeft, jumpHeight);

            var tile = reachable.Where(t => t.Item1.Position == position.CubeCoord).FirstOrDefault();
            if (tile == null)
                return false;
            int requiredMovement = tile.Item2;
            if (requiredMovement > MovementLeft)
                return false;

            if (!StartMovingToInternal(position, arrivalCallback))
                return false;

            MovementLeft -= requiredMovement;
            return true;
        }

        private bool StartMovingToInternal(IHexCoord position, Action arrivalCallback)
        {
            var tile = _map.GetTile(position);

            if (tile.HasEntityOnTop)
                return false;

            var oldTile = _map.GetTile(Position);
            oldTile.Entity = null;

            tile.Entity = _owner;
            Position = position.CubeCoord;

            StartCoroutine(AnimatedMoveTo(tile.GetTop(), arrivalCallback));

            return true;
        }

        private IEnumerator AnimatedMoveTo(Vector3 target, Action callback)
        {
            var anim = GetComponent<Animator>();

            var targetRot = Quaternion.LookRotation(target - transform.position, Vector3.up);
            var currentRot = transform.rotation;
            while (currentRot != targetRot)
            {
                currentRot = Quaternion.RotateTowards(currentRot, targetRot, 2);

                transform.rotation = currentRot;

                yield return null;
            }

            anim.SetBool("walking", true);

            while (Vector3.Distance(transform.position, target) > 0.1f)
            {
                transform.position = Vector3.MoveTowards(transform.position, target, 0.02f);

                yield return null;
            }

            transform.position = target;
            anim.SetBool("walking", false);

            callback();
        }
    }
}
