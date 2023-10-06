# Reinforcement Learning T-809-DATA

This repository contains a Reinforcement Learning (RL) project where an agent (represented as a block) is trained to move towards a goal (represented as a ball) in a Unity environment. The project uses ML-Agents, an open-source project that enables games and simulations in Unity to serve as environments for training intelligent agents.

## Directory Structure

```
.
├── BallMaterial.mat
├── BoxMaterial.mat
├── Environment.prefab
├── LoseMaterial.mat
├── PlatformMaterial.mat
├── WallMaterial.mat
├── WinMaterial.mat
│
├── ML-Agents
│   ├── Timers
│   │   └── SampleScene_timers.json
│
├── Scenes
│   └── SampleScene.unity
│
└── Scripts
    ├── Goal.cs
    ├── MoveToGoalAgent.cs
    └── Wall.cs
```

## Components

- **Materials**: The various `.mat` files represent materials used in the Unity environment. These help in distinguishing between different objects in the scene, such as the ball (goal), box (agent), walls, etc.
- **Environment.prefab**: A prefab is a reusable game object. In this case, it represents a single game environment where the agent learns.
- **ML-Agents**: This directory contains configurations and timers related to the ML-Agents toolkit, specifically for the sample scene.
- **Scenes**: Contains the main Unity scene file where the game simulation runs.
- **Scripts**: Contains the primary C# scripts that define the behavior and learning mechanisms of the agent and other game objects.
