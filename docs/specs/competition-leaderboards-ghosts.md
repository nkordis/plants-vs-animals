# Spec: leaderboards, ghosts and fair waves

Status: proposal for after 1.2.0. Written 2026-10-19. Policy facts come from `docs/analysis/play-policy-2026-10.md` (research of Google's official pages on 2026-10-19, checked by a second agent).

Labels: **[code]** from the repo, **[policy]** quoted Google policy (see the policy doc for quotes and URLs), **[data]** Play Console figures from the product audit, **[estimate]**, **[decision]** my proposal.

## 1. Constraints that decide this

1. **Audience includes children** (Play Console, 2026-10-15), so the Families policy applies **[policy]**:
   - Mixed-audience apps "must not implement APIs or SDKs that are not approved for use in child-directed services unless they are used behind a neutral age screen", and must not require sign-in through such an SDK.
   - A feature that lets users share freeform content needs an in-app online-safety reminder and parental controls; letting children exchange personal information needs adult verification.
   - No Google page says whether Play Games Services (PGS) counts as approved for child-directed services. Google's consumer help shows supervised child accounts can use PGS with parental approval and auto-generated gamer names only. So PGS is plausible but not cleared **[policy + interpretation]**.
   - Account deletion is required only if the app lets users create an account. A custom backend with player accounts would bring that duty plus a support channel **[policy]**.
2. **Semi-passive:** no moderation, live-ops or support (Nikos, 2026-10-19).
3. **Audience size:** about 20 monthly active users and 77 first opens in the 90 days to 2026-10-14, while the app was removed from Play **[data]**. Before removal the installed audience was about 200 **[data]**. Any feature that needs other active players at the same time, or a populated leaderboard, will look empty at this size.
4. **Waves are random and the simulation is frame-rate dependent** **[code: docs/analysis/codebase-and-content.md, F1 and F2]**. Scores mix skill and luck, and a run cannot be replayed from inputs.

## 2. Recommendation [decision]

| Step | What | Backend | Policy risk | When |
|---|---|---|---|---|
| A | **Race mode with fixed waves** (seeded spawner) | none | none | with or after challenges |
| B | **Ghosts without a server:** race your own best run, and a "gardener" ghost per level recorded by Nikos | none | none | after A |
| C | **PGS achievements**, then **per-level PGS leaderboards** for race mode, sign-in optional | Google | low-medium (not cleared, not forbidden) | only when DAU is in the hundreds |
| D | Online ghost 1v1 against other players (own server) | Firebase or similar | medium-high, plus accounts/deletion/support | not recommended before ~1,000 DAU **[estimate]** |

Reasoning: A and B give the "compete and come back" loop now, with no data leaving the device, no moderation and no new Data safety entries. C is cheap to add later because the scores from A are already comparable. D costs the most, conflicts with semi-passive (cheating reports, data deletion, server upkeep), and would show empty opponent pools at today's audience size; B already gives most of its feel.

## 3. Step A: fair waves (race mode)

- `AttackerSpawner` today calls `Random.Range` on the shared Unity generator **[code]**. Give each spawner its own `System.Random` built from `(levelSeed, laneIndex)`; draw the delay and the animal from it in the same order as now.
- **Normal campaign play keeps unseeded random waves** (same feel as today; the levels do not change) **[decision]**. Race mode and ghosts use a fixed seed per level (for example `seed = level * 7919`), so everyone faces the same spawn sequence: same animals, same lanes, same delays.
- Waves become identical; outcomes still depend on the player and, slightly, on frame timing (F2). That is the right kind of fairness for scores.
- `AttackerSpawner.cs` is not touched by PR #2 or #4, so this change can be written before #2 merges, but race mode's UI depends on #2.
- Acceptance: with the same seed, the sequence of (time, lane, animal) logged by a test harness is identical across 3 runs and 2 devices to within one frame of delay; with no seed, the distribution of animals per lane over 1,000 draws matches the arrays' weights (EditMode test).

## 4. Step B: ghosts without a server

A ghost is a **timeline of outcomes**, not a replay: every 5 seconds of level time, record `{t, starsNow, livesLost, animalsDefeated}`. Level 40 (900 s) gives 180 samples, under 4 KB **[estimate]**.

- **Your best run:** saved locally when a race-mode run beats the best (`SaveStore`, see the challenges spec).
- **Gardener ghost:** one recording per level made by Nikos in a development build with a "record ghost" toggle, shipped as `StreamingAssets/ghosts.json`. Players race "the gardener"; beating him is an achievement. No player data involved.
- **In-level display:** a small ghost marker next to the star counter: "Ghost 340 (you +20)". At the end: "You beat the gardener by 45" or "Short by 12".
- Ghost runs and the race score count only without a rewarded continue (same rule as 1.2.0 best score).
- Analytics: `race_start`, `race_end` (`ghost_type`, `margin`).
- Acceptance: a ghost recorded on one device plays back on another at the same level time; no network calls; files are versioned so a format change does not crash old saves.

## 5. Step C: Play Games Services, later and optional

Only when 1.2.0 data shows enough players for a board to look alive (rule of thumb: daily active users in the hundreds **[estimate]**).

- Order: achievements first (no shared content), then per-level race-mode leaderboards.
- Sign-in never required; the game is fully playable signed out.
- Before shipping: re-check the Families pages for any PGS statement, update Data safety for what PGS collects (gamer identity, achievements, scores; per Google's PGS data-collection page), and set a maximum plausible score per leaderboard in the Play Console to cap tampered scores.
- Nikos's Play Console work: create the PGS project, achievements and leaderboards. The 1.2.0 leaderboard code is in git history (commit a8cb6f2 removed it) and can be restored.

## 6. Step D: why not online ghost 1v1 now

- Needs a server to store and match recordings, a player identity (at least an installation ID, which is a "device or other ID" in Data safety) and likely an account-deletion path **[policy]**.
- Client-reported runs can be faked; without re-simulation (impossible, F2) the only defence is plausibility limits and reports, which means support work.
- Other players' recorded runs would probably count as user-generated content, which requires terms of use, moderation and "an in-app system for reporting and blocking objectionable UGC and users" **[policy + interpretation]**. Player-chosen names would add a Families "social feature" risk (safety reminder, parental controls). Auto-generated names reduce this but do not remove the report/block duty.
- With about 20 monthly players today, matchmaking would mostly return developer or bot ghosts, which step B already provides.

Revisit D only if the game reaches roughly 1,000 DAU and ghost races (step B) show strong engagement in analytics.

## 7. Dependencies

PR #2 merged (runtime UI, `LevelRun`, `Analytics`); challenges spec's `SaveStore` and `GameEvents` (PR #7) for saving ghosts and best race runs.
