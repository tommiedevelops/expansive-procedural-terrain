# HeightMap Procedural Terrain

## Overview
- This Unity Asset provides a simple and intuitive system for generating stylized Height-Map based terrain.

## Features
- An Editor Mode preview to set terrain dimensions and place it accordinly in your scene.
- An Editor Mode Noise Editor / Layering system which allows you to write custom Height Map functions, layer them onto each other and preview them before applying it to the Terrain Generation system.
- An optimised run-time mesh generator which makes use of Quad Trees, distance-based LOD's and object pooling to ensure terrain is  dynamically generated efficiently.

## How To Use
### TerrainGenerator
- Start by inserting the TerrainGenerator Prefab into your scene. Each TerrainGenerator is independent so you can have as many in your scene as you like.
- Click on the TerrainGenerator GameObject and customise to your liking. You can adjust the Terrain dimensions, add Materials to stylize the Terrain and add Noise Layers.
- You can access the HeightMap through the TerrainGenerator's API if you need to use it for other systems. 
### NoiseEditor
- Go to 'Tools' drop down and open the NoiseEditor.
- You can edit the paremeters of the included Noise generators by selecting the Scriptable Object. 
- If you would like to generate your own HeightMap functions, add a new script to NoiseSystem/NoiseLayers and inherit from NoiseLayersSO. You then just have to implement the required functions to use it as a noise layer.

