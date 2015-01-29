MeshPainter - Paint directly on meshes
v2.0.2 - Released 03/09/2014 by Alan Baylis
----------------------------------------


Foreword
----------------------------------------
Thank you for your purchase of MeshPainter. This program allows you to create custom artwork painting directly on your game objects or collaborate by leaving directions, color choices and other design notes to the team. Now you can literally sketch out your scene before you build!


Notes
----------------------------------------
The first thing to do is drag the folder named 'Editor Default Resources' from the main folder and drag it to the root of the Assets folder in your project. This will add a small icon next to the title on the tab for the window the next time Unity starts.

There is no undo function yet but that will be the next addition to the program.

Lighting calculations are not always quite right depending on the object being painted or the shader being used on that object. This will be fixed in the next full update.

If the program is not painting on an object make sure the object has a Mesh Collider attached and that you are in Brush or Texture mode. Later versions of Unity don't automatically remove the existing collider when adding a new collider so you may end up with two colliders on the same object, one box collider and one mesh collider. Either remove the box collider or turn it off by unselecting it in the inspector. If you are still having problems then selecting a color before painting may also help.

MeshPainter uses meshes to paint on that it stores in the Meshes folder with a unique 20 character name. If you find these and are sure they are not in use you can safely delete them. But to avoid this you should clear an object of its paintings before deleting it as this will destroy the meshes and textures used in the painting process.


To-do List.
----------------------------------------
Done - Direct painting of meshes in Scene View window.
UV unwrapping and editing window.
Better painting across triangle edges.
Different brushes.
Fill option.


Common Issues / FAQ
----------------------------------------
Please visit the home page at http://www.meshmaker.com for the latest news and help forum.


Contact
----------------------------------------
Alan Baylis
www.meshmaker.com
support@meshmaker.com


Update Log
-----------------------------------------
v1.0.0 released 19/02/14
First release of MeshPainter.

v2.0.0 released 15/05/14
MeshPainter now paints directly on objects in the Scene View window.
Original MeshPainter functionality moved to Mesh Tools.

v2.0.1 released 04/06/14
Cleaned up all of the mesh leaks though one texture leak warning still remains.

v2.0.2 released 03/09/14
New GUI layout.



