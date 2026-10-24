using System;
using System.IO;
using UnityEditor.Android;

// Crashlytics refuses to start, and takes the app down with it before Unity loads, when the
// build has no Crashlytics build ID resource. The Firebase Unity SDK normally generates it,
// but a CI build from a cached Library came out without it (1.2.0 test build, 2026-10-24).
// After Unity generates the Gradle project, add one to the launcher module if no module has it.
class CrashlyticsBuildId : IPostGenerateGradleAndroidProject
{
	const string ResourceName = "com.crashlytics.android.build_id";

	public int callbackOrder { get { return 110; } }

	public void OnPostGenerateGradleAndroidProject(string unityLibraryPath)
	{
		string project = Path.GetFullPath(Path.Combine(unityLibraryPath, ".."));
		foreach (string xml in Directory.GetFiles(project, "*.xml", SearchOption.AllDirectories))
		{
			string relative = xml.Substring(project.Length);
			if (relative.Contains(Path.DirectorySeparatorChar + "build" + Path.DirectorySeparatorChar)) { continue; }
			if (File.ReadAllText(xml).Contains("\"" + ResourceName + "\"")) { return; }
		}

		string values = Path.Combine(project, "launcher", "src", "main", "res", "values");
		if (!Directory.Exists(Path.Combine(project, "launcher"))) { values = Path.Combine(unityLibraryPath, "src", "main", "res", "values"); }
		Directory.CreateDirectory(values);
		string id = Guid.NewGuid().ToString("N");
		File.WriteAllText(Path.Combine(values, "crashlytics_build_id.xml"),
			"<?xml version=\"1.0\" encoding=\"utf-8\"?>\n<resources>\n" +
			"\t<string name=\"" + ResourceName + "\" translatable=\"false\">" + id + "</string>\n" +
			"</resources>\n");
		UnityEngine.Debug.Log("Added Crashlytics build ID " + id);
	}
}
