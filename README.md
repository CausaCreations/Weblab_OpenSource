Weblab Documentation

Weblab was developed in the Unity Engine and uses Unity's own solutions for input handling, rendering, collision detection, and so on. Building on this, the basic functionality of the game was developed:

Tile
A tile consists of several square subcomponents that can be combined to create different shapes.

Tile Spawner
The tile spawners exist on the UI level of the game. When touched with a finger or clicked with the cursor, the corresponding tile prefab is instantiated and drag and drop is activated. The tile spawner also keeps track of how many tiles can still be placed and deactivates itself when all available tiles of this type are already on the playing field. 

Drag and drop
When a tile is instantiated by the tile spawners or picked up from the playing field, drag and drop is activated and ensures that the tile is placed at the position of the finger or cursor until it is released. At the same time, the position of the tile is restricted to the screen. 

When the tile is released, it is first moved so that it matches the grid exactly. Then, a 2D box collider is used to check whether the tile is within the playing field and does not overlap with other tiles. If it is incorrectly placed, it is sent back to the tile spawner and deleted. 

Tile Grid
The tile grid is based on Unity's grid component, which allows world space coordinates to be mapped to a grid. When a tile is correctly placed on the playing field, the X and Y positions of each subcomponent are stored in the grid coordinate system.

Tile Connection
Once tiles have been placed on the playing field, the surrounding fields of the grid are searched for neighboring tiles. If a neighbor is found, this is saved.
After all new neighbors have been noted, an algorithm is started that searches for a path from the starting point to the end points via all connected tiles. Various paths are traversed and saved. At the end, overlapping paths are sorted out and the remaining paths are drawn on the playing field using a Unity Line Renderer. All of this happens every time the playing field changes – i.e., when a new tile is placed or an existing tile is removed or moved. 

Objectives
Each level has different objectives—tasks that must be completed in order to earn one to three stars and finish the level. Each task counts individually for how much of it has been completed—these percentages are added together to form an overall percentage. However, tasks with higher stars can only be unlocked once the tasks with lower stars have been completed. 
The tasks that are tracked include, for example: which endpoints are connected or not connected, whether too many resources are being used, or whether there is enough power supply. 

Provided Graphics, Project and Code are (c)Causa Creations 2025 and are released with a CC-BY 4.0 license (https://creativecommons.org/licenses/by/4.0/)
