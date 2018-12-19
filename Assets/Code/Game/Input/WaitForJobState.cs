using Klaesh.Game.Job;

namespace Klaesh.Game.Input
{
    public class WaitForJobState : AbstractInputState
    {
        private IJob _job;
        private IInputState _targetState;

        public WaitForJobState(InputStateMachine context, IJob job, IInputState targetState)
            : base(context)
        {
            _job = job;
            _job.OnComplete += OnJobComplete;

            _targetState = targetState;
        }

        public override void Enter()
        {
            ExecuteAndSendJob(_job);
        }

        public override void Exit()
        {
            _job.OnComplete -= OnJobComplete;
        }

        private void OnJobComplete(IJob job)
        {
            Context.SetState(_targetState);
        }
    }
}
