using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesWeb.Runtime;

namespace ApplitoolsEyesWeb.Activities
{
    [DisplayName("Eyes Check")]
    [Description("Run a visual checkpoint against an active Applitools Eyes web session.")]
    [Category("Applitools Eyes")]
    public sealed class EyesWebCheckActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> CheckpointName { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));
            var checkpointName = RequireText(CheckpointName.Get(context), nameof(CheckpointName));
            EyesWebSessionRegistry.GetRequired(sessionId).Check(checkpointName);
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
