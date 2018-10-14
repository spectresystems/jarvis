// Licensed to Spectre Systems AB under one or more agreements.
// Spectre Systems AB licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Jarvis.Core.Diagnostics;
using Jarvis.Core.Threading;
using JetBrains.Annotations;

namespace Jarvis.Services
{
    [UsedImplicitly]
    public sealed class ServiceOrchestrator
    {
        private readonly IJarvisLog _log;
        private readonly List<Task> _tasks;
        private readonly List<IBackgroundWorker> _workers;
        private readonly ManualResetEvent _stopped;

        private CancellationTokenSource _source;

        public ServiceOrchestrator(IEnumerable<IBackgroundWorker> workers, IJarvisLog log)
        {
            _log = new LogDecorator("ServiceOrchestrator", log);
            _workers = new List<IBackgroundWorker>(workers);
            _tasks = new List<Task>();
            _stopped = new ManualResetEvent(true);
        }

        public void Start()
        {
            if (!_stopped.WaitOne(0))
            {
                return;
            }

            _stopped.Reset();
            _source = new CancellationTokenSource();
            _tasks.Clear();

            // Start all tasks.
            foreach (var worker in _workers)
            {
                if (!worker.Enabled())
                {
                    _log.Information($"Service {worker.Name} will not be started.");
                    continue;
                }

                _tasks.Add(new TaskWrapper(worker, _log).Start(_source));
            }

            // Configure the tasks.
            Task.WhenAll(_tasks).ContinueWith(task => _stopped.Set());
        }

        public void Stop()
        {
            if (!_source.IsCancellationRequested)
            {
                _log.Information("We were instructed to stop.");
                _source.Cancel();
            }
        }

        public void Join()
        {
            _log.Information("Waiting for background workers to stop...");
            _stopped.WaitOne();
            _log.Information("Background workers stopped.");
        }
    }
}
