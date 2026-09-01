using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesHelper.Runtime;

namespace ApplitoolsEyesHelper.Activities
{
    [DisplayName("Eyes Start Session")]
    [Description("Attach Applitools Eyes to an existing UiPath Appium session and start a visual session.")]
    [Category("Applitools Eyes")]
    public sealed class EyesStartSessionActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> AppiumUrl { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> AppName { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> TestName { get; set; } = string.Empty;

        public InArgument<string> ApiKey { get; set; } = string.Empty;

        public InArgument<string> BatchName { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var appiumUrl = RequireText(AppiumUrl.Get(context), nameof(AppiumUrl));
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));
            var apiKey = ResolveApiKey(ApiKey.Get(context));
            var appName = RequireText(AppName.Get(context), nameof(AppName));
            var testName = RequireText(TestName.Get(context), nameof(TestName));
            var batchName = BatchName.Get(context);

            DebugLogging.Log($"Eyes Start Session resolved inputs: AppiumUrl='{appiumUrl}', SessionId='{sessionId}', ApiKey='{DebugLogging.Mask(apiKey)}', AppName='{appName}', TestName='{testName}', BatchName='{batchName}'");
            var session = EyesSession.Start(appiumUrl, sessionId, apiKey, appName, testName, batchName);
            EyesSessionRegistry.Register(sessionId, session);
            DebugLogging.Log($"Eyes Start Session registered Eyes session for SessionId='{sessionId}'.");
        }

        private static string ResolveApiKey(string? apiKey)
        {
            if (!string.IsNullOrWhiteSpace(apiKey))
            {
                return apiKey;
            }

            var environmentApiKey = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");
            if (!string.IsNullOrWhiteSpace(environmentApiKey))
            {
                return environmentApiKey;
            }

            throw new InvalidOperationException("Provide Applitools ApiKey or set the APPLITOOLS_API_KEY environment variable.");
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
