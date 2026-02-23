Unity Memory Management Simulation

Overview

This project demonstrates how Unity handles memory allocation and deallocation through resource loading, texture management, and object pooling. The goal of this assignment is to show how proper memory management improves performance and prevents issues such as memory fragmentation, lag, and unnecessary garbage collection.

The project includes scripts that simulate loading resources efficiently, reusing objects instead of constantly creating and destroying them, and testing performance inside the Unity engine.

Features Implemented

	Resource loading using Unity’s built-in systems

	Texture and asset memory optimization

	Object pooling system to reuse game objects

	Performance testing to observe memory behavior

	How Memory Management Works in This Project

Unity allocates memory when assets such as textures, models, or prefabs are loaded into a scene. If these resources are repeatedly created and destroyed, it can cause memory fragmentation and trigger garbage collection, which may result in performance drops.

To avoid this, this project uses:

	Object Pooling to reuse objects instead of destroying them

	Controlled resource loading to prevent unnecessary memory allocation

	Proper cleanup to reduce memory fragmentation

This approach results in smoother gameplay and better performance.

How to Run the Project

	Install Unity Hub

	Open Unity Hub and select Open Project

	Choose the project folder from this repository

	Open the main scene located in the Assets folder

	Press the Play button to run the simulation

How to Recreate the Results

	Run the scene normally and observe object behavior

	Monitor memory usage using Unity’s Profiler

	Observe how pooled objects are reused instead of recreated

	Compare performance stability during runtime

Technologies Used

	Unity Game Engine

	C#
Image Cycling Controls (in ResourceTester.cs)

Control	Action

 	Space	Toggle auto-cycling mode - continuously cycles through all 5 images (3 seconds each)
		Right Arrow	Next image - manually show the next texture, stops auto-cycling
		Left Arrow	Previous image - manually show the previous texture, stops auto-cycling
		M	Display all images simultaneously using multiple pooled displays
		U	Unload a texture from memory
		C	Clear cache and hide all displays
		H	Hide current image and stop cycling
		P	Print pool statistics
