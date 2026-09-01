# Build And UiPath Setup

Use this to build the custom activity package and install it in UiPath Studio.

## Build

On the Windows VM:

1. Open `C:\UiPath\UiPath-ApplitoolsEyesHelper`.
2. Bump `VersionPrefix` in the relevant project file if you want UiPath Studio to pick up a new package version.
3. Run:

```powershell
dotnet build .\UiPath-ApplitoolsEyesMobile\UiPath-ApplitoolsEyesMobile.csproj -c Release
```

The mobile `.nupkg` is written to `.\UiPath-ApplitoolsEyesMobile\nupkg`.

To build the web package, run:

```powershell
dotnet build .\UiPath-ApplitoolsEyesWeb\UiPath-ApplitoolsEyesWeb.csproj -c Release
```

Its package is written to `.\UiPath-ApplitoolsEyesWeb\nupkg`.

## Install in UiPath Studio

1. Open your UiPath project.
2. Add the appropriate package output folder as a local feed:
   - Mobile: `C:\UiPath\UiPath-ApplitoolsEyesHelper\UiPath-ApplitoolsEyesMobile\nupkg`
   - Web: `C:\UiPath\UiPath-ApplitoolsEyesHelper\UiPath-ApplitoolsEyesWeb\nupkg`
3. Install the desired package.

## Use in workflow

1. Start `Mobile Device Connection`.
2. Get `Session Identifier`.
3. Call `Eyes Start Session`.
4. Add `Eyes Check` where you want a screenshot.
5. Finish with `Eyes Close Session`.
6. Use `Eyes Abort Session` in `Catch` or cleanup.

## Required inputs

- `AppiumUrl`: same URL used by the mobile connection
- `SessionId`: output of `Get Session Identifier`
- `AppName`: Applitools app name
- `TestName`: Applitools test name
- `ApiKey`: optional; falls back to `APPLITOOLS_API_KEY`
- `BatchName`: optional

## Web package

Install `UiPath.ApplitoolsEyes.Web` separately from the mobile package. The web package uses Selenium and exposes the same activity names as the mobile package: `Eyes Start Session`, `Eyes Check`, `Eyes Close Session`, and `Eyes Abort Session`.

`Eyes Start Session` requires `WebDriverUrl`, `SessionId`, `AppName`, `TestName`, and `UfgConfigJson`. `UfgConfigJson` contains the UFG `browsersInfo` list and may also contain `concurrency` and `viewportSize`; see `UiPath-ApplitoolsEyesWeb\README.md` for an example.
