# APNest-Client
Client for the Archipelago implementation for "IRON NEST: Heavy Turret Simulator".

## What even is Archipelago?
Archipelago is a cross-game randomizer. You send items to friends playing other games and receive theirs in return.

If you want to know more, look here:
https://archipelago.gg

## Installation
To install this Mod, first install MelonLoader in the IRON NEST directory. You can find a guide on how to do that here:
https://github.com/lavagang/melonloader#how-to-use-the-installer

The rest is simple, just download the `APNest.dll` from the release page and move/copy it into the `Mods` directory in
the game's install directory.

To find that directory via Steam, right click on the game --> Manage --> Browse local files.

It should be `~/.local/share/Steam/steamapps/common/Iron Nest Heavy Turret Simulator` or comparable on Linux and
`C:\Program Files (x86)\Steam\steamapps\common\Iron Nest Heavy Turret Simulator` or comparable on Windows.

## How to get the apworld
The easiest way would be to download it from the release page, right besides the `APNest.dll`.
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

## Features
### Goals

- Mission 15 (White Shells) completed
  - Medal Checks opt-in
- Bronze/Silver/Gold on all medals in all Misions 
  - Challenge Levels opt-in

> [!WARNING]
> The Mission goal is currently unbalanced — it yields about 15 fewer locations than items.
> It can't be used for a solo run, or for a Multiworld unless the other games provide a surplus of at least ~15 checks.

### Locations

- 1 for every completed Mission
- 1 for every Medal and medal tier --> gold fires 3 checks (bronze, silver, gold), so a mission with 4 medals has 12 checks.

### Items
#### Progression

- all the Shell Punchcards
- Powder Charges Punchcard
- Scout Plane punchcard
- Emergency Move punchcard

#### Useful

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
If you find any issues related to this mod, please open an issue in this repo or ping me (@vergeslich)
in the APNest thread in the modding channel on the official discord.

## Roadmap
What do I plan to implement next?

- BepinEx port
- Fixing the 15 check deficit
- Displaying sent checks in-game
- Implementing Death-Link
- Saving progress for multiple Multiworlds

## License
This project is under the MIT License, for more info see [LICENSE.md](./LICENSE.md)

## AI Disclaimer
I will be honest with you all, without AI I would never have gotten this project to a working state,
so credit where credit is due.

This project was made by me (a human) to about 90%. The remaining ~10% — mostly the main-menu UI —
was done by Claude Sonnet 5, along with most of the referencing, research, and debugging throughout.