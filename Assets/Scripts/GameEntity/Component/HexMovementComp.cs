using System;
using System.Collections;
using System.Collections.Generic;
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

            var path = _map.GetPathTo(tile.Item1, reachable, jumpHeight);

            var targetTile = path.Last();
            if (targetTile.HasEntityOnTop)
                return false;

            var oldTile = _map.GetTile(Position);
            oldTile.Entity = null;

            targetTile.Entity = _owner;
            Position = targetTile.Position;

            path.Insert(0, oldTile);
            StartCoroutine(MoveAlongPath(path, arrivalCallback));

            MovementLeft -= requiredMovement;
            return true;
        }

        private IEnumerator MoveAlongPath(List<HexTile> path, Action callback)
        {
            for (int i = 1; i < path.Count; i++)
            {
                var lastTile = path[i - 1];
                var targetTile = path[i];

                yield return StartCoroutine(AnimatedMoveTo(targetTile.GetTop(), targetTile.Height - lastTile.Height));
            }

            callback();
        }

        private IEnumerator AnimatedMoveTo(Vector3 target, int heightDiff)
        {
            var anim = GetComponent<Animator>();

            var dir = target - transform.position;
            dir.y = 0f;

            var targetRot = Quaternion.LookRotation(dir, Vector3.up);
            var currentRot = transform.rotation;
            while (currentRot != targetRot)
            {
                currentRot = Quaternion.RotateTowards(currentRot, targetRot, 4);

                transform.rotation = currentRot;

                yield return null;
            }

            anim.SetTrigger("move");
            anim.SetFloat("heightDiff", heightDiff);
            yield return null;

            while (anim.GetCurrentAnimatorStateInfo(0).IsName("ForwardMovement"))
            {
                yield return null;
            }
            transform.position = target;
        }
    }
}
