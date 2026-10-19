# Spec: level challenges and achievements (offline)

Status: proposal for after 1.2.0 ships. Written 2026-10-19. Base: master fe533c2 plus the 1.2.0 code in PR #2.

Labels: **[code]** from the repo, **[decision]** a design choice I am proposing, **[assumption]** not yet checked, **[estimate]** my number.

## 1. Why this, and why offline

- The 40 levels must not change (Nikos, 2026-10-14), the game must stay semi-passive (no live-ops, moderation or support), and the audience includes children, so the Families policy applies to anything social or data-related.
- The product audit found almost no reason to replay a level or come back (DAU/MAU 5%, 3 of 77 first-opens still active on day 7 in the 90 days to 2026-10-14; audit sections 10 and 12).
- Challenges give each of the 40 levels several new goals **without touching level data**: they only add restrictions or objectives on top of an unchanged level. Achievements turn those goals and the existing campaign into a visible collection.
- Both run fully on the device. No account, no server, no data shared with other users, so no new Families, UGC or Data safety questions (the 1.2.0 analytics events just gain a few more names) **[decision]**. Online features (leaderboards, ghosts) are covered separately because they depend on the policy check.

## 2. Player-facing design [decision]

### Challenges

- Each level gets **3 challenges**, shown as three small medals on its Choose Level tile once the level is beaten. Tapping the tile opens a small panel: "Play" (normal) or one of the three challenges.
- A challenge is the same level with one or two **rules** from this list. Every rule only restricts the player or adds a goal, so no level becomes harder in its data:

| Rule | Meaning | Implementation hook |
|---|---|---|
| `NoLivesLost` | Win without losing a life | `LevelRun.LivesLost == 0` at win |
| `MinStarsLeft(n)` | Win with at least n stars left (the existing score, before difficulty multiplier) | stars at win |
| `MaxPlants(n)` | Place at most n defenders | placement veto in `DefenderSpawner` |
| `Ban(defender)` | One defender's button is disabled (e.g. no Mushroom: no star income) | hide/disable that `DefenderButton` at level start |
| `OnlyColumns(a..b)` | Plant only in columns a to b | placement veto |
| `SpendCap(n)` | Spend at most n stars in total | placement veto |

