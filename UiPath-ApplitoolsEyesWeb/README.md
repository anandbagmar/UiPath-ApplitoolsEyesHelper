# UiPath Applitools Eyes Web Activities

This project packages web-browser activities for attaching Applitools Eyes to an existing Selenium-compatible browser session.

## Activities

- `Eyes Start Session`
- `Eyes Check`
- `Eyes Close Session`
- `Eyes Abort Session`

## Start Session inputs

- `WebDriverUrl`: Selenium server URL used by the existing browser session.
- `SessionId`: existing Selenium session identifier.
- `AppName`: Applitools application name.
- `TestName`: Applitools test name.
- `ApiKey`: optional; falls back to `APPLITOOLS_API_KEY`.
- `BatchName`: optional batch name.
- `UfgConfigJson`: JSON containing the UFG browser/device matrix.

Example:

```json
{
  "concurrency": 20,
  "viewportSize": { "width": 1280, "height": 768 },
  "browsersInfo": [
    { "name": "chrome", "width": 1440, "height": 900 },
    { "name": "safari", "width": 1200, "height": 857 },
    { "name": "edgechromium", "width": 1080, "height": 1920 },
    {
      "chromeEmulationInfo": {
        "deviceName": "Galaxy S22 Ultra",
        "screenOrientation": "portrait"
      },
      "displayOs": "android 15"
    },
    { "iosDeviceInfo": { "deviceName": "iPhone 15 Pro Max" } }
  ]
}
```

Desktop browser names supported by the activity are `chrome`, `firefox`, `safari`, `edge`, and `edgechromium`. Device entries use `chromeEmulationInfo` or `iosDeviceInfo` as shown above.

The activity attaches to the browser session and does not create, navigate, or close the Selenium browser itself. Use `Eyes Abort Session` in workflow cleanup when a close activity might not be reached.
