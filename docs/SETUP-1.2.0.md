# 1.2.0 setup: accounts and Play Console

1.2.0 (versionCode 7) adds Google Play Games leaderboards, Firebase Analytics + Crashlytics and
AdMob rewarded ads. The code is ready and runs safely without the ids below (the features just stay
off). These one-time steps need the owner's Google accounts. Release 1.1.0 must be live first.

## 1. Play Games Services and leaderboards (Play Console)

1. Play Console > the app > **Grow users > Play Games Services > Setup and management > Configuration**.
   Choose "No, my game doesn't use Google APIs" and create the Play Games Services project.
2. **Credentials**: add an Android credential (OAuth client) for `com.oeogames.plantsvsanimals`.
   Use the **app signing key** SHA-1 (Play Console > Test and release > App integrity), because
   Play App Signing is on. Adding a second credential with the upload key SHA-1
   (`35:26:FF:D1:...:C0:18:C9`) makes the CI test APK work too.
3. **Leaderboards**: create 41, each with score format *Numeric*, *0 decimal places*, ordering
   *Larger is better*, no limits:
   - `Level 1` … `Level 40`
   - `Total score` (sum of the best score of every level)
4. Publish the Play Games Services configuration (Configuration > Review and publish).
5. Configuration > **Get resources** > *Android (XML)*: copy it and send it to Claude. It contains
   the app id and all 41 leaderboard ids; Claude puts them in the game.

## 2. Firebase (console.firebase.google.com)

1. Create a project "Plants vs Animals" (Google Analytics on, linked to a new GA4 property).
2. Add an Android app, package `com.oeogames.plantsvsanimals`, and download `google-services.json`.
3. GitHub repo > Settings > Secrets and variables > Actions > New secret
   `FIREBASE_GOOGLE_SERVICES_JSON`, with the full contents of that file as the value.
4. Enable Crashlytics in the Firebase console (Release & monitor > Crashlytics).

## 3. AdMob (admob.google.com)

1. Sign in with the developer account, add the app (Android, "published on Google Play", search
   the package name).
2. Create one **Rewarded** ad unit named `continue_after_loss`.
3. Privacy & messaging: create and publish a **European regulations (GDPR)** message for the app
   (Google's consent form, which the game already shows when required).
4. Send Claude the **App ID** (`ca-app-pub-…~…`) and the **ad unit ID** (`ca-app-pub-…/…`).
5. Optional: link AdMob to the Firebase project (AdMob > Settings > Integrations).

## 4. Play Console app content

- **Ads**: "Yes, my app contains ads".
- **Target audience**: it already includes children, so the Families policy applies. 1.2.0 is
  built for that: ad requests are child-directed and G-rated, the advertising ID permission is
  removed, and Firebase does not collect the advertising ID. In AdMob, also turn on
  "child-directed treatment" for the app (App settings) to match.
- **Advertising ID** declaration: "No" stays correct (the AD_ID permission is removed).
- **Families**: in the ads questionnaire, confirm the app uses only Families self-certified ad SDKs
  (AdMob is one).
- **Data safety**: answers below.

### Data safety answers for 1.2.0

Collected (none of it is sold; all of it is encrypted in transit):

| Data type | Collected by | Purpose | Shared |
| --- | --- | --- | --- |
| Device or other IDs (Firebase app instance ID; no advertising ID) | Firebase | Analytics | No |
| App interactions (levels played, scores) | Firebase Analytics | Analytics | No |
| Crash logs, diagnostics | Firebase Crashlytics | App functionality (stability) | No |
| Approximate location (from IP, by AdMob) | AdMob | Advertising | Yes |
| Name / user IDs (Play Games gamer profile) | Google Play Games | App functionality (leaderboards) | No |

- Data collection is required (cannot be turned off in-app). Ads are never personalised.
- Users can't request deletion through the app; Play Games profile data is managed in the
  Play Games app.

## 5. Score thresholds for the third star

`Assets/Scripts/Competition/LevelThresholds.cs` holds provisional values. Screenshots of the
level select screen from a device with good runs (it shows the best score per level) let Claude
set realistic thresholds before release; analytics refines them afterwards.
