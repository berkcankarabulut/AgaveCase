 
<html lang="en">
<body>
    <div style="display: flex; justify-content: center; margin: 30px 0;">
        <img src="https://github.com/user-attachments/assets/1d798ffc-9029-4810-b152-c7b0f89ba85d" alt="Gameplay GIF" style="max-width: 100%; border-radius: 8px; box-shadow: 0 4px 8px rgba(0,0,0,0.2);">
    </div>
    <h1> 
        <span class="emoji">🎮</span>
        Agave Case Match-3 Game
    </h1>    
    <p>A modular, component-based Match-3 game framework built with Unity, utilizing state machines and service-oriented architecture.</p>    
    <h2>
        <span class="emoji">📋</span>
        Overview
    </h2>    
    <p>This project demonstrates a robust design architecture for a Match-3 game, implementing modern software design principles including:</p>    
    <ul>
        <li>State Machine Architecture</li>
        <li>Service-oriented Design</li>
        <li>Object Pooling</li>
        <li>Component-based Structures</li>
        <li>Adapter Pattern</li>
        <li>Event-driven Communication</li>
    </ul>    
    <h2>
        <span class="emoji">🏗️</span>
        Architecture
    </h2>    
    <p>The system is built using several interconnected modules, each with specific responsibilities:</p>    
    <div class="container">
        <div class="module">
            <h3>State Machine Framework</h3>
            <p>Core infrastructure managing state transitions throughout the application:</p>
            <ul>
                <li><code>IState</code>: Base interface for all states</li>
                <li><code>BaseState</code>: Abstract implementation for generic states</li>
                <li><code>BaseStateMachine</code>: State management and transition control</li>
                <li><code>BaseGameState</code>: Game-specific state implementation</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Game State System</h3>
            <p>Controls the main game flow and coordinates between systems:</p>
            <ul>
                <li><code>GameStateMachine</code>: Main game flow controller</li>
                <li><code>PlayingState</code>: Active gameplay state</li>
                <li><code>WinState</code>: Game victory state</li>
                <li><code>LoseState</code>: Game over state</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Service Interfaces</h3>
            <p>Defines communication contracts between modules:</p>
            <ul>
                <li><code>IBoardService</code>: Board operations and grid management</li>
                <li><code>IScoreService</code>: Score tracking and target management</li>
                <li><code>IMoveService</code>: Move counting and limitation</li>
                <li><code>IUIService</code>: UI display and interaction</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Board System</h3>
            <p>Manages the game board, grid structure, and element interactions:</p>
            <ul>
                <li><code>BoardManager</code>: Central controller for board operations</li>
                <li><code>GridHandler</code>: Creates and manages grid structure</li>
                <li><code>ElementHandler</code>: Manages game elements on the grid</li>
                <li><code>MatchDetectionHandler</code>: Evaluates potential and actual matches</li>
                <li><code>BoardAnimationHandler</code>: Controls board-wide animations</li>
                <li><code>ShuffleHandler</code>: Reorganizes board when no matches are possible</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Element System</h3>
            <p>Handles individual game elements:</p>
            <ul>
                <li><code>ElementBase</code>: Base class for grid elements</li>
                <li><code>DefaultElement</code>: Standard game element implementation</li>
                <li><code>ElementDataSO</code>: Scriptable Object for element configuration</li>
                <li><code>ElementAnimationHandler</code>: Controls element animations</li>
                <li><code>ElementStateHandler</code>: Manages element states</li>
                <li><code>ElementPoolHandler</code>: Interface with pooling system</li>
            </ul>
        </div>        
   <div class="module">
    <h3>UI System</h3>
    <p>Manages game interface and state transitions:</p>
    <ul>
        <li><code>GameUIStateMachine</code>: Controls UI state transitions (Playing, Win, Lose)</li>
        <li><code>PlayingState</code>: Manages in-game UI elements, score/move updates, and animations</li>
        <li><code>WinState</code>: Handles win screen display and reset functionality</li>
        <li><code>LoseState</code>: Manages lose screen display and reset functionality</li>
    </ul>
