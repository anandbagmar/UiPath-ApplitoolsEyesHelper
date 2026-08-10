# Build And UiPath Setup

This guide walks through building the `UiPath-ApplitoolsEyesHelper` activity package and adding it to UiPath Studio.

## What this project is for

The package adds Applitools Eyes activities to a UiPath Android mobile test that already connects to an Appium server running on your Mac.

## Prerequisites

On the Windows VM:

1. UiPath Studio installed.
2. VS Code installed.
3. .NET SDK or Visual Studio Build Tools installed so `dotnet build` works.
4. Access to the local project folder on the Windows VM.

On the Mac:

1. Appium server running for the Android emulator.
2. The emulator and app already working with your existing UiPath test.

## Project Layout

Important files in this repo:

- `ApplitoolsEyesHelper.csproj`: builds the custom activity package.
- `README.md`: short overview and links to the docs.
- `docs/SAMPLE_UIPATH_WORKFLOW.md`: example UiPath activity placement.
- `docs/Appium-setup.md`: Appium server/tunnel notes for your Mac.
- `Activities/`: the four Eyes activities.
- `Runtime/`: the Eyes session attach logic.

## Step 1. Open the project in VS Code

1. Open VS Code on the Windows VM.
2. Choose `File` -> `Open Folder`.
3. Open:
   - `C:\UiPathActivity-ApplitoolsEyesHelper`
4. Confirm you can see `ApplitoolsEyesHelper.csproj`.

## Step 2. Build the package

1. Open the integrated terminal in VS Code.
2. Make sure you are in the project root:
   ```bat
   cd /d C:\UiPathActivity-ApplitoolsEyesHelper
   ```
3. Build the project:
   ```bat
   dotnet build ApplitoolsEyesHelper.csproj -c Release
   ```

If you hit restore issues, confirm:

- the VM has internet access for NuGet restore
- the project is using the current package versions
- `UiPath.Workflow` packages are available from the configured feeds

## Step 3. Find the NuGet package

1. After a successful build, open:
   - `C:\UiPathActivity-ApplitoolsEyesHelper\nupkg`
2. You should see a `.nupkg` file, for example:
   - `ApplitoolsEyesHelper.1.0.2.nupkg`

## Step 4. Add the folder as a local package source in UiPath Studio

1. Open UiPath Studio.
2. Open your existing mobile automation project.
3. Click `Manage Packages`.
4. Click `Settings`.
5. Under `Package sources` or `User defined package sources`, click `Add feed`.
6. Set:
   - `Name`: `UiPath-ApplitoolsEyesHelper Local`
   - `Source`: `C:\UiPathActivity-ApplitoolsEyesHelper\nupkg`
7. Save the settings.
8. Go back to `All Packages`.
9. Search for `ApplitoolsEyesHelper`.
10. Install the newest version shown there.

## Step 5. Add the activities to your workflow

Use the Eyes activities inside your working UiPath Android flow.

Recommended order:

1. `Mobile Device Connection`
2. `Get Session Identifier`
3. `Eyes Start Session`
4. Mobile steps
5. `Eyes Check`
6. `Eyes Close Session`
7. `Eyes Abort Session` in a `Catch` block

## Step 6. Fill the activity inputs

### Eyes Start Session

- `AppiumUrl`
  - Use the same Appium URL your `Mobile Device Connection` is using.
- `SessionId`
  - Use the output of `Get Session Identifier`.
- `AppName`
  - Use your Applitools app name, for example `e2eDemo`.
- `TestName`
  - Use your test name, for example `MyTest`.
- `ApiKey`
  - Optional if `APPLITOOLS_API_KEY` is already set on the VM.
- `BatchName`
  - Optional.

### Eyes Check

- `SessionId`
  - Use the same `sessionId` variable.
- `CheckpointName`
  - Use the screen name you want to validate, for example `App Launched`.

### Eyes Close Session

- `SessionId`
  - Use the same `sessionId` variable.

### Eyes Abort Session

- `SessionId`
  - Use the same `sessionId` variable.

## Step 7. Add the session id variable

1. In UiPath Studio, open the `Variables` panel.
2. Create a variable named `sessionId`.
3. Set the type to `String`.
4. Set the scope to include:
   - `Get Session Identifier`
   - `Eyes Start Session`
   - `Eyes Check`
   - `Eyes Close Session`
   - `Eyes Abort Session`

## Step 8. Use the sample workflow as a template

Open:

- [SAMPLE_UIPATH_WORKFLOW.md](./SAMPLE_UIPATH_WORKFLOW.md)

That file shows the exact placement pattern and the recommended `Try / Catch / Finally` layout.

## Step 9. Keep the Mac Appium connection in mind

Because the emulator and Appium server are on your Mac:

- use the same Appium URL that already works in your mobile test
- do not switch to `localhost` on the Windows VM unless the Appium server is actually running there
- if you use a tunnel or host URL, keep that same value in `Eyes Start Session`

## Quick checklist

- Build succeeds.
- `.nupkg` exists in the `nupkg` folder.
- UiPath Studio can see the local feed.
- `ApplitoolsEyesHelper` is installed.
- `sessionId` variable exists and is in scope.
- `Eyes Start Session` uses `AppiumUrl` and `SessionId`.
- `Eyes Check` uses the same `SessionId`.
- `Eyes Close Session` and `Eyes Abort Session` use the same `SessionId`.

## If something fails

If UiPath Studio shows a package compatibility error or the test fails at runtime, keep this order:

1. Verify the original mobile test still works without the Eyes package.
2. Verify `Get Session Identifier` is inside the mobile connection scope.
3. Verify the `sessionId` variable is in scope.
4. Verify `Eyes Start Session` is using the same Appium URL as the working mobile test.
5. Reinstall the latest package version from the local feed.

