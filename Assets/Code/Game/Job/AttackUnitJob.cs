using System;
using System.Collections;
using Klaesh.Core;
using Klaesh.Game.Data;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
using Klaesh.Hex;
using Newtonsoft.Json;
using UnityEngine;

namespace Klaesh.Game.Job
{
    public class AttackUnitJob : EntityJob
    {
        private ICoroutineStarter _starter;
        private Entity _cachedTarget;

        private Entity Target
        {
            get
            {
                if (_cachedTarget == null)
                    _cachedTarget = ServiceLocator.Instance.GetService<IGameManager>().ResolveEntityRef(TargetEntityRef);
                return _cachedTarget;
            }
        }

        [JsonProperty("target")]
        public SquadEntityRefData TargetEntityRef { get; set; }

        public AttackUnitJob() { }

        public AttackUnitJob(Entity entity, Entity target)
            : base(entity)
        {
            _cachedTarget = target;
            TargetEntityRef = target.GetModule<SquadMember>().RefData;
        }

        public override void StartJob()
        {
            _starter = ServiceLocator.Instance.GetService<ICoroutineStarter>();

            _starter.StartCoroutine(DoAttack());
        }

        private IEnumerator DoAttack()
        {
            var routine1 = _starter.StartCoroutine(RotateTowards(Entity.transform, Target.transform.position));
            var routine2 = _starter.StartCoroutine(RotateTowards(Target.transform, Entity.transform.position));

            yield return routine1;
            yield return routine2;

            var weapon = Entity.GetComponent<WeaponComp>();
            var vitality = Target.GetComponent<VitalityComp>();

            // MELEE ANIMATION; TODO: RANGED ATTACK? effects for individual attacks???
            var myAnim = Entity.GetComponent<Animator>();
            var targetAnim = Target.GetComponent<Animator>();

            var animEvent = Entity.GetComponent<AnimationEventComp>();
            EventHandler<AnimationEvent> hand = delegate(object sender, AnimationEvent e)
            {
                vitality.Damage(weapon.damage);
                weapon.Use();

                targetAnim.SetTrigger(vitality.IsDead ? "fallOver" : "shake");
            };

            animEvent.AnimationEvent += hand;

            myAnim.SetTrigger("attack");

            yield return null;
            while (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
                yield return null;

            // can be Idle or FallOver (as Animator stays at FallOver)
            //while (!targetAnim.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            //    yield return null;

            animEvent.AnimationEvent -= hand;

            // do scrolling text!

            yield return new WaitForSecondsRealtime(0.5f);

            if (vitality.IsDead)
            {
                var pos = Target.GetComponent<HexPosComp>().Position;

                var gem = ServiceLocator.Instance.GetService<IEntityManager>();
                gem.KillEntity(Target);

                var map = ServiceLocator.Instance.GetService<IHexMap>();
                var tile = map.GetTile(pos);

                Debug.Log($"AFTER DAETH: entityontop {tile.HasEntityOnTop}  {tile.Entity}");
            }

            Completed();
        }

        private IEnumerator RotateTowards(Transform transform, Vector3 target)
        {
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
        }
    }
}
