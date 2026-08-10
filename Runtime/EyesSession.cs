using System;
using Applitools;
using Applitools.Appium;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
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

            var appiumDriver = AttachToExistingSession(new Uri(appiumUrl), sessionId);

            var eyes = new Eyes
            {
                ApiKey = apiKey
            };

            if (!string.IsNullOrWhiteSpace(batchName))
            {
                eyes.Batch = new BatchInfo(batchName);
            }

            eyes.Open(appiumDriver, appName, testName);
            return new EyesSession(eyes);
        }

        private static AppiumDriver<IWebElement> AttachToExistingSession(Uri appiumServerUrl, string sessionId)
        {
            var driver = (AppiumDriver<IWebElement>)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(AppiumDriver<IWebElement>));
            var executor = new HttpCommandExecutor(appiumServerUrl, TimeSpan.FromMinutes(3));
            var desiredCapabilities = new DesiredCapabilities();
            var webdriverType = typeof(RemoteWebDriver);

            SetField(webdriverType, driver, "commandExecutor", executor);
            SetField(webdriverType, driver, "executor", executor);
            SetField(webdriverType, driver, "capabilities", desiredCapabilities);
            SetField(webdriverType, driver, "sessionId", new SessionId(sessionId));
            SetField(webdriverType, driver, "session_id", new SessionId(sessionId));

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
