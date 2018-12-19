using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Klaesh.Core;
using Klaesh.Core.Message;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using Klaesh.Utility;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Game.Job
{
    public class MoveUnitJob : EntityJob
    {
        private ICoroutineStarter _starter;

        [JsonProperty("path")]
        public List<HexCubeCoord> Path { get; set; }

        public MoveUnitJob() { }

        public MoveUnitJob(Entity entity, List<HexCubeCoord> path)
            : base(entity)
        {
            Path = path;
        }

        public override void StartJob()
        {
            _starter = ServiceLocator.Instance.GetService<ICoroutineStarter>();

            _starter.StartCoroutine(MoveAlongPath());
        }

        private IEnumerator MoveAlongPath()
        {
            var bus = ServiceLocator.Instance.GetService<IMessageBus>();
            var map = ServiceLocator.Instance.GetService<IHexMap>();
            var movement = Entity.GetComponent<HexMovementComp>();

            var gem = ServiceLocator.Instance.GetService<IEntityManager>();

            var arrows = new List<Entity>();

            var lastTile = map.GetTile(movement.Position);
            for (int i = 0; i < Path.Count; i++)
            {
                var tile = map.GetTile(Path[i]);
                tile.SetColor(Colors.TileDistances[i]);

                var arrow = gem.CreateEntity("arrow");
                arrows.Add(arrow);
                var line = arrow.GetComponent<LineArrowComp>();
                line.UpdateLine(lastTile.GetTop(), tile.GetTop());
                line.SetColor(Entity.GetModule<SquadMember>().Squad.Config.Color);

                lastTile = tile;
            }

            var oldTile = map.GetTile(movement.Position);
            oldTile.Entity = null;

            var endTile = map.GetTile(Path.Last());
            endTile.Entity = Entity;

            movement.Position = Path.Last();
            movement.MovementLeft -= Path.Count;

            bus.Publish(new FocusCameraMessage(this, endTile.GetTop()));

            lastTile = oldTile;
            for (int i = 0; i < Path.Count; i++)
            {
                var targetTile = map.GetTile(Path[i]);

                yield return _starter.StartCoroutine(AnimatedMoveTo(targetTile.GetTop(), targetTile.Height - lastTile.Height));
                lastTile = targetTile;
            }

            // Cleanup!
            Path.ForEach(c => map.GetTile(c).SetColor(Color.white));
            gem.KillEntities(arrows);

            Completed();
        }

        private IEnumerator AnimatedMoveTo(Vector3 target, int heightDiff)
        {
            var anim = Entity.gameObject.GetComponent<Animator>();
            var transform = Entity.transform;

            var dir = target - transform.position;
            dir.y = 0f;

            var targetRot = Quaternion.LookRotation(dir, Vector3.up);
            var currentRot = transform.rotation;
            while (currentRot != targetRot)
            {
                currentRot = Quaternion.RotateTowards(currentRot, targetRot, 6);

                transform.rotation = currentRot;

                yield return null;
            }

            anim.SetTrigger("move");
            anim.SetFloat("heightDiff", heightDiff);

            yield return null;

            while (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                yield return null;

            transform.position = target;
        }
    }
}
