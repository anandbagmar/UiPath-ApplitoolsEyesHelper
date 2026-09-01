using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesHelper.Runtime;

namespace ApplitoolsEyesHelper.Activities
{
    [DisplayName("Eyes Close Session")]
    [Description("Close the active Applitools Eyes session and finalize the visual test.")]
    [Category("Applitools Eyes")]
    public sealed class EyesCloseActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));
            var session = EyesSessionRegistry.GetRequired(sessionId);
            session.Close();
            EyesSessionRegistry.Remove(sessionId);
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
