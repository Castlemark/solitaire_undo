# Solitaire Undo System

A minimal Solitaire-style card game prototype in Unity demonstrating an **Undo/Redo system**. Players can drag cards anywhere on the screen and revert or replay their moves using UI buttons.

https://github.com/user-attachments/assets/285f7563-8a86-4d3f-a7cb-8284510ca23e

## What Was Built

### Core Features

- **Card Drag-and-Drop**: After clicking on a card, it'll follow the player's mouse until another click is performed. Independently of where the player click on the card, the card will center itself so the mouse is slightly below the top of the card.
- **Card Stacking/Unstacking**: If you click on a card while already dragging another card, the cards will stack. When stacking cards, they appear ordered with a slight vertical offset to be able to see all cards in a stack. To unstack, simply click on a card and drag it away.
- **Visual Feedback**: Cards smoothly lerp when traveling, with a slight rotation to improve the look and feel. Cards also scale up when dragged and use sorting layers to appear on top.
- **Undo/Redo Functionality**: Revert or restore the last move with a single click, maintaining full state.
  - **Undo/Redo** buttons dynamically enable/disable based on history availability.

### Architecture

#### Scripts

```
Assets/Scripts/
├── Card/
│   ├── CardController.cs        - Main card logic, drag handling, stacking
│   ├── CardMover.cs             - Card movement
│   ├── CardHolder.cs            - Container for cards
│   ├── CardVisuals.cs           - Changes the card's visuals
│   └── IClickable.cs            - Interface for click detection, used by CardController and CardHolder
├── Move History/
│   ├── MoveHistoryManager.cs    - Store the history of move, and handles undo/redo actions
│   ├── Move.cs                  - Data structure for recording moves
│   └── MoveHistoryButtonController.cs - UI button state management
├── InputHandler.cs              - Raycast-based click/drag input processing, impplemente using new input system
└── EventBus.cs                  - Serves as an intermediary to send/receive various messages
```

#### Prefabs

```
Assets/Prefabs/
├── Card                  - Self contained card, with movement and visuals logic included
├── CardHolder            - Utility to place cards, doesn't count as a card and can't be picked up
└── MoveHistoryManager    - Contains all the logic needed to undo/redo, UI included 
```

### Notes

- **Card Controller**: By storing and handling references to the cards stacked above and below, we don't have to keep track of stacks. This mean we don't need a centralised manager to handle the interactions between cards, greatly simplifing the logic.
- **Event Bus**: All major actions (drag started, move performed, undo) are broadcast via events, keeping components loosely coupled.
- **Input Handler**: By using the `IClickable` interface, the input handler doesn't need to know anything about what's being clicked.
- **MoveHistoryManager**: To simplify the logic of undo/redo, I've used the command pattern, where each stored `Move` object represents an atomic operation. The system uses two `Stack<Move>` collections, one for undo and one for redo. New moves clear the redo stack to prevent conflicts.
- **Customization**: All relevant paramenters for customization are accessible via the inspector

## Areas for Improvement (Given More Time)

- **IDs for cards**: Right now we store the card references in `Move`. If we used a deterministic way to assign IDs to cards, and stored the ID instead of a reference, it would enable us to implement object polling, save/load system, etc.
- **Abstract card components**: Create an interfacte to abstract `CardMover` and `CardVisuals`. This would enable us to create new types of card, where to movement or visuals logic could be different without having to change anything else.
- **Object Pooling**: Reuse card objects instead of instantiating/destroying them. Right now there is no way to add or remove cards, you can only use the ones present in the scene.
- **Gameplay Logic**: Implement Solitaire rules (e.g., only place lower-rank cards on higher ranks).

## AI-Assisted Development

### How AI Was Used

AI was generally used for better autocompletion, with some occasional guided promting. The most notable highlight is the Card movement, which was done mostly via promting, with fixes and refinement done by me. The Y-sorting logic in the input handler was also done via prompting.

## Tools

- **Engine**: Unity (Latest LTS)
  - **Input System**: New Input System
  - **Animation**: DoTween for card scaling
- **AI**: Github Copilot, via VS Code

## Running the Project

1. Open the project in Unity 6.1 (6000.1.17f1)
2. Load `Assets/Scenes/MainScene.unity`
3. Press Play to start
4. Use mouse to drag cards and click Undo/Redo buttons

## Assets

- Cards by Kenney, [Playing Cards Pack](https://www.kenney.nl/assets/playing-cards-pack)
- Buttons by Kenney, [UI Pack](https://www.kenney.nl/assets/ui-pack-sci-fi)
