# Game Engine Architecture Overview

This document provides an overview of the architecture for our custom game engine, including diagrams for various
subsystems.

## Switchable DirectX Architecture

```mermaid
classDiagram
    class IRenderingAPI {
        <<interface>>
        +Initialize()
        +CreateTexture()
        +DrawMesh()
        +Present()
    }
    class DirectX12API {
        +Initialize()
        +CreateTexture()
        +DrawMesh()
        +Present()
    }
    class DirectX11API {
        +Initialize()
        +CreateTexture()
        +DrawMesh()
        +Present()
    }
    class DirectX10API {
        +Initialize()
        +CreateTexture()
        +DrawMesh()
        +Present()
    }
    class RenderingSystem {
        -currentAPI: IRenderingAPI
        +SetAPI(api: IRenderingAPI)
        +Render()
    }
    IRenderingAPI <|.. DirectX12API
    IRenderingAPI <|.. DirectX11API
    IRenderingAPI <|.. DirectX10API
    RenderingSystem --> IRenderingAPI
```

## UI System Architecture

```mermaid
classDiagram
    class UIElement {
        +Render()
        +Update()
        +HandleInput()
    }
    class Button {
        -onClick: Action
        +SetClickHandler(handler: Action)
    }
    class TextBox {
        -text: string
        +SetText(text: string)
        +GetText(): string
    }
    class Panel {
        -children: List~UIElement~
        +AddChild(child: UIElement)
        +RemoveChild(child: UIElement)
    }
    class UIParser {
        +ParseFromString(markup: string): UIElement
    }
    class StyleManager {
        +ApplyStyle(element: UIElement, style: string)
    }
    UIElement <|-- Button
    UIElement <|-- TextBox
    UIElement <|-- Panel
    UIParser --> UIElement
    StyleManager --> UIElement
```

## GameObject System Architecture

```mermaid
classDiagram
    class GameObject {
        -components: List~Component~
        +AddComponent(component: Component)
        +GetComponent~T~(): T
        +Update()
    }
    class Component {
        <<abstract>>
        +Initialize()
        +Update()
    }
    class TransformComponent {
        +Position: Vector3
        +Rotation: Quaternion
        +Scale: Vector3
    }
    class RenderComponent {
        -mesh: Mesh
        -material: Material
        +Render()
    }
    GameObject "1" *-- "many" Component
    Component <|-- TransformComponent
    Component <|-- RenderComponent
```

## Data Saving System Architecture

```mermaid
classDiagram
    class SaveSystem {
        +Save(data: ISerializable, filename: string)
        +Load~T~(filename: string): T
    }
    class ISerializable {
        <<interface>>
        +Serialize(): byte[]
        +Deserialize(data: byte[])
    }
    class JsonSerializer {
        +Serialize(obj: object): string
        +Deserialize~T~(json: string): T
    }
    class BinarySerializer {
        +Serialize(obj: object): byte[]
        +Deserialize~T~(data: byte[]): T
    }
    SaveSystem --> ISerializable
    SaveSystem --> JsonSerializer
    SaveSystem --> BinarySerializer
```

## Core Game Engine Architecture

```mermaid
classDiagram
    class Engine {
        +Initialize()
        +Run()
        +Shutdown()
    }
    class Renderer {
        -DirectX12API
        +RenderFrame()
    }
    class UISystem {
        -HTMLParser
        -CSSParser
        +RenderUI()
    }
    class InputManager {
        +ProcessInput()
    }
    class AudioSystem {
        +PlaySound()
    }
    class PhysicsEngine {
        +SimulatePhysics()
    }
    class ResourceManager {
        +LoadAsset()
        +UnloadAsset()
    }
    class SceneManager {
        +LoadScene()
        +UpdateScene()
    }
    Engine --> Renderer
    Engine --> UISystem
    Engine --> InputManager
    Engine --> AudioSystem
    Engine --> PhysicsEngine
    Engine --> ResourceManager
    Engine --> SceneManager
```

This document provides a high-level overview of the architecture for our custom game engine. Each diagram represents a
key subsystem of the engine, showing the relationships between major components and classes.

As the engine development progresses, these diagrams should be updated to reflect any changes in the architecture. They
serve as a valuable reference for understanding the overall structure of the engine and how different parts interact
with each other.
