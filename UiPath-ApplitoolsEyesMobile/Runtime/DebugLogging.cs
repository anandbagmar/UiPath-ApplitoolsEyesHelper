using System;
using System.Diagnostics;

namespace ApplitoolsEyesHelper.Runtime
{
    internal static class DebugLogging
    {
        private const string DebugFlagName = "UIPATH_APPLITOOLS_EYES_DEBUG";

        public static bool IsEnabled
        {
            get
            {
                var value = Environment.GetEnvironmentVariable(DebugFlagName);
                return !string.IsNullOrWhiteSpace(value)
                    && (string.Equals(value, "1", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(value, "true", StringComparison.OrdinalIgnoreCase)
                        || string.Equals(value, "yes", StringComparison.OrdinalIgnoreCase));
            }
        }

        public static void Log(string message)
        {
            if (!IsEnabled)
            {
                return;
            }

            Trace.WriteLine($"[ApplitoolsEyesHelper] {message}");
        }

        public static string Mask(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return "<empty>";
            }

            if (value.Length <= 8)
            {
                return "****";
            }

            return value.Substring(0, 4) + "..." + value.Substring(value.Length - 4);
        }
    }
}
