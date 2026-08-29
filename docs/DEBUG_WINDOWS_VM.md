# Debugging UiPath Session Attach On Windows

Use this when you want to debug `Eyes Start Session` against a live UiPath mobile session without running the whole UiPath workflow under the debugger.

## What this does

The debug harness calls the same attach/open path as the custom activity:

- `EyesStartSessionActivity`
- `EyesSession.Start`
- `eyes.Open(...)`

It does not start a new Appium session. It reuses the `sessionId` that UiPath already created.

## Temporary debug logging

Set this environment variable to turn on masked runtime logs:

- `UIPATH_APPLITOOLS_EYES_DEBUG=true`

When enabled, the helper logs:

- the resolved `AppiumUrl`
- the resolved `SessionId`
- the resolved `AppName` and `TestName`
- a masked `ApiKey`
- the attach attempt
- whether `Eyes.Open(...)` completed successfully

## Build

Open the project on the Windows VM and build the debug harness:

- `DebugHarness/UiPath-ApplitoolsEyesHelper.DebugHarness.csproj`

## Run

You can pass values as arguments:

```bat
dotnet run --project DebugHarness ^
  --appiumUrl "https://eyes-mid-cap-edinburgh.trycloudflare.com/wd/hub" ^
  --sessionId "6ee8a8cf-b538-4d0b-8a56-1f5b99a4d981" ^
  --apiKey "%APPLITOOLS_API_KEY%" ^
  --appName "MockedE2EDemo" ^
  --testName "UiPath Eyes Debug"
```

Or set environment variables:

- `APPIUM_URL`
- `SESSION_ID`
- `APPLITOOLS_API_KEY`

Then run the harness with no arguments.

## Debugger breakpoints

Set breakpoints in:

- `UiPath-ApplitoolsEyesHelper/Activities/EyesStartSessionActivity.cs`
- `UiPath-ApplitoolsEyesHelper/Runtime/EyesSession.cs`
- `UiPath-ApplitoolsEyesHelper/Debugging/DebugEyesSession.cs`
- `UiPath-ApplitoolsEyesHelper.DebugHarness/Program.cs`

## Expected flow

1. UiPath creates the mobile session.
2. You copy the live `sessionId` from UiPath.
3. You run the debug harness with that same `sessionId`.
4. The harness calls `eyes.Open(...)` against the live session.
5. If that succeeds, you know the attach path is working.
