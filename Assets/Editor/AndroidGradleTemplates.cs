using System;
using System.IO;
using UnityEditor;
using UnityEngine;

// The Android dependencies of Firebase, Play Games and AdMob are added to the Gradle build
// by the External Dependency Manager, which edits Unity's custom Gradle templates. The
// templates must match the Unity version exactly, so instead of committing a copy they are
// taken from the installed Android module the first time the project opens (locally and in CI).
[InitializeOnLoad]
static class AndroidGradleTemplates
{
	const string Destination = "Assets/Plugins/Android";

	static readonly string[] Templates =
	{
		"mainTemplate.gradle",
		"settingsTemplate.gradle",
		"gradleTemplate.properties",
	};

	static AndroidGradleTemplates()
	{
		EnsureTemplates();
	}

	[MenuItem("Tools/Plants vs Animals/Create Android Gradle Templates")]
	static void EnsureTemplates()
	{
		string source;
		try
		{
			string engine = BuildPipeline.GetPlaybackEngineDirectory(BuildTarget.Android, BuildOptions.None);
			source = Path.Combine(engine, "Tools", "GradleTemplates");
		}
		catch (Exception)
		{
			return; // Android module not installed.
		}
		if (!Directory.Exists(source)) { return; }

		bool created = false;
		foreach (string template in Templates)
		{
			string target = Path.Combine(Destination, template);
			if (File.Exists(target)) { continue; }
			string from = Path.Combine(source, template);
			if (!File.Exists(from))
			{
				Debug.LogWarning("Gradle template not found: " + from);
				continue;
			}
			Directory.CreateDirectory(Destination);
			File.Copy(from, target);
			created = true;
			Debug.Log("Created " + target + " from " + from);
		}
		if (created) { AssetDatabase.Refresh(); }
	}
}
