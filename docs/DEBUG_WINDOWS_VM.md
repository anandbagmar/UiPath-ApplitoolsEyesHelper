# Debugging UiPath Session Attach On Windows

Use the debug harness when you want to test `Eyes Start Session` against a live UiPath session without running the full workflow under the debugger.

## What it does

- Reuses the UiPath `SessionId`
- Calls the same `EyesSession.Start(...)` path as the activity
- Opens Eyes against the already-running Appium session

## Build

```powershell
dotnet build .\DebugHarness\UiPath-ApplitoolsEyesHelper.DebugHarness.csproj
```

## Run

Set these values:

- `APPIUM_URL`
- `SESSION_ID`
- `APPLITOOLS_API_KEY`

Then run:

```powershell
dotnet run --project .\DebugHarness -- --appiumUrl 'https://your-appium-url/wd/hub' --sessionId 'your-session-id' --apiKey $env:APPLITOOLS_API_KEY --appName 'MockedE2EDemo' --testName 'UiPath Eyes Debug' --checkpointName 'Launch screen'
```

## Optional logging

Set `UIPATH_APPLITOOLS_EYES_DEBUG=true` to log resolved values and the `Eyes.Open(...)` call.

## Useful breakpoints

- `Activities/EyesStartSessionActivity.cs`
- `Runtime/EyesSession.cs`
- `Debugging/DebugEyesSession.cs`
- `DebugHarness/Program.cs`
