
Footstepper: Complete Footstep Solution
http://gamingislove.com/products/footstepper/

Need support? Drop by the support forum:
http://forum.gamingislove.com/

Or contact me via email:
contact@gamingislove.com


-------------------------------------------------------------------------------------------------------------------------------------------------------
Quick Setup Guide
-------------------------------------------------------------------------------------------------------------------------------------------------------

Create reusable 'Footstep Materials' assets to set up the audio clips and prefabs (e.g. for particle effects) 
that should be used for something - you can set up separate clips/prefabs for walking, running, sprinting, 
jumping and landing.
They're created in the Project tab in Unity.

Create reusable 'Footstep Texture Materials' assets to link textures and sprites to footstep materials - this is used 
by terrains to determine the ground you're moving on and can also be used to determine the ground based on the 
renderer of a game object.
They're created in the Project tab in Unity.

Add footstep sources to game objects in your scenes:
- 'Object Footstep Sources' add footstep materials to individual game objects 
(e.g. for stones, wooden planks, brides or water).
- 'Terrain Footstep Sources' handle finding the correct footstep material for terrains
- 'Trigger Footstep Sources' add footstep materials to an area in your scene and overrule all other sources 
(e.g. for water or to make sure things in an area sound the way you want).

Add a 'Footstepper' component to something that should use footsteps (e.g. the player).

Set up how footsteps are played.

More detailed explanations can be found in the documentation.


-------------------------------------------------------------------------------------------------------------------------------------------------------
Documentation
-------------------------------------------------------------------------------------------------------------------------------------------------------

You can find the full documentation here:
http://gamingislove.com/products/footstepper/#documentation

Or the individual parts here:

1. First Steps:
http://gamingislove.com/tutorials/footstepper/first-steps/

2. Footstep Materials:
http://gamingislove.com/tutorials/footstepper/footstep-materials/

3. Footstep Sources:
http://gamingislove.com/tutorials/footstepper/footstep-sources/

4. Footsteppers:
http://gamingislove.com/tutorials/footstepper/footsteppers/

5. Playing Footsteps:
http://gamingislove.com/tutorials/footstepper/playing-footsteps/

6. Footstep Manager:
http://gamingislove.com/tutorials/footstepper/footstep-manager/

7. Foot IK:
http://gamingislove.com/tutorials/footstepper/foot-ik/

8. Editor Tips:
http://gamingislove.com/tutorials/footstepper/editor-tips/

9. Effect Tags:
http://gamingislove.com/tutorials/footstepper/effect-tags/

Integrations:
http://gamingislove.com/tutorials/footstepper/integrations/


-------------------------------------------------------------------------------------------------------------------------------------------------------
Demo Assets
-------------------------------------------------------------------------------------------------------------------------------------------------------

Download the demo assets here:
http://gamingislove.com/products/footstepper/#demo-assets

The 'Footstepper Demo' includes a 2D and 3D demo scene.
The demo uses assets from Unity's 'Standard Assets'
(https://assetstore.unity.com/packages/essentials/asset-packs/standard-assets-for-unity-2017-3-32351)
as well as audio clips and textures from opengameart.org (https://opengameart.org/).


To get the 'Footstepper Demo' running, do the following:
- import the unitypackage into a Unity project (matching the version of the demo)
- import Footstepper into the Unity project
- open the build settings: 'File > Build Settings...' in the Unity menu)
- add the 2 demo scenes to the 'Scenes In Build':
	- Footstepper/Demo/Demo3D/Footstepper Demo 3D
	- Footstepper/Demo/Demo2D/Footstepper Demo 2D


-------------------------------------------------------------------------------------------------------------------------------------------------------
Version Changelog
-------------------------------------------------------------------------------------------------------------------------------------------------------

Version 1.4.0:
- new: Footstep Trigger: 'Limit Layer' settings available. Optionally limit the layers that can cause footsteps.
- new: Footstepper: 'Speed Update Type' setting available. Select when the 'Footstepper' component's current movement speed is updated, either 'None' (no automatic calculation), 'Update', 'Late Update' or 'Fixed Update'. This should match how the game object is moved (e.g. 'Fixed Update' when using rigidbodies for movement).
- new: Scripting: Footstepper: You can now set the speed of a 'Footstepper' component (e.g. used for speed checks or auto play) via the 'Speed' property (Vector2).


Version 1.3.0:
- new: Footstep Materials: 'Custom Effects' settings available in 'Default Effects' and 'Tag Effects'. Custom footstep effects are identified by their 'Custom Name' and can be played using the 'FootstepCustom' function name in animation events (providing 'Int' value for foot index and 'String' value for custom effect name) and 'FootstepCustomIndex' function via scripting. Use it to play other effects, e.g. sliding.
- new: Footstepper: 'Custom Effect Volume' setting available. Defines the volume used to play custom footstep effects.


Version 1.2.0:
- new: Footstepper: 'Mode' setting available. Defines if the footstepper is enabled, disabled or only plays audio clips or spawns prefabs.
- new: Footstepper: 'No Raycast Fallback' setting available. Optionally use the fallback material even if the raycast didn't hit anything.


Version 1.1.0:
- new: Tilemap Footstep Source: Plays footstep effects based on the sprite of the tile at the raycast hit's position. You can use multiple tilemaps in a single source, the first tilemap that has a sprite for the position will be used. Requires Unity 2017.2 or newer.
- new: Footstepper: 'Auto Find' settings available. If no footstep source was found on the game object hit by the raycast, the footstepper can search for effects based on hit tilemaps or renderers. This is now optional.
- new: Gizmo Icons: Added new gizmo icons for 'Footstepper', 'Footstep Manager', 'Foot IK', 'Footstep Trigger', 'Object Footstep Source', 'Terrain Footstep Source', 'Tilemap Footstep Source' and 'Trigger Footstep Source' components.
- new: Footstep Materials, Footstep Texture Materials: Materials now have their own, separate icons.


Version 1.0.0:
- Initial Release