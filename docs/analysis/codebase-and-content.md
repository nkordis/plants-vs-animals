# Codebase and content analysis (for the post-approval phase)

Date: 2026-10-19. Base: `master` at fe533c2 (1.1.0, versionCode 6). Open PR #2 (1.2.0) is described where it changes the picture, but nothing here depends on it being merged.

Labels: **[code]** = read directly from source or scene/prefab YAML in this repo; **[inferred]** = my reading of how the code behaves, not observed on a device; **[estimate]** = a number I derived.

## 1. Summary

1. **Content (verified):** 7 placeable defenders and 5 attackers are in the build **[code]**. The "8 defenders / 6 attackers" figure does not match the current repo; history shows three more defenders (Gnome, Gravestone, Trophy) and two attackers (Fox, Lizard) from the original course project that were deleted in 2019 **[code: git history, commit 069b6f6 and earlier]**. If 8/6 counts something else (for example the Watermelon bomb as two units), it needs Nikos's confirmation; nothing below depends on it.
2. **Every run is random and unseeded.** Spawn delays and which animal spawns use `UnityEngine.Random` with no seed **[code: AttackerSpawner.cs]**. Two players on the same level face different waves, so best scores mix skill and luck. This matters for any competitive feature (leaderboards, challenges, ghost).
3. **The simulation is not deterministic.** Movement uses `Time.deltaTime`, shooting and star income are driven by animation events, and hits come from 2D physics triggers **[code]**. Replaying a player's taps will not reproduce their run on another device **[inferred]**. A "ghost" therefore has to be a recording of outcomes (stars/score over time), not an input replay.
4. **Sessions get very long.** Level timers grow from 20 s (level 1) to 900 s (level 40); the 40 timers add up to about **237 minutes** of minimum play **[code + estimate]**. Losing at minute 14 of level 40 restarts the whole 15 minutes. This is the strongest lever for both retention (a speed-up button, checkpoints are out because levels must not change) and rewarded ads (a "continue" is worth most late in a long level).
5. **Progress lives only in PlayerPrefs on the device** (`max level`, `Level Score N`, `difficulty`, `master volume`) **[code]**. There is no cloud save. Whether Android Auto Backup restores it after reinstall is not verified.
6. **Code is small (about 1,260 lines on master) and readable, but logic sits inside MonoBehaviours** and talks through `FindAnyObjectByType` and UI text (lives are parsed back from the lives label) **[code]**. There are no automated tests. PR #2 starts a better pattern (`LevelRun`, `ScoreRules`, `Analytics` as plain static classes, runtime-added UI via `GameServices`), and new features should follow it.

## 2. Content inventory [code]

Defenders (prefabs in `Assets/Prefabs/Defenders`):

| Name in game (prefab) | Cost | HP | Role |
|---|---|---|---|
| Bean (`Cucumber`) | 50 | 50 | Shooter; projectile `cactus_gun`, 20 damage |
| Mushroom | 50 | 20 | Only star income: +5 stars per animation cycle (animation event `AddStars`) |
| Thorn | 30 | 250 | Blocker; the Boar jumps over it |
| Canon (`CanonPlant`) | 80 | 50 | Shooter; projectile `seed_ammo`, 50 damage |
| Apple | 100 | 5000 | `Runner`: rolls right at speed 1 and hits each attacker it meets for 5000 |
| Watermelon | 120 | 1000 | `Bomb`: 5000 damage to every attacker within 2 units, then destroys itself |
| Guava | 150 | 5000 | `Runner` at speed 0.4, 5000 per hit |

Attackers (`Assets/Prefabs/Attackers`): Cow 220 HP, Boar 150 HP (jumps Thorn), Goat 2000 HP (deals 2000 per hit, so it kills any defender in one hit, and dies when it hits a Mushroom), Turkey 400 HP, Sheep 50 HP. Speeds and attack damage come from animation events, not from scripts.

Leftovers: levels 4-12 carry a scene override that points the (inactive) Canon button at the deleted Gnome prefab. It is harmless because the button is inactive there, but a missing reference shows in the editor. Animations for Fox, Lizard, Gnome, Trophy and others are still in `Assets/Animations`.

## 3. Level matrix [code]

