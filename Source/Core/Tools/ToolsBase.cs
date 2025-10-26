
    // Port of Gambas ToolsBase.class -> C#
    //
    // Notes:
    // - This is a direct behavioral translation, not a full idiomatic rewrite.
    // - External globals and types referenced here (Gcd, gl, glx, Mouse, clsEntities, cadSelection, clsMouseTracking, Paint, Color, DrawingAIds, etc.)
    //   must exist in your project. Replace or adapt references to match your codebase.
    // - Gambas "Float" -> double. Collections -> List<T> or project-specific collection types.
    // - Some methods are left as thin wrappers/stubs (NewParameter, Run, KeyPress/KeyDown/KeyUp) because their behavior depends on other classes.
    public static class ToolsBase
    {
        public const string Gender = "TOOLSBASE";
        public const string USEWITH = "";

        // Selection pixel coordinates (initialize to 0)
        public static int SelStartX = 0;
        public static int SelStartY = 0;
        public static int SelEndX = 0;
        public static int SelEndy = 0;

        // Selection pan start (pixels)
        public static int SelStartPanX = 0;
        public static int SelStartPanY = 0;

        // Selection in real coordinates (meters)
        public static double SelStartXr = 0.0;
        public static double SelStartYr = 0.0;
        public static double SelEndXr = 0.0;
        public static double SelEndyr = 0.0;

        // Start/End in real coordinates for other uses
        public static double StartXr = 0.0;
        public static double StartYr = 0.0;
        public static double EndXr = 0.0;
        public static double Endyr = 0.0;

        // Mouse tracking
        public static double LastX = 0.0;
        public static double LastY = 0.0;

        public static int MouseX = 0;
        public static int MouseY = 0;
        public static int MouseButton = 0;
        public static bool MouseFakeClick = false;

        // Parameters for builders/tools (initialize to sensible defaults)
        public static int PointsDone = 0;
        public static int PointsTotal = 0;
        public static string NextParamType = string.Empty;      // "P","F","C","S","M" ...
        public static object NextParamDefault = null;
        public static string NextParam = string.Empty;         // description
        public static string Prompt = string.Empty;

        public static bool Active = false;
        public static bool PoiChecking = false;
        public static bool EntityChecking = false;
        public static int Mode = 0;
        public static Entity EntityForEdit = null;
        public static Entity OriginalEntityForEdit = null;

        public static string MenuRightClick = string.Empty;

        // selected indices (was Integer[] in Gambas)
        public static List<int> inxSelected = new List<int>();

        // transforms used for GL drawing (initialized to identity / defaults)
        public static double[] glTranslate = new double[] { 0.0, 0.0, 0.0 }; // dX,dY,dZ
        public static double[] glRotate = new double[] { 0.0, 0.0, 1.0 };    // rX,rY,rZ
        public static double glAngle = 0.0;                                  // deg
        public static double[] glScale = new double[] { 1.0, 1.0, 1.0 };     // sX,sY,sZ

        public static int cursorX = 0;
        public static int cursory = 0;

        public const string ContextMenu = "Finish;_FINISH;;;Cancel;_CANCEL;;";

        // Static ctor kept minimal (arrays already initialized above)
        static ToolsBase()
        {
            // All fields have explicit initial values at declaration.
            // This static constructor is retained for future initialization needs.
        }

        // Start the tool (optionally with an element to build)
        public static bool Start(object ElemToBuild = null, int _mode = 0)
        {
            PointsDone = 0;
            Mode = _mode;
            // TODO: hook context menu into UI if required.
            return true;
        }

        // Called by drawing loop to render tool overlays / selected entities
        public static void Draw()
        {
            // Translate by sheet pan, then apply tool transforms and call the selected entities GL list.
            // Assumes 'gl' API exists with PushMatrix/PopMatrix/Translatef/Rotatef/Scalef/CallList methods.
            try
            {
                gl.Translatef(Gcd.Drawing.Sheet.PanBaseRealX, Gcd.Drawing.Sheet.PanBaseRealY, 0.0f);
                gl.PushMatrix();
                gl.Translatef((float)glTranslate[0], (float)glTranslate[1], (float)glTranslate[2]);
                gl.Rotatef((float)glAngle, (float)glRotate[0], (float)glRotate[1], (float)glRotate[2]);
                gl.Scalef((float)glScale[0], (float)glScale[1], (float)glScale[2]);
                gl.CallList(Gcd.Drawing.GlListEntitiesSelected);
                gl.PopMatrix();
                gl.Translatef(-(float)Gcd.Drawing.Sheet.PanBaseRealX, -(float)Gcd.Drawing.Sheet.PanBaseRealY, 0.0f);
            }
            catch (Exception)
            {
                // swallow to avoid crash when GL not ready; consider logging
            }
        }

        // keyboard hooks (override in specific tools)
        public static void KeyPress(int iCode, string sKey) { }
        public static void KeyDown(int iCode) { }
        public static void KeyUp(int iCode) { }

        // Called when user submits text (Enter)
        public static void KeyText(string EnteredText)
        {
            if (string.IsnullOrWhiteSpace(EnteredText)) return;

            EnteredText = EnteredText.Trim();
            var upper = EnteredText.ToUpperInvariant();

            // handle tool-global commands
            switch (upper)
            {
                case "_CANCEL":
                    Cancel();
                    return;
                case "_FINISH":
                    Finish();
                    return;
                case "_ADD":
                    Gcd.clsJobPrevious = Gcd.clsJob;
                    Gcd.clsJob = cadSelection.Instance;
                    cadSelection.Instance.Mode = cadSelection.ModeAddEntities;
                    return;
                case "_REM":
                case "_REMOVE":
                    Gcd.clsJobPrevious = Gcd.clsJob;
                    Gcd.clsJob = cadSelection.Instance;
                    cadSelection.Instance.Mode = cadSelection.ModeRemoveEntities;
                    return;
                // Snap commands
                case "_MIDPOINT":
                case "_MID":
                    Gcd.SnapMode = Gcd.poiMIdPoint;
                    return;
                case "_ENDPOINT":
                case "_END":
                    Gcd.SnapMode = Gcd.poiEndPoint;
                    return;
                case "_PERPENDICULAR":
                case "_PER":
                    Gcd.SnapMode = Gcd.poiPerpendicular;
                    return;
                case "_NEAREST":
                case "_NEA":
                    Gcd.SnapMode = Gcd.poiNearest;
                    return;
                case "_CENTER":
                case "_CEN":
                    Gcd.SnapMode = Gcd.poiCenter;
                    return;
                case "_INTERSECTION":
                case "_INT":
                    Gcd.SnapMode = Gcd.poiIntersection;
                    return;
                case "_BASEPOINT":
                case "_BAS":
                    Gcd.SnapMode = Gcd.poiBasePoint;
                    return;
                case "_TANGENT":
                case "_TAN":
                    Gcd.SnapMode = Gcd.poiTangent;
                    return;
                case "_QUADRANT":
                case "_QUA":
                    Gcd.SnapMode = Gcd.poiQuadrant;
                    return;
            }

            // If the input looks like a point, send as point parameter; otherwise text parameter.
            if (Gcd.IsPoint(EnteredText))
            {
                string errtxt = ", expected a valid point like 12.4,9.5 or @12.34,10.5";
                EnteredText = EnteredText.Trim();
                bool relative = false;

                if (EnteredText.Contains("@"))
                {
                    relative = true;
                    EnteredText = EnteredText.Replace("@", "");
                }

                var parts = EnteredText.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 2 &&
                    double.TryParse(parts[0].Trim(), out double xt) &&
                    double.TryParse(parts[1].Trim(), out double yt))
                {
                    if (relative)
                    {
                        xt += LastX;
                        yt += LastY;
                    }

                    LastX = xt;
                    LastY = yt;

                    // store last point in drawing
                    Gcd.Drawing.LastPoint.Clear();
                    Gcd.Drawing.LastPoint.Insert(new double[] { xt, yt });

                    NewParameter("point", Gcd.Drawing.LastPoint, true);
                }
                else
                {
                    // invalid point - ignore or notify user
                }
            }
            else
            {
                NewParameter("text", EnteredText, true);
            }
        }

        // Called by the tool to accept a new parameter (to be implemented by specific tools / entity handlers)
        public static void NewParameter(string tipo, object vValor, bool Definitive = false)
        {
            // default implementation does nothing.
            // Specific tools should override / provide callbacks to receive parameters.
        }

        // Double click handling: choose viewport under mouse etc.
        public static void DblClick()
        {
            SelStartX = Mouse.X;
            SelStartY = Mouse.Y;
            SelStartXr = Gcd.Xreal(SelStartX);
            SelStartYr = Gcd.Yreal(SelStartY);

            // If a viewport is active and click outside it, deactivate
            if (Gcd.Drawing.Sheet.Viewport != null)
            {
                var v = Gcd.Drawing.Sheet.Viewport;
                if (SelStartXr < v.X0 || SelStartXr > v.X1 || SelStartYr < v.Y0 || SelStartYr > v.Y1)
                {
                    Gcd.Drawing.Sheet.Viewport = null;
                }
            }

            // Find any viewport that contains the clicked point and activate it
            if (Gcd.Drawing.Sheet.Viewports != null)
            {
                foreach (var v in Gcd.Drawing.Sheet.Viewports)
                {
                    if (SelStartXr >= v.X0 && SelStartXr <= v.X1 && SelStartYr >= v.Y0 && SelStartYr <= v.Y1)
                    {
                        Gcd.Drawing.Sheet.Viewport = v;
                        break;
                    }
                }
            }
        }

        // Mouse up: handle point selection and parameter submission
        public static void MouseUp()
        {
            // Right click -> cancel via NewParameter
            if (Mouse.Right)
            {
                NewParameter("action", "_CANCEL", true);
                return;
            }

            double x = Gcd.Xreal(Mouse.X);
            double y = Gcd.Yreal(Mouse.Y);

            // POI checking (snaps): pick best POI if available
            if (!Gcd.flgSearchingPOI)
            {
                var iEntity = clsMouseTracking.CheckBestPOI(x, y);
                if (iEntity != null && iEntity.Length >= 3 && iEntity[2] > 0)
                {
                    x = iEntity[0];
                    y = iEntity[1];
                }
            }

            x = Gcd.Near(x);
            y = Gcd.Near(y);

            // orthogonal mode: snap axis depending on which delta is larger
            if (PointsDone > 0 && Gcd.Orthogonal)
            {
                if (Math.Abs(x - LastX) > Math.Abs(y - LastY))
                {
                    y = LastY;
                }
                else
                {
                    x = LastX;
                }
            }

            LastX = x;
            LastY = y;

            NewParameter("point", new double[] { LastX, LastY }, true);

            Gcd.redraw();
        }

        // Mouse move: update preview parameter with current point
        public static void MouseMove()
        {
            double x = Gcd.Xreal(Mouse.X);
            double y = Gcd.Yreal(Mouse.Y);

            if (!Gcd.flgSearchingPOI)
            {
                var iEntity = clsMouseTracking.CheckBestPOI(x, y);
                if (iEntity != null && iEntity.Length >= 3 && iEntity[2] > 0)
                {
                    x = iEntity[0];
                    y = iEntity[1];
                }
            }

            x = Gcd.Near(x);
            y = Gcd.Near(y);

            if (PointsDone > 0 && Gcd.Orthogonal)
            {
                if (Math.Abs(x - LastX) > Math.Abs(y - LastY))
                {
                    y = LastY;
                }
                else
                {
                    x = LastX;
                }
            }

            NewParameter("point", new double[] { x, y }, false);
        }

        // Mouse down: mostly handled at higher level; handle right clicks to change jobs etc.
        public static void MouseDown()
        {
            if (Mouse.Right)
            {
                if (Gcd.clsJob != null && Gcd.clsJob.Gender == "BUILDER")
                {
                    Gcd.clsJob.KeyText("U");
                }
                else if (Gcd.clsJob != null && Gcd.clsJob.Gender == "SELECT")
                {
                    Gcd.clsJob = Gcd.clsJobPrevious;
                    Gcd.clsJob?.Start();
                }
            }
            // Left and middle handled elsewhere in application
            Gcd.redraw();
        }

        // Mouse wheel for dynamic zoom, keeping mouse-point stationary in world coordinates
        public static void MouseWheel()
        {
            bool outside;
            if (Gcd.Drawing.Sheet.Viewport != null)
            {
                var v = Gcd.Drawing.Sheet.Viewport;
                double xr = Gcd.Xreal(Mouse.X);
                double yr = Gcd.Yreal(Mouse.Y);
                outside = xr < v.X0 || xr > v.X1 || yr < v.Y0 || yr > v.Y1;
            }
            else
            {
                outside = true;
            }

            double px = Gcd.Xreal(Mouse.X);
            double py = Gcd.Yreal(Mouse.Y);
            double factor = (1 + 0.075 * Mouse.Delta);

            if (outside)
            {
                Gcd.Drawing.Sheet.ScaleZoom *= factor;
                double dx = Gcd.Xreal(Mouse.X);
                double dy = Gcd.Yreal(Mouse.Y);

                Gcd.Drawing.Sheet.PanX += Gcd.pixels(dx - px);
                Gcd.Drawing.Sheet.PanY += Gcd.pixels(dy - py);

                Gcd.flgNewPosition = true;
            }
            else
            {
                // inside viewport: apply zoom to viewport coordinates
                px -= Gcd.Drawing.Sheet.Viewport.X0;
                py -= Gcd.Drawing.Sheet.Viewport.Y0;

                Gcd.Drawing.Sheet.Viewport.ScaleZoom *= factor;

                double dx = Gcd.Xreal(Mouse.X) - Gcd.Drawing.Sheet.Viewport.X0;
                double dy = Gcd.Yreal(Mouse.Y) - Gcd.Drawing.Sheet.Viewport.Y0;

                Gcd.Drawing.Sheet.Viewport.PanX += Gcd.pixels(dx - px);
                Gcd.Drawing.Sheet.Viewport.PanY += Gcd.pixels(dy - py);
            }

            Gcd.redraw();
        }

        // Finish the tool: cleanup transforms, regenerate lists, reset job to selection
        public static void Finish()
        {
            try
            {
                gl.DeleteLists(Gcd.Drawing.GlListEntitiesSelected, 1);

                glAngle = 0;
                glTranslate[0] = glTranslate[1] = glTranslate[2] = 0.0;
                glScale[0] = glScale[1] = glScale[2] = 1.0;

                clsEntities.GlGenDrawListSelected();
                clsEntities.GlGenDrawListLayers();
                clsEntities.DeSelection();
                clsEntities.CollectVisibleEntities();

                Gcd.Drawing.RequiresSaving = true;

                Gcd.Drawing.iEntity.Clear();
                Gcd.Drawing.iEntity.Insert(new double[] { 0, 0, -1, -1 });

                Gcd.clsJobPrevious = null;
                Gcd.clsJobCallBack = null;
                Gcd.clsJob = cadSelection.Instance;
                cadSelection.Instance.AllowSingleSelection = true;
                Gcd.clsJob.Start();

                DrawingAIds.CleanTexts();
                Gcd.StepsDone = 0;
                Gcd.DrawOriginals = false;
                Active = false;

                Gcd.Redraw();
            }
            catch (Exception)
            {
                // ignore errors during finish
            }
        }

        // Cancel the tool: revert to selection job
        public static void Cancel()
        {
            Gcd.clsJobPrevious = null;
            Gcd.clsJobCallBack = null;
            clsEntities.DeSelection();
            Gcd.clsJob = cadSelection.Instance;
            Gcd.clsJob.Start();
            DrawingAIds.CleanTexts();
            Gcd.redraw();
        }

        // Callback placeholder
        public static void Run()
        {
            // Implement tool-specific run-loop if needed
        }
    }
