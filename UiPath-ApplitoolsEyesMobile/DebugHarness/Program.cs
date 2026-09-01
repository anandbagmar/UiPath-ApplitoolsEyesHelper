using System;
using System.Collections.Generic;
using ApplitoolsEyesHelper.Debugging;

namespace UiPath_ApplitoolsEyesHelper_DebugHarness
{
    internal static class Program
    {
        private static int Main(string[] args)
        {
            try
            {
                var config = DebugConfig.FromArgs(args);

                Console.WriteLine("Starting Eyes debug harness...");
                Console.WriteLine($"AppiumUrl: {config.AppiumUrl}");
                Console.WriteLine($"SessionId: {config.SessionId}");
                Console.WriteLine($"AppName: {config.AppName}");
                Console.WriteLine($"TestName: {config.TestName}");
                Console.WriteLine($"CheckpointName: {config.CheckpointName}");

                using var session = DebugEyesSession.Start(
                    config.AppiumUrl,
                    config.SessionId,
                    config.ApiKey,
                    config.AppName,
                    config.TestName,
                    config.BatchName);

                Console.WriteLine("Eyes.Open completed successfully.");

                if (!string.IsNullOrWhiteSpace(config.CheckpointName))
                {
                    Console.WriteLine($"Running checkpoint: {config.CheckpointName}");
                    session.Check(config.CheckpointName);
                    Console.WriteLine("Checkpoint completed successfully.");
                }

                if (config.WaitForEnter)
                {
                    Console.WriteLine("Press Enter to close the Eyes session.");
                    Console.ReadLine();
                }

                session.Close();
                Console.WriteLine("Eyes session closed successfully.");
                return 0;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return 1;
            }
        }

        private sealed class DebugConfig
        {
            public string AppiumUrl { get; private set; } = string.Empty;
            public string SessionId { get; private set; } = string.Empty;
            public string ApiKey { get; private set; } = string.Empty;
            public string AppName { get; private set; } = "Android App";
            public string TestName { get; private set; } = "UiPath Eyes Debug";
            public string BatchName { get; private set; } = string.Empty;
            public string CheckpointName { get; private set; } = string.Empty;
            public bool WaitForEnter { get; private set; } = true;

            public static DebugConfig FromArgs(string[] args)
            {
                var values = ParseArgs(args);
                return new DebugConfig
                {
                    AppiumUrl = Require(values, "appiumUrl", "APPIUM_URL"),
                    SessionId = Require(values, "sessionId", "SESSION_ID"),
                    ApiKey = Require(values, "apiKey", "APPLITOOLS_API_KEY"),
                    AppName = values.TryGetValue("appName", out var appName) ? appName : "Android App",
                    TestName = values.TryGetValue("testName", out var testName) ? testName : "UiPath Eyes Debug",
                    BatchName = values.TryGetValue("batchName", out var batchName) ? batchName : string.Empty,
                    CheckpointName = values.TryGetValue("checkpointName", out var checkpointName) ? checkpointName : string.Empty,
                    WaitForEnter = !values.TryGetValue("waitForEnter", out var waitForEnter) || IsTruthy(waitForEnter)
                };
            }

            private static Dictionary<string, string> ParseArgs(string[] args)
            {
                var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                for (var i = 0; i < args.Length - 1; i += 2)
                {
                    var key = args[i].TrimStart('-', '/');
                    var value = args[i + 1];
                    result[key] = value;
                }

                return result;
            }

            private static string Require(Dictionary<string, string> values, string key, string envKey)
            {
                if (!values.TryGetValue(key, out var value) || string.IsNullOrWhiteSpace(value))
                {
                    value = Environment.GetEnvironmentVariable(envKey) ?? string.Empty;
                }

                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException($"Missing required argument: --{key} or environment variable {envKey}");
                }

                return value;
            }

            private static bool IsTruthy(string value)
            {
                return string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