</div>     
        <div class="module">
            <h3>Data System</h3>
            <p>Contains configuration and game parameters:</p>
            <ul>
                <li><code>DataContainer</code>: Main data holder Scriptable Object</li>
                <li><code>GameData</code>: Game settings and parameters</li>
                <li><code>GridData</code>: Grid configuration data</li>
                <li><code>ScoreData</code>: Score tracking data</li>
                <li><code>MoveData</code>: Move tracking data</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Object Pooling System</h3>
            <p>Optimizes object creation and destruction:</p>
            <ul>
                <li><code>ObjectPooler</code>: Main pooling manager</li>
                <li><code>IPoolable</code>: Interface for poolable objects</li>
                <li><code>PoolObjectTypeSO</code>: Type definition for pooled objects</li>
            </ul>
        </div>        
        <div class="module">
            <h3>Scene Management</h3>
            <p>Handles scene transitions and loading:</p>
            <ul>
                <li><code>GameSceneManager</code>: Manages scene flow</li>
                <li><code>SceneLoader</code>: Handles scene loading operations</li>
                <li><code>VoidEventChannelSO</code>: Event communication channel</li>
            </ul>
        </div>
    </div>      
    <h2>
        <span class="emoji">🎮</span>
        Gameplay Features
    </h2>    
    <ul class="feature-list">
        <li>Grid-based Match-3 mechanics</li>
        <li>Element matching and chain reactions</li>
        <li>Score tracking with target goals</li>
        <li>Move limitations</li>
        <li>Board shuffling when no matches are possible</li>
        <li>Win/Lose conditions</li>
    </ul>    
    <h2>
        <span class="emoji">🛠️</span>
        Technical Features
    </h2>    
    <ul class="feature-list">
        <li>Scalable and modular architecture</li>
        <li>Reusable component design</li>
        <li>Memory-efficient object pooling</li>
        <li>Event-driven communication between systems</li>
        <li>Scriptable Objects for data management</li>
        <li>Service Layer abstraction for system communication</li>
    </ul>    
    <h2>
        <span class="emoji">🧩</span>
        Design Patterns Used
    </h2>    
    <ul class="feature-list">
        <li><strong>State Pattern</strong>: Game flow management</li>
        <li><strong>Observer Pattern</strong>: Event handling</li>
        <li><strong>Adapter Pattern</strong>: System communication</li>
        <li><strong>Factory Pattern</strong>: Element creation</li>
        <li><strong>Object Pool Pattern</strong>: Resource management</li>
        <li><strong>Strategy Pattern</strong>: Match detection algorithms</li>
        <li><strong>Command Pattern</strong>: Input handling</li>
    </ul>    
    <h2>
        <span class="emoji">🚀</span>
        Getting Started
    </h2>    
    <ol>
        <li>Clone the repository</li>
        <li>Open the project in Unity (version 2021.3 or higher recommended)</li>
        <li>Open the Persistent Scene in <code>Assets/Scenes/</code></li>
        <li>Press Play to run the game</li>
    </ol>    
    <h2>
        <span class="emoji">⚙️</span>
        Configuration
    </h2>    
    <p>Game parameters can be modified through the following Scriptable Objects:</p>
    <ul>
        <li><code>GameDataContainer</code>: Main configuration object</li>
        <li><code>ElementData</code> assets: Individual element properties</li>
    </ul>    
    <h2>
        <span class="emoji">📄</span>
        License
    </h2>    
    <p>This project is licensed under the MIT License - see the LICENSE file for details.</p>    
    <h2>
        <span class="emoji">🤝</span>
        Acknowledgements
    </h2>    
    <p>This project was developed as a case study implementation showcasing modern game architecture principles and patterns in Unity.</p>
</body>
</html>
