General ReadMe:
Use Scene 3

Coding ReadMe:

All scripts are in the folder Scripts.

The LifeSuport script holds most of the game logic.

The SystemScript and ComponentScript hold all of each type of item’s code.


Creating a new system:

1. Create a new class that inherits from the “SystemClass” abstract class.
2. Add a constructor that adds each required components to the “requiredComponents” arrayList.
3a. If your system does anything each tick, override the “public void runTick()” method and place your code in that method.
3b. If your system has an unusual set of requirements to be working, you may want to override the “public bool isWorking(bool consumeCharge)” and change how it checks if it is working.
4. Add your system type to the enum at the top of the “SystemScript” file and it into the factory in the “Awake” method of that same file.
5. Create a game object and add the “SystemScript” to it. Select your system in the dropdown.
6. Create game objects with trigger colliders with the “SystemSocket” script, use it just as you would a normal Unity XR socket script.
7. Add a reference (at the bottom of the “SystemSocket” inspector) to the game object that the system is attached to, and (From the dropdown menu) what type of component this socket is looking for.


Creating a new Component:

1. Add your new Component type to the enum at the top of the “ComponentScript” file.
2. Create a new class that inherits from the “Component” abstract class.
3. Add a constructor that sets the component’s “repairCost” (How long it takes to repair) and “type” using the enum value you created in step 1.
4. Add it into the factory in the “Awake” method of the “ComponentScript” file.
5. Create a game object and add the “ComponentScript” to it. Select your new component in the dropdown.
6. Add an intractable component script, as you would any interactable object in the Unity XR system.
