# Vehicle Persistence Mod for GTA V

I created this mod some time ago, and I decided to share it now, because it could be still useful for others either as it is, or for derivative purposes.

This simple GTA V mod has the ability to persist vehicles onto disk, then later load them automatically, and spawn them in the game. It uses standard serialization and deserialization practices, and stores each vehicle in a separate JSON file.

One nice aspect of it is that the code is relatively well-structured and should be easy to work with. Of course, due to the small scale of the mod, I tried not to go overboard with abstractions, so I forgo certain aspects of professional software development.

The mod strives to persist most persistable details of the vehicle, which includes:
- Broken or intact windows
- Flat or intact tires
- Open/closed state of doors
- Vehicle health value
- Vehicle roof open/closed state
- Custom colors
- Mods*

**(*Due to the weird, haphazard nature the vehicle mod system I can't be 100% sure that all vehicle mods are stored.)**

My aspiration was to persist actual vehicle damage and deformation, but sadly this seemed to be impossible even after plenty of hours of research and experimentation. 😔

## Controls:
The mod simply reacts to certain keys and key combinations on the numerical pad to carry out the saving and spawning of vehicles, and communicates the result of the opearation through primitive messages shown on the screen.

- Saving/Updating the currently used vehicle: NumPad *
- Removing the currently used vehicle from persistence: Ctrl + Shift + NumPad *
- Selecting the next vehicle in the list of available vehicles: NumPad +
- Selecting the previous vehicle in the list of available vehicles: NumPad -
- Spawning the selected vehicle (in front of the player): NumPad /
- Despawning the selected vehicle from nearby: Ctrl + Shift + NumPad /

After a vehicle is spawned, the mod doesn't try to track its state. But you can update the saved state of the vehicle any time by pressing NumPad * again.

## Use:
- Install [ScriptHookVDotnet](https://github.com/crosire/scripthookvdotnet/releases)
- Place the dll files ([see releases](https://github.com/baratgabor/GtaVehiclePersistence/releases/)) into the mod folder
- The mod should automatically load upon starting the game

## Important note:
The mod largely depends on the license plate number to identify vehicles, so if you mess with these values or set them all to the same, you'll experience some bugs.

Also, please do note that I haven't played GTA V that much. I tested the mod with normal cars, and it seemed stable. But given the complexity of the game I can't exclude the possiblity that I'm completely unaware of certain aspects of persistence when it comes to tanks, planes, etc. Feel free to let me know if you experience any issues or shortcomings.
