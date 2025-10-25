/// <summary>
    /// Interface representing the public API of the old ToolsBase static tool helper.
    /// Implement this interface to provide a tool instance with the same behavior as the original ToolsBase class.
    /// Note: this interface intentionally mirrors the fields and methods from the Gambas->C# translation of ToolsBase.
    /// Implementations should integrate with the surrounding application (gcd, gl, Mouse, clsEntities, etc.).
    /// </summary>
    public interface IToolsBase
    {
        // Identity
        string Gender { get; }
        string USEWITH { get; }

        // Selection pixel coordinates
        int SelStartX { get; set; }
        int SelStartY { get; set; }
        int SelEndX { get; set; }
        int SelEndy { get; set; }

        // Selection pan start (pixels)
        int SelStartPanX { get; set; }
        int SelStartPanY { get; set; }

        // Selection in real coordinates (meters)
        double SelStartXr { get; set; }
        double SelStartYr { get; set; }
        double SelEndXr { get; set; }
        double SelEndyr { get; set; }

        // Start/End in real coordinates for other uses
        double StartXr { get; set; }
        double StartYr { get; set; }
        double EndXr { get; set; }
        double Endyr { get; set; }

        // Mouse tracking
        double LastX { get; set; }
        double LastY { get; set; }

        int MouseX { get; set; }
        int MouseY { get; set; }
        int MouseButton { get; set; }
        bool MouseFakeClick { get; set; }

        // Parameters for builders/tools (defaults can be provided by implementations)
        int PointsDone { get; set; }
        int PointsTotal { get; set; }
        string NextParamType { get; set; }      // "P","F","C","S","M" ...
        object NextParamDefault { get; set; }
        string NextParam { get; set; }         // description
        string Prompt { get; set; }

        bool Active { get; set; }
        bool PoiChecking { get; set; }
        bool EntityChecking { get; set; }
        int Mode { get; set; }
        Entity EntityForEdit { get; set; }
        Entity OriginalEntityForEdit { get; set; }

        string MenuRightClick { get; set; }

        // selected indices (was Integer[] in Gambas)
        List<int> inxSelected { get; set; }

        // transforms used for GL drawing (initialized by implementation)
        double[] glTranslate { get; set; } // dX,dY,dZ
        double[] glRotate { get; set; }    // rX,rY,rZ
        double glAngle { get; set; }       // deg
        double[] glScale { get; set; }     // sX,sY,sZ

        int cursorX { get; set; }
        int cursory { get; set; }

        // Context menu definition (constant in original)
        string ContextMenu { get; }

        // Lifecycle & interaction API
        /// <summary>
        /// Starts the tool. Optionally pass an element to build and a mode.
        /// </summary>
        /// <param name="ElemToBuild">Optional element to build / edit.</param>
        /// <param name="_mode">Optional mode id.</param>
        /// <returns>true if started successfully.</returns>
        bool Start(object ElemToBuild = null, int _mode = 0);

        /// <summary>
        /// Called on each draw frame to render tool overlays / transforms.
        /// </summary>
        void Draw();

        /// <summary>
        /// Keyboard press handler.
        /// </summary>
        void KeyPress(int iCode, string sKey);

        /// <summary>
        /// Key down event.
        /// </summary>
        void KeyDown(int iCode);

        /// <summary>
        /// Key up event.
        /// </summary>
        void KeyUp(int iCode);

        /// <summary>
        /// Called when the user enters text (e.g. presses Enter in command line).
        /// </summary>
        /// <param name="EnteredText">Text entered by the user.</param>
        void KeyText(string EnteredText);

        /// <summary>
        /// Provides a parameter (point/text/float/color, etc.) to the tool.
        /// </summary>
        /// <param name="tipo">Type: "point", "text", "float", etc.</param>
        /// <param name="vValor">Value object (double[], string, etc.).</param>
        /// <param name="Definitive">Whether the parameter is final (true for MouseUp/Enter) or preview (false for MouseMove).</param>
        void NewParameter(string tipo, object vValor, bool Definitive = false);

        /// <summary>
        /// Mouse double-click handler.
        /// </summary>
        void DblClick();

        /// <summary>
        /// Mouse button released.
        /// </summary>
        void MouseUp();

        /// <summary>
        /// Mouse moved.
        /// </summary>
        void MouseMove();

        /// <summary>
        /// Mouse button pressed.
        /// </summary>
        void MouseDown();

        /// <summary>
        /// Mouse wheel event (zoom/pan).
        /// </summary>
        void MouseWheel();

        /// <summary>
        /// Finish the tool: cleanup transforms and return to selection job.
        /// </summary>
        void Finish();

        /// <summary>
        /// Cancel the tool and revert to selection.
        /// </summary>
        void Cancel();

        /// <summary>
        /// Callback run loop (optional).
        /// </summary>
        void Run();
    }
