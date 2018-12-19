using System.Collections.Generic;
using Klaesh.Core;
using UnityEngine;

namespace Klaesh.Game.Job
{
    public delegate void AllJobsFinishedEvent();

    public interface IJobManager
    {
        bool NoJobsLeft { get; }

        event AllJobsFinishedEvent AllJobsFinished;

        void AddJob(IJob job);
        void ExecuteJobs();
    }

    public class JobManager : ManagerBehaviour, IJobManager
    {
        private Queue<IJob> _jobQueue;
        private IJob _currentJob;

        public bool NoJobsLeft => _currentJob == null && _jobQueue.Count == 0;

        public event AllJobsFinishedEvent AllJobsFinished;

        protected override void OnAwake()
        {
            _locator.RegisterSingleton<IJobManager>(this);

            _jobQueue = new Queue<IJob>();
            _currentJob = null;
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
            if (_currentJob != null)
                return;

            if (_jobQueue.Count == 0)
            {
                AllJobsFinished?.Invoke();
                return;
            }

            _currentJob = _jobQueue.Dequeue();
            _currentJob.OnComplete += OnJobComplete;

            Debug.Log($"[JobManager] Starting Job! {_currentJob}");
            _currentJob.StartJob();
        }

        private void OnJobComplete(IJob job)
        {
            Debug.Log($"[JobManager] Job complete! {job}");

            _currentJob.OnComplete -= OnJobComplete;
            _currentJob = null;

            ExecuteJobs();
        }
    }
}
