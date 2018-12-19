using System;
using Klaesh.Core;
using Klaesh.Game.Job;
using Klaesh.Input;
using Klaesh.Network;
using UnityEngine;

namespace Klaesh.Game.Input
{
    public interface IInputState : IInputProcessor, IGameInputProcessor
    {
        InputStateMachine Context { get; }

        /// <summary>
        /// Is called when state machine enters this state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Is called when state machine exits this state.
        /// </summary>
        void Exit();
    }

    public abstract class AbstractInputState : IInputState
    {
        public InputStateMachine Context { get; }

        public AbstractInputState(InputStateMachine context)
        {
            Context = context;
        }

        /// <summary>
        /// Is called when state machine enters this state.
        /// </summary>
        public virtual void Enter() { }

        /// <summary>
        /// Is called when state machine exits this state.
        /// </summary>
        public virtual void Exit() { }

        public virtual void ProcessInput(InputCode code, object data) { }

        public virtual void OnEnter(GameObject go) { }
        public virtual void OnExit(GameObject go) { }
        public virtual void OnDown(GameObject go) { }
        public virtual void OnUp(GameObject go) { }
        public virtual void OnClick(GameObject go) { }

        protected bool ForwardCall<T>(GameObject go, Action<T> target)
            where T : MonoBehaviour
        {
            var comp = go.GetComponent<T>();
            if (comp != null)
            {
                target(comp);
                return true;
            }
            return false;
        }

        protected void ExecuteAndSendJob(IJob job)
        {
            var jm = ServiceLocator.Instance.GetService<IJobManager>();
            jm.AddJob(job);
            jm.ExecuteJobs();
            ServiceLocator.Instance.GetService<INetworker>().SendData(EventCode.DoJob, job);
        }
    }
}