- Rules are picked per level by hand in one data file (120 entries), not generated, so every challenge is known to be winnable **[decision]**. The `MinStarsLeft` thresholds need real scores: set them from the 1.2.0 `level_win` score distribution (for example the 60th percentile per level) after 2-4 weeks of data **[assumption: enough wins per level by then; fall back to Nikos's own test runs]**.
- **Weekly featured challenge:** one challenge from the pool is highlighted on the Start screen each week, chosen on the device from the ISO week number (`pool[hash(year, week) % pool.Count]`) among levels the player has unlocked. No server, nothing to operate. Clock changes only affect that player.
- Challenges are not available with a rewarded "continue": a continued run fails the challenge (same rule as best score in 1.2.0) **[decision]**.
- Difficulty setting: challenges count on any difficulty; the medal records the highest difficulty it was won on.

### Achievements

About 25 achievements, all local, grouped so most players unlock the first few in the first session:

| Group | Examples | Count |
|---|---|---|
| Campaign | Beat level 1, 10, 20, 30, 40 | 5 |
| New units | First Mushroom placed; first Watermelon explosion; first Goat defeated | 6 |
| Skill | 5 / 20 levels with no life lost; a level won on difficulty 2 | 4 |
| Challenges | 1 / 10 / 40 / 120 challenge medals; 3 medals on one level | 5 |
| Totals | 100 / 1,000 animals defeated; 10,000 stars left in total | 3 |
| Weekly | Complete 1 / 5 weekly challenges | 2 |

Rewards are cosmetic only (the badge, a counter on the Start screen). No currency and no gameplay advantage, because meta-progression is still an open decision for Nikos and rewards must not distort score comparisons **[decision]**.

Google Play Games achievements could later mirror these (same ids), only if the policy check clears PGS for this audience.

## 3. Technical design

Follow the 1.2.0 pattern: plain C# logic classes, UI added at runtime, no edits to the 40 level scenes.

```
Assets/Scripts/Progress/
  SaveStore.cs          versioned JSON blob in one PlayerPrefs key; wraps the old keys
  ChallengeDef.cs       id, level, rules[], (loaded from StreamingAssets/challenges.json)
  ChallengeRules.cs     pure functions: CanPlace(...), IsWon(runSnapshot)
  Achievements.cs       definitions + Evaluate(event, stats) -> newly unlocked ids
  GameEvents.cs         static events: LevelStarted, DefenderPlaced, AttackerKilled, LevelWon, LevelLost
Assets/Scripts/UI/
  ChallengePanel.cs     Choose Level tile panel (runtime)
  AchievementsPanel.cs  list on the Start screen (runtime)
Assets/StreamingAssets/challenges.json
Assets/Tests/EditMode/   ChallengeRulesTests, AchievementsTests, SaveStoreTests
```

- **Events instead of scene scans.** `GameEvents` is raised from the places that already know: `LevelRun.Begin`, `DefenderSpawner` after a placement, `Health` on an attacker's death (new, one line), `LevelController` at win/lose. Achievements and challenges subscribe. This also fixes finding F6 in `docs/analysis/codebase-and-content.md`.
- **Placement veto.** `DefenderSpawner.AttemptToPlaceDefenderAt` asks `ChallengeRules.CanPlace(defender, cell, runState)` before spending stars; a refused tap shows a short message (reuse the 1.2.0 blocked-columns message style).
- **Save data.** `SaveStore` holds `{version, challengeMedals{id: difficulty}, achievements[], totals{}, weeklyDone[]}`. Existing keys (`max level`, `Level Score N`, `difficulty`, `master volume`) stay where they are so 1.1.0/1.2.0 progress is untouched; the new blob only adds. One version field makes a later cloud save or migration simple (finding F4).
- **Weekly pick** uses the ISO week number (`System.Globalization.ISOWeek` if Unity's .NET profile has it, otherwise a 10-line helper; not verified) and a stable hash (not `string.GetHashCode`, which can differ between runtimes).
- **Analytics** (1.2.0's `Analytics.Log`): `challenge_start`, `challenge_win`, `challenge_fail` (rule that failed), `achievement_unlock`. Used to tune thresholds and to measure whether challenges raise D7.
- **Tests** run as EditMode tests in the editor; no CI build needed (Nikos's Actions minutes).

## 4. Dependencies and order

1. PR #2 merged (this builds on `LevelRun`, `Analytics`, `GameServices`, and edits `DefenderSpawner`, which #2 also edits). Starting before the merge would conflict.
2. 2-4 weeks of 1.2.0 `level_win` data for `MinStarsLeft` thresholds. Challenges without that rule can ship first.
3. Optional: seeded waves (backlog) are not needed for local challenges.

## 5. Acceptance criteria

- Level scene files and prefabs under `Assets/Scenes/Levels` and `Assets/Prefabs` are unchanged in the diff.
- Each of the 120 challenges has been won at least once in a test run or by a scripted check of its rules against the level table (no impossible combination such as banning the only shooter on levels 1-3).
- Old saves from 1.1.0/1.2.0 load with progress and best scores intact (test plan U2).
- A continued run never awards a medal or best score.
- EditMode tests cover every rule, achievement evaluation and save-version upgrade.
- No new permission, SDK or network call; Data safety answers unchanged except the new analytics event names (same data types).
- Works offline.

## 6. Effort and risk [estimate]

- Code: about 800-1,200 lines including UI and tests; 3-5 focused sessions. Challenge data entry: 1 session, plus a tuning pass after data.
- Risks: runtime UI on Choose Level must be cloned under the original canvas and tested on Nikos's tablet (lesson from 1.2.0); thresholds set too high make medals feel impossible, so start lenient and tighten with data.

## 7. Not in scope

Leaderboards, ghosts, cloud save, currency/upgrades, IAP. Those depend on the policy check or on Nikos's decision on meta-progression.
