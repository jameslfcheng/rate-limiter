﻿using RateLimiter.Interfaces;
using System;
using System.Collections.Concurrent;

namespace RateLimiter.Rules
{
    /// <summary>
    /// A rate limiting rule that allows a maximum number of requests within a specified timespan.
    /// </summary>
    public class TimespanSinceLastCallRule : IRateLimitingRule
    {
        private readonly TimeSpan _minTimespan;
        private readonly ConcurrentDictionary<string, DateTime> _requestsStorage = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="TimespanSinceLastCallRule"/> class.
        /// </summary>
        /// <param name="minTimespan">Minimum timespan that has elapsed since the last request</param>
        public TimespanSinceLastCallRule(TimeSpan minTimespan)
        {
            _minTimespan = minTimespan;
        }

        /// <summary>
        /// Determines whether a request is allowed for the given client, based on the minimum timespan that has elapsed since the last request.
        /// </summary>
        /// <param name="clientId">Requesting client id</param>
        /// <returns>Result if request is allowed</returns>
        public bool IsRequestAllowed(string clientId)
        {
            var nowDateTime = DateTime.Now;
            var lastRequestTime = _requestsStorage.GetOrAdd(clientId, DateTime.MinValue);
            var elapsedTimespan = nowDateTime - lastRequestTime;

            _requestsStorage[clientId] = nowDateTime;

            // Allow the request if the elapsed timespan is greater than or equal to the minimum timespan
            return elapsedTimespan >= _minTimespan;
        }
    }
}
