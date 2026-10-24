using System;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;

// AdMob ads behind Google's consent form (UMP). From level 10 an interstitial plays when
// the player leaves a finished level (see LevelHud.AfterLevelAd). Ads never change a score.
public static class Ads
{
	// AdMob interstitial unit for the end of a level (app id in
	// Assets/GoogleMobileAds/Resources/GoogleMobileAdsSettings.asset). Empty until the unit
	// exists in AdMob: no ads are requested while it is empty.
	const string InterstitialUnitId = "";

	// The Play Console target audience includes children, so every ad request is
	// child-directed (Families policy): no personalised ads, G-rated content only.
	const bool ChildDirected = true;

	static bool started;
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
		MobileAds.Initialize(status => LoadInterstitial());
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
}
