using System;
using ApplitoolsEyesHelper.Runtime;

namespace ApplitoolsEyesHelper.Debugging
{
    public sealed class DebugEyesSession : IDisposable
    {
        private readonly EyesSession session;

        private DebugEyesSession(EyesSession session)
        {
            this.session = session;
        }

        public static DebugEyesSession Start(
            string appiumUrl,
            string sessionId,
            string apiKey,
            string appName,
            string testName,
            string? batchName)
        {
            var session = EyesSession.Start(appiumUrl, sessionId, apiKey, appName, testName, batchName);
            return new DebugEyesSession(session);
        }

        public void Check(string checkpointName)
        {
            session.Check(checkpointName);
        }

        public void Close()
        {
            session.Close();
        }

        public void Abort()
        {
            session.Abort();
        }

        public void Dispose()
        {
            session.Dispose();
        }
    }
}
