using System;
using System.Collections;
using Klaesh.Core;
using Klaesh.Game.Data;
using Klaesh.GameEntity;
using Klaesh.GameEntity.Component;
using Klaesh.GameEntity.Module;
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
            // PLAY ANIMATION
            // parallel ausführen???
            var routine1 = _starter.StartCoroutine(RotateTowards(Entity.transform, Target.transform.position));
            var routine2 = _starter.StartCoroutine(RotateTowards(Target.transform, Entity.transform.position));

            yield return routine1;
            yield return routine2;

            var weapon = Entity.GetComponent<WeaponComp>();
            var vitality = Target.GetComponent<VitalityComp>();

            // MELEE ANIMATION; TODO: TEST FOR OTHER
            var myAnim = Entity.GetComponent<Animator>();
            var targetAnim = Target.GetComponent<Animator>();

            var animEvent = Entity.GetComponent<AnimationEventComp>();
            EventHandler<AnimationEvent> hand = delegate(object sender, AnimationEvent e)
            {
                vitality.Damage(weapon.damage);
                weapon.Use();

                if (vitality.Health <= 0)
                    targetAnim.SetTrigger("fallOver");
                else
                    targetAnim.SetTrigger("shake");
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
