# UiPath Applitools Eyes activities

This project packages a small UiPath activity set for attaching Applitools Eyes to an Appium session that UiPath already created.

## Activities

- `Eyes Start Session`
- `Eyes Check`
- `Eyes Close Session`
- `Eyes Abort Session`

## How it works

- `Eyes Start Session` accepts the UiPath Appium URL and the current session id, then opens an Applitools session against that live mobile session.
- `Eyes Check`, `Eyes Close Session`, and `Eyes Abort Session` use the same session id to find the active Eyes session.

## Inputs

- `AppiumUrl`: the Appium server URL used by your Mobile Device Connection.
- `SessionId`: the output of UiPath's `Get Session Identifier` activity.
- `ApiKey`: optional; if omitted, the activity reads `APPLITOOLS_API_KEY` from the environment.
- `AppName`: the Applitools application name.
- `TestName`: the Applitools test name.
- `BatchName`: optional batch grouping.

## Usage pattern in UiPath

1. Start a `Mobile Device Connection` scope.
2. Add `Get Session Identifier` inside the scope and store its `Session Id` output.
3. Add `Eyes Start Session` after the connection is established.
4. Place one or more `Eyes Check` activities at the points you want to validate.
5. Finish with `Eyes Close Session`.
6. If the workflow fails early, use `Eyes Abort Session` in a cleanup path.

## Studio setup

1. Build the project to produce the NuGet package in `UiPath-ApplitoolsEyesHelper/nupkg`.
2. Open your UiPath Studio project.
3. Use `Manage Packages` and add the local `nupkg` folder as a package source.
4. Install `ApplitoolsEyesHelper`.
5. Drag the Eyes activities into your workflow and pass the Appium URL plus the same Session Id into each one.
6. See [SAMPLE_UIPATH_WORKFLOW.md](/Users/anand.bagmar/Documents/UiPath/UiPath-ApplitoolsEyesHelper/SAMPLE_UIPATH_WORKFLOW.md) for the exact placement pattern.

## Build output

The project is configured to generate a NuGet package on build and drop it in `UiPath-ApplitoolsEyesHelper/nupkg`.

## Package versions

- `Eyes.Appium` 5.89.26
- `Selenium.WebDriver` 3.141.0
- `System.Text.Json` 8.0.5

## Notes

- The activities intentionally do not create or quit the mobile connection.
- This keeps the connection lifecycle in UiPath and keeps Eyes focused on visual checkpoints.
- The implementation is a straightforward WF-style activity set, which is easiest to consume from UiPath Studio Desktop.
- The same Session Id must be passed to each Eyes activity so the package can attach to the same live Appium session.
