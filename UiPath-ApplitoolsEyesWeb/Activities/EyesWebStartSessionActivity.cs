using System;
using System.Activities;
using System.ComponentModel;
using ApplitoolsEyesWeb.Runtime;

namespace ApplitoolsEyesWeb.Activities
{
    [DisplayName("Eyes Start Session")]
    [Description("Attach Applitools Eyes to an existing UiPath Selenium session and start a web visual session.")]
    [Category("Applitools Eyes")]
    public sealed class EyesWebStartSessionActivity : CodeActivity
    {
        [RequiredArgument]
        public InArgument<string> WebDriverUrl { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> SessionId { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> AppName { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> TestName { get; set; } = string.Empty;

        public InArgument<string> ApiKey { get; set; } = string.Empty;

        public InArgument<string> BatchName { get; set; } = string.Empty;

        [RequiredArgument]
        public InArgument<string> UfgConfigJson { get; set; } = string.Empty;

        protected override void Execute(CodeActivityContext context)
        {
            var webDriverUrl = RequireText(WebDriverUrl.Get(context), nameof(WebDriverUrl));
            var sessionId = RequireText(SessionId.Get(context), nameof(SessionId));
            var apiKey = ResolveApiKey(ApiKey.Get(context));
            var appName = RequireText(AppName.Get(context), nameof(AppName));
            var testName = RequireText(TestName.Get(context), nameof(TestName));
            var ufgConfigJson = RequireText(UfgConfigJson.Get(context), nameof(UfgConfigJson));

            var session = EyesWebSession.Start(
                webDriverUrl,
                sessionId,
                apiKey,
                appName,
                testName,
                BatchName.Get(context),
                ufgConfigJson);

            EyesWebSessionRegistry.Register(sessionId, session);
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
