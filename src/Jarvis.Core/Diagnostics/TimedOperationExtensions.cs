using System;
using System.Diagnostics;
using System.Linq;

namespace Jarvis.Core.Diagnostics
{
    public static class TimedOperationExtensions
    {
        public static IDisposable TimedOperation(this IJarvisLog logger, string messageTemplate,
            params object[] propertyValues)
        {
            return logger.TimedOperation(LogLevel.Information, messageTemplate, propertyValues);
        }

        public static IDisposable TimedOperation(this IJarvisLog logger, LogLevel level, string messageTemplate,
            params object[] propertyValues)
        {
            return logger.TimedOperation(level, level, messageTemplate, propertyValues);
        }

        public static IDisposable TimedOperation(this IJarvisLog logger, LogLevel completionLevel, LogLevel cancelledLevel, string messageTemplate,
            params object[] propertyValues)
        {
            return new Operation(logger, messageTemplate, propertyValues, completionLevel, cancelledLevel);
        }
    }

    public class Operation : IDisposable
    {
        private readonly IJarvisLog _logger;
        private readonly string _messageTemplate;
        private readonly object[] _propertyValues;
        private readonly long _startTime;
        private long _stopTime;
        private Guid _operationId;
        private IDisposable _operationScope;

        public bool IsCompleted { get; private set; }
        public bool IsCancelled { get; private set; }
        public LogLevel CompletionLevel { get; }
        public LogLevel CancelledLevel { get; }
        public TimeSpan Elapsed => IsCompleted ? new TimeSpan(_stopTime - _startTime) : new TimeSpan(Stopwatch.GetTimestamp() - _startTime);

        public Operation(IJarvisLog logger, string messageTemplate, object[] propertyValues, LogLevel completionLevel, LogLevel? cancelledLevel = null)
        {
            _logger = logger;
            _messageTemplate = messageTemplate;
            _propertyValues = propertyValues;
            CompletionLevel = completionLevel;
            CancelledLevel = cancelledLevel ?? completionLevel;
            _operationId = Guid.NewGuid();
            _operationScope = logger.BeginScope("operationId", _operationId);
            _startTime = Stopwatch.GetTimestamp();
        }

        public void Cancel()
        {
            IsCancelled = true;
            Stop();
        }

        public void Complete()
        {
            Stop();
        }

        public void Dispose()
        {
            Stop();
        }

        private void Stop()
        {
            _stopTime = Stopwatch.GetTimestamp();
            IsCompleted = true;
            Write();
            _operationScope.Dispose();
        }

        private void Write()
        {
            string outcome;
            LogLevel logLevel;
            if (IsCancelled)
            {
                outcome = "cancelled";
                logLevel = CancelledLevel;
            }
            else
            {
                outcome = "completed";
                logLevel = CompletionLevel;
            }
            var finalMsgTempalte = $"{_messageTemplate} {{outcome:l}} in {{elapsed:0.0}} ms.";
            var finalPropertyValues = _propertyValues
                .Concat(new object[] { outcome, Elapsed.TotalMilliseconds })
                .ToArray();
            _logger.Write(logLevel, null, finalMsgTempalte, finalPropertyValues);
        }
    }
}
