# Yet Another Prune Physics

This mod can turn normal parts into physicsless parts to some extent, making it possible to build huge structures or vessel without fighting against annoying physics and unacceptable frame rate.

The YAPP technically won't break any saves apart from breaking a loaded vessel and doesn't affect collision so parts won't fall under the terrain.

Inspired by [PrunePhysics](https://github.com/peteletroll/PrunePhysics), YAPP is highly optimized Comparing to the original mod so players are able to launch their vessel (with thousands of parts) in several minutes without waiting for an hours while their computer are doing repetitive works.

## Dependencies
* Kerbal Space Program ([Steam](https://store.steampowered.com/app/220200))
* Mod Manager ([Forum](https://forum.kerbalspaceprogram.com/index.php?/topic/50533-18x-19x-module-manager-413-november-30th-2019-right-to-ludicrous-speed/), [Github](https://github.com/sarbian/ModuleManager))

## Usage
Activate the "`YAPP Enabled`" switch in the PAW and launch/reload the vessel.

To disable, simply deactivate the "`YAPP Enabled`" switch and reload the vessel.

This mod uses a regex white list to prevent unsafe usage on part with unrecognized module.
To add an module to white list, create/modify a text file with "`.yappwl`" extension under the "`GameData`" folder or subdirectories.

For example:
```
// mywhitelist.yappwl

// some modules
ModuleFoo
ModuleBar

// add white list for all modules whose name start with "Module" (regex)
Module.*
```

## Known Issues
* Parts with YAPP enabled will simply disappear when destroyed instead of exploding and sometimes all child parts will also disappear.
* Cannot be used on robotics parts.
