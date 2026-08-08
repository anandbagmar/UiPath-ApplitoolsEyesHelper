using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesHelper.Runtime;

namespace ApplitoolsEyesHelper.Activities
{
    [DisplayName("Eyes Abort Session")]
    [Description("Abort an Applitools Eyes session if the workflow fails or exits early.")]
    [Category("Applitools Eyes")]
    public sealed class EyesAbortActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));

            if (EyesSessionRegistry.TryRemove(sessionId, out var session))
            {
                session.Abort();
            }
        }

        private static string RequireText(string? value, string parameterName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The {parameterName} argument is required.", parameterName);
            }

            return value;
        }
    }
}
