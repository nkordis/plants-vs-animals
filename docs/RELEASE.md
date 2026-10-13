# Restoring Plants vs Animals to Google Play

Release 1.1.0 (versionCode 4) of `com.oeogames.plantsvsanimals`.

## Why the app was removed

Google Play removed the app under the Device and Network Abuse policy because it was built with
Unity 2019.3.3f1, which contains the Unity runtime vulnerability CVE-2025-59489
([Unity advisory](https://unity.com/security/sept-2025-01)). Any build made with Unity 2017.1 or
later before the September 2025 patches is affected.

## What this release changes

| Area | Before (1.0.0, code 3) | After (1.1.0, code 4) |
| --- | --- | --- |
| Unity | 2019.3.3f1 (vulnerable) | 6000.3.26f1, Unity 6.3 LTS (patched, supported to Dec 2027) |
| Target SDK | Automatic (API 28/29 era) | 36 (Android 16) |
| Min SDK | 19 | 25 (Unity 6.3 minimum) |
| 16 KB pages | Not supported | Supported by Unity 6.3; checked in CI |
| Ads | Unity Ads 2.0.8, initialised in **test mode** (no revenue) | None in this release |
| Analytics / crash reporting | Unity Analytics package, Cloud Diagnostics on | Removed / off |
| Android TV | Declared compatible | Not declared (touch only game) |

## Play Console steps (owner)

1. **Clean every track.** Google treats the app as vulnerable while *any* track (internal, closed,
   open, production) still serves a build made with the old Unity. Upload 1.1.0 to production and
   retire or replace every older release on the other tracks.
2. **Upload** the signed `.aab` produced by the `Android build` GitHub Actions workflow.
3. **App content** updates:
   - *Ads*: "No, my app does not contain ads".
   - *Data safety*: see below.
   - *Privacy policy*: link to the hosted `docs/privacy-policy.md`.
   - *Target audience and content*: confirm the existing answers still hold.
4. **Send for review**, then reply to the policy notice if the console offers an appeal/resubmit option.

## Data safety answers for 1.1.0

The build contains no third-party SDKs, no ads, no analytics and no account system. Progress,
scores and settings are stored only on the device (Unity `PlayerPrefs`).

- Does your app collect or share any of the required user data types? **No**
- Privacy policy URL: required, see `docs/privacy-policy.md`.

Re-do this form whenever an SDK (ads, analytics, crash reporting, billing) is added.

## Signing

Play App Signing is on ("Releases signed by Play"), so Google holds the app signing key.
The original upload keystore was lost; a new upload key (alias `upload`, created 2026-10-13)
replaces it through Play Console's upload key reset.

## CI secrets

The workflow in `.github/workflows/android.yml` builds when these repository secrets exist:

| Secret | Contents |
| --- | --- |
| `UNITY_LICENSE` | Contents of `Unity_lic.ulf` from a Unity Hub Personal licence |
| `UNITY_EMAIL` / `UNITY_PASSWORD` | The Unity account that owns the licence |
| `ANDROID_KEYSTORE_BASE64` | `base64` of the upload keystore |
| `ANDROID_KEYSTORE_PASS` | Keystore password |
| `ANDROID_KEYALIAS_NAME` | Key alias (`upload`) |
| `ANDROID_KEYALIAS_PASS` | Key password |

Without the Unity secrets the job skips the build with a warning. Without the keystore secrets
it builds an unsigned bundle.
