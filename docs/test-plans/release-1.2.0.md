# Release test plan: 1.2.0 (versionCode 7)

For the build made from PR #2 after 1.1.0 is approved. Written 2026-10-19 against branch `claude/project-thread-xiqu3r` at a8cb6f2. One on-demand CI build per batch (the Unity build costs Actions minutes), so this plan is meant to be run once per candidate build and stop at the first blocker.

Result per case: PASS / FAIL / N/A, with device and a screenshot for UI cases. A FAIL in a **Gate** case blocks the Play upload.

## 0. Before building (no build needed)

| ID | Check | How | Pass when |
|---|---|---|---|
| P1 Gate | Live Play state | Play Console > Latest releases and bundles, every track | Highest live versionCode is 6 (or note the real value); no track still serves 1.0.0 |
| P2 Gate | Real ad ids in place | `Ads.cs` `RewardedUnitId` and `GoogleMobileAdsSettings.asset` app id | Not the Google test ids (`ca-app-pub-3940256099942544…`) |
| P3 Gate | Firebase config | GitHub secret `FIREBASE_GOOGLE_SERVICES_JSON` exists | CI log shows Firebase added |
| P4 | AdMob app settings | AdMob > App settings | Child-directed treatment on; GDPR message published |
| P5 Gate | Privacy policy | Hosted policy (flycricket) | Mentions Firebase Analytics, Crashlytics, AdMob, child-directed ads, contact info@oeogames.com |

## 1. Build artefact (CI logs and the AAB, no device)

| ID | Check | How | Pass when |
|---|---|---|---|
| B1 Gate | Version | `bundletool dump manifest` step in CI | versionName 1.2.0, versionCode 7 (above the live value from P1) |
| B2 Gate | Target and min SDK | same | targetSdkVersion 36, minSdkVersion 25 |
| B3 Gate | 16 KB alignment | CI step "Verify 16 KB page alignment" | Passes for every `.so` |
| B4 Gate | No advertising ID permission | manifest dump | No `com.google.android.gms.permission.AD_ID`, no `ACCESS_ADSERVICES_*` |
| B5 Gate | Firebase ad ID collection off | manifest dump | `google_analytics_adid_collection_enabled=false`, `google_analytics_default_allow_ad_personalization_signals=false` |
| B6 Gate | Signing | keytool step | Upload key SHA1 35:26:FF:D1…C0:18:C9 |
| B7 | No leftover SDKs | manifest dump, `unzip -l` on the universal APK | No Play Games, Unity Ads or Unity Analytics components |
| B8 | Size | AAB size in CI | Note it; flag if more than 2× the 1.1.0 AAB |

## 2. Upgrade from 1.1.0 (device that has 1.1.0 with progress)

| ID | Check | Pass when |
|---|---|---|
| U1 Gate | Install 1.2.0 over 1.1.0 (adb install -r, or internal testing) | App opens, no crash |
| U2 Gate | Progress kept | Same unlocked levels and the same best score per level on Choose Level (PlayerPrefs keys `max level`, `Level Score N` unchanged) |
| U3 | Settings kept | Volume and difficulty as before |

## 3. First launch and consent

| ID | Check | Pass when |
|---|---|---|
| C1 Gate | Fresh install, device region in the EEA/UK (or UMP debug geography EEA) | Google consent form appears once, before any ad request |
| C2 | Fresh install outside the EEA | No consent form; game starts normally |
| C3 | Options screen | Privacy options button visible only where required (EEA); it reopens the form |
| C4 Gate | No network (airplane mode) | Game starts and plays; no hang on splash |
| C5 | Splash version label | Shows v1.2.0 |

## 4. Gameplay regression (levels must behave as in 1.1.0)

| ID | Check | Pass when |
|---|---|---|
| G1 Gate | Play levels 1, 2, 4, 13, 16, 28, 36 (new defender or attacker each) | Same defenders and animals as the level table in `docs/analysis/codebase-and-content.md`; level completes |
| G2 Gate | Win | Win panel shows a big star with stars left, and the best; Continue goes to the next level |
| G3 | New best | Beating a best score updates it on the win panel and on Choose Level |
| G4 Gate | Lose without ad | Lose panel; Play Again restarts; Main Menu works |
| G5 | Difficulty 1 and 2 | Lives 2 and 1; score ×1.5 and ×2 |
| G6 | Columns 8-9 | Shaded; a tap there shows the "cannot plant" message; columns 1-7 still plant |
| G7 | Level 40 complete | "Coming soon" tile on Choose Level as before |

## 5. New 1.2.0 features

| ID | Check | Pass when |
|---|---|---|
| N1 | Tutorial, fresh install, level 1 | Steps appear on screen (not off screen: known 1.2.0 issue fixed in c8037aa), can be finished; not shown again on replay |
| N2 Gate | Pause button | Visible on phone and tablet; game freezes (timer, animals, star income); Resume continues; Restart and Main Menu work from pause |
| N3 | Android back button in a level | Opens pause; back again resumes |
| N4 | App to background mid-level | Game is paused on return |
| N5 Gate | Rewarded continue: watch to the end | Life restored, level continues; on win the score is shown but **not** saved as best |
| N6 Gate | Rewarded continue: close the ad early | No reward; lose panel stays; game not stuck; audio returns |
| N7 | No ad available (airplane mode) | Continue offer hidden or disabled; no hang |
| N8 | Continue after the last animal | Losing the last life to the last animal after the timer, then continuing, finishes the level (fix e42d9e6) |
| N9 | Continue offered once per level | Second loss in the same run shows no offer (or as designed in LevelHud) |
| N10 Gate | Ad content | Ads are G-rated and not personalised (AdMob ad inspector, or visual check over ~5 ads) |
| N11 Gate | Families close rule | Every rewarded ad shows a close control within 5 seconds (Families policy: rewarded or opt-in ads must be "closeable after 5 seconds"; see docs/analysis/play-policy-2026-10.md finding 6) |

## 6. Analytics and crashes (Firebase console)

| ID | Check | Pass when |
|---|---|---|
| A1 | DebugView (`adb shell setprop debug.firebase.analytics.app com.oeogames.plantsvsanimals`) | `level_start`, `level_win`, `level_lose`, `level_quit`, `continue_offered`, `continue_used`, `tutorial_step`, `tutorial_complete`, `blocked_tile_tap` arrive with their parameters |
| A2 Gate | No advertising ID in Firebase | DebugView/device logs show no AAID; Firebase project setting "Google signals" off |
| A3 | Crashlytics | A test crash (development build only) appears in the console; release build has no test-crash trigger |

## 7. Devices

Minimum: Nikos's tablet (the device used for 1.1.0) and one phone. His phone blocks sideloading, so phones go through the internal testing track. Worth adding one low-end device or emulator at API 25 (Android 7.1), since reviews come mostly from cheap phones. Check text fits at 16:9 and 20:9 and on the tablet.

## 8. Play Console before submitting (Nikos)

Ads: yes. Advertising ID: no. Data safety: per `docs/SETUP-1.2.0.md`. Families ads questionnaire: only self-certified ad SDKs. Every track updated to code 7 or left empty. Release notes in English and Spanish.
