# 📝 Game Documentation

A place for nots on any classes created for our game in the engine.

---

## ⚙️ Custom Systems

### 🔉Audio Controller
- Created in `Main`
- Encapsulates functions for playing audio to clear up main
- Takes in the Sounds Dictionary when constructed

### ⛰️ Scene Controller
- Created in `Main`
- Stores the current scene and any new ones created
- `_scene` in `Main` is gotten from `SceneController.CurrentScene`

### 🏡 Scene Generator
- Created in `Main`
- Generates the Skybox and Ground

### 🗽 Model Generator
- Created in `Main`
- Returns created Models

### 💻 User Interface Controller
- Created in `Main`
- Takes in the Fonts and 2D Textures on construction

### 🎮 Input Manager
- Created in `Main`
- Creates the Input System based off of the given settings

### 🖌️ Material Generator
- Created in `Main`
- Generates the Lit, Unlit and UnlitGround Materials

---

## ⚙️ Player Systems

### 🕹️ Player Controller
- Created in `Main`
- Stores the `PlayerMovement` and `PlayerCamera` logic

### 🎥 Player Camera
- Created in `PlayerController`
- Handles the first person perspective logic

### 🚶 Player Movement
- Created in `PlayerController`
- Handles the physics based player movement

---

## 🖥️ User Interface Systems

### 🖱️ Cursor Controller
- Created in `Main`
- Creates the Reticle in in the middle of the screen.
- Loads the texture named `reticle` from the JSON

---

## 🎮 Game Systems

### ⚙️ Trap Manager
- Created in `Main`
- Creates a list of `TrapBase` to add to the scene
- Initialises all of the traps
- Updates the list of traps each frame

### 🏹 Trap Base
- Base abstract class all traps inherit from
- Holds the trap `GameObject` and core abstract functions all traps need

### 🪓Moving Trap
- Inherits from `TrapBase`
- Holds the logic for a moving trap such as platforms or axes

---
