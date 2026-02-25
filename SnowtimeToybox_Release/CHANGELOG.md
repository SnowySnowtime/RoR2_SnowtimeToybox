`1.2.0`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- fixed inheritance for nautilus items
- added bread turret
- fixed shortcake turret proccing against allies it heals
- combat turrets with "targets highest health enemy" now does just that, instead of attacking currently high health enemy overall
- they were meant to target high value targets entirely (like bosses), this makes them no longer target beetles and vaporizing them (that fucking beetle that i hate.)
- remove risingtides/elitevariety support
- i suck at coding, aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
- i am now taking a 1-2 week break from working on mods

icebro
---
- thats just how the cookie crumbles ,.., ,.,
- fixed literally everything about bread turret (made it actually work)
- stuff like its skill/overlays, basically made their turret functional
- updated overlays so they arent as silly (no longer stacks, perf improvement!)
</details>

`1.1.9`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- Adjusted modded item inheritance for all turrets
- Increased max health of Acanthi Turret, reduced regeneration
- turret was getting one tapped a little too much. made it have high hp, but it can only reasonably heal from attacking
### Turrets can now inherit a few items, globally or turret specific; from these mods...
| Mod |
| --- |
| [LoLItems](https://thunderstore.io/package/Debo/LoLItems/) |
| [LunarSoap](https://thunderstore.io/package/Wolfo/LunarSoap/) |
| [Nautilus](https://thunderstore.io/package/hex3/Nautilus/) |
| [RobItems](https://thunderstore.io/package/rob/RobItems/) |
| [Sandswept](https://thunderstore.io/package/SandsweptTeam/Sandswept/) |
| [SeekingTheVoid](https://thunderstore.io/package/acanthic/SeekingTheVoid/) |
| [Starstorm 2](https://thunderstore.io/package/TeamMoonstorm/Starstorm2/) |
| [Swansong (2R4R)](https://thunderstore.io/package/RiskOfBrainrot/2R4R/) |
| [VanillaVoid](https://thunderstore.io/package/Zenithrium/VanillaVoid/) |
</details>

`1.1.8`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- Added Acanthi Turret
- Shortcake turret now sends healing orbs to nearby allies

icebro
---
- Pointed Snowy in the right direction for fixing a few NREs and implementing proc coefficient chance on Acanthi Turret debuff
</details>


`1.1.7`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- Borbo/Snowtime Turrets now target the highest combined health enemy
- Updated the description of each turret with its intended role, and removed some unnecessary banter
- Reduced damage coefficient of Strawberry Shortcake turret from 1 to 0.7 (100% to 70%)
- the skill was always meant to specifically be used to taunt and aggro enemies, and i left its damage too high after testing it on release	
- Strawberry Shortcake turret has a (configurable) way of gaining aggro; change it to however you may prefer in the config
- Added RoR2AggroTools as a dependency (RATS) for Strawberry Shortcake Turret
- its made for ralsei but it seems to be fine to use as a general library so i dont care
</details>

`1.1.6`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- Removed junk custom tags since ItemAPI is now updated
- Added configs to enable or disable certain flags on the Friendly Turrets
- Adjusted the stats of Friendly Turrets if 2R4R is installed
- Fixed an issue with the Snowtime Turret's interactable, you can now ping it
</details>

`1.1.5`
- forgot to update the readme with the preview of the new turret.... actual readme update after tuesday

`1.1.4`
<details>
<summary>Update Summary (includes summary of commits by users)</summary>

Snowy Snowtime
---
- Added Snowtime Turret
- Added Golden Apple and Heavy Boots from RobItems to the Global Friendly Turret Whitelist
- nautilus support too but whatever
- theres like 15 junk tags temporarily to fix the itemtag issues until itemapi is updated

icebro
---
- Fixed an issue where the friendly turrets did not disallow purchasing a duplicate turret
- helped debug an issue with itemtags for turret whitelists (it was partially r2api being outdated though)

.score
---
- Rewrote the Friendly Turret Inheritance component to be significantly more performant

</details>

`1.1.3`
<details>
<summary>Update Summary</summary>

- fixed an issue where all turrets were inheriting items meant for another turret
- updated equipment check on turrets again
- readme,,
- (icebro) remove onInventoryChanged listener when destroyed
</details>

`1.1.2`
<details>
<summary>Update Summary</summary>


- Fixed an issue where if any equipment had an ID of 0, it would be given to a turret if there was no valid equipment on the owner to give the turrets
- added icon. whoops.
</details>

`1.1.1`
<details>
<summary>Update Summary</summary>

- Strawberry Shortcake Friendly Turret!!!!!!
- Friendly Turrets now emit sounds,,, DLC3 may be required
- Friendly Turrets can now inherit items
- Fixed a few floating spawns for the Friendly Turrets
- (icebro) Friendly Turrets stop spawning when all players have that type of turret
- some other changes i probably forgot, this update was a doozy.
- shortcake shortcake shortcake shortcake (she is the best);;;
</details>

`1.1.0`
<details>
<summary>Update Summary</summary>

- Friendly Turrets!!!!!
- Added just borbo turret for now, additional friendly turrret types pending
- so many thanks to icebro and acanthi for this update, without them this wouldnt of even happened
- updated how damagetypes are handled. no longer will other hitscan characters have issues. (how did this only just recently break? lost 10h of my life trying to debug this)
</details>

`1.0.6`
<details>
<summary>Update Summary</summary>

- added configs to disable my artstyle clashing skins!
- overhauled material parameters to make the artstyle-clashing skins fit in a bit more (irony)
- new skin on artificer for my pookie!
- Strike Drone(s) are now Vampire(s) when using my Operator skin.
- Mega Drone(s) are now Vulture(s) when using my Operator skin.
</details>

`1.0.5`
<details>
<summary>Update Summary</summary>

- new icon for H4-L0 PL45 Rifle by anartoast
- added credits (silly me)
</details>

`1.0.4`
<details>
<summary>Update Summary</summary>

- Change one of the legendary items from Knurl to Irradiant Pearl
- No longer get lasered, instead, get one shot randomly. Good Luck.
</details>

`1.0.3`
<details>
<summary>Update Summary</summary>

- attempt to make H4-L0 PL45 Rifle's stock increase more consistent
- remove shield effects from my operator, looked too weird
</details>

`1.0.2`
<details>
<summary>Update Summary</summary>

- Updated my Operator skin to inherit item/shield effects
- this was annoying
</details>

`1.0.1`
<details>
<summary>Update Summary</summary>

- Renamed H3-17 P14S Rifle to H4-L0 PL45 Rifle
- Fixed Skill H4-L0 PL45 Rifle's gimmick not being networked (thank you .score)
- Skill H4-L0 PL4S Rifle's attack speed was increased by default, and its stocks are increased for every 30% attack speed(2 Soldier Syringes), but it doesnt scale as well with attack speed
- Skill H4-L0 PL4S Rifle's ricochet no longer applies burn, its damage is increased to 75% TOTAL damage, and increased its proc coefficient to 0.3
</details>

`1.0.0`
<details>
<summary>Update Summary</summary>

- Add new primary skill for Operator (Its a halo plasma rifle :3)
- H3-17 P14S Rifle (have fun with it maybe idk), needs new icon
- Added LEGENDARY, with 2R4R support. Pulled from my stages
</details>

`SWAP TO SNOWTIMETOYBOX`
<details>
<summary>Update Summary</summary>


`1.0.3 (SnowtimeSkins)`
- Reskinned standard Gunner Drones to Heretic Banshees when using my Operator skin
- Reskinned Missile Drones to (Snow) Hornet's when using my Operator skin
- Added small trails to the Banshees

`1.0.2 (SnowtimeSkins)`
- Adjusted the rigging of the coat.
- Reskinned Operator's CROSSHAIR drones to Banshees when using my Operator skin
- cat ears are floppy now,,

`1.0.1 (SnowtimeSkins)`
- Tweaked dynamic bones on my Operator skin
- Added missing tail for my Operator skin
- Added skin preview image (henceforth present for all skins)

`1.0.0 (SnowtimeSkins)`
- Initial Release
- i am a fish
</details>