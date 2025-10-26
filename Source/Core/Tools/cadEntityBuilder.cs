   // Port of Gambas cadEntityBuilder.class -> C#
    //
    // Notes:
    // - This is a behavioral translation that relies on other converted classes and global objects:
    //   Gcd, clsEntities, DrawingAIds, fMain, cadSelection, clsMouseTracking, puntos, etc.
    // - Gambas Float -> double, Collections -> List<T> / arrays depending on context.
    // - Entity.P is expected to be double[] (x,y pairs). Adjust if your Entity model differs.
    // - Methods such as Gcd.CCC[gender].NewParameter / Finish / BuildGeometry are called as in the original.
    // - Error handling: Gambas Try blocks are translated to try/catch where sensible.
    public static class cadEntityBuilder
    {
        // Identity
        public const string Gender = "BUILDER";

        // Pixel coordinates
        public static int LastMouseDownX = 0;
        public static int LastMouseDownY = 0;
        public static int SelStartX = 0;
        public static int SelStartY = 0;
        public static int SelEndX = 0;
        public static int SelEndY = 0;

        // Real coordinates (meters) - note: Y used as Z in original
        public static double SelStartXr = 0.0;
        public static double SelStartZr = 0.0;
        public static double SelEndXr = 0.0;
        public static double SelEndZr = 0.0;

        // Last picked coordinates in real units
        public static double LastX = 0.0;
        public static double LastY = 0.0;
        public static bool LastPoint = false;

        public static int LastMouseX = -1;
        public static int LastMouseY = -1;

        // Indicators and parameters
        public static int StepsTotal = 0;
        public static int PointsTotal = 0;
        public static string NextParamType = string.Empty;
        public static string[] ParamHelperList = new string[0];
        public static string[] ParamDefault = new string[0];
        public static string prompt = string.Empty;

        // indices of parameter arrays
        public static int iPoints = 0;
        public static int iFloat = 0;
        public static int iString = 0;

        public static int iCounter = 0;

        // current element being built
        public static Entity elem = null;
        public static bool PoiChecking = true;
        public static bool EntityChecking = false;
        public static Entity LastEntity = null;

        public static double[] XYreal = new double[0];

        public const string ContextMenu = "Cancel;_CANCEL;;";

        // Start building a new entity or editing an existing type
        public static bool Start(object ElemToBuild = null, int Mode = -1)
        {
            // Reset and prepare builder
            LastPoint = false;
            clsEntities.DeSelection();

            // regenerate selection draw list
            clsEntities.GlGenDrawListSel(false);

            // Determine element to build
            if (ElemToBuild == null)
            {
                if (LastEntity != null)
                {
                    // create a new entity of the same gender as last
                    elem = Gcd.CCC[LastEntity.Gender].NewEntity();
                }
                else
                {
                    DrawingAIds.ErrorMessage = "Can't create entity";
                    Gcd.clsJob = cadSelection.Instance;
                    Gcd.clsJobPrevious = cadSelection.Instance;
                    return false;
                }
            }
            else
            {
                // If the caller passed a class/descriptor that can create an entity
                // we assume it provides a NewEntity() factory (mirrors Gambas dynamic behavior).
                try
                {
                    // if ElemToBuild is an Entity factory object in your port, call .NewEntity()
                    var factory = ElemToBuild as dynamic;
                    elem = factory.NewEntity();
                }
                catch (Exception)
                {
                    // fallback: if provided an Entity instance, clone it
                    elem = ElemToBuild as Entity;
                    if (elem == null)
                    {
                        DrawingAIds.ErrorMessage = "Can't create entity";
                        Gcd.clsJob = cadSelection.Instance;
                        Gcd.clsJobPrevious = cadSelection.Instance;
                        return false;
                    }
                }
            }

            // reset state
            Gcd.StepsDone = 0;

            try
            {
                elem.Colour = Gcd.CurrentColor();
            }
            catch { }

            try
            {
                elem.LineWidth = Gcd.Drawing.CurrLineWt;
                if (elem.LineWidth == 0) elem.LineWidth = 1;
            }
            catch { }

            try
            {
                elem.PaperSpace = !Gcd.Drawing.Sheet.IsModel;
            }
            catch { }

            iPoints = 0;
            iFloat = 0;
            iString = 0;

            // PointsTotal: number of x,y pairs in elem.P
            try
            {
                PointsTotal = (elem.P != null) ? elem.P.Length / 2 : 0;
            }
            catch
            {
                PointsTotal = 0;
            }

            // StepsTotal is length of ParamType string for this gender
            try
            {
                var paramType = Gcd.CCC[elem.Gender].ParamType ?? string.Empty;
                StepsTotal = paramType.Length;
            }
            catch
            {
                StepsTotal = 0;
            }

            // helper lists and defaults (semicolon separated)
            try
            {
                var helper = Gcd.CCC[elem.Gender].ParamHelper ?? string.Empty;
                ParamHelperList = string.IsnullOrEmpty(helper) ? new string[0] : helper.Split(';');
            }
            catch
            {
                ParamHelperList = new string[0];
            }

            try
            {
                var pdef = Gcd.CCC[elem.Gender].ParamDefault ?? string.Empty;
                ParamDefault = string.IsnullOrEmpty(pdef) ? new string[0] : pdef.Split(';');
            }
            catch
            {
                ParamDefault = new string[0];
            }

            // First expected parameter type
            try
            {
                var paramType = Gcd.CCC[elem.Gender].ParamType ?? string.Empty;
                NextParamType = paramType.Length > Gcd.StepsDone ? paramType[Gcd.StepsDone].ToString().ToUpperInvariant() : "+";
            }
            catch
            {
                NextParamType = "+";
            }

            if (Mode >= 0)
            {
                try
                {
                    elem.iParam[Gcd.CCC[elem.Gender].iiiMode] = Mode;
                }
                catch { }
            }

            if (Gcd.SnapMode != 0)
            {
                try { Gcd.SnapMode = Config.SnapModeSaved; } catch { }
            }

            // set popup menu for sheet
            try
            {
                Gcd.Drawing.Sheet.GlSheet.PopupMenu = elem.Gender;
            }
            catch { }

            fMain.KeysAccumulator = string.Empty;
            try { prompt = Gcd.CCC[elem.Gender].Prompt; } catch { prompt = string.Empty; }

            Gcd.DrawHoveredEntity = false;

            return true;
        }

        // Entry from command-line/text input
        public static void KeyText(string EnteredText)
        {
            if (string.IsnullOrWhiteSpace(EnteredText)) return;

            double Xt = 0, yt = 0;
            string sText = null;
            string ErrTxt = string.Empty;
            bool Relative = false;
            bool bResult = false;

            var upper = EnteredText.ToUpperInvariant();

            // handle quick commands / snap modes
            switch (upper)
            {
                case "_CANCEL":
                    Cancel();
                    return;
                case "_MIDPOINT":
                case "_MID":
                    Gcd.SnapMode = Gcd.poiMIdPoint;
                    break;
                case "_ENDPOINT":
                case "_END":
                    Gcd.SnapMode = Gcd.poiEndPoint;
                    break;
                case "_PERPENDICULAR":
                case "_PER":
                    Gcd.SnapMode = Gcd.poiPerpendicular;
                    break;
                case "_NEAREST":
                case "_NEA":
                    Gcd.SnapMode = Gcd.poiNearest;
                    break;
                case "_CENTER":
                case "_CEN":
                    Gcd.SnapMode = Gcd.poiCenter;
                    break;
                case "_INTERSECTION":
                case "_INT":
                    Gcd.SnapMode = Gcd.poiIntersection;
                    break;
                case "_BASEPOINT":
                case "_BAS":
                    Gcd.SnapMode = Gcd.poiBasePoint;
                    break;
                case "_TANGENT":
                case "_TAN":
                    Gcd.SnapMode = Gcd.poiTangent;
                    break;
                case "_QUADRANT":
                case "_QUA":
                    Gcd.SnapMode = Gcd.poiQuadrant;
                    break;
            }

            // Determine behavior depending on expected parameter
            if (NextParamType == "P" || NextParamType == "+" || NextParamType == "M")
            {
                ErrTxt = ", expected a valid point like 12.4,9.5 or @12.34,10.5";

                var trimmed = EnteredText.Trim();
                if (trimmed.Length == 0) return;
                var first = trimmed.Substring(0, 1);

                if ("@0123456789.".IndexOf(first) >= 0)
                {
                    if (trimmed.Contains("@"))
                    {
                        Relative = true;
                        trimmed = trimmed.Replace("@", "");
                    }

                    var parts = trimmed.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 2 &&
                        double.TryParse(parts[0].Trim(), out Xt) &&
                        double.TryParse(parts[1].Trim(), out yt))
                    {
                        if (Relative && elem?.P != null && elem.P.Length > 2)
                        {
                            Xt += LastX;
                            yt += LastY;
                        }

                        LastX = Xt;
                        LastY = yt;

                        try
                        {
                            if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "point", Xt, yt }, true))
                            {
                                // save last point to drawing
                                Gcd.Drawing.LastPoint.Clear();
                                Gcd.Drawing.LastPoint.Insert(new double[] { Xt, yt });
                                AdvanceStep();
                            }
                        }
                        catch
                        {
                            // ignore errors from entity-specific NewParameter
                        }
                    }
                    else
                    {
                        DrawingAIds.ErrorMessage = "Bad input" + ErrTxt;
                    }
                }
                else
                {
                    // text input for this parameter
                    try
                    {
                        if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "text", EnteredText }, true))
                            AdvanceStep();
                    }
                    catch { }
                }
            }
            else if (NextParamType == "T")
            {
                ErrTxt = ", expected text, not a point";
                if (string.IsnullOrEmpty(EnteredText))
                    sText = ParamDefault.Length > Gcd.StepsDone ? ParamDefault[Gcd.StepsDone] : string.Empty;
                else sText = EnteredText;

                try
                {
                    if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "text", sText }, true))
                        AdvanceStep();
                }
                catch { }
            }
            else if ("FARL".IndexOf(NextParamType) >= 0) // "F", "A", "L", "R"
            {
                ErrTxt = "enter a valid numeric value";
                if (string.IsnullOrEmpty(EnteredText) || EnteredText == "U")
                {
                    try
                    {
                        var def = ParamDefault.Length > Gcd.StepsDone ? ParamDefault[Gcd.StepsDone] : "0";
                        bResult = Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", Utils.ParseDoubleSafe(def) }, true);
                    }
                    catch { bResult = false; }
                }
                else
                {
                    try
                    {
                        bResult = Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", Convert.ToDouble(EnteredText) }, true);
                    }
                    catch { bResult = false; }
                }

                if (bResult) AdvanceStep();
            }
            else if (NextParamType == "C")
            {
                if (Dialog.SelectColor())
                {
                    try
                    {
                        elem.fParam[iFloat] = (double)Dialog.Color;
                        iFloat++;
                        AdvanceStep();
                    }
                    catch { }
                }
            }
        }

        public static void KeyUp(int iCode)
        {
            if (iCode == Key.ControlKey) Gcd.Orthogonal = false;
        }

        public static void AdvanceStep()
        {
            Gcd.StepsDone++;
            if (Gcd.StepsDone == StepsTotal)
            {
                Finish();
            }
            else
            {
                // prepare next parameter
                try
                {
                    var paramType = Gcd.CCC[elem.Gender].ParamType ?? string.Empty;
                    NextParamType = paramType.Length > Gcd.StepsDone ? paramType[Gcd.StepsDone].ToString().ToUpperInvariant() : "+";
                }
                catch { NextParamType = "+"; }

                if (NextParamType == "+")
                {
                    StepsTotal++;
                    NextParamType = "P";
                }

                if (ParamDefault.Length > Gcd.StepsDone)
                {
                    ParamDefault[Gcd.StepsDone] = ParamDefault[Gcd.StepsDone].Trim();
                }

                // trigger preview
                MouseMove();
            }

            fMain.KeysAccumulator = string.Empty;
            try { prompt = Gcd.CCC[elem.Gender].Prompt; } catch { }
            Gcd.redraw();
        }

        // Mouse movement: used for preview and snap logic
        public static void MouseMove()
        {
            double X = 0, Y = 0, f = 0;
            int MouseTry = -1000000;

            try { MouseTry = Mouse.X; } catch { MouseTry = -1000000; }

            if (MouseTry >= 0)
            {
                LastMouseX = Mouse.X;
                LastMouseY = Mouse.Y;
            }

            X = Gcd.Xreal(LastMouseX);
            Y = Gcd.Yreal(LastMouseY);

            // POI checking
            if (!Gcd.flgSearchingPOI)
            {
                Gcd.Drawing.iEntity = clsMouseTracking.CheckBestPOI(X, Y);
            }

            X = Gcd.Near(X);
            Y = Gcd.Near(Y);

            // if we are not expecting a parameter that takes mouse input, return
            if ("LAPRM".IndexOf(NextParamType) < 0) return;

            // special case for SPLINE
            if (elem != null && elem.Gender == "SPLINE" && (iPoints != ((elem.P != null) ? elem.P.Length / 2 - 1 : 0)))
            {
                iPoints++;
            }

            if (NextParamType == "A")
            {
                f = puntos.Ang(Gcd.Xreal(LastMouseX) - LastX, Gcd.Yreal(LastMouseY) - LastY);
                f *= 180.0 / Math.PI;
                Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", f });
                Gcd.redraw();
                return;
            }

            if (NextParamType == "R")
            {
                Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "point", X, Y });
                Gcd.redraw();
                return;
            }

            if (NextParamType == "L")
            {
                f = puntos.distancia(Gcd.Xreal(LastMouseX), Gcd.Yreal(LastMouseY), LastX, LastY);
                Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", f });
                Gcd.redraw();
                return;
            }

            // If snap engaged, adjust to POI coordinates
            if (Gcd.Drawing.iEntity != null && Gcd.Drawing.iEntity.Length >= 3 && Gcd.Drawing.iEntity[2] > 0)
            {
                X = Gcd.Drawing.iEntity[0];
                Y = Gcd.Drawing.iEntity[1];
            }
            else
            {
                if (Gcd.Orthogonal && LastPoint)
                {
                    if (Math.Abs(X - LastX) > Math.Abs(Y - LastY))
                        Y = LastY;
                    else
                        X = LastX;
                }
            }

            Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "point", X, Y });
            Gcd.redraw();
        }

        public static void KeyPress(int iCode, string sKey)
        {
            // left intentionally blank; entity-specific behavior may override
        }

        // MouseUp: finalize point input or numeric parameter
        public static void MouseUp()
        {
            if (Mouse.Right)
            {
                Gcd.clsJob.KeyText("U");
                return;
            }

            DrawingAIds.ErrorMessage = string.Empty;
            if ("LAPRM".IndexOf(NextParamType) < 0) return;

            double X = 0, Y = 0, f = 0;

            if (NextParamType == "A")
            {
                f = puntos.Ang(Gcd.Xreal(Mouse.X) - LastX, Gcd.Yreal(Mouse.Y) - LastY) * 180.0 / Math.PI;
                if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", f }, true)) AdvanceStep();
                Gcd.redraw();
                return;
            }

            if (NextParamType == "R")
            {
                if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "point", Gcd.Xreal(Mouse.X), Gcd.Yreal(Mouse.Y) }, true))
                {
                    Gcd.Drawing.LastPoint.Clear();
                    Gcd.Drawing.LastPoint.Insert(new double[] { Gcd.Xreal(Mouse.X), Gcd.Yreal(Mouse.Y) });
                    AdvanceStep();
                }
                Gcd.redraw();
                return;
            }

            if (NextParamType == "L")
            {
                f = puntos.distancia(Gcd.Xreal(Mouse.X), Gcd.Yreal(Mouse.Y), LastX, LastY);
                if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "float", f }, true)) AdvanceStep();
                Gcd.redraw();
                return;
            }

            // expecting a point
            X = Gcd.Near(Gcd.Xreal(Mouse.X));
            Y = Gcd.Near(Gcd.Yreal(Mouse.Y));

            if (Gcd.Drawing.iEntity != null && Gcd.Drawing.iEntity.Length >= 3 && Gcd.Drawing.iEntity[2] > 0)
            {
                X = Gcd.Drawing.iEntity[0];
                Y = Gcd.Drawing.iEntity[1];
            }
            else
            {
                if (Gcd.Orthogonal && LastPoint)
                {
                    if (Math.Abs(X - LastX) > Math.Abs(Y - LastY))
                        Y = LastY;
                    else
                        X = LastX;
                }
            }

            LastX = X;
            LastY = Y;
            LastPoint = true;

            if (Gcd.CCC[elem.Gender].NewParameter(elem, new object[] { "point", X, Y }, true))
            {
                Gcd.Drawing.LastPoint.Clear();
                Gcd.Drawing.LastPoint.Insert(new double[] { X, Y });
                AdvanceStep();
            }

            Gcd.redraw();
        }

        public static void MouseDown()
        {
            // No default behavior here - left for higher-level handling
        }

        public static void MouseWheel()
        {
            ToolsBase.MouseWheel();
        }

        public static void DblClick()
        {
            // not used in builder by default
        }

        public static void Draw()
        {
            try
            {
                // ensure geometry built for preview
                clsEntities.BuildGeometry(elem);
                clsEntities.Draw(elem);
                DrawingAids.DrawSnapText();
                iCounter++;
            }
            catch { }
        }

        // Called when finishing creating the entity
        public static bool Finish()
        {
            if (elem == null) return false;

            // assign id if missing
            if (string.IsnullOrEmpty(elem.Id)) elem.Id = Gcd.NewId();

            try
            {
                if (Gcd.Drawing.Sheet.IsModel)
                {
                    Gcd.Drawing.Sheet.Entities.Add(elem);
                    Gcd.Drawing.Sheet.EntitiesVisibles.Add(elem);
                }
                else
                {
                    Gcd.Drawing.Sheet.Entities.Add(elem);
                }
            }
            catch { /* adapt to your collection API */ }

            elem.Container = Gcd.Drawing.Sheet.Block;

            try
            {
                if (elem.pBlock != null && elem.pBlock.name == "*D")
                {
                    elem.pBlock.name += elem.Id;
                    elem.pBlock.Id = Gcd.NewId();
                    Gcd.Drawing.Blocks.Add(elem.pBlock);
                }
            }
            catch { }

            Gcd.CCC[elem.Gender].Finish(elem);
            Gcd.CCC[elem.Gender].BuildGeometry(elem);

            try
            {
                Gcd.CCC[elem.Gender].LastMode = elem.iParam[Gcd.CCC[elem.Gender].iiiMode];
            }
            catch { }

            LastEntity = elem;
            if (elem.Gender != null && elem.Gender.StartsWith("DIM")) Gcd.Drawing.LastDimension = elem;

            // UNDO
            Gcd.Drawing.uUndo.OpenUndoStage("Draw a " + elem.Gender, Undo.TypeCreate);
            Gcd.Drawing.uUndo.AddUndoItem(elem);
            Gcd.Drawing.uUndo.CloseUndoStage();

            Gcd.Drawing.RequiresSaving = true;
            Gcd.DrawHoveredEntity = true;
            Gcd.Drawing.iEntity.Clear();
            Gcd.clsJobPrevious = cadEntityBuilder; // store current builder as previous job

            // select created entity
            Gcd.Drawing.Sheet.EntitiesSelected.Clear();
            Gcd.Drawing.Sheet.EntitiesSelected.Add(elem);
            fMain.fp.FillProperties(Gcd.Drawing.Sheet.EntitiesSelected);

            Gcd.Drawing.Sheet.Grips.Clear();
            Gcd.CCC[elem.Gender].GenerateGrips(elem);

            Gcd.clsJob = cadSelection.Instance;
            Gcd.clsJob.Start();

            Gcd.Drawing.Sheet.GlSheet.PopupMenu = string.Empty;
            cadSelection.Instance.PoiChecking = true;
            DrawingAIds.CleanTexts();

            // Generate GL lists
            clsEntities.GlGenDrawList(elem);
            clsEntities.GlGenDrawListLAyers(elem.pLayer);

            Gcd.redraw();

            return true;
        }

        public static void Cancel()
        {
            elem = null;
            Gcd.clsJobPrevious = cadEntityBuilder;
            Gcd.clsJob = cadSelection.Instance;
            DrawingAIds.CleanTexts();
            Gcd.redraw();
            try { Gcd.Drawing.Sheet.GlSheet.PopupMenu = string.Empty; } catch { }
        }

        public static void KeyDown(int iCode)
        {
            if (iCode == Key.ControlKey) Gcd.Orthogonal = true;
        }
    }

    // Utility helpers (small placeholder) - replace with your real implementations
    internal static class Utils
    {
        public static double ParseDoubleSafe(string s)
        {
            if (double.TryParse(s, out var v)) return v;
            return 0.0;
        }
    }