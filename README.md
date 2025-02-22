# 🃏 **CardGameTemplate** 🃏

Welcome to **CardGameTemplate**, a Unity-based game where players test their memory skills by matching pairs of cards within the fewest possible turns.

---

## 📋 **Table of Contents**
- [Overview](#overview)
- [Features](#features)
- [Gameplay](#gameplay)
- [Installation](#installation)
- [Project Structure](#project-structure)
- [Scripts Breakdown](#scripts-breakdown)
- [How to Contribute](#how-to-contribute)
- [License](#license)
- [Contact](#contact)

---

## 🗃️ **Overview**
This game challenges players to match pairs of cards in a grid by flipping them over two at a time. The game keeps track of the number of turns taken, and the best scores are saved in a leaderboard. Built using Unity, the game also features background music, sound effects, and customizable levels.

---

## ✨ **Features**
- 🎮 Simple and intuitive gameplay
- 📦 Object pooling for performance optimization
- 🗂️ Easily configurable card sprites and audio clips using ScriptableObjects
- 🏆 Leaderboard that saves top 5 scores
- 🎶 Background music and sound effects with adjustable volume
- 🧩 Flexible grid sizes for different difficulty levels
- 💾 Persistent leaderboard using Unity’s PlayerPrefs
- 📱 Mobile-friendly UI

---

## 🕹️ **Gameplay**
1. Flip two cards by clicking on them.
2. If the cards match, they stay flipped. Otherwise, they flip back.
3. Continue until all pairs are matched.
4. The game tracks the number of turns. Fewer turns mean a better score!
5. When all pairs are matched, the game saves your score if it's in the top 5.

---

## 💻 **Installation**
1. Ensure you have [Unity](https://unity.com/) installed (Unity 2020 or later recommended).
2. Clone this repository:
    ```bash
    git clone https://github.com/your-username/CardGameTemplate.git
    ```
3. Open the project in Unity.
4. Press **Play** in the Unity Editor to start the game.

---

## 🧩 **Scripts Breakdown**
### 1. **AudioManager.cs**
- Singleton to manage audio across scenes.
- Plays background music and sound effects.

### 2. **Card.cs**
- Represents an individual card.
- Handles flipping, matching, and initialization.

### 3. **CardGrid.cs**
- Generates the card grid using object pooling.
- Ensures optimal performance by reusing card objects.

### 4. **GameData.cs**
- ScriptableObject storing references to card sprites and audio clips.

### 5. **GameManager.cs**
- Core gameplay logic, including card interactions, scoring, and win conditions.
- Controls the game flow and updates the UI.

### 6. **LeaderboardManager.cs**
- Singleton that manages the leaderboard.
- Saves and loads scores using PlayerPrefs.

### 7. **LevelManager.cs**
- Simplified manager for loading levels with different grid sizes.

---