using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Applitools;
using Applitools.Selenium;
using Applitools.VisualGrid;
using Applitools.Utils.Geometry;

namespace ApplitoolsEyesWeb.Runtime
{
    internal sealed class UfgConfiguration
    {
        [JsonPropertyName("concurrency")]
        public int? Concurrency { get; set; }

        [JsonPropertyName("viewportSize")]
        public ViewportConfiguration? ViewportSize { get; set; }

        [JsonPropertyName("browsersInfo")]
        public List<BrowserConfiguration>? BrowsersInfo { get; set; }

        public static UfgConfiguration Parse(string json)
        {
            try
            {
                var configuration = JsonSerializer.Deserialize<UfgConfiguration>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                if (configuration?.BrowsersInfo == null || configuration.BrowsersInfo.Count == 0)
                {
                    throw new ArgumentException("UfgConfigJson must contain a non-empty browsersInfo array.", nameof(json));
                }

                return configuration;
            }
            catch (JsonException exception)
            {
                throw new ArgumentException("UfgConfigJson is not valid JSON or contains an unsupported value.", nameof(json), exception);
            }
        }

        public VisualGridRunner CreateRunner()
        {
            var options = new RunnerOptions();
            if (Concurrency.HasValue)
            {
                if (Concurrency.Value < 1)
                {
                    throw new ArgumentException("UfgConfigJson concurrency must be greater than zero.", nameof(Concurrency));
                }

                options.TestConcurrency(Concurrency.Value);
            }

            return new VisualGridRunner(options);
        }

        public Configuration ApplyTo(Configuration configuration)
        {
            if (ViewportSize != null)
            {
                if (ViewportSize.Width < 1 || ViewportSize.Height < 1)
                {
                    throw new ArgumentException("viewportSize width and height must be greater than zero.", nameof(ViewportSize));
                }

                configuration.SetViewportSize(new RectangleSize(ViewportSize.Width, ViewportSize.Height));
            }

            foreach (var browser in BrowsersInfo!)
            {
                browser.ApplyTo(configuration);
            }

            return configuration;
        }
    }

    internal sealed class ViewportConfiguration
    {
        [JsonPropertyName("width")]
        public int Width { get; set; }

        [JsonPropertyName("height")]
        public int Height { get; set; }
    }

    internal sealed class BrowserConfiguration
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("width")]
        public int? Width { get; set; }

        [JsonPropertyName("height")]
        public int? Height { get; set; }

        [JsonPropertyName("displayOs")]
        public string? DisplayOs { get; set; }

        [JsonPropertyName("chromeEmulationInfo")]
        public ChromeEmulationConfiguration? ChromeEmulationInfo { get; set; }

        [JsonPropertyName("iosDeviceInfo")]
        public IosDeviceConfiguration? IosDeviceInfo { get; set; }

        public void ApplyTo(Configuration configuration)
        {
            if (ChromeEmulationInfo != null)
            {
                configuration.AddDeviceEmulation(
                    ParseDeviceName(RequireDeviceName(ChromeEmulationInfo.DeviceName, "chromeEmulationInfo.deviceName")),
                    ParseOrientation(ChromeEmulationInfo.ScreenOrientation));
                return;
            }

            if (IosDeviceInfo != null)
            {
                configuration.AddDeviceEmulation(
                    ParseDeviceName(RequireDeviceName(IosDeviceInfo.DeviceName, "iosDeviceInfo.deviceName")),
                    ScreenOrientation.Portrait);
                return;
            }

            if (string.IsNullOrWhiteSpace(Name) || !Width.HasValue || !Height.HasValue)
            {
                throw new ArgumentException("Each desktop browser entry requires name, width, and height.", nameof(UfgConfiguration.BrowsersInfo));
            }

            if (Width.Value < 1 || Height.Value < 1)
            {
                throw new ArgumentException("Browser width and height must be greater than zero.", nameof(UfgConfiguration.BrowsersInfo));
            }

            configuration.AddBrowser(Width.Value, Height.Value, ParseBrowserType(Name));
        }

        private static string RequireDeviceName(string? value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"The {fieldName} value is required for a device entry.", nameof(UfgConfiguration.BrowsersInfo));
            }

            return value;
        }

        private static ScreenOrientation ParseOrientation(string? value)
        {
            return string.Equals(value, "landscape", StringComparison.OrdinalIgnoreCase)
                ? ScreenOrientation.Landscape
                : ScreenOrientation.Portrait;
        }

        private static BrowserType ParseBrowserType(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "chrome" => BrowserType.CHROME,
                "firefox" => BrowserType.FIREFOX,
                "safari" => BrowserType.SAFARI,
                "edge" => BrowserType.EDGE_CHROMIUM,
                "edgechromium" => BrowserType.EDGE_CHROMIUM,
                _ => throw new ArgumentException($"Unsupported browser '{value}'. Supported values are chrome, firefox, safari, edge, and edgechromium.", nameof(UfgConfiguration.BrowsersInfo))
            };
        }

        private static DeviceName ParseDeviceName(string value)
        {
            return value.Trim().ToLowerInvariant() switch
            {
                "galaxy s22 ultra" => DeviceName.Galaxy_S22_Ultra,
                "galaxy note 9" => DeviceName.Galaxy_Note_9,
                "pixel 5" => DeviceName.Pixel_5,
                "iphone 15 pro max" => DeviceName.iPhone_15_Pro_Max,
                "iphone 14 pro max" => DeviceName.iPhone_14_Pro_Max,
                "iphone 13" => DeviceName.iPhone_13,
                "ipad pro (12.9-inch) (3rd generation)" => DeviceName.iPad_Pro_12_9_inch_3,
                _ => throw new ArgumentException($"Unsupported device '{value}'. Add a supported Applitools DeviceName mapping before using it in UfgConfigJson.", nameof(UfgConfiguration.BrowsersInfo))
            };
        }
    }

    internal sealed class ChromeEmulationConfiguration
    {
        [JsonPropertyName("deviceName")]
        public string? DeviceName { get; set; }

        [JsonPropertyName("screenOrientation")]
        public string? ScreenOrientation { get; set; }
    }

    internal sealed class IosDeviceConfiguration
    {
        [JsonPropertyName("deviceName")]
        public string? DeviceName { get; set; }
    }
}
