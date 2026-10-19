using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

// AdMob rewarded ads behind Google's consent form (UMP). Ads never change a competitive
// score: the only reward is a "continue" after losing, and that run then does not count
// for best score or leaderboards (see LevelRun.MarkContinued).
public static class Ads
{
	// Google's public test ids. Replace with the real AdMob ids (and the app id in
	// Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset) before release.
	const string RewardedUnitId = "ca-app-pub-3940256099942544/5224354917";

	// The Play Console target audience includes children, so every ad request is
	// child-directed (Families policy): no personalised ads, G-rated content only.
	const bool ChildDirected = true;

	static bool started;
	static bool loading;
	static RewardedAd rewarded;

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
		MobileAds.Initialize(status => LoadRewarded());
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
