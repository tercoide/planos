namespace Gaucho;

class cadSelection
{
 // Gambas class file

 // GambasCAD
 // A simple CAD made in Gambas
 //
 // Copyright (C) Ing Martin P Cristia
 //
 // This program is free software; you can redistribute it and/or modify
 // it under the terms of the GNU General Public License as published by
 // the Free Software Foundation; either version 3 of the License, or
 // (at your option) any later version.
 //
 // This program is distributed in the hope that it will be useful,
 // but WITHOUT ANY WARRANTY; without even the implied warranty of
 // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 // GNU General Public License for more details.
 //
 // You should have received a copy of the GNU General Public License
 // along with this program; if not, write to the Free Software
 // Foundation, Inc., 51 Franklin St, Fifth Floor,
 // Boston, MA  02110-1301  USA

 // Tool maintained by Terco

 // this is the Main Job, either we are doing this or other job
 // Create Static
const string Gender = "SELECT";

private string[] EntityType ;         

private Grip GripPoint ;         
private Grip GripHovered ;         
private bool GripCopying = False;
 //Public GripCopying As Boolean = False

private double dX ;         
private double dY ;         
 //Public ToolActive As Boolean = False
private bool PanActive = False;
private bool GripActive = False;
private bool RectActive = False;

 // nuevo, para integrar el resto de las tools con esta, que es la principal
public bool AllowSingleSelection = True;
public bool AllowRectSelection = True;
public bool AllowPolySelection = True;
public bool AllowGripEdit = True;
public bool AllowTextInput = True;

public bool ReturnOnFirstSelection = True;

 // mas nuevo
public int SelectType = 1;
const int SelectTypeSingleAndRect = 1;
const int SelectTypeSingle = 0;
const int SelectTypeRect = 2;
const int SelectTypePoly = 3;
const int SelectTypePoint = 4;

public int SelectMode = 0;
const int SelectModeNew = 0;
const int SelectModeAdd = 1;
const int SelectModeRem = 2;

public bool SelectCrossing = false;                 // las entidades puedn estar parcialmente dentro del contorno

public double[] SelectionPoly ;         

 // Funcionamiento:

 // ToolActive: significa que otra Tool (Copy, Move, etc) le pidio a esta que modifique la seleccion de entidas

 // A MouseDown
 // A.1 Izquierdo
 // A.1.1 No estoy sobre ninguna entidad -> Inicio una seleccion rectangular ActionActive = ActionRectActive
 // A.1.2 Estoy sobre una entidad
 // A.1.2.1 No esta seleccionada -> seleccionar
 // A.1.2.2 Esta seleccionada previamente
 // A.1.2.2.1 Estoy sobre un grip -> Inicio edicion por grip ActionActive = ActionGripActive (solo si ToolActive=False)
 // A.1.2.2.2 No estoy sobre un grip -> deseleccionar
 // A.2 Derecho
 // A.2.1 ToolActive = False?
 // A.2.1.1 No tengo ninguna seleccion previa-> Repito el ultimo comando, independientemente de si estoy sobre una entidad o no
 // A.2.1.2 Tengo una seleccion previa -> Menu: Cortar, Copiar, Agrupar, Desagrupar, Llevar al layer actual, etc
 // A.2.2 ToolActive = True? -> Finalizo la seleccion y vuelvo a la Tool
 // A.3 Medio -> Inicio el Paneo ActionActive = ActionPanActive

 // B MouseMove
 // B.1 ActionActive = ActionRectActive: actualizo coordenadas del punto final del Rect
 // B.2 ActionActive = ActionGripActive: modifico la entidad con la nueva posicion del punto
 // B.3 ActionActive = ActionPanActive: mando la coordenada a cadPan
 // B.4 ActionActive = 0: nada

 // C MouseUp
 // C.1 Izquierdo
 // C.1.1 ActionActive = ActionRectActive -> Finalizo la seleccion por recuadro
 // C.1.2 ActionActive = ActionGripActive -> Finalizo la edicion por grips
 // C.1.3 ActionActive = ActionPanActive -> nada
 // C.1.4 ActionActive = 0
 // C.2 Derecho -> nada
 // C.3 Medio -> ActionActive = ActionPanActive -> finalizo el paneo

public static bool Start(Variant ElemToBuild, int _Mode= 2)
    {

     // Modes:
     //       0 = Move, all points in the element must be selected, or click on it.
     //       1 = Stretch, selection may be partial, each element is called to see if the support stretching

    this.Mode = _Mode;

    this.Prompt = ("Selected") + " " + Str$(gcd.Drawing.Sheet.EntitiesSelected.Count) + " " + ("elements") + " " + ("New/Add(Ctrl)/Remove(Shft)/Previous selection");

     // If Me.Mode = ModeNewSelection Then
     //     fMain.PopupMenu = ""                    // no hay menu contextual
     //     Me.Prompt = "New selection"
     //     If gcd.clsJobPrevious.gender = cadEntityBuilder.gender Then
     //         If cadEntityBuilder.LastEntity Then
     //             Me.Prompt &= " or <rigth click> to repeat " & cadEntityBuilder.LastEntity.gender
     //         End If
     //     End If
     // Else If Me.Mode = ModeAddEntities Then
     //     Me.Prompt = "Add entities to selection"
     // Else If Me.Mode = ModeRemoveEntities Then
     //     Me.Prompt = "Remove entities from selection"
     //
     // Else
     //     Utils.MenuMaker(fMain, "mToolsBase", Me.ContextMenu)
     //     //gcd.Drawing.Sheet.GlSheet.PopupMenu = "mToolsBase"  // TODO: ver si damos soporte a menus
     // End If
    gcd.Drawing.iEntity.Clear;
    this.PoiChecking = True;
    gcd.DrawHoveredEntity = True;
    GripPoint = null;

}

