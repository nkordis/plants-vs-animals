using System.IO;
using System.Xml;
using UnityEditor.Android;

// The game's Play Console target audience includes children, so the Families policy applies.
// After Unity generates the Gradle project, the app manifest is changed so that:
// - the advertising ID permissions added by AdMob and Firebase are removed, and
// - Firebase Analytics does not collect the advertising ID or allow ad personalisation.
// AdMob requests are tagged child-directed in Ads.cs.
class FamiliesManifest : IPostGenerateGradleAndroidProject
{
	const string AndroidNs = "http://schemas.android.com/apk/res/android";
	const string ToolsNs = "http://schemas.android.com/tools";

	static readonly string[] RemovedPermissions =
	{
		"com.google.android.gms.permission.AD_ID",
		"android.permission.ACCESS_ADSERVICES_AD_ID",
		"android.permission.ACCESS_ADSERVICES_ATTRIBUTION",
		"android.permission.ACCESS_ADSERVICES_TOPICS",
	};

	static readonly string[,] MetaData =
	{
		{ "google_analytics_adid_collection_enabled", "false" },
		{ "google_analytics_default_allow_ad_personalization_signals", "false" },
	};

	public int callbackOrder { get { return 100; } }

	public void OnPostGenerateGradleAndroidProject(string unityLibraryPath)
	{
		string path = Path.Combine(unityLibraryPath, "..", "launcher", "src", "main", "AndroidManifest.xml");
		if (!File.Exists(path)) { path = Path.Combine(unityLibraryPath, "src", "main", "AndroidManifest.xml"); }

		var doc = new XmlDocument();
		doc.Load(path);
		XmlElement manifest = doc.DocumentElement;
		manifest.SetAttribute("xmlns:tools", ToolsNs);

		foreach (string permission in RemovedPermissions)
		{
			XmlElement element = doc.CreateElement("uses-permission");
			element.SetAttribute("name", AndroidNs, permission);
			element.SetAttribute("node", ToolsNs, "remove");
			manifest.AppendChild(element);
		}

		XmlNode application = manifest.SelectSingleNode("application");
		if (application == null)
		{
			application = doc.CreateElement("application");
			manifest.AppendChild(application);
		}
		for (int i = 0; i < MetaData.GetLength(0); i++)
		{
			XmlElement element = doc.CreateElement("meta-data");
			element.SetAttribute("name", AndroidNs, MetaData[i, 0]);
			element.SetAttribute("value", AndroidNs, MetaData[i, 1]);
			element.SetAttribute("replace", ToolsNs, "android:value");
			application.AppendChild(element);
		}
		doc.Save(path);
	}
}
