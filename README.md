# Ecosystem Simulation (Unity)

This project is a **real-time ecosystem simulation** built in Unity.  
It focuses on **emergent behavior**, **needs-driven decision making**, and **observable simulation data**, rather than traditional player-centric gameplay.

The goal of the project is to simulate a living world where plants, herbivores, and carnivores interact through simple rules that collectively produce complex and dynamic outcomes.

---

## 1. High-Level Overview

The simulation consists of four major layers:

1. **World & Map Layer** – Terrain, tiles, biomes, and environmental conditions  
2. **Entity Layer** – Plants and animals that exist within the world  
3. **Behavior Layer** – Needs, decision-making, and state execution  
4. **Observation Layer** – Events, UI, admin tools, and statistics

The system is intentionally designed so that:
- Core gameplay logic is **deterministic and polling-based**
- Events are used for **observation, analytics, and UI**, not core logic
- Entities react to the world rather than following scripted paths

---

flowchart TB

UI[UI / Admin Panel<br/>(Observer & Control)]
EVENTS[Event System<br/>(WorldEvents Hub)]
CORE[Simulation Core<br/>World • Plants • Animals]

CORE -->|raises events| EVENTS
EVENTS -->|subscribes| UI

---

## 2. World & Map System

### Tile-Based World
The world is built on a **grid-based tile system**.  
Each tile represents a small unit of the environment and holds only **state data**, not behavior.

A tile may contain information such as:
- Walkability
- Biome type
- Whether a plant is currently occupying it

Tiles are intentionally kept passive.  
They do not make decisions and do not control simulation flow.

### Procedural Generation
The map is generated procedurally using noise-based techniques and supports:
- Different biome types
- Water bodies
- Blocked and walkable areas

Generation data can be saved and loaded while preserving visual consistency by storing underlying noise values rather than derived colors.

---

## 3. Plant System

Plants represent the **base resource** of the ecosystem.

Key characteristics:
- Plants can be alive or consumed
- They occupy tiles but do not control tiles
- They provide food for herbivores

When a plant is consumed:
- It marks itself as inactive
- The tile is updated to reflect the absence of a plant
- A global event is raised to notify observers (UI, statistics, admin tools)

At this stage, plant respawning is simple and deterministic.  
Future versions may include growth stages, seasons, or nutrient systems.

---

## 4. Animal System

### Animal Base Class
All animals inherit from a shared `Animal` base class.

The base class encapsulates:
- Hunger and thirst
- Age and lifecycle
- Gender and reproduction eligibility
- Vision and perception
- Memory of observed entities
- Movement and navigation
- Death handling

This ensures that herbivores and carnivores share a unified lifecycle while differing only in behavior specialization.

---

## 5. Vision & Memory

### Vision System
Animals perceive the world through a radius-based vision system.

Vision allows animals to detect:
- Food sources (Plants, Herbivores)
- Water sources
- Potential mates

This perception is continuous and updated frequently to allow reactive behavior.

### Memory System
Observed entities are stored temporarily in memory.

Currently, memory is:
- Short-lived
- Passive
- Non-strategic

However, the system is designed to later support:
- Remembering dangerous locations
- Tracking resource-rich areas
- Avoiding previously failed actions

Memory exists as a foundation for more advanced emergent behavior.

---

## 6. Behavior Architecture (FSM + Needs)

### Hybrid FSM Design

The behavior system is **not a classical FSM**, but a **Need-driven Hybrid FSM**.

The system is divided into two conceptual layers:

1. **Decision Layer (Needs)**
2. **Execution Layer (States)**

#### Needs System
Each animal continuously evaluates internal needs such as:
- Hunger
- Thirst
- Fear
- Reproductive urge

Needs are numerical and updated over time.

At each decision point, the most dominant need determines which state should be active.

#### State System (FSM)
States represent *how* an action is executed, not *why* it was chosen.

Examples include:
- Wander
- Seek Food
- Seek Water
- Eat
- Die

Each state defines:
- `Enter()` – setup logic
- `Update()` – execution logic
- `Exit()` – cleanup logic

States themselves do **not** decide transitions.  
They are activated externally by the decision layer.

This allows:
- Flexible overrides
- Priority-based behavior
- Easier extension without rigid transition graphs

---

flowchart TB

NEEDS[Need System<br/>(Decision Layer)<br/><br/>Hunger<br/>Thirst<br/>Mate Urge]
FSM[FSM States<br/>(Execution Layer)<br/><br/>Wander<br/>Seek Food<br/>Seek Water<br/>Eat<br/>Die]
ACTION[Movement / Action]

NEEDS -->|selects dominant need| FSM
FSM --> ACTION

---

## 7. Carnivores & Herbivores

### Herbivores
Herbivores:
- Search for plants as food
- Seek water sources
- Flee from nearby carnivores
- Attempt to reproduce when conditions allow

Their behavior is highly reactive and environment-driven.

### Carnivores
Carnivores:
- Hunt herbivores
- Chase and attack prey
- Consume prey to satisfy hunger
- Share the same need-based decision system

Future extensions may include pack behavior, threat assessment, and territorial logic.

---

## 8. Event System (Observation, Not Control)

The project includes a global event hub.

Events are raised for significant occurrences such as:
- Plant consumption
- Animal birth
- Animal death
- Environmental changes (e.g., drought, disease)

**Important design principle:**
Events do **not** drive core simulation logic.

Instead, they are used for:
- UI updates
- Analytics and statistics
- Debugging and admin tools
- Visualization of simulation history

This separation ensures that simulation behavior remains deterministic and debuggable.

---

flowchart TB

CORE[Simulation Core]
EVENTS[WorldEvents Hub]

UI[UI]
STATS[Statistics / Analytics]
ADMIN[Admin Panel]

CORE -->|raises event| EVENTS
EVENTS --> UI
EVENTS --> STATS
EVENTS --> ADMIN

---

## 9. UI & Admin Panel

The project includes an in-game admin panel designed as a **simulation control interface**, not a traditional game UI.

Capabilities include:
- Spawning animals
- Triggering world events (e.g., drought, disease)
- Killing plants or animals
- Inspecting population and state data

The UI acts as an observer and controller, allowing experimentation without interfering with core logic flow.

---

## 10. Current State of the Simulation

At its current stage, the simulation supports:
- Persistent world generation
- Living plant-animal food chains
- Reactive animal behavior
- Population changes over time
- Real-time observation through UI

The ecosystem is **not yet self-balancing** and does not include learning or long-term adaptation.  
These are intentional future milestones rather than missing features.

---

## 11. Future Direction

Planned or possible extensions include:
- Advanced plant growth cycles
- Long-term memory-based decision making
- Seasonal and environmental events
- Population balancing mechanisms
- Group and pack behaviors
- Data visualization and simulation analytics

---

## 12. Philosophy

This project prioritizes:
- Clarity over premature optimization
- Simple rules over hard-coded behavior
- Observability over hidden logic
- Evolutionary growth of systems over rigid architecture

The simulation is designed to **grow organically**, mirroring the ecosystem it represents.

