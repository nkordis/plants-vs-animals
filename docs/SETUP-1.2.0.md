# 1.2.0 setup: accounts and Play Console

1.2.0 (versionCode 7) adds Firebase Analytics + Crashlytics and AdMob ads (an interstitial when leaving a finished level from level 10). Play Games
leaderboards were left out of 1.2.0 (Families policy risk; Nikos, 2026-10-18). The code is ready and runs safely without the ids below (the features just stay
off). These one-time steps need the owner's Google accounts. Release 1.1.0 must be live first.

## 1. Firebase (console.firebase.google.com)

1. Create a project "Plants vs Animals" (Google Analytics on, linked to a new GA4 property).
2. Add an Android app, package `com.oeogames.plantsvsanimals`, and download `google-services.json`.
3. GitHub repo > Settings > Secrets and variables > Actions > New secret
   `FIREBASE_GOOGLE_SERVICES_JSON`, with the full contents of that file as the value.
4. Enable Crashlytics in the Firebase console (Release & monitor > Crashlytics).

## 2. AdMob (admob.google.com)

1. Sign in with the developer account, add the app (Android, "published on Google Play", search
   the package name).
2. Create one **Interstitial** ad unit named `level_end` (shown when leaving a finished level from level 10 on). Its id goes in
   `InterstitialUnitId` in `Assets/Scripts/Services/Ads.cs`.
3. Privacy & messaging: create and publish a **European regulations (GDPR)** message for the app
   (Google's consent form, which the game already shows when required).
4. Send Claude the **App ID** (`ca-app-pub-…~…`) and the **ad unit ID** (`ca-app-pub-…/…`).
5. Optional: link AdMob to the Firebase project (AdMob > Settings > Integrations).

## 3. Play Console app content

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

Answer for every SDK in the build, not only for what the game's own code sends. Google's
disclosure pages for the SDKs are the source:
[AdMob / Google Mobile Ads](https://developers.google.com/admob/android/privacy/play-data-disclosure),
[Firebase](https://firebase.google.com/docs/android/play-data-disclosure).

Collected (none of it is sold; all of it is encrypted in transit):

| Data type | Collected by | Purposes | Shared |
| --- | --- | --- | --- |
| Approximate location (derived from IP address) | AdMob, Firebase Analytics | Advertising or marketing, Analytics, Fraud prevention | Yes (AdMob) |
| App interactions (levels played, scores; ad views and taps) | Firebase Analytics, AdMob | Analytics, Advertising or marketing | Yes (AdMob) |
| Crash logs | Firebase Crashlytics, AdMob | App functionality, Analytics | Yes (AdMob) |
| Diagnostics (performance, device info) | Firebase Crashlytics, AdMob | App functionality, Analytics, Advertising or marketing | Yes (AdMob) |
| Device or other IDs (Firebase installation and app instance IDs, app set ID; no advertising ID) | Firebase, AdMob | Analytics, App functionality, Advertising or marketing, Fraud prevention | Yes (AdMob) |

- "Shared" follows Google's AdMob disclosure page; Firebase data is processed by Google on our
  behalf, which Play does not count as sharing.
- Data collection is required (cannot be turned off in-app). Ads are never personalised.
- Deletion: users can ask by email (info@oeogames.com); there is no in-app deletion button.
  Analytics data expires after the retention set in Firebase (keep the GA4 default, 2 months).
- Before answering, re-check both disclosure pages: Google updates them with SDK releases.
