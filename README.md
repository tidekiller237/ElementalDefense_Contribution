# ElementalDefense_Contribution
My Capstone project at Full Sail University was a game called "Elemental Defense". These are my primary contributions.

Elemental Defense is a tower defense style game that focuses on the elemental interactions between enemies and the turrets and traps that the player has.

This project was worked on by 4 developers (including me) and 2 producers as our bachelor degree program's Capstone Project. This was the final project that we worked on for the curriculum.

In the files here is all of the code that I wrote for the project. I have removed sections of the code that were not written by me (with the exception of variable declarations) to avoid stealing other people's work. These files do not include all of the non-coding related work that I did on the project however majority of my contribution is related to the code that I wrote.

### My Contributions

AI:
 -  I made a contribution to the AI's locomotion script by simplifying the process of moving from one waypoint node to another. I also implemented the ability for the AI to move backwards along the path by moving through the waypoint list in reverse order. The reverse waypoint functionality was part of a mechanic of the "Push Trap".

Projectiles:

 -  At first the "Projectile" script simply fired a projectile prefab in the direction of the selected target from the turret. However it was found to be frustrating when the projectiles would miss due to rounding corners or moving too quickly. So I updated the functionality to instead select a target and Lerp towards that target to give the appearance of a "homing projectile". This reduces frustration and made the projectiles feel less chance based, as the now always hit.

 -  I created the Bouce Projectile as a unique mechanic for one of the turrets. The idea is that it is fired at it's initial target and upon reaching that target, it performs it effect on that target then selects a new target within range if applicable. There are a set-able and upgradable number of bounces that it gets. The projectile also makes sure to not select a previously selected target unless it has no choice.

 -  I fixed the "FireMortar" script to utilize the updated "Projectile" script's implementation.

 -  I create the "FireUnit3FireWall" script to be a unique projectile that is fired from a specific turret. The projectile starts as a small projectile and expands outward to form a rectangular shape filled with fire. It deals damage to all enemies it passes over. Since the functionality for the "Projectile" script had been changed (shown above), I changed the Fire Wall to start as a regular projectile and on impact it then becomes the expanding Fire Wall. The wall travels forward and expands to either side (locally), the forward travel is determined by the direction the regular projectile was traveling at the moment of impact.

Traps:

 -  The traps idea was implementy by me to increase the roster of tools that the player has access to in the game. All traps have some shared functionalily in the "Trap" parent class. This includes only being able to be placed ON the path unlike the turrets. It also includes an arming time, meaning that once you place the trap it won't become armed until the set time has elapsed and it cannot trigger while not armed. They also have a radius that activates the trigger effect when an enemy enters it.

 -  The "BlastTrap" is a simple trap that, once armed, grabs all enemies within it's blast radius and deals damage to them. The blast radius is larger than the trigger radius to make the trap feel impactful.

 -  The "SpikeTrap" is a trap that, when triggered, deals a small initial amount of damage and leaves behind an area of spikes on the ground. Enemies take damage for every second they spend in the spiked area. The spiked area is handled by the "SpikeTrapResidual" script.

 -  The "PushTrap" is the coolest trap to me. When triggered, it creates a wave that expands in radius and pushes all enemies backwards along the path. The "PushTrapSphere" spawns the collider that pushes the enemies and controls is radius. The "Push" script controls the additional pathfinding mechanic that was outlined in the "AI" section above.
    * Note: I am most proud of this trap out of all three of them because I got to utilize my skills in Linear Algebra. When the enemies are pushed by the sphere, there is no guarentee that they will stay on the path while moving backwards. So I used their position and the position of waypoints to correct their position to ensure that they stay along a line between the appropriate waypoints, which also works if they were to be pushe backwards beyond a waypoint. Additionally, I used it to determine if the enemies should be immune to the push effect. If the enemy were already past the waypoints that the trap is between or if they were further along the imaginary line between those waypoints, then that enemy should be ignored by the "Push" collider. Both of these functionalities work as intended and the enemies are pushed backwards along the path perfectly.

UI:

 -   One of my responsabilities was the shop's functionality, so most of the scripts here are parts of the shop. The shop is meant to behave like a "sliding drawer" from the side of the screen. It can be clicked or you can press the spacebar to open/close it, where it will slide on/off the screen depending on the state it was already in. In the shop there are a horizontally grouped list of buttons that show the pictures of the 9 turrets, 3 traps, and 3 "totems". Hovering over any of these buttons will open the "LargeUnitView" where the picture will be bigger and the name, cost, and description will be displayed. Each of the items is held in the "ShopOptions" object so they can be instantiated and placed in the scene.

 -   The other part of my work in the UI department was the UI component of the unit upgrade system. When you hover your mouse over a placed turret (in the game world) it will open a little pop-up that shows you all of the current stats and what level it is. By left-clicking the turret you can upgrade it and by right-clicking the turret you can sell it. The upgraded stats can be previewed in the pop-up that opens.

Unit:

 -  I wrote the functionality for unit upgrading. When you place a turret in the game world, it starts at level 1 and you can spend mana (the games primary resource) to increase it's level. I wrote a function that passes the level through an equation I wrote to procedurally generate the values utilizing the initial values. This is used in the upgrade pop-up meantioned above to predict what the next level's values are going to be.

Utility:

 -  I wrote a simple and short utility script that was a contribution, but it isn't anything crazy. I just thought the "GetPointUnderMouse2D" method would be easier to type than the two lines otherwise. Not the most important contribution that I made.
