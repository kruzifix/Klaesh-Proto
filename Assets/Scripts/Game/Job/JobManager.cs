using System.Collections.Generic;
using Klaesh.Core;

namespace Klaesh.Game.Job
{
    public interface IJobManager
    {
        void AddJob(IJob job);

        void ExecuteJobs();
    }

    public class JobManager : ManagerBehaviour, IJobManager
    {
        private Queue<IJob> _jobQueue;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IJobManager>(this);

            _jobQueue = new Queue<IJob>();
        }

        //private void OnDestroy()
        //{
        //    _locator.DeregisterSingleton<IJobManager>();
        //}

        public void AddJob(IJob job)
        {
            _jobQueue.Enqueue(job);
        }

        public void ExecuteJobs()
        {
            if (_jobQueue.Count == 0)
                return;
            var job = _jobQueue.Dequeue();
            job.OnComplete += OnJobComplete;

            job.StartJob();
        }

        private void OnJobComplete(IJob job)
        {
            ExecuteJobs();
        }
    }
}
