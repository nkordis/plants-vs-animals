#!/usr/bin/env python3
"""Adds the Firebase Analytics + Crashlytics packages to the Unity project for a CI build.

Firebase's Unity packages are only distributed as tarballs from dl.google.com, so they are
downloaded at build time instead of being committed. The project builds and runs without
them (analytics calls become no-ops), which keeps local editor use simple.

Usage: add_firebase.py [google-services.json path]
"""
import json
import pathlib
import shutil
import sys
import urllib.request

VERSION = "13.16.0"
PACKAGES = ["com.google.firebase.app", "com.google.firebase.analytics", "com.google.firebase.crashlytics"]
URL = "https://dl.google.com/games/registry/unity/{name}/{name}-{version}.tgz"

root = pathlib.Path(__file__).resolve().parents[2]
target = root / "GooglePackages"
target.mkdir(exist_ok=True)

manifest_path = root / "Packages" / "manifest.json"
manifest = json.loads(manifest_path.read_text(encoding="utf-8-sig"))

for name in PACKAGES:
    filename = f"{name}-{VERSION}.tgz"
    destination = target / filename
    if not destination.exists():
        url = URL.format(name=name, version=VERSION)
        print(f"Downloading {url}")
        with urllib.request.urlopen(url) as response, open(destination, "wb") as out:
            shutil.copyfileobj(response, out)
    print(f"{filename}: {destination.stat().st_size / 1e6:.1f} MB")
    manifest["dependencies"][name] = f"file:../GooglePackages/{filename}"

manifest_path.write_text(json.dumps(manifest, indent=2) + "\n", encoding="utf-8")

if len(sys.argv) > 1 and pathlib.Path(sys.argv[1]).is_file():
    shutil.copy(sys.argv[1], root / "Assets" / "google-services.json")
    print("Added google-services.json")
else:
    print("::warning::No google-services.json: Firebase is built in but will not start.")
