# Lab 1 – Steering: Code vs Graph & Flocking

Team: Erika, Gabriele, Bianca. Unity **6000.6.4f1** (use exactly this version).

## Open the project
1. `git clone https://github.com/erikastep/lab-1.git`
2. Unity Hub → **Add → Add project from disk** → select the `lab-1` folder.

## Who works where
| Folder | Owner | Test scene |
| --- | --- | --- |
| `Assets/_Project/Shared/` | Erika | – (Player, Arena, Drive.cs) |
| `Assets/_Project/CodeAgent/` | Gabriele | `CodeAgentTest.unity` |
| `Assets/_Project/GraphAgent/` | Bianca | `GraphAgentTest.unity` |
| `Assets/_Project/Flocking/` | Erika | `FlockingTest.unity` |
| `Assets/_Project/Scenes/Lab1_Main.unity` | Erika | final scene, only Erika edits it |

Only work in your own folder and your own test scene.

## Already set up
- **Arena** prefab: ground, walls, 4 obstacles, NavMeshSurface (already baked in every scene).
- **Player** prefab: blue capsule, tag `Player`, moves with the **arrow keys** (`Drive.cs`, exposes `currentSpeed`).
- Packages: **Behavior** and **AI Navigation**.

## Git
1. Make your own branch: `part1-code-agent` (Gabriele), `part2-graph` (Bianca), `part4-flocking` (Erika).
2. Commit often, always together with the `.meta` files.
3. When done, open a pull request into `main`. Erika merges.