 // Public Sub DblClick()
 //
 //     Dim k As Single
 //     Dim e As Entity
 //     Dim te As Entity
 //
 //
 //
 //     Me.EntityForEdit = clsMouseTracking.CheckAboveEntity(gcd.Xreal(Mouse.x), gcd.Yreal(Mouse.y))
 //     Return
 //
 //     If Not gcd.flgSearchingPOI Then
 //         gcd.Drawing.iEntity = clsMouseTracking.CheckBestPOI(gcd.Xreal(mouse.x), gcd.Yreal(mouse.Y))
 //     Else    // estoy buscando, pero me movi, asi que me desengancho del POI anterior
 //
 //         gcd.Drawing.iEntity[0] = gcd.Xreal(mouse.x)
 //         gcd.Drawing.iEntity[1] = gcd.Yreal(mouse.y)
 //         gcd.Drawing.iEntity[2] = -1                 // POI type
 //
 //     End If
 //
 //     If gcd.Drawing.iEntity[3] >= 0 Then
 //
 //         //Stop
 //         // I comment the abobe line because its stop my tool also. What is the idea whit stop?
 //         //  aca podes lanzar tu editor de texto u otras propiedades
 //         k = gcd.Drawing.iEntity[3]
 //         e = gcd.Drawing.Entities[k]
 //
 //         // Select e.Gender
 //         //   Case "Text"
 //         //     If EditingText = False Then
 //         //       // Copying the entity for undo
 //         //       te = clsEntities.ClonEntity(e)
 //         //       te.Handle = e.Handle
 //         //       ftx = New FText([pnlDrawing.ScreenX + 7, pnlDrawing.ScreenY + pnlDrawing.H - 7], e)
 //         //       ftx.Run()
 //         //       While EditingText = False
 //         //         Wait 0.1
 //         //       Wend
 //         //       gcd.regen
 //         //       EditingText = False
 //         //     Endif
 //         // End Select
 //     Endif
 //
 // End

public static void MouseDown()
    {


    int i ;         
    Entity e ;         

     //Return
    if ( RectActive ) return;

     //GripPoint = -1
    this.SelStartX = mouse.X;
    this.SelStartY = mouse.Y;
    this.SelStartXr = gcd.Xreal(Me.SelStartX);
    this.SelStartYr = gcd.Yreal(Me.SelStartY);

    dX = this.SelStartXr;
    dY = this.SelStartYr;

    this.SelEndX = this.SelStartX;
    this.SelEndy = this.SelStartY;

    this.SelEndXr = this.SelStartXr;
    this.SelEndyr = this.SelStartYr;

    this.PoiChecking = False;
    if ( Mouse.Right ) return;
    if ( gcd.clsJobCallBack ) return;
    if ( RectActive ) return;

     // A.1 Izquierdo
    if ( Mouse.Left )
    {

            // veo si esta en un grip(solo si ToolActive = False)

            // chequeo si estoy sobre un grip
            if (AllowGripEdit) 
                {
                GripPoint = FindGrip(Me.SelStartXr, this.SelStartYr);
                }
            else
                {
                GripPoint = null;
            }
        if ( GripPoint )
        {

             // creo una entidad al vuelo, clonada de la engripada
            this.OriginalEntityForEdit = GripPoint.AsociatedEntity;
            this.EntityForEdit = clsEntities.ClonEntity(GripPoint.AsociatedEntity, False);
            GripPoint.AsociatedEntity = this.EntityForEdit;
            gcd.Drawing.Sheet.SkipSearch = GripPoint.AsociatedEntity;

             // rectifico la posicion del punto
            this.SelStartXr = GripPoint.X;
            this.SelStartYr = GripPoint.Y;

            return;
        } //Or Me.ModeRectSelection Then
        else if ( gcd.Drawing.HoveredEntity )
        {

             // // A.1.2 Estoy sobre una entidad
             //
             // If Not gcd.Drawing.Sheet.EntitiesSelected.ContainsKey(gcd.Drawing.HoveredEntity.Id) Then
             //     // A.1.2.1 No esta seleccionada -> seleccionar
             //     gcd.Drawing.Sheet.EntitiesSelected.add(gcd.Drawing.HoveredEntity, gcd.Drawing.HoveredEntity.Id)
             //     gcd.Drawing.Sheet.Grips.Clear
             //     gcd.CCC[gcd.Drawing.HoveredEntity.Gender].generategrips(gcd.Drawing.HoveredEntity)
             //     clsEntities.glGenDrawListSel
             //     fProps.FillProperties(gcd.Drawing.Sheet.EntitiesSelected)
             //     gcd.Redraw
             // Else
             //     // A.1.2.2 Esta seleccionada previamente
             //     // A.1.2.2.2 No estoy sobre un grip -> deseleccionar
             //     gcd.Drawing.Sheet.EntitiesSelected.Remove(gcd.Drawing.HoveredEntity.Id)
             //     fprops.FillProperties(gcd.Drawing.Sheet.EntitiesSelected)
             //     clsEntities.glGenDrawListSel
             //     gcd.Redraw
             // End If

        }

        return; // este return es para evitar clicks simultaneos
    }

    if ( Mouse.MIddle )
    {
         // A.3 Medio -> Inicio el Paneo ActionActive = ActionPanActive

        return; // este return es para evitar clicks simultaneos
    }

     //         If gcd.Drawing.Sheet.Viewport Then
     //
     //             // si el click esta fuera del viewport, lo desestimo
     //             If Me.SelStartXr < gcd.Drawing.Sheet.Viewport.X0 Or Me.SelStartXr > gcd.Drawing.Sheet.Viewport.X1 Or Me.SelStartYr < gcd.Drawing.Sheet.Viewport.Y0 Or Me.SelStartYr > gcd.Drawing.Sheet.Viewport.Y1 Then
     //                 gcd.Drawing.Sheet.Viewport = null // Desactivo el viewport
     //             Else
     //
     //                 gcd.Drawing.GLAreaInUse.Mouse = Mouse.SizeAll
     //                 Me.active = True
     //             End If
     //         Else
     //
     //
     //
     //             If Me.Mode = Me.ModeRectSelection Then
     //                 Me.Active = True
     //
     //             Else
     //
     //
     //                 If gcd.Drawing.HoveredEntity.selected Then
     //
     //
     //                 Else
     //                     clsEntities.SelectElem(gcd.Drawing.HoveredEntity)                 //   -> la selecciono
     //
     //                 Endif
     //                 Me.Active = False
     //
     //                 clsEntities.GLGenDrawListSel(0)
     //
     //
     //             End If
     //         End If
     //     End If
     // End If

}

public static void MouseUp()
    {


    string s ;         
    string tipo ;         
    double t = Timer;
    Entity e ;         
    Dictionary<string, Entity> cSel =[] ;         

    gcd.Drawing.iEntity.Clear;
    gcd.Drawing.Sheet.SkipSearch = null;
    gcd.Drawing.LastPoint.Clear;
     // If gcd.Drawing.Sheet.Viewport Then
     //     //
     //     gcd.Drawing.GLAreaInUse.Mouse = Mouse.Cross
     //     gcd.flgNewPosition = True
     //     Me.active = False
     //     Return
     // End If

    tipo = "new";
    if ( Mouse.Shift || Me.SelectMode == Me.SelectModeRem ) tipo = "rem"; // estos elementos de la seleccion anterior
    if ( Mouse.Control || Me.SelectMode == Me.SelectModeAdd ) tipo = "add"; // elementos a la seleccion anterior
     //

    if ( Mouse.Left )
    {
         // C.1 Izquierdo

         // determino que hacer con la seleccion
        if ( gcd.clsJobCallBack && Me.ReturnOnFirstSelection && Me.SelectType == Me.SelectTypePoint )
        {
            gcd.clsJob = gcd.clsJobCallBack;
            gcd.clsJobCallBack = null;
            gcd.clsJob.run;
            return;
        }

        this.PoiChecking = True;
        if ( RectActive )
        {

            if ( Me.SelectType == Me.SelectTypePoly )
            {
                this.SelectionPoly.Add(gcd.Xreal(Mouse.x));
                this.SelectionPoly.Add(gcd.Yreal(Mouse.y));
                return; // porque esto requiere un RigthClick para terminar
            }
             // C.1.1 ActionActive = ActionRectActive -> Finalizo la seleccion por recuadro
            RectActive = False;
            gcd.flgSearchingAllowed = True;
                // corrijo para start<end  <- DEPRE
                // If Me.SelStartX > Me.SelEndX Then
                //     crossing = True <- DEPRE
                //     Swap Me.SelStartX, Me.SelEndX
                // Else
                //     crossing = False  <- DEPRE
                // End If

            if (Me.SelStartX > Me.SelEndX) Utils.Swap(ref this.SelStartX, ref this.SelEndX);
            if ( Me.SelStartY < Me.SelEndy ) Utils.Swap(ref this.SelStartY, ref this.SelEndy); // this is FLIPPED

            if ( Me.SelStartXr > Me.SelEndXr ) Utils.Swap(ref this.SelStartXr, ref this.SelEndXr);
            if ( Me.SelStartYr > Me.SelEndyr ) Utils.Swap(ref this.SelStartYr, ref this.SelEndyr);

             // veo si el rectangulo es suficientemente grande como para representar una seleccion por rectangulo
            if ( (Me.SelEndX - Me.SelStartX + (-Me.SelEndy + Me.SelStartY)) > 10 )
            {

                cSel = clsEntities.SelectionSquare(Me.SelStartXr, this.SelStartYr, this.SelEndXr, this.SelEndyr, this.SelectCrossing);

                 // Else // TODO: ver si tengo que desseleccionar
                 //     clsEntities.DeSelection()

            }
            gcd.Drawing.Sheet.EntitiesSelected = clsEntities.SelectionCombine(gcd.Drawing.Sheet.EntitiesSelected, cSel, tipo);

             // determino que hacer con la seleccion
            if ( gcd.clsJobCallBack && Me.ReturnOnFirstSelection )
            {
                gcd.clsJob = gcd.clsJobCallBack;
                gcd.clsJobCallBack = null;
                gcd.clsJob.run;
                return;
            }
             // e = gcd.Drawing.Sheet.EntitiesSelected[gcd.Drawing.Sheet.EntitiesSelected.Last]
             // If e Then
             //     //gcd.Drawing.Sheet.Grips.Clear
             //     gcd.CCC[e.Gender].generategrips(e)
             // Endif
            if ( gcd.Drawing.Sheet.EntitiesSelected.Count > 0 )
            {
                fMain.fp.FillProperties(gcd.Drawing.Sheet.EntitiesSelected);
            }
            else
            {
                fMain.fp.FillGeneral(gcd.Drawing.Sheet);
            }
            clsEntities.GLGenDrawListSel();

             //Try s = gcd.clsJobCallBack.gender
            this.Prompt = ("Selected") + " " + gcd.Drawing.Sheet.EntitiesSelected.Count.ToString() + " " + ("elements") + " " + ("New/Add(Ctrl)/Remove(Shft)/Previous selection");

             // If gcd.clsJobCallBack Then
             //     Try gcd.clsJobCallBack.run()
             //     Return
             // Else
             //
             //     // vamos a darle mas funcionalidad
             //     If gcd.Drawing.Sheet.EntitiesSelected.Count = 0 Then
             //
             //     Else If gcd.Drawing.Sheet.EntitiesSelected.Count = 1 Then
             //
             //         //gcd.Drawing.Sheet.GlSheet.PopupMenu = "mColors"
             //
             //     Else // tenemos varias entidades
             //
             //         //gcd.Drawing.Sheet.GlSheet.PopupMenu = "mEntities"
             //
             //     End If
             // Endif

        }
        else if ( RectActive == False && ! GripPoint )
        {

             // A.1.2 Estoy sobre una entidad
            if ( gcd.Drawing.HoveredEntity && AllowSingleSelection )
            {
                if ( tipo == "new" ) gcd.Drawing.Sheet.EntitiesSelected.Clear;
                if ( AllowSingleSelection )
                {

                     // A.1.2.1 No esta seleccionada -> seleccionar
                    if ( ! gcd.Drawing.Sheet.EntitiesSelected.ContainsKey(gcd.Drawing.HoveredEntity.Id) )
                    {
                         // excepto que estos removiendo
                        if ( tipo != "rem" )
                        {
                            gcd.Drawing.Sheet.EntitiesSelected.add(gcd.Drawing.HoveredEntity, gcd.Drawing.HoveredEntity.Id);
                        }
                    } // esta en la seleccion
                    else
                    {
                        if ( tipo == "rem" )
                        {
                            gcd.Drawing.Sheet.EntitiesSelected.Remove(gcd.Drawing.HoveredEntity.Id);
                        }
                    }
                    if ( gcd.clsJobCallBack && Me.ReturnOnFirstSelection )
                    {
                        gcd.clsJob = gcd.clsJobCallBack;
                        gcd.clsJobCallBack = null;
                        gcd.clsJob.run;
                        return;
                    }
                }
                if ( AllowGripEdit )
                {

                    gcd.Drawing.Sheet.Grips.Clear;
                    clsEntities.GenGrips(gcd.Drawing.HoveredEntity);

                     // Else  // TODO: ver que pasa con esto

                     //     // A.1.2.2 Esta seleccionada previamente
                     //     // A.1.2.2.2 No estoy sobre un grip -> deseleccionar
                     //     gcd.Drawing.Sheet.EntitiesSelected.Remove(gcd.Drawing.HoveredEntity.Id)
                     //     fprops.FillProperties(gcd.Drawing.Sheet.EntitiesSelected)
                     //     clsEntities.glGenDrawListSel
                     //     gcd.Redraw
                }
                fMain.fp.FillProperties(gcd.Drawing.Sheet.EntitiesSelected);
                clsEntities.GLGenDrawListSel();
                gcd.Redraw;
                this.Prompt = ("Selected") + " " + (gcd.Drawing.Sheet.EntitiesSelected.Count.ToString()) + " " + ("elements");

            } // inicio la seleccion por recuadro
            else
            {

                if ( Me.SelectType > 0 ) RectActive = True;
                gcd.flgSearchingAllowed = False;
                this.SelStartX = mouse.x;
                this.SelStartY = mouse.Y;
                 // Paso a coordenadas reales
                this.selStartXr = gcd.Xreal(Me.SelStartX);
                this.selStartYr = gcd.Yreal(Me.SelStartY);
                if ( Me.SelectType == Me.SelectTypePoly )
                {
                    this.SelectionPoly.Add(Me.SelStartXr);
                    this.SelectionPoly.Add(Me.SelStartYr);
                }
                gcd.Drawing.Sheet.Grips.Clear;

            }

        }
        else if ( GripPoint )
        {

             // C.1.2 ActionActive = ActionGripActive -> Finalizo la edicion por grips
             // guardo todo

            GripEdit;
            gcd.Drawing.Sheet.Grips.Clear;
            if ( ! GripCopying )
            {
                this.EntityForEdit.id = this.OriginalEntityForEdit.id;
                s = GripPoint.ToolTip + (" in ") + GripPoint.AsociatedEntity.Gender;
                gcd.Drawing.uUndo.OpenUndoStage(s, Undo.TypeModify);
                gcd.Drawing.uUndo.AddUndoItem(Me.OriginalEntityForEdit);

                gcd.Drawing.Sheet.Entities.Remove(Me.OriginalEntityForEdit.id);
                gcd.Drawing.Sheet.EntitiesVisibles.Remove(Me.OriginalEntityForEdit.id);
                gcd.Drawing.Sheet.EntitiesSelected.Remove(Me.OriginalEntityForEdit.id);
            }
            else
            {
                this.EntityForEdit.id = gcd.NewId();
                gcd.Drawing.uUndo.OpenUndoStage("Grip copy ", Undo.TypeCreate);
                gcd.Drawing.uUndo.AddUndoItem(Me.EntityForEdit);

            }

            gcd.Drawing.uUndo.CloseUndoStage();

             //gcd.CCC[Me.EntityForEdit.Gender].finish(Me.EntityForEdit)
            gcd.CCC[Me.EntityForEdit.Gender].generategrips(Me.EntityForEdit);
            gcd.Drawing.Sheet.entities.Add(Me.EntityForEdit, this.EntityForEdit.id);
            gcd.Drawing.Sheet.EntitiesVisibles.Add(Me.EntityForEdit, this.EntityForEdit.id);
            gcd.Drawing.Sheet.EntitiesSelected.Add(Me.EntityForEdit, this.EntityForEdit.id);

            clsEntities.glGenDrawList(Me.EntityForEdit);
            clsEntities.glGenDrawListSel();
            clsEntities.glGenDrawListLAyers(Me.EntityForEdit.pLayer);
            this.EntityForEdit = null;
            this.OriginalEntityForEdit = null;
            GripPoint = null;
             //GripCopying = False

        }

        if ( Me.Mode == PanActive )
        {
             // C.1.3 ActionActive = ActionPanActive -> nada
        }

        if ( Me.Mode == False )
        {
             // C.1.4.1 Estoy sobre una entidad

        }
         // C.1.4 ActionActive = 0

        return; // este return es para evitar clicks simultaneos
    }

     // A.2 Derecho
    if ( Mouse.Right )
    {

        if ( RectActive && Me.SelectType == Me.SelectTypePoly )
        {

             // C.1.1 Finalizo la seleccion por POLY
            RectActive = False;
            this.SelectionPoly.Add(gcd.Xreal(Mouse.x));
            this.SelectionPoly.Add(gcd.Yreal(Mouse.y));

            cSel = clsEntities.SelectionPoly(Me.SelectionPoly, this.SelectCrossing);
            this.SelectionPoly.Clear;
            gcd.Drawing.Sheet.EntitiesSelected = clsEntities.SelectionCombine(gcd.Drawing.Sheet.EntitiesSelected, cSel, tipo);

             // determino que hacer con la seleccion
            if ( gcd.clsJobCallBack && Me.ReturnOnFirstSelection )
            {
                gcd.clsJob = gcd.clsJobCallBack;
                gcd.clsJobCallBack = null;
                gcd.clsJob.run;
                return;
            }

            e = gcd.Drawing.Sheet.EntitiesSelected[gcd.Drawing.Sheet.EntitiesSelected.Last];
            if ( e )
            {
                 //gcd.Drawing.Sheet.Grips.Clear
                gcd.CCC[e.Gender].generategrips(e);
            }
            fProps.FillProperties(gcd.Drawing.Sheet.EntitiesSelected);
            clsEntities.GLGenDrawListSel();

            this.Prompt = ("Selected") + " " + gcd.Drawing.Sheet.EntitiesSelected.Count.ToString() + " " + ("elements");

        }
        else if ( ! gcd.clsJobCallBack )
        {
             // Si estoy aca es porque:
             // -No estoy en un proceso de seleccion
             // -No estoy aplicando una herramienta
             // -No hay entidades selecionadas

            if ( gcd.clsJobPrevious )
            {
                if ( (gcd.Drawing.Sheet.EntitiesSelected.Count == 1) && (gcd.clsJobPrevious.gender == cadEntityBuilder.gender) )
                {
                    if ( cadEntityBuilder.LastEntity )
                    {

                        gcd.clsJob = gcd.clsJobPrevious;
                        gcd.clsJob.Start();

                    }

                }
                else
                {
                     //If gcd.Drawing.Sheet.EntitiesSelected.Count = 0 Then              //Sin entidades seleccionadas podria significar cancelar
                     //
                     //                 gcd.clsJob = gcd.clsJobPrevious
                     //                 gcd.clsJob.Cancel
                     //             Else If gcd.Drawing.Sheet.EntitiesSelected.Count = 1 Then
                     //                 gcd.clsJob = gcd.clsJobPrevious
                     //                 gcd.clsJob.Start()
                     //
                     //             Else // tenemos varias entidades
                    gcd.clsJob = gcd.clsJobPrevious;
                    gcd.clsJob.Start();

                }
            }
             //     End If
             // Else
             //     // A.2.1.2 Tengo una seleccion previa -> Menu: Cortar, Copiar, Agrupar, Desagrupar, Llevar al layer actual, etc
             //     // Esto salta solo, pero debo configurarlo en algun lado
             //     gcd.clsJob = gcd.clsJobCallBack
             //     gcd.clsJob.run()
             //
             //End If
             // FIXME: ver esta pafrte dudosa
             // Else If gcd.Drawing.Sheet.EntitiesSelected.Count > 0 Then
             //     // Si estoy aca es porque:
             //     // -No estoy en un proceso de seleccion
             //     // -No estoy aplicando una herramienta
             //     // -Hay entidades selecionadas
             //     gcd.Drawing.Sheet.EntitiesSelected.Clear
             //     clsEntities.GLGenDrawListSel()
             //     fProps.FillProperties(gcd.Drawing.Sheet.EntitiesSelected)

        }
        else
        {

             // A.2.2 ToolActive = True? -> Finalizo la seleccion y vuelvo a la Tool
            gcd.Drawing.Sheet.EntitiesSelectedPrevious = gcd.Drawing.Sheet.EntitiesSelected.Copy();
            gcd.clsJob = gcd.clsJobCallBack;
            gcd.clsJob.run();

        }
        return; // este return es para evitar clicks simultaneos
    }

    if ( Mouse.MIddle )
    {

         // C.3 Medio -> ActionActive = ActionPanActive -> finalizo el paneo
        return; // este return es para evitar clicks simultaneos
    }

    gcd.Redraw;

}

public void MouseMove()
    {


    Grip g ;         
    Entity e ;         

    this.SelEndX = mouse.X;
    this.SelEndy = mouse.Y;
    this.SelEndXr = gcd.Xreal(Me.SelEndX);
    this.SelEndyr = gcd.Yreal(Me.SelEndy);
    this.SelEndXr = gcd.Near(Me.SelEndXr);
    this.SelEndYr = gcd.Near(Me.SelEndYr);

     // // yo soy el responsable de chequear POI
     // If Not gcd.flgSearchingPOI Thenntity

     //End If

     //end If
    if ( RectActive && Me.SelectType != Me.SelectTypeSingle )
    {
            // B.1 ActionActive = ActionRectActive: actualizo coordenadas del punto final del Rect
            // (se hace mas arriba)

            if (Me.SelEndXr <= Me.SelStartXr)
            {
                this.SelectCrossing = True;
        }
            else
            {
                this.SelectCrossing = False;
            }
    }
    else if ( GripPoint )
    {
         // B.2 ActionActive = ActionGripActive: modifico la entidad con la nueva posicion del punto
        gcd.Drawing.iEntity = clsMouseTracking.CheckBestPOI(Me.SelEndXr, this.SelEndYr);

        if ( (gcd.Drawing.iEntity[2] > 0) )
        {

            this.SelEndXr = gcd.Drawing.iEntity[0];
            this.SelEndYr = gcd.Drawing.iEntity[1];

             // Else
        } // puedo hacer ortogonal
        else
        {
            if ( gcd.Orthogonal ) // hablame de operadores logicos
            {

                if ( Abs(Me.SelEndX - Me.SelStartX) > Abs(Me.SelEndY - Me.SelStartY) ) // prevalece X
                {
                    this.SelEndYr = this.SelStartYr;

                }
                else
                {
                    this.SelEndXr = this.SelStartXr;
                }

            }

        } //
        if ( GripCopying )
        {

        }
        else
        {

        }
        GripEdit;
        DrawingAids.txtFrom = GripPoint.ToolTip;

    }
    else if ( Me.Active && PanActive )
    {
         // B.3 ActionActive = ActionPanActive: mando la coordenada a cadPan
         // (ni siquiera deberiamos estar aca, deberiamos estar en cadPan.class)
    }
    else
    {
         // busco algun grip
        GripHovered = FindGrip(Me.SelEndXr, this.SelEndYr);
         // B.4 ActionActive = 0: nada

        if ( gcd.Drawing.HoveredEntity )
        {
            if ( Config.ShowEntityInspector ) FInspector.Run(gcd.Drawing.HoveredEntity);
        }
        else
        {
            FInspector.Close();
        }
    }

     // Else

     // End If

}

public void KeyText(string EnteredText)
    {


     // in this case, we try to run the command wich is a class
    Object o ;         
    string RunWith ;         
    Class c ;         

    EnteredText = UCase(Trim(EnteredText));
    if ( EnteredText == "" ) return; // no BS here

    switch ( EnteredText)
    {
        case "_CANCEL":
            this.Cancel;
        case "R":
            this.SelectMode = this.SelectModeRem;
        case "A":
            this.SelectMode = this.SelectModeAdd;
        case "N": // seleccion previa
            gcd.Drawing.Sheet.EntitiesSelected.Clear; // = gcd.Drawing.Sheet.EntitiesSelectedPrevious.Copy()
            clsEntities.GLGenDrawListSel();
            gcd.redraw;

        case "P": // seleccion previa
            gcd.Drawing.Sheet.EntitiesSelected = gcd.Drawing.Sheet.EntitiesSelectedPrevious.Copy();
            clsEntities.GLGenDrawListSel();
            gcd.redraw;

        case "CENTER":
            gcd.PanTo(0, 0);
            gcd.Redraw;
             //gcd.regen
        case "REGEN":
            gcd.regen;
        case "REGENALL":
            gcd.PanToOrigin;
            gcd.regen;
        case "REDRAW":
            gcd.Redraw;
        case "STL":
            clsEntities.ExportSTL;

        default:
             //o = cadDimension // a test

             // Intercepto Alias
            if ( Config.oAlias.ContainsKey(Lower(EnteredText)) )
            {
                EnteredText = Upper(Config.oAlias[Lower(EnteredText)]);
            }

            if ( gcd.CCC.ContainsKey(EnteredText) )
            {
                o = gcd.CCC[EnteredText];
            }
            else
            {

                DrawingAIds.ErrorMessage = "Command not recognized";
                return;
            }

             // check if the class needs to be run trough other
            if ( o.usewith == "" ) // its a tool
            {
                gcd.clsJobPrevious = gcd.clsJob;
                gcd.clsJob = o;
                gcd.clsJob.start;

            } // its propably an eentity
            else
            {
                gcd.clsJobPrevious = gcd.clsJob;
                gcd.clsJob = gcd.CCC[o.usewith];
                gcd.clsJob.start(o);

            }

    }
    gcd.Drawing.iEntity.Clear;
    return;

     // TODO: dejar comentado mientras hagamos debug
     // catch

    DrawingAIds.ErrorMessage = "Command not recognized";
     //
     //

}

public void Draw()
    {


     double[] flxPoly = new double[] {};
    int iColor ;         

    Super.Draw();

    if ( Me.SelectCrossing )
    {
        iColor = Color.Red;
    }
    else
    {
        iColor = Color.Green;
    }
    if ( RectActive )
    {

         // si estamos dentro un viewport no hay nada que dibujar, porque no estamos seleccionando nada
        if ( gcd.Drawing.Sheet.Viewport ) return;
        if ( Me.SelectType == Me.SelectTypeRect || Me.SelectType == Me.SelectTypeSingleAndRect )
        {
            this.SelectionPoly.Clear; // aprovecho

            glx.Rectangle2D(Me.SelStartXr, this.SelStartYr, this.SelEndXr - this.SelStartXr, this.SelEndyr - this.SelStartYr, Color.RGB(224, 220, 207, 215),0,0,0, iColor, 1, gcd.stiDashedSmall, 2);

            return;
        }
        else if ( Me.SelectType == Me.SelectTypePoly )
        {

            fMain.PopupMenu = ""; // no hay menu contextual

             // como pude habver cambiado en este momento el modo de seleccion, chequeo
            if ( Me.SelectionPoly.Count == 0 )
            {
                this.SelectionPoly.Add(Me.SelStartXr);
                this.SelectionPoly.Add(Me.SelStartYr);
            }
            flxPoly.Clear;
            flxPoly.Insert(Me.SelectionPoly.Copy());
            flxPoly.Add(Me.SelEndXr);
            flxPoly.Add(Me.SelEndyr);
            flxPoly.Add(Me.SelectionPoly[0]);
            flxPoly.Add(Me.SelectionPoly[1]);
            glx.PolyLines(flxPoly, Color.red, 1, gcd.stiDashedSmall);
        }
        else
        {
            this.SelectionPoly.Clear;

        }

    }

     // No vamos a dibujar nada relativo a los grips si estamsos selecionando para alguna Tool
    if ( gcd.clsJobCallBack ) return;

    DrawingAids.DrawGrips(gcd.Drawing.Sheet.Grips);

    if ( Me.EntityForEdit ) Gcd.CCC[Me.EntityForEdit.Gender].Draw(Me.EntityForEdit);
    if ( Me.OriginalEntityForEdit )
    {
        if ( GripCopying )
        {
        }
        else
        {
            Gcd.CCC[Me.OriginalEntityForEdit.Gender].DrawShadow(Me.OriginalEntityForEdit);
        }
    }
    if ( GripPoint )
    {

        Gcd.CCC[GripPoint.AsociatedEntity.Gender].DrawEditing(GripPoint);
        if ( GripCopying )
        {
        }
        else
        {

            Gcd.CCC[GripPoint.AsociatedEntity.Gender].DrawShadow(Me.EntityForEdit);
        }
        DrawingAids.DrawSnapText();

    }
    else if ( GripHovered )
    {

        DrawingAids.Helper.texto = GripHovered.ToolTip;
        DrawingAids.Helper.dX = 15;
        DrawingAids.Helper.dY = 15;
    }
    else
    {
        DrawingAids.Helper.texto = "";

    }

}

public void GripEdit()
    {


    if ( GripPoint )
    {
        if ( GripCopying )
        {
             // GripPoint.X = Me.SelEndXr
             // GripPoint.Y = Me.SelEndYr
             // gcd.CCC[GripPoint.AsociatedEntity.Gender].translate(GripPoint.AsociatedEntity, Me.SelEndXr - Me.SelStartXr, Me.SelEndYr - Me.SelStartYr)

        }

        GripPoint.X = this.SelEndXr;
        GripPoint.Y = this.SelEndYr;
        gcd.CCC[GripPoint.AsociatedEntity.Gender].GripEdit(GripPoint);
        gcd.CCC[GripPoint.AsociatedEntity.Gender].buildgeometry(GripPoint.AsociatedEntity);
        gcd.redraw;

    }

}

public  Grip FindGrip(double x, double y)
    {


    Grip g;

    foreach ( var g in gcd.Drawing.Sheet.Grips)
    {
        if (puntos.Around(x, y, g.x, g.y, gcd.Metros(Config.GripProximityDistance)))
        {

            return g;
        }
         //
    }
    DrawingAids.txtFrom = "";
    return null;

}

public void KeyDown(int iCode)
    {


    if ( iCode == Key.ControlKey )
    {
        GripCopying = True;
        fMain.GLArea1.Cursor = gcd.CursorSelectAdd;
        SelectMode = SelectModeAdd;
    }
    else if ( iCode == Key.ShiftKey )
    {
        GripCopying = False;
        fMain.GLArea1.Cursor = gcd.CursorSelectRem;
        SelectMode = SelectModeRem;
    }
    else if ( iCode == Key.AltKey )
    {
        GripCopying = False;
        fMain.GLArea1.Cursor = gcd.CursorSelectXchange;
    }

}

public void KeyUp(int iCode)
    {


    if ( iCode == Key.ControlKey )
    {
        GripCopying = False;
        fMain.GLArea1.Cursor = gcd.CursorCross;
        SelectMode = SelectModeNew;
    }
    else if ( iCode == Key.ShiftKey )
    {
        GripCopying = False;
        SelectMode = SelectModeNew;
        fMain.GLArea1.Cursor = gcd.CursorCross;
    }
    else if ( iCode == Key.AltKey )
    {
        GripCopying = False;
        SelectMode = SelectModeNew;
        fMain.GLArea1.Cursor = gcd.CursorCross;
    }

}

}