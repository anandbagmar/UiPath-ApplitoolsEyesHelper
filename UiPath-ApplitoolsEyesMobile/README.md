# UiPath Applitools Eyes Mobile Activities

Custom UiPath activities for attaching Applitools Eyes to an Appium session that UiPath already owns.

## What’s included

- `Eyes Start Session`
- `Eyes Check`
- `Eyes Close Session`
- `Eyes Abort Session`

## Where to look

- [`BUILD_AND_SETUP.md`](../docs/BUILD_AND_SETUP.md) for build and install steps
- [`SAMPLE_UIPATH_WORKFLOW.md`](../docs/SAMPLE_UIPATH_WORKFLOW.md) for the UiPath activity order
- [`DEBUG_WINDOWS_VM.md`](../docs/DEBUG_WINDOWS_VM.md) for the Windows debug harness

## Inputs

- `AppiumUrl`: the same Appium URL used by the UiPath mobile connection
- `SessionId`: the `Get Session Identifier` output
- `AppName`: Applitools app name
- `TestName`: Applitools test name
- `ApiKey`: optional, falls back to `APPLITOOLS_API_KEY`
- `BatchName`: optional
