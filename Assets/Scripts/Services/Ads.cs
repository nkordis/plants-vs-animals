using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

// AdMob ads behind Google's consent form (UMP). Ads never change a competitive score: the
// only reward is a "continue" after losing, and that run then does not count for best
// score (see LevelRun.MarkContinued). From level 10 an interstitial plays when the player
// leaves a finished level (see LevelHud.AfterLevelAd).
public static class Ads
{
	// AdMob rewarded unit "continue_after_loss" (app id in
	// Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset).
	const string RewardedUnitId = "ca-app-pub-2232453977466422/7141921516";

	// AdMob interstitial unit for the end of a level. Empty until the unit exists in AdMob:
	// no interstitials are requested while it is empty.
	const string InterstitialUnitId = "";

	// The Play Console target audience includes children, so every ad request is
	// child-directed (Families policy): no personalised ads, G-rated content only.
	const bool ChildDirected = true;

	static bool started;
	static bool loading;
	static RewardedAd rewarded;
	static bool interstitialLoading;
	static InterstitialAd interstitial;

	public static bool PrivacyOptionsRequired
	{
		get
		{
			return ConsentInformation.PrivacyOptionsRequirementStatus ==
				PrivacyOptionsRequirementStatus.Required;
		}
	}

	public static void Init()
	{
		var request = new ConsentRequestParameters
		{
			TagForUnderAgeOfConsent = ChildDirected,
		};
		ConsentInformation.Update(request, updateError =>
		{
			if (updateError != null) { Debug.LogWarning("Consent update failed: " + updateError.Message); }
			ConsentForm.LoadAndShowConsentFormIfRequired(formError =>
			{
				if (formError != null) { Debug.LogWarning("Consent form failed: " + formError.Message); }
				StartIfAllowed();
			});
		});
		// Consent given in an earlier session lets ads start without waiting for the update.
		StartIfAllowed();
	}

	public static void ShowPrivacyOptions(Action done)
	{
		ConsentForm.ShowPrivacyOptionsForm(error =>
		{
			StartIfAllowed();
			done?.Invoke();
		});
	}

	static void StartIfAllowed()
	{
		if (started || !ConsentInformation.CanRequestAds()) { return; }
		started = true;
		MobileAds.RaiseAdEventsOnUnityMainThread = true;
		var configuration = new RequestConfiguration
		{
			MaxAdContentRating = MaxAdContentRating.PG,
		};
		if (ChildDirected)
		{
			configuration.AgeRestrictedTreatment = AgeRestrictedTreatment.Child;
			configuration.MaxAdContentRating = MaxAdContentRating.G;
		}
		MobileAds.SetRequestConfiguration(configuration);
		MobileAds.Initialize(status =>
		{
			LoadRewarded();
			LoadInterstitial();
		});
	}

	static void LoadRewarded()
	{
		if (!started || loading || (rewarded != null && rewarded.CanShowAd())) { return; }
		loading = true;
		RewardedAd.Load(RewardedUnitId, new AdRequest(), (ad, error) =>
		{
			loading = false;
			if (error != null || ad == null)
			{
				Debug.LogWarning("Rewarded ad failed to load: " + error);
				return;
			}
			rewarded = ad;
		});
	}

	static void LoadInterstitial()
	{
		if (!started || InterstitialUnitId.Length == 0 || interstitialLoading ||
			(interstitial != null && interstitial.CanShowAd())) { return; }
		interstitialLoading = true;
		InterstitialAd.Load(InterstitialUnitId, new AdRequest(), (ad, error) =>
		{
			interstitialLoading = false;
			if (error != null || ad == null)
			{
				Debug.LogWarning("Interstitial ad failed to load: " + error);
				return;
			}
			interstitial = ad;
		});
	}

	public static bool InterstitialReady
	{
		get { return interstitial != null && interstitial.CanShowAd(); }
	}

	// Shows the interstitial if one is loaded; done runs after it closes, or at once if
	// there is none.
	public static void ShowInterstitial(Action done)
	{
		if (!InterstitialReady) { done(); LoadInterstitial(); return; }
		InterstitialAd ad = interstitial;
		interstitial = null;
		bool finished = false;
		Action finish = () =>
		{
			if (finished) { return; }
			finished = true;
			AudioListener.pause = false;
			ad.Destroy();
			done();
			LoadInterstitial();
		};
		ad.OnAdFullScreenContentClosed += () => finish();
		ad.OnAdFullScreenContentFailed += adError => finish();
		AudioListener.pause = true;
		ad.Show();
	}

	public static bool RewardedReady
	{
		get { return rewarded != null && rewarded.CanShowAd(); }
	}

	// Shows the rewarded ad. done(true) runs after the ad closes if the player earned the
	// reward, done(false) otherwise.
	public static void ShowRewarded(Action<bool> done)
	{
		if (!RewardedReady) { done(false); LoadRewarded(); return; }
		RewardedAd ad = rewarded;
		rewarded = null;
		bool earned = false;
		bool finished = false;
		Action finish = () =>
		{
			if (finished) { return; }
			finished = true;
			AudioListener.pause = false;
			ad.Destroy();
			done(earned);
			LoadRewarded();
		};
		ad.OnAdFullScreenContentClosed += () => finish();
		ad.OnAdFullScreenContentFailed += adError => finish();
		AudioListener.pause = true;
		ad.Show(reward => earned = true);
	}
}
