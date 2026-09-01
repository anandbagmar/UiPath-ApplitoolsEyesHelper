using System;
using Applitools;
using Applitools.Selenium;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;

namespace ApplitoolsEyesWeb.Runtime
{
    internal sealed class EyesWebSession : IDisposable
    {
        private readonly Eyes eyes;
        private bool closed;

        private EyesWebSession(Eyes eyes)
        {
            this.eyes = eyes;
        }

        public static EyesWebSession Start(
            string webDriverUrl,
            string sessionId,
            string apiKey,
            string appName,
            string testName,
            string? batchName,
            string ufgConfigJson)
        {
            if (!Uri.TryCreate(webDriverUrl, UriKind.Absolute, out var serverUrl))
            {
                throw new ArgumentException("WebDriverUrl must be an absolute Selenium server URL.", nameof(webDriverUrl));
            }

            var ufg = UfgConfiguration.Parse(ufgConfigJson);
            var driver = AttachToExistingSession(serverUrl, sessionId);
            var runner = ufg.CreateRunner();
            var eyes = new Eyes(runner)
            {
                ApiKey = apiKey
            };

            if (!string.IsNullOrWhiteSpace(batchName))
            {
                eyes.Batch = new BatchInfo(batchName);
            }

            var configuration = new Configuration();
            configuration.SetApiKey(apiKey);
            configuration.SetAppName(appName);
            configuration.SetTestName(testName);
            ufg.ApplyTo(configuration);

            try
            {
                eyes.SetConfiguration(configuration);
                eyes.Open(driver);
            }
            catch
            {
                eyes.AbortIfNotClosed();
                throw;
            }

            return new EyesWebSession(eyes);
        }

        private static RemoteWebDriver AttachToExistingSession(Uri serverUrl, string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("Session id is required.", nameof(sessionId));
            }

            // Build a driver shell around the session UiPath already created.
            var driver = (RemoteWebDriver)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(RemoteWebDriver));
            var executor = new DebugCommandExecutor(serverUrl, TimeSpan.FromMinutes(3));
            var capabilities = new DesiredCapabilities();
            capabilities.SetCapability("browserName", "unknown");
            var webdriverType = typeof(RemoteWebDriver);
            SetField(webdriverType, driver, "commandExecutor", executor);
            SetField(webdriverType, driver, "executor", executor);
            SetField(webdriverType, driver, "capabilities", capabilities);
            SetField(webdriverType, driver, "sessionId", new SessionId(sessionId));
            SetField(webdriverType, driver, "session_id", new SessionId(sessionId));
            return driver;
        }

        public void Check(string checkpointName)
        {
            EnsureOpen();
            eyes.CheckWindow(checkpointName);
        }

        public void Close()
        {
            if (closed)
            {
                return;
            }

            eyes.Close();
            closed = true;
        }

        public void Abort()
        {
            if (closed)
            {
                return;
            }

            try
            {
                eyes.AbortIfNotClosed();
            }
            finally
            {
                closed = true;
            }
        }

        public void Dispose()
        {
            if (!closed)
            {
                try
                {
                    Abort();
                }
                catch
                {
                    // Cleanup should not block UiPath workflow completion.
                }
            }

            GC.SuppressFinalize(this);
        }

        private void EnsureOpen()
        {
            if (closed)
            {
                throw new InvalidOperationException("The Applitools Eyes web session is already closed.");
            }
        }

        private sealed class DebugCommandExecutor : ICommandExecutor, IDisposable
        {
            private readonly Uri URL;
            private readonly HttpCommandExecutor inner;
            private CommandInfoRepository commandInfoRepository;

            public DebugCommandExecutor(Uri url, TimeSpan timeout)
            {
                URL = url;
                inner = new HttpCommandExecutor(url, timeout);
                commandInfoRepository = new W3CWireProtocolCommandInfoRepository();
            }

            public CommandInfoRepository CommandInfoRepository
            {
                get => commandInfoRepository;
                set => commandInfoRepository = value ?? throw new ArgumentNullException(nameof(value));
            }

            public Response Execute(Command commandToExecute)
            {
                return inner.Execute(commandToExecute);
            }

            public void Dispose()
            {
                inner.Dispose();
            }
        }

        private static void SetField(Type type, object instance, string fieldName, object value)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (field == null)
            {
                throw new InvalidOperationException($"Selenium field '{fieldName}' was not found. The attached-session implementation is incompatible with this Selenium version.");
            }

            field.SetValue(instance, value);
        }
    }
}
