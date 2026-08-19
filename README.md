# AP_NEST — Archipelago integration for IRON NEST

Archipelago multiworld randomizer support for **IRON NEST: Heavy Turret Simulator**,
loaded via MelonLoader.

## Status

Not started. No existing AP integration for Iron Nest exists yet (checked Aug 2026) —
this would be the first.

## Facts established so far

- Iron Nest uses the **IL2CPP** flavor of MelonLoader (not Mono). MelonLoader
  regenerates interop assemblies into `MelonLoader/Il2CppAssemblies` in the game
  install — the C# mod project references those, not the game's raw DLLs.
- Prior art to look at for hooks/state: Iron Nest's existing Nexus mods (co-op sync
  mod, cheat pack) and Open Nest Co-op.
- The game runs on Linux via Proton (Windows build, not a native Linux build).
  MelonLoader installs via a `version.dll` proxy-DLL hijack (confirmed: strings in
  the game folder's `version.dll` reference `MelonLoader.Bootstrap.dll`/`hostfxr`).
  Proton's built-in `version.dll` wins by default, so MelonLoader silently never
  loads unless Wine is told to prefer the native one. **Required Steam launch
  option:** `WINEDLLOVERRIDES="version=n,b" %command%`. Without it, nothing in
  `Mods`/`UserLibs` ever runs. Confirmed working 2026-08-15.
- `Mods/UnityExplorer.ML.IL2CPP.CoreCLR.dll` + `UserLibs/UniverseLib.ML.IL2CPP.Interop.dll`
  (from a modding Discord) verified legitimate — match the real
  [sinai-dev/UnityExplorer](https://github.com/sinai-dev/UnityExplorer) release
  naming/placement, correct internal version string, no suspicious strings, valid
  unobfuscated .NET PE. **F7** toggles the in-game overlay (scene tree, live
  object/field inspector, C# REPL console, hook manager) — this is our primary tool
  for confirming the `Il2Cpp` class/field names from `iron-nest-api-notes.md`
  against the live running game before writing any Harmony patches against them.

## Project layout

- `client/` — the in-game C# MelonLoader mod (Harmony patches + `Archipelago.MultiClient.Net`
  to talk to the AP server). This is what actually runs inside Iron Nest.
- `apworld/` — the Python apworld package that plugs into the Archipelago *generator*:
  defines regions/locations/items/logic rules. Runs server-side, not in-game.

Reference implementation with the same two-folder shape (MelonLoader mod +
apworld): [PVZFusionArchipelago](https://github.com/GraymonDgt/PVZFusionArchipelago).

- `reference/iron-nest-coop/` — cloned copy of [not-so-sure/iron-nest-coop](https://github.com/not-so-sure/iron-nest-coop),
  a Harmony/MelonLoader coop mod for Iron Nest. Source-only, no build project — used
  purely as an API map of the game's real IL2CPP classes.
- `reference/iron-nest-api-notes.md` — distilled notes from that repo: confirmed
  `Il2Cpp`/`Il2CppSleepyNodes` namespaces, candidate hook points (`MissionManager`,
  `FireMissionSceneTemplate`, `MapEntity`, `TurretController`/`GunController`,
  the `Interactable` system) mapped to likely AP location/item roles, and the
  MelonMod entry-point boilerplate to copy.

## Planned starting order

1. Install MelonLoader (IL2CPP build) on Iron Nest, confirm it boots, generate
   Il2Cpp interop assemblies.
2. Decompile the game (dnSpy/ILSpy on the generated assemblies, or Il2CppDumper)
   to find candidate "check" events (mission complete, turret unlock, upgrade
   purchased, etc.) and candidate "item" grants.
3. Scaffold a minimal `MelonMod` that just logs those hook points firing — no
   Archipelago yet.
4. Add `Archipelago.MultiClient.Net` (NuGet, no dependencies) and get a bare
   connect/login working against a local AP server
   (`ArchipelagoSessionFactory.CreateSession(...)`, `TryConnectAndLogin()`).
5. Write the apworld, now backed by a concrete list of locations/items from step 2.
