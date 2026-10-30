# Lab 1 – Steering: Code vs Graph & Flocking

Team: Erika, Gabriele, Bianca. Unity **6000.6.4f1** (use exactly this version).

## Open and run
1. `git clone https://github.com/erikastep/lab-1.git`
2. Unity Hub → **Add → Add project from disk** → select the `lab-1` folder.
3. Open `Assets/_Project/Scenes/Lab1_Main.unity` and press **Play**.

### Controls (Lab1_Main)
| Key | What it does |
| --- | --- |
| Arrow keys | Move the Player (blue capsule) in that direction. Both arenas with a Player react. |
| 1 / 2 / 3 / 4 | Camera: Code Agent / Graph Agent / Flocking / All |

## What is in the main scene
`Lab1_Main` has three labelled arenas side by side:

| Area | What it shows | Made by |
| --- | --- | --- |
| **CODE AGENT** | Part 1 – C# agent that **pursues** the player when far and **evades** when closer than `Safety Distance` | Gabriele |
| **GRAPH AGENT** | Part 2 – Behavior Graph agent that **seeks** the player with built-in nodes only | Bianca |
| **FLOCKING** | Part 4 – flock with leaders, adjustable forces and obstacle avoidance | Erika |

## Part 1 – Code agent (`CodeAgent/CodeAgent.cs`)
- `Seek` / `Flee` / `Pursue` / `Evade` steering from Topic 1, moving with a NavMeshAgent.
- The switch between pursue and evade is in `Update()`: closer than `safetyDistance - buffer` → evade, farther than `safetyDistance + buffer` → pursue. The buffer stops it flickering at the border.
- Inspector: **Target**, **Safety Distance**, **Buffer**. Yellow gizmo circle = safety distance.

## Part 2 – Graph seek (`GraphAgent/BehaviourGraphs/Seek`)
- `On Start (Repeat)` → `Navigate To Target`. Only built-in nodes.
- **Target** is an exposed blackboard variable, set per instance on the Behavior Agent component.

## Part 3 – Graph flee attempts (`GraphAgent/BehaviourGraphs/Flee_*`)
Tested in `GraphAgent/GraphAgentTest.unity` (it has the escape points). Swap the **Graph** on the Behavior Agent to try each one.
- `Flee_1_Waypoints` – runs to one fixed escape point.
- `Flee_2_Patrol` – patrols between several escape points.
- `Flee_3_FleePoint_Hidden` – runs to a hidden flee point.

There is no built-in node that computes a point away from the target, so each attempt only works for some player positions. Details are in the report.

## Part 4 – Flocking (`Flocking/Scripts/`)
- `FlockManager.cs` spawns the flock and holds all settings (sliders, can be changed in Play mode):
  - **Num Boids**, **Num Leaders** – the flock respawns when these change
  - **Cohesion / Align / Separation / Leader** weights, **Separation Distance**, **Follow Distance**
  - **Avoid Weight**, **Avoid Distance** – obstacle avoidance
- `Flock.cs` is one boid. Leaders wander; followers use cohesion + align + separation and follow a point behind the closest leader. Sphere casts keep boids out of walls and obstacles, and a personal-space check stops them overlapping.

## Project structure
| Folder | Owner | Test scene |
| --- | --- | --- |
| `Assets/_Project/Shared/` | Erika | – (Player, Arena, `Drive.cs`, `CameraSwitcher.cs`) |
| `Assets/_Project/CodeAgent/` | Gabriele | `CodeAgentTest.unity` |
| `Assets/_Project/GraphAgent/` | Bianca | `GraphAgentTest.unity` |
| `Assets/_Project/Flocking/` | Erika | `FlockingTest.unity` |
| `Assets/_Project/Scenes/Lab1_Main.unity` | Erika | final scene |

Packages: **Behavior** and **AI Navigation**. Every arena has a baked NavMesh.

## Git workflow
Each person worked on their own branch and opened a pull request into `main`; Erika reviewed and merged.
