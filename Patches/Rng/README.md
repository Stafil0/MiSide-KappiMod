# BlessRng mod patches

List of mod patches here. If you found an error, or you have some suggestions, you are welcome to DM on discord [MrS4g0](https://discord.com/users/234742888666234880) or Github Issues.

> The information will be supplemented.

## Rng Patches

1. [DeterministicRandomPatch](./DeterministicRandomPatch.cs)

   Makes random rolls in the game repeatable / forceable so other patches can remove RNG.

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

8. [MilaMinigamesPatch](./MilaMinigamesPatch.cs) (+ [Game1](./MilaMinigamesPatch.Game1.cs), [Game2](./MilaMinigamesPatch.Game2.cs), [Game3](./MilaMinigamesPatch.Game3.cs), [Game4](./MilaMinigamesPatch.Game4.cs))

   Pre-solves Mila's 4 minigames (laser, tower, figures, home invaders).

9. [RunCorridorPatch](./RunCorridorPatch.cs)

   Forces the Run & Hide corridor to only straight paths — no turns or doors.

10. [TramEnemySpawnPatch](./TramEnemySpawnPatch.cs)

    Makes tram enemies before Ghost Mita always spawn in the same easy spot.

11. [PCGamesPatch](./PCGamesPatch.cs)

    Real-world PC file-drag and tree-slider games are always already solved.

12. [PlayerIdPatch](./PlayerIdPatch.cs)

    Forces the player ID screen to always show `0000`.

13. [MilaGrabPatch](./MilaGrabPatch.cs) — **not registered**

    Broken Mita only grabs once; later grabs are blocked.
