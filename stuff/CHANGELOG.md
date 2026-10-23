# SellBodiesFixed Changelog
## V 1.14.0: 
- Added bodies for the Backwater Gunkfish, Cadaver Bloom and Feiopar.
- Added a config to change the body spawn delay.

## V 1.13.9: 
- Fixed some RPC/Sync issues with the Gameboy.

## V 1.13.8: 
- You can now raise and lower the Gameboy volume.

## V 1.13.7: 
- Fixed the formula used to calculate the total multiplier.

## V 1.13.6: 
- Optimized assetbundle size.

## V 1.13.5: 
- Reverted Some changes made in V1.13.4 of the mod.
- Reworked (again) the body value multiplier (should improve compatibility with other mods).

## V 1.13.4: 
- Reworked the body value multiplier (should improve compatibility with other mods).
- Added legacy multiplier options in the congfig file.

## V 1.13.3: 
- Now The EE support videos
- Added one more cartridge (Test)
- Added a config to disable shotgun price override

## V 1.13.2: 
- Reduced the volume of an audio file
- Fixed a collision issue with an item (which made it uncollectable)
- Special thanks to **A Glitched Npc** for always reporting this kind of problem to me.

## V 1.13.1: 
- Rewrote the EasterEgg from scratch (this is much better than the old one)

## V 1.13.0: 
- Rewrote body initialization code to allow for the creation of addons (currently experimental). 

## V 1.12.4:
- Possible Fix for several compatibility issues with SelfSortingStorage and the cruiser.
- Possible Fix for the bodies rotation when they spawn.

## V 1.12.3:
- Fixed Networking Errror caused by V73.

## V 1.12.2:
- Recompiled for V72.
- The new enemy does not have a custom body yet.

## V 1.12.1:
- I should have fixed the issue where shotguns were worth 0 when reloading a savefile.
- It should now be impossible for the game to freeze due to body spawns.

## V 1.12.0:
- Added a rare (and sad) EasterEgg.

## V 1.11.2:
- Minor code changes.

## V 1.11.1:
- Fixed a bug that caused the game to freeze when a mask was spawned from a gift.

## V 1.11.0:
- Fixed a Sync issue with shotguns and masks.

## V 1.10.6:
- Potential fix to improve compatibility with other mods that modify the Masked.

## V 1.10.5:
- Potential fix to improve compatibility with other mods that modify the Masked.

## V 1.10.4:
- By popular demand, masked now drop their mask when killed.

## V 1.10.3:
- Added Blacklist for modded enemy. you have to type the name of the enemy in the blacklist config section like this (enemy1,enemy2,enemy3,)

## V 1.10.2:
- Fixed a bug where if the ship left and you had a body in your inventory, without ever dropping it, it would cause the body to be despawned for server and an error on the clients.
- Fixed a bug that caused level 3 drops for modded enemies to cost as much as if they were level 2 modded enemies.

## V 1.10.1:
- Change Confetti code to make sure the "Cheer" sound and "Yippee" sound are only played once when a mob is killed.

## V 1.10.0:
- Added the body for the Maneater. 
- Improved the V1.9.9 fix.

## V 1.9.9:
- Fixed an issue that would cause console spam if a player instantly picked up a body while it was still finishing "spawning".

## V 1.9.8:
- I accidentally broke the mod with the latest update, mb and sorry.

## V 1.9.7:
- Added some checks to make sure the "Cheer" sound and "Yippee" sound are only played once when a mob is killed.

## V 1.9.6:
- Fixed an issue where the "Cheer" sound the and "Yippee" sound would cause an error that would make creatures immortal.

## V 1.9.5:
- Confetti can now be enabled from config (disabled by default).
- Added a config to replace the "cheer" sound with "yippee" one (disabled by default).
- Potential fix for an issue that would cause console spam if a player instantly picked up a body while it was still finishing "spawning".

## V 1.9.4:
- I hope you like confetti (1M downloads update).

