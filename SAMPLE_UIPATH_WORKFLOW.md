# UiPath Workflow Pattern for Applitools Eyes

This is the smallest practical workflow shape for using the Eyes activities with a UiPath mobile connection.

## Goal

Use the same `Session Id` everywhere, and keep the same `Appium Url` used by the mobile connection:

- `Eyes Start Session`
- `Eyes Check`
- `Eyes Close Session`
- `Eyes Abort Session`

## Sequence

```text
Try
  Mobile Device Connection is established
  -> Get Session Identifier
  -> Eyes Start Session
  -> perform mobile steps in UiPath
  -> Eyes Check (one or more times)
  -> Eyes Close Session
Catch
  -> Eyes Abort Session
  -> rethrow or log the failure
Finally
  -> Close the mobile connection if your UiPath flow owns it
```

## Exact Placement

### 1. After connection setup

Drop `Get Session Identifier` inside the `Mobile Device Connection` scope, then drop `Eyes Start Session` immediately after the connection is established.

Example:

```text
Mobile Device Connection
-> Get Session Identifier
-> Eyes Start Session
```

### 2. After the UI is ready

Drop `Eyes Check` after the screen has fully loaded and the UI is stable.

Example:

```text
Tap Login
Wait for Home screen
-> Eyes Check "Home screen"
```

### 3. At each visual checkpoint

Add another `Eyes Check` anywhere the screen meaningfully changes.

Example:

```text
Enter details
Tap Submit
Wait for Confirmation
-> Eyes Check "Confirmation"
```

### 4. At the end of the test

Drop `Eyes Close Session` after the last checkpoint.

Example:

```text
Last Eyes Check
-> Eyes Close Session
```

### 5. In cleanup

Drop `Eyes Abort Session` in a `Catch` or `Finally` branch so the session is not left open if the workflow fails.

Example:

```text
Catch
  -> Eyes Abort Session
```

## Minimal Variable Set

- `appiumUrl` from the `Mobile Device Connection` activity
- `sessionId` from `Get Session Identifier`
- `apiKey` only if you do not want to use `APPLITOOLS_API_KEY`
- `appName`
- `testName`
- `checkpointName` if you want to feed the name from a variable

## Recommended UiPath Shape

Use a `Try Catch Finally` block:

1. `Try` holds the normal test path.
2. `Catch` aborts Eyes if anything fails.
3. `Finally` disposes the driver if your workflow owns it.

## Example Flow

```text
Sequence
  Try Catch Finally
    Try
      Mobile Device Connection
      Get Session Identifier
      Eyes Start Session (AppiumUrl = appiumUrl, SessionId = sessionId)
      Tap Login
      Wait for Home screen
      Eyes Check (SessionId = sessionId, CheckpointName = "Home screen")
      Tap Profile
      Wait for Profile screen
      Eyes Check (SessionId = sessionId, CheckpointName = "Profile screen")
      Eyes Close Session (SessionId = sessionId)
    Catch
      Eyes Abort Session (SessionId = sessionId)
      Rethrow
    Finally
      Close Mobile Device Connection
```

## One Rule To Remember

Pass the same `Session Id` into every Eyes activity, and keep the `Appium Url` consistent with the mobile connection. Do not create a second Appium session for Eyes.
