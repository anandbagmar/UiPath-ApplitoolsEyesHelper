using System;
using System.Collections.Concurrent;

namespace ApplitoolsEyesWeb.Runtime
{
    internal static class EyesWebSessionRegistry
    {
        private static readonly ConcurrentDictionary<string, EyesWebSession> Sessions = new ConcurrentDictionary<string, EyesWebSession>(StringComparer.Ordinal);

        public static void Register(string sessionId, EyesWebSession session)
        {
            if (Sessions.TryGetValue(sessionId, out var existing))
            {
                existing.Abort();
            }

            Sessions[sessionId] = session;
        }

        public static EyesWebSession GetRequired(string sessionId)
        {
            if (!Sessions.TryGetValue(sessionId, out var session))
            {
                throw new InvalidOperationException("No Applitools Eyes web session exists for the provided session id. Call Eyes Start Session first.");
            }

            return session;
        }

        public static bool TryRemove(string sessionId, out EyesWebSession? session)
        {
            return Sessions.TryRemove(sessionId, out session);
        }

        public static void Remove(string sessionId)
        {
            Sessions.TryRemove(sessionId, out _);
        }
    }
}
