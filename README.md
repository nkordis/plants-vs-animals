# Plants vs Animals

A tower defence game developed after the course on Udemy: Complete C# Unity Developer 2D: Learn to Code Making Games.

# New features 

* Save level 
* Choose level and help screens
* Sound effects and images
* 8 defenders and 6 attackers
* Scoring system per level
* 40 game levels
* Localization 

# Releases

The project is published on Google Play at: https://play.google.com/store/apps/details?id=com.oeogames.plantsvsanimals

# Resources

* Complete C# Unity Developer 2D: Learn to Code Making Games - https://www.udemy.com/course/unitycourse/


# Development

* Unity 6000.3.26f1 (Unity 6.3 LTS). Open the project with that exact version in Unity Hub.
* Android: target API 36, minimum API 25, IL2CPP, ARMv7 + ARM64, built as an Android App Bundle.
* CI: `.github/workflows/android.yml` builds a signed `.aab` with GameCI and checks 16 KB page alignment.
* Release steps, Data safety answers and required CI secrets: [docs/RELEASE.md](docs/RELEASE.md).
