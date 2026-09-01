using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesWeb.Runtime;

namespace ApplitoolsEyesWeb.Activities
{
    [DisplayName("Eyes Close Session")]
    [Description("Close the active Applitools Eyes web session and finalize the visual test.")]
    [Category("Applitools Eyes")]
    public sealed class EyesWebCloseActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));
            var session = EyesWebSessionRegistry.GetRequired(sessionId);
            session.Close();
            EyesWebSessionRegistry.Remove(sessionId);
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
