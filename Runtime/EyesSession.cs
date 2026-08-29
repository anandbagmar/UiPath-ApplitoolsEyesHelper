using System;
using Applitools;
using Applitools.Appium;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Remote;

namespace ApplitoolsEyesHelper.Runtime
{
    internal sealed class EyesSession : IDisposable
    {
        private readonly Eyes eyes;
        private bool closed;

        private EyesSession(Eyes eyes)
        {
            this.eyes = eyes;
        }

        public static EyesSession Start(
            string appiumUrl,
            string sessionId,
            string apiKey,
            string appName,
            string testName,
            string? batchName)
        {
            if (string.IsNullOrWhiteSpace(appiumUrl))
            {
                throw new ArgumentException("Appium URL is required.", nameof(appiumUrl));
            }

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ArgumentException("Session id is required.", nameof(sessionId));
            }

            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentException("Api key is required.", nameof(apiKey));
            }

            DebugLogging.Log($"Starting Eyes session attach: AppiumUrl='{appiumUrl}', SessionId='{sessionId}', ApiKey='{DebugLogging.Mask(apiKey)}', AppName='{appName}', TestName='{testName}', BatchName='{batchName}'");
            var appiumDriver = AttachToExistingSession(new Uri(appiumUrl), sessionId);
            DebugLogging.Log($"Attached to existing Appium session '{sessionId}' using driver type '{appiumDriver.GetType().FullName}'.");

            var eyes = new Eyes
            {
                ApiKey = apiKey
            };

            if (!string.IsNullOrWhiteSpace(batchName))
            {
                eyes.Batch = new BatchInfo(batchName);
            }

            DebugLogging.Log("Calling Eyes.Open(...).");
            eyes.Open(appiumDriver, appName, testName);
            DebugLogging.Log("Eyes.Open(...) completed successfully.");
            return new EyesSession(eyes);
        }

        private static AndroidDriver<IWebElement> AttachToExistingSession(Uri appiumServerUrl, string sessionId)
        {
            // AndroidDriver is a concrete Appium driver, so Eyes will accept it.
            // We create an uninitialized instance and graft the live session id onto it
            // without triggering a new session.
            var driver = (AndroidDriver<IWebElement>)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(AndroidDriver<IWebElement>));
            var executor = new HttpCommandExecutor(appiumServerUrl, TimeSpan.FromMinutes(3));
            var desiredCapabilities = new DesiredCapabilities();
            var webdriverType = typeof(RemoteWebDriver);

            DebugLogging.Log($"Creating RemoteWebDriver shell for session attach against '{appiumServerUrl}'.");
            SetField(webdriverType, driver, "commandExecutor", executor);
            SetField(webdriverType, driver, "executor", executor);
            SetField(webdriverType, driver, "capabilities", desiredCapabilities);
            SetField(webdriverType, driver, "sessionId", new SessionId(sessionId));
            SetField(webdriverType, driver, "session_id", new SessionId(sessionId));
            DebugLogging.Log("RemoteWebDriver shell initialized with existing session id.");

            return driver;
        }

        private static void SetField(Type type, object instance, string fieldName, object value)
        {
            var field = type.GetField(fieldName, System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public);
            if (field != null)
            {
                field.SetValue(instance, value);
            }
        }

        public void Check(string checkpointName)
        {
            if (closed)
            {
                throw new InvalidOperationException("The Applitools Eyes session is already closed.");
            }

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
                    // Cleanup should never block workflow completion.
                }
            }

            GC.SuppressFinalize(this);
        }
    }
}
