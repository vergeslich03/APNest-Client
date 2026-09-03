# APNest-Client
Client for the Archipelago implementation for "IRON NEST: Heavy Turret Simulator".

## What even is Archipelago?
Archipelago is a cross-game randomizer. You send items to friends playing other games and receive theirs in return.

If you want to know more, look here:
https://archipelago.gg

## Installation
### MelonLoader
To install this mod under MelonLoader, first install MelonLoader in the IRON NEST directory. You can find a guide on how to do that here:
https://github.com/lavagang/melonloader#how-to-use-the-installer

After installing MelonLoader, start the game, this will take a moment, because Melonloader needs to build some files and directories.
After you are in the main menu quit and proceed to install the mod itself.

This mod is tested with MelonLoader v0.7.3, so i recommend you use that version. It may run with other versions, but no promises.

For the mod itself, download the `APNest-Client-x.x.x-MelonLoader.zip` and extract it into the game's install directory.
If you have other mods installed, extract it somewhere else and copy the DLL's into their respective folders.

To find that directory via Steam, right click on the game --> Manage --> Browse local files.

It should be `~/.local/share/Steam/steamapps/common/Iron Nest Heavy Turret Simulator` or comparable on Linux and
`C:\Program Files (x86)\Steam\steamapps\common\Iron Nest Heavy Turret Simulator` or comparable on Windows.

### BepInEx
To install the mod under BepInEx, first install BepInEx: https://builds.bepinex.dev/projects/bepinex_be

You will need BepInEx 6 bleeding-edge version for Windows (even on Linux, since the game is a Windows build and needs to run behind Proton),
more specifically:

`BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.788+5b766a3.zip`

Direct link:

https://builds.bepinex.dev/projects/bepinex_be/788/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.788%2B5b766a3.zip

Since bleeding-edge versions are not archived forever, plese use the newest version. But be warned,
the mod was not tested on another version and may be broken (or the BepInEx build).

Extract BepInEx into the game's install directory and start the game. This will take a moment, because BepInEx needs to
build some files and directories it needs. After arriving in the main menu, quit and proceed with the install of the mod itself.

To install this mod, download the `APNest-Client-x.x.x-BepInEx.zip` and extract it into the game's install directory.

>[!IMPORTANT]
> If you play on Linux with Proton, you need to make sure you set the following launch options:
>  `WINEDLLOVERRIDES="winhttp=n,b" %command%`

> [!WARNING]
> Whatever you do, do not use both loaders at once, since both loaders hook into the same GameAssembly.
> Should you for some reason use both, make sure to rename the loader specific DLL's: `version.dll` --> rename to disable MelonLoader,
> `winhttp.dll` --> rename to disable BepInEx

## How to get the apworld
The easiest way would be to download it from the release page, right besides the `APNest-Client-x.x.x-Loader.zip`s.
It's named `iron_nest.apworld`.

You can also visit my fork of the Archipelago repository and download it from that one's release page:
https://github.com/vergeslich03/APNest/releases

## How to get the yaml
You can either install the apworld in the Archipelago Client and use the `Options Generator` there
or manually edit the template yaml from this repo's release page or the Archipelago fork's one.

## Compatibility
The latest version is tested for:

- Game: 1.0 (1663)
- MelonLoader: v0.7.3
- BepInEx: 6.0.0-be.788+5b766a3 (Unity.IL2CPP, win-x64)
- Proton: 11.0

## Usage
To connect to Archipelago, simply click on the new `AP`-Button in the main menu, type in your connection info
and click connect.

Don't be alarmed when you find your progress reset, the mod redirects the game's Save data dir,
so your actual progress is untouched and will be playable as normal when you remove the mod.

> [!NOTE]
> Due to current limitations you will find your progress reset every time you connect to a new Archipelago Multiworld,
> even when switching back to an old one.
> I am still trying to find a solution to save the progress in a way it does not get reset.

>[!NOTE]
> Since BepInEx and MelonLoader have different user dirs, it will look like your progress was reset, should you switch Loaders.
> If you want to switch loaders, copy the top APNestClient folder from the directory of one Loader to the other: `BepInEx/APNestClient`
> --> `UserData/APNestClient` for switching to Melon and other direction if you want to switch to BepInEx.

## Features
### Goals

- Mission 15 (White Shells) completed
  - Medal Checks opt-in
- Bronze/Silver/Gold on all medals in all Missions
- All Endings

> [!Note]
> Due to the ~15 Checks deficit in the Mission Goal, only the bare minimum of shells is guaranteed.
> This includes SMK, AP, TEAR and ATMC, as well as utility cards such as 'Emergency Move', 'Scout Plane' and 'Powder Charges'
> everything else is randomized and not guaranteed to be in the multiworld. If you enable filler or traps in this goal type,
> only one will be added and replaces another item in the pool.

> [!WARNING]
> Due to some missions having impossible Gold Medals, the 'All Gold Medals' goal is currently not achievable.
> For more info look here: https://steamcommunity.com/sharedfiles/filedetails/?id=3779182733

### Locations

- 1 for every completed Mission
- 1 for every Medal and medal tier --> gold fires 3 checks (bronze, silver, gold), so a mission with 4 medals has 12 checks.

### Items
#### Progression

- AP, SMK, TEAR and ATMC Shell Punchcards
- Powder Charges Punchcard
- Scout Plane punchcard
- Emergency Move punchcard

#### Useful

- All Shell Punchcards not listed under progression
- Spotter Punchcard
- Spotter spawn
- Location Report Punchcard
- Location Report spawn

#### Filler

- Powder Charge spawn --> adds 5-25 charges to your inventory
- Requisition Points spawn --> adds 10-150 points to your wallet

#### Traps

- Emergency move --> triggers an emergency move
- Magazine filler --> fills your magazines with STAR shells
- Counter-Battery --> spawns an artillery enemy and starts a Counter-Battery-Timer
- Sabotage --> Engine stops and 2-5 Valves open up

## Issue/Bug reports
If you find any issues or bugs related to this mod, please open an issue in this repo or ping me (@vergeslich)
in the 'Archipelago Mod' thread in the modding channel on the game's official discord.

## Roadmap
What do I plan to implement next?

- Displaying sent checks in-game
- Implementing Death-Link
- Saving progress for multiple Multiworlds

## License
This project is under the MIT License, for more info see [LICENSE](./LICENSE)

## AI Disclaimer
I will be honest with you all, without AI I would never have gotten this project to a working state,
so credit where credit is due.

This project was made by me (a human) to about 90%. The remaining ~10% — mostly the main-menu UI —
was done by Claude Sonnet 5 and Claude Opus 5, along with most of the referencing, research, and debugging throughout.