Extracted from the 40 level scenes (GameTimer `levelTime`, the active `DefenderButton`s, and each lane's `AttackerSpawner`). A lane whose minimum spawn delay is longer than the level timer never spawns, so its animals are left out. Every level starts with 250 stars and 3 lives on the default difficulty (fewer on harder settings).

| Level | Timer (s) | Defenders available | Attackers that can spawn | Spawn delay range (s) |
|---|---|---|---|---|
| 1 | 20 | Bean | Cow | 2-10 |
| 2 | 30 | Bean | Cow | 2-14 |
| 3 | 40 | Bean | Cow | 2-14 |
| 4 | 50 | Bean, Mushroom | Cow | 3-20 |
| 5 | 60 | Bean, Mushroom | Cow | 3-20 |
| 6 | 70 | Bean, Mushroom | Cow | 4-14 |
| 7 | 80 | Bean, Mushroom, Thorn | Cow | 3-14 |
| 8 | 90 | Bean, Mushroom, Thorn | Cow | 3-14 |
| 9 | 100 | Bean, Mushroom, Thorn | Cow | 3-14 |
| 10 | 110 | Bean, Mushroom, Thorn | Cow, Boar | 5-16 |
| 11 | 120 | Bean, Mushroom, Thorn | Cow, Boar | 5-16 |
| 12 | 130 | Bean, Mushroom, Thorn | Cow, Boar | 5-16 |
| 13 | 140 | Bean, Mushroom, Thorn, Canon | Cow, Boar | 5-20 |
| 14 | 150 | Bean, Mushroom, Thorn, Canon | Cow, Boar | 6-16 |
| 15 | 160 | Bean, Mushroom, Thorn, Canon | Cow, Boar | 5-16 |
| 16 | 200 | Bean, Mushroom, Thorn, Canon | Cow, Boar, Goat | 5-20 |
| 17 | 210 | Bean, Mushroom, Thorn, Canon | Cow, Boar, Goat | 5-20 |
| 18 | 240 | Bean, Mushroom, Thorn, Canon | Cow, Boar, Goat | 5-20 |
| 19 | 240 | Bean, Mushroom, Thorn, Canon | Cow, Boar, Goat | 6-20 |
| 20 | 270 | Bean, Mushroom, Thorn, Canon, Apple | Cow, Boar, Goat | 3-20 |
| 21 | 300 | Bean, Mushroom, Thorn, Canon, Apple | Cow, Boar, Goat | 3-20 |
| 22 | 330 | Bean, Mushroom, Thorn, Canon, Apple | Cow, Boar, Goat | 5-20 |
| 23 | 360 | Bean, Mushroom, Thorn, Canon, Apple | Cow, Boar, Goat | 5-20 |
| 24 | 390 | Bean, Mushroom, Thorn, Canon, Apple | Boar, Goat, Turkey | 6-20 |
| 25 | 420 | Bean, Mushroom, Thorn, Canon, Apple | Boar, Goat, Turkey | 3-20 |
| 26 | 450 | Bean, Mushroom, Thorn, Canon, Apple | Boar, Goat, Turkey | 3-20 |
| 27 | 480 | Bean, Mushroom, Thorn, Canon, Apple | Boar, Goat, Turkey | 3-20 |
| 28 | 510 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Boar, Goat, Turkey | 3-20 |
| 29 | 540 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Boar, Goat, Turkey | 3-20 |
| 30 | 570 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Boar, Goat, Turkey | 3-18 |
| 31 | 600 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Boar, Goat, Turkey | 3-18 |
| 32 | 630 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Goat, Turkey, Sheep | 3-18 |
| 33 | 660 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Goat, Turkey, Sheep | 3-18 |
| 34 | 690 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Goat, Turkey, Sheep | 4-18 |
| 35 | 720 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon | Goat, Turkey, Sheep | 4-18 |
| 36 | 750 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon, Guava | Goat, Turkey, Sheep | 4-18 |
| 37 | 780 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon, Guava | Goat, Turkey, Sheep | 4-18 |
| 38 | 810 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon, Guava | Goat, Turkey, Sheep | 4-18 |
| 39 | 840 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon, Guava | Goat, Turkey, Sheep | 4-18 |
| 40 | 900 | Bean, Mushroom, Thorn, Canon, Apple, Watermelon, Guava | Goat, Turkey, Sheep | 3-18 |

Pattern: a new defender arrives at levels 4, 7, 13, 20, 28 and 36; a new attacker at 10 (Boar), 16 (Goat), 24 (Turkey) and 32 (Sheep, which is also the weakest animal). Timers grow by 10-30 s per level, with no plateau.

## 4. Architecture and game loop [code]

- **Scene flow (build order):** 0 `preload` (loads localisation JSON) → 1 Splash → 2 Start → 3 Choose Level → 4-43 Level 1-40 → Win, Help, Options, Lose. The level number is computed from the build index (`currentSceneIndex - 3`), so inserting a scene before the levels shifts every level number and every saved score key.
- **Level:** `GameTimer` counts `Time.timeSinceLevelLoad` to `levelTime`, then `LevelController.LevelTimerFinished` stops the spawners. The level is won when the timer has finished and the live attacker count (incremented in `Attacker.Awake`, decremented in `OnDestroy`) reaches zero with lives left. An attacker reaching the left edge (`DamageCollider`) costs a life; at 0 lives `Time.timeScale = 0` and the lose panel shows.
- **Placement:** `DefenderSpawner.OnMouseDown` on the play-area collider snaps to integer grid cells and spends stars. The collider covers 7 columns; columns 8-9 are visible grass but not plantable (PR #2 shades them).
- **Score:** stars left when the level is won, ×1.5 on difficulty 1 and ×2 on difficulty 2. Best score per level is saved if higher. Difficulty (0-2) only removes lives (3/2/1).
- **Save data:** PlayerPrefs only, four kinds of key (section 1, point 5). No versioning or migration.
- **Ads:** `AdManager.ShowAdd()` is an empty stub on master, still called on every level change after level 1. PR #2 replaces monetisation with `Services/Ads.cs`.

What PR #2 (1.2.0) adds that the next phase builds on [code, branch claude/project-thread-xiqu3r]:
- `LevelRun` (static run state: level, difficulty, lives lost, defenders placed, continued, duration) and `ScoreRules.FinalScore`.
- `Analytics` events: `level_start`, `level_win` (score, lives lost, defenders, seconds, continued), `level_lose` (progress %), `level_quit`, `continue_offered/used`, `tutorial_*`, `blocked_tile_tap`.
- `GameServices`: a bootstrap object that adds UI to scenes at load time, so features can ship without editing the 40 level scenes. Lesson from 1.2.0: cloned UI must be parented under the original canvas and checked on a device.
- Rewarded "continue" that keeps the run going but excludes it from best score.

## 5. Findings that shape the next phase

| # | Finding | Evidence | Consequence |
|---|---|---|---|
| F1 | Waves are random and unseeded | `AttackerSpawner.Start/SpawnAttacker` use `Random.Range` | Seed the spawner per level (same distribution, so the levels do not change) before any leaderboard, challenge or ghost. Then all players face the same wave on a given level and seed. |
| F2 | Simulation is frame-rate dependent | `Translate(... * Time.deltaTime)`, animation events, physics triggers | Input replays are not reproducible across devices [inferred]. A ghost must store a score/stars timeline. Anti-cheat for any online score has to be plausibility checks, not re-simulation. |
| F3 | Long late levels (up to 15 min) | GameTimer values, section 3 | A 2× speed button changes no level data and saves late-game time; a rewarded continue is worth most late. Physics at higher `timeScale` must be checked for missed collisions. |
| F4 | Local-only save | `PlayersScoreController`, `PlayersMaxLevelController` | Any new progression (achievements, challenge records) should go through one save module with a version number, so cloud save can be added later without a migration mess. |
| F5 | Level number from build index | `LevelLoader.GetCurrentLevel` | New scenes (e.g. a challenge hub) must be added after the levels, or level detection must change first. Runtime UI (PR #2 pattern) avoids new scenes entirely. |
| F6 | Fragile state passing | lives parsed from `livesText.text`; `LevelDisplay` reads the level every frame | New code should read `LevelRun` and expose events (level started/won/lost, defender placed) rather than scanning the scene. |
| F7 | No tests | no test assemblies | Pure logic (score, challenge rules, achievement conditions, save migration) should live in plain C# classes with EditMode tests that run locally; that does not need the paid Unity CI build. |
| F8 | Runners never leave the scene | `Runner.Update` moves right forever; no off-screen destroy found in scripts [code; scene-level colliders not fully checked] | Possible small leak of Apple/Guava objects in long levels; worth a check during device testing. |
| F9 | Debug logging in gameplay paths | `Bomb.BombNow` ("Oleeee"), `LivesDisplay.Start`, `LevelLoader.LoadNextScene` | Strip or guard with a development-build define; minor performance on low-end phones. |

## 6. How the data was produced

The tables come from parsing the scene YAML: `levelTime` overrides on the GameTimer prefab instance, `defenderPrefab` and `m_IsActive` overrides on the `Buttons` prefab instance plus scene-native `DefenderButton`s (Watermelon from level 28, Guava from 36), and every `AttackerSpawner`'s `minSpawnDelay`, `maxSpawnDelay` and `attackerPrefabArray`. Prefab GUIDs were resolved through the `.meta` files. Nothing was checked in the Unity editor or on a device.
