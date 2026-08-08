using System;
using System.Collections.Concurrent;

namespace ApplitoolsEyesHelper.Runtime
{
    internal static class EyesSessionRegistry
    {
        private static readonly ConcurrentDictionary<string, EyesSession> Sessions = new ConcurrentDictionary<string, EyesSession>(StringComparer.Ordinal);

        public static void Register(string sessionId, EyesSession session)
        {
            if (session == null)
            {
                throw new ArgumentNullException(nameof(session));
            }

            if (Sessions.TryGetValue(sessionId, out var existing))
            {
                existing.Abort();
            }

            Sessions[sessionId] = session;
        }

        public static EyesSession GetRequired(string sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session))
            {
                throw new InvalidOperationException("No Applitools Eyes session exists for the provided session id. Call Eyes Start Session first.");
            }

            return session;
        }

        public static bool TryRemove(string sessionId, out EyesSession? session)
        {
            return Sessions.TryRemove(sessionId, out session);
        }

        public static void Remove(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return;
            }

            Sessions.TryRemove(sessionId, out _);
        }
    }
}
