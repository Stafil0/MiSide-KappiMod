# BlessRng mod patches

List of mod patches here. If you found an error, or you have some suggestions, you are welcome to DM on discord [MrS4g0](https://discord.com/users/234742888666234880) or Github Issues.

> The information will be supplemented.

## Rng Patches

1. [DeterministicRandomPatch](./DeterministicRandomPatch.cs)

   Makes random rolls in the game repeatable / forceable so other patches can remove RNG.
   Scoped patches use `GetState` / `SetState` around game methods to force zero random temporarily.

2. [ChibiDoorUnlockerPatch](./ChibiDoorUnlockerPatch.cs)

   Unlocks the chibi door by interacting with the box — no catching required.

3. [ChipMiniGamePatch](./ChipMiniGamePatch.cs)

   Removes randomness from the chip mini-game by fixing start/end points.

4. [FixedItemSpawnPatch](./FixedItemSpawnPatch.cs)

   Sets fixed item spawn positions in Chapter 2 and Chapter 3.

5. [PassableDummiesPatch](./PassableDummiesPatch.cs)

   Makes all dummies passable regardless of emotions or red eyes.

6. [RingInstantReadyPatch](./RingInstantReadyPatch.cs)

   Skips the ring wait event and instantly makes the ring ready in Cappie chapter.

7. [LoopClockPatch](./LoopClockPatch.cs)

   Auto-matches the loop chapter clocks so you don't have to dial them.

8. [MilaMinigame1Patch](./MilaMinigame1Patch.cs), [MilaMinigame2Patch](./MilaMinigame2Patch.cs), [MilaMinigame3Patch](./MilaMinigame3Patch.cs), [MilaMinigame4Patch](./MilaMinigame4Patch.cs)

   Pre-solves Mila's 4 minigames (laser, towers, figures, home invaders).

9. [RunCorridorPatch](./RunCorridorPatch.cs)

   Forces the Run & Hide corridor to only straight paths — no turns or doors.

10. [ArenaBombPatch](./ArenaBombPatch.cs)

    Fixes Run & Hide bomb timers: longest music-on / shortest music-off waits, and longest eyes-closed / shortest eyes-open windows.

11. [TramEnemySpawnPatch](./TramEnemySpawnPatch.cs)

    Makes tram enemies before Ghost Mita always spawn in the same easy spot.

12. [ErrorWindowsPatch](./ErrorWindowsPatch.cs)

    Forces the Ghostly OK error window to always jump to the first position.

13. [PCGamesPatch](./PCGamesPatch.cs)

    Real-world PC file-drag and tree-slider games are always already solved.

14. [PlayerIdPatch](./PlayerIdPatch.cs)

    Forces the player ID screen to always show `0000`.

15. [MilaGrabPatch](./MilaGrabPatch.cs) — **not registered**

    Broken Mita only grabs once; later grabs are blocked.
