# Build And UiPath Setup

Use this to build the custom activity package and install it in UiPath Studio.

## Build

On the Windows VM:

1. Open `C:\UiPath\UiPath-ApplitoolsEyesHelper`.
2. Bump `VersionPrefix` in `UiPath-ApplitoolsEyesHelper.csproj` if you want UiPath Studio to pick up a new package version.
3. Run:

```powershell
dotnet build .\UiPath-ApplitoolsEyesHelper.csproj -c Release
```

The `.nupkg` is written to `.\nupkg`.

## Install in UiPath Studio

1. Open your UiPath project.
2. Add `C:\UiPath\UiPath-ApplitoolsEyesHelper\nupkg` as a local feed.
3. Install the newest `UiPath-ApplitoolsEyesHelper` package.

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