## V 1.9.3:
- Small Bug Fix for Lethal Company Variables compatibility.

## V 1.9.2:
- Fix for rideable enemies.

## V 1.9.1:
- By popular demand, changed the Eyeless Dog drop. The Eyeless Dog now drops its teeth (the drop stats are the same as the old drop/body).

## V 1.9.0:
- Added a drop for the Barber.
- Added a drop for the Kidnapper Fox.
- Changed the Eyeless Dog's body hitbox to be more accurate and less intrusive.
- changed some names in config (I suggest to delete the old config file to regenerate them).

## V 1.8.9:
- Reverted Version 1.8.8 of the mod.

## V 1.8.8:
- Fixed an issue created with the last patch that caused the price of all shotguns to be 0.

## V 1.8.7:
- The price of the shotgun should be the same as the one originally held by the Nutcracker.

## V 1.8.6:
- I hope i managed to fix the problem of the original bodies not disappearing correctly (I'm going crazy because of this problem).

## V 1.8.5:
- Shotgun spawn with the correct rotation after the Nutcracker has been killed.

## V 1.8.4:
- Bodies spawn with the correct rotation after the enemy has been killed.

## V 1.8.3:
- Replaced the compatibility system, implemented for other mods, added in version 1.8.2.
- Removed redundant functions from the code.
- (Hopefully for the last time) Fixed an issue where the original enemy bodies could remain visible for some players (Update: apparently it didn't work).

## V 1.8.2:
- Increased compatibility with other mods.

## V 1.8.1:
- Fixed the config name for the Tulip Snake body weight.

## V 1.8.0:
- Added Tulip Snake body.
- Changed Manticoil body name.

## V 1.7.2:
- Fixed an issue where bodies/remains of modded enemy would not spawn.

## V 1.7.1:
- Fixed an issue where the original enemy body would not disappear if it was killed by a client and not the server.

## V 1.7.0:
- The mod supports v50 of the game.
- Added bodies for the Manticoil and RadMech.
- Fixed an issue with Scan Nodes on some bodies.

## V 1.6.0:
- Now even unkillable enemies in vanilla have drops.
- Modded enemies now drop remains whose value varies based on the spawn weight of the killed enemy.
- Added the ability to configure whether a body/drop is one-handed or two-handed.
- Fixed an issue with Scan Nodes on some bodies.
- Changed the structure of the config file (It is advisable to delete and regenerate the config file).

## V 1.5.0:
- Now the value of the bodies has a multiplier that varies based on the "Maximum Power" of the moons (see the wiki).
- Added config to change the multiplier.
- Code optimization.

## V 1.4.1:
- Dependencies update.

## V 1.4.0:
- Finally fixed the issue where the value of the bodies was not synced with the host.
- Now shotguns also have the value synced with the host.

## V 1.3.0:
- Added a custom icon for bodies when held.
- Changed the position in which the Thumper's body spawns.

## V 1.2.2:
- Small changes to the code, nothing should have changed in terms of gameplay.

## V 1.2.1:
- Fixed all issues caused by only making the original body mesh invisible.
- Changed how the Nutcracker shotgun spawns.

## V 1.2.0:
- The Nutcracker body has been reintroduced.
- Enemies should now have a death animation before being turned into collectible bodies.
- Fixed a bug where the Nutcracker shotgun would appear at the initial spawn location of the Nutcracker after its death.
- Fixed a bug where the Nutcracker would not drop ammunition upon death.
- Fixed a bug where the Spider webs would remain indestructible after the Spider death.

## V 1.1.0:
- Changed the model of the Bracken remains.
- Changed the position of the Thumper's body when carrying it. This should fix the body drop position (previously it was dropped too far to the left).

## V 1.0.1:
- Fixed a bug where the mod tried to load the Nutcracker body.

## V 1.0.0:
- Fixed the Baboon Hawk body bugged price.
- Removed Nutcracker body due to issues with shotgun and shells drop.