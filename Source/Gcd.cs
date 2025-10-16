using System.Drawing;
using OpenTK.Graphics.OpenGL;
using Gaucho;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http.Headers;
using Gtk;
using OpenTK;


namespace Gaucho
{
    public static class Gcd
    {
        // Equivale a public static  entities As New Entity[] en Gambas
        // Todos los arrays de clases no nativas se arman como List<TipoDeClase>





        // Gambas module file

        // GambasCAD
        // Software para diseño CAD
        //
        // Copyright (C) Ing Martin P Cristia
        //
        // This program is free software; you can redistribute it and/or modify
        // it under the terms of the GNU General public static  License as published by
        // the Free Software Foundation; either version 2 of the License, or
        // (at your option) any later version.
        //
        // This program is distributed in the hope that it will be useful,
        // but WITHOUT ANY WARRANTY; without even the implied warranty of
        // MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
        // GNU General public static  License for more details.
        //
        // You should have received a copy of the GNU General public static  License
        // along with this program; if not, write to the Free Software
        // Foundation, Inc., 51 Franklin St, Fifth Floor,
        // Boston, MA  02110-1301  USA
        // Gambas class file

        public static double printingScale = 1;  // This is needed by some clases, like Text and MText.
        public static double ScreenDensity = 1080 / 29.5;   // pixele by cm wich gives a 1:100 scale in my monitor

        // Filtros del dibujo
        public static bool DrawOriginals = false;
        public static bool DrawMarcados = true;
        public static bool DrawSoloColumnas = false;
        // public static  DrawBounds As Boolean = false // ¿Para que sirve esta variable?: es para dibujar un contorno d las entidades como Text para debugging unicamente
        public static bool DrawHatch = true;   // Debugging si dibuja los hatch o no

        public static double ScaleLines = 1;               // la uso en Inserts para evitar lineas gordas
        public static string FormatoCotas = "0.00";

        public static bool ToolActive;

        public static int HookSize = 16;                        // for use with poi
        public static bool Orthogonal = false;               // idem Ortho F8 del CAD
        public static int SnapMode;
        public static int GrIdMode;
        public static int GrIdModePrev;
        public static bool MultiDraw;          // dibuja repetidamente la misma entidad

        public static Dictionary<string, IEntity> CCC;          // CAD Classes Collection
        public static Object clsJob;          // what I am doing now, thats either selecting or something else
        public static Object clsJobPrevious;          // what was doing before
        public static Object clsJobPreZoom;          // what was doing before Zooming or Panning
        public static int clsJobPreviousParam;          // a param to pass to clsJob.Start( param )
        public static Object clsJobCallBack;          // An object to call back after finishing something (like selecting). Must have .Run() sub
        public static int StepsDone;          // una variable de entorno util
                                              //public static  gColor As New Integer[]

        public static int drwDrawingClass = 0;  // 0 = Paint, 1 = OpenGL
        const int drwPaintClass = 2;
        const int drwOpenGLClass = 4;

        public static bool LoadingFinished = false;
        public static string[] FontList;          // esto deberia estar en otro lado
        public static string[] TextureList;          // esto deberia estar en otro lado
        public static string FileName;          // current work filename
        public static string[] LineTypes;

        // dibujos
        public static Image picVisibleOn;
        public static Image picVisibleOff;
        public static Image picFrozenOn;
        public static Image picFrozenOff;
        public static Image picLockedOn;
        public static Image picLockedOff;
        public static Image picPrintOn;
        public static Image picPrintOff;

        //---------------------------------------------------------------
        // Point of Interest

        enum poi {
            poiEndPoint = 1,
            poiMIdPoint = 2,
            poiQuadrant = 4,
            poiTangent = 8,
            poiCenter = 16,
            poiIntersection = 32,
            poiPerpendicular = 64,
            poiNearest = 128,
            poiBasePoint = 256,
            poiAparentCenter = 512
        }
        // CAD colors
        public const int ColorBlack = 0;
        public const int ColorRed = 1;
        public const int ColorBlue = 2;
        public const int ColorGreen = 3;
        public const int ColorYellow = 4;
        public const int ColorGray = 5;
        public const int ColorLightGray = 6;
        public const int ColorLightBlue = 7;
        public const int ColorWhite = 8;
        public const int ColorByBlock = 256;
        public const int ColorByLayer = 257;

        // Stipple lines
        public static double[] stiDashed = new double[] { };
        public static double[] stiDashedSmall = new double[] { };
        public static double[] stiAxis = new double[] { };

        // Intercambio de datos entre clases y modulos
        public static Dictionary<string, object> cATTDEF = new Dictionary<string, object>();

        // dim types
        public const int dimRotated = 0;
        public const int dimAligned = 1;
        public const int dimAngular = 2;
        public const int dimDiameter = 3;
        public const int dimRadius = 4;
        public const int dimAngular3point = 5;
        public const int dimOrdinate = 6;
        public const int dimHorizontal = 7;
        public const int dimVertical = 8;

        // PrintStyles
        public static Dictionary<string, object> PrintStyles;

        // patterns
        public static Dictionary<string, object> HatchPatterns;

        // fonts replacements

        public static Dictionary<string, object> FontReplacements;

        // Entity Flags
        const int flgDWG_Changed = 1;
        const int flgDWG_UnChanged = 2;
        const int flgDWG_Deleted = 4;
        const int flgGcd_New = 8;
        //public const flg5 As Integer = 16
        //public const flg6 As Integer = 32
        //public const flg7 As Integer = 64
        //public const flg8 As Integer = 128
        //public const flg9 As Integer = 256

        //variables de ambiente

        public static bool flgSearchingAllowed = true;   // impide cuellos de botellla
        public static bool flgSearchingPOI;
        public static bool flgShowingLayers;
        public static bool flgNewPosition;          // seteada cuando hay un cambio en pan o zoom
        public static bool flgQuitSearch;
        public static double Chronos;

        public static bool DrawHoveredEntity = false;

        public static string dirResources;          // para compatibilidad con GambasCAD

        public static bool DrawingReady = false;
        public static Entity currenyEntity;
        // public static  New Design Design ;         
        public static Drawing Drawing;

        public static List<Entity> entities = new();


        public static Dictionary<string, Drawing> Drawings = new();
        public static Dictionary<string, Entity> EntitiesSelected = new();
        public static int[] gColor = [];          // Colors list

        public static Gdk.Cursor CursorCross;
        public static Gdk.Cursor CursorSelect;
        public static Gdk.Cursor CursorSelectAdd;
        public static Gdk.Cursor CursorSelectRem;
        public static Gdk.Cursor CursorSelectXchange;

        public static double PrintingScale { get => printingScale; set => printingScale = value; }

        public static void SetDashes()
        {


            int i;

            Array.Clear(stiDashed);
            Array.Clear(stiDashedSmall);
            Array.Clear(stiAxis);
            stiDashed = new double[] { 10, -10 };
            stiDashedSmall = new double[] { 2.5f, -2.5f };
            stiAxis = new double[] { 10, -2.5f, 2.5f, -2.5f };

            for (i = 0; i <= stiDashed.Length - 1; i++)
            {
                stiDashed[i] *= Metros(1);
            }

            for (i = 0; i <= stiDashedSmall.Length - 1; i++)
            {
                stiDashedSmall[i] *= Metros(1);
            }

            for (i = 0; i <= stiAxis.Length - 1; i++)
            {
                stiAxis[i] *= Metros(1);
            }
        }



        // public static TextStyle FindStyle(string sName)
        // {




        //     foreach (TextStyle st in Gcd.Drawing.oStyles.Values)
        //     {

        //         if (st.Name == sName) return st;

        //     }

        //     return null;

        // }

        public static TextStyle? FindStyleById(string sId)
        {




            foreach (TextStyle st in Gcd.Drawing.TextStyles.Values)
            {

                if (st.Id == sId) return st;

            }

            return null; //Gcd.Drawing.arrStyles.First

        }

        public static Entity? FindEntity(string sId, bool SearchInBlocks = false)
        {




            if (Gcd.Drawing.Sheet.Entities.ContainsKey(sId)) return Gcd.Drawing.Sheet.Entities[sId];

            if (SearchInBlocks)
            {
                foreach (Block oBlock in Gcd.Drawing.Blocks.Values)
                {
                    if (oBlock.entities.ContainsKey(sId)) return oBlock.entities[sId];

                }
            }

            return null;

        }

        public static LineType? FindLType(string sName)
        {



            foreach (var LT in Gcd.Drawing.LineTypes.Values)
            {

                if (LT.Name.ToLower() == sName.ToLower()) return LT;

            }

            return null;

        }

        // public static int FindLtIndex(string sName)
        // {


        //     int i;

        //     for (i = 0; i <= Gcd.Drawing.arrLTYpes.Length - 1; i++)
        //     {

        //         if (Gcd.Drawing.arrLTYpes[i].Name.ToLower() == sName.ToLower()) return i;

        //     }

        //     return -1;

        // }

        public static Layer? GetLayerById(string Id)
        {




            foreach (Layer l in Drawing.Layers.Values)
            {

                if (l.id == Id) return l;

            }

            return null;

        }

        public static int iCadColor(string sColor)
        {

            int i;
            sColor = sColor.ToLower();
            if (sColor == "bylayer") return 256;
            if (sColor == "byblock") return 257;
            if (sColor == "byobject") return 258;
            try
            {
                i = int.Parse(sColor);
                return i;
            }
            catch (System.Exception)
            {

                Console.WriteLine("Error converting color " + sColor);
                return 0;
            }

        }

        public static double Metros(int pixeles)
        { // converts pixels to meters

            return pixeles / Gcd.Drawing.Sheet.ScaleZoom;

        }

        public static double Pixels(double distancia)
        {
            return distancia * Gcd.Drawing.Sheet.ScaleZoom;
        }


        public static void main()
        {


            switch (Config.WindowBackColor)
            {
                case Color.Black:
                    //Case &1B2224
                    Config.WhiteAndBlack = RGB(Color.White);
                    break;
                default:
                    Config.WhiteAndBlack = RGB(Color.Black);

            }

            //If WindowBackColor = 0 Then WhiteAndBlack = Color.White Else WhiteAndBlack = Color.Black
            // // armo el array de colores
            // gcolor = DecodeColor()
            //

            //FileName = User.Home &/ "autosaveV5.xml"

            debuginfo("Reading fonts from " + System.IO.Path.Combine(Gcd.dirResources, "fonts", "lff"), false, false, true);
            FontList = glx.LoadFonts(System.IO.Path.Combine(Gcd.dirResources, "fonts", "lff"));
            //FontList = glx.LoadFonts(Gcd.sFonts)
            glx.SelectFont("romans");

            // agrego la lista a los reemplazos
            foreach (string s in FontList)
            {
                FontReplacements.Add(s.ToLower(), s.ToLower());
            }

            FontReplacements.Add("kochigothic", "arial");

            TextureList = glx.LoadTextures(System.IO.Path.Combine(Gcd.dirResources, "textures"));
            //texturelist = glx.LoadTextures(Gcd.sTextures)

            // otros recursos
            picVisibleOn = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "visible_on.png"));
            picVisibleOff = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "visible_off.png"));
            picFrozenOn = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "frozen_on.png"));
            picFrozenOff = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "frozen_off.png"));
            picLockedOn = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "locked_on.png"));
            picLockedOff = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "locked_off.png"));
            picPrintOn = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "printOn.png"));
            picPrintOff = Image.File(System.IO.Path.Combine(Gcd.dirResources, "png", "printOff.png"));

            // this is what we are doing now
            Gcd.clsJob = cadSelection;
            Gcd.clsJobPrevious = cadSelection;
            Gcd.clsJobPreZoom = cadSelection;
            Gcd.clsJobPreviousParam = 0;

            debuginfo("Gcd initialized OK", false, false, true);

            // test
            // Dim flxTest As New double[]
            // flxTest = puntos.DashedLineStrip(0.5, 0, [0, 0, 10, 0, 10, 10], [2, -1], 1, true)
            // Stop
            // Dim p0, p1, p As New Punto2d
            // p1.x = 0
            // p1.y = 0
            //
            // p0.x = 10
            // p0.y = 10
            //
            // p = puntos.ExtendLine2D(p0, p1, 1)
            //
            // Stop

        }

        public static void LoadCommon()
        {


            string[] sFiles;


            sFiles = System.IO.Directory.GetFiles(System.IO.Path.Combine(Gcd.dirResources, "common"), "*.dxf");
            if ((sFiles.Length == 0)) return;
            foreach (string sBlocksFiles in sFiles)
            {
                FBlocks.AddBlock(System.IO.Path.Combine(Gcd.dirResources, "common", sBlocksFiles), Utils.FileWithoutExtension(sBlocksFiles), Gcd.Drawing.Blocks);

            }

        }

        public static Layer GetLayer(string LayerName)
        {




            foreach (var Lay in Gcd.Drawing.Layers)
            {

                if (Lay.Name == LayerName) return Lay;
            }
            return null;

        }

        // Reads entities and fill arrLayers
        // public static Sub FillLayers(drw As Drawing)
        //
        //     Dim hLay As Layer
        //     Dim e As Entity
        //
        //     Gcd.debugInfo("Filling layers",false,false,true)
        //
        //     // primero eliminamos lo q haya
        //     For Each hLay In drw.Layers
        //         hLay.Entities.Clear
        //     Next
        //     If drw.Entities Then
        //         For Each e In drw.Entities
        //             If e.pLayer Then e.pLayer.Entities.Add(e)
        //         Next
        //     End If
        //     Gcd.debugInfo("Llenados los Layers",false,false,true)
        //
        // End

        // // Reads entities and fill arrLayers
        // public static Sub FillSheets(drw As Drawing)
        //
        //     Dim e As Entity
        //     Dim s As Sheet
        //
        //     Gcd.debugInfo("Filling Sheets",false,false,true)
        //
        //     If drw.Entities Then
        //         For Each e In drw.Entities
        //             If drw.Sheets.ContainsKey(e.HandleOwner) Then
        //                 drw.Sheets[e.HandleOwner].entities.Add(e, e.Handle)
        //             Else // las coloco en Model
        //                 e.HandleOwner = drw.Model.Handle
        //                 drw.Model.Entities.Add(e, e.Handle)
        //             Endif
        //         Next
        //     End If
        //     Gcd.debugInfo("Llenadas las Sheets",false,false,true)
        //
        // End

        // // Reads entities and fill arrLayers
        // public static Sub FillInserts(drw As Drawing)
        //
        //     Dim e As Entity
        //
        //     Gcd.debugInfo("Filling Inserts",false,false,true)
        //
        //     If drw.Entities Then
        //         For Each e In drw.Entities
        //             If drw.Inserts.ContainsKey(e.IdContainer) Then
        //                 drw.Inserts[e.IdContainer].Entities.Add(e, e.Id)
        //             Endif
        //         Next
        //     End If
        //     Gcd.debugInfo("Llenados los Inserts",false,false,true)
        //
        // End

        // // Reads LineTypes collection and puts data in arrLTypes
        // public static Sub ReadLTypes()
        //
        //     Dim hlty As LineType
        //     Dim t, i As Integer
        //     Dim fTrameLength As double
        //     Dim sNextKey As String
        //
        //     // primero eliminamos lo q haya
        //     Drawing.oLTYpes.Clear
        //     For Each c As Collection In Drawing.cLtypes
        //         hlty = New LineType
        //         hlty.Name = c[dxf.codName]
        //         hlty.Description = c["3"]
        //         hlty.handle = c[dxf.codHandle]
        //         If hLty.handle = "" Then hLty.handle = Gcd.NewHandle()
        //         hlty.nTrames = CInt(c["73"])
        //         If hlty.nTrames > 0 Then hlty.Length = c["40"]
        //         i = 0
        //         For t = 1 To hlty.nTrames
        //             Do
        //                 If t > 1 Then
        //                     sNextKey = "49_" & CStr(i)
        //                 Else
        //                     sNextKey = "49"
        //                 Endif
        //                 If c.ContainsKey(sNextKey) Then
        //                     Inc i
        //                     hlty.TrameLength.Add(Abs(Cdouble(c[sNextKey])) * 2.5) // TODO: verificar este valor arbitrario
        //                     If hlty.TrameLength.Last = 0 Then hlty.TrameLength.Last = 1
        //
        //                     Break
        //                 Endif
        //                 Inc i
        //                 If i = 1000 Then Return
        //             Loop
        //
        //         Next
        //
        //         Drawing.oLTYpes.Add(hlty, hlty.handle)
        //
        //     Next
        //
        // End

        // // Reads TextStyle and DimStyles collection and puts data in arrStyles
        // public static Sub ReadStyles()
        //
        //     Dim hlty As TextStyle
        //     Dim t, i As Integer
        //     Dim fTrameLength As double
        //     Dim sH2, sNextKey As String
        //     Dim RefStyle As TextStyle
        //
        //     // primero eliminamos lo q haya
        //     Drawing.oStyles.Clear
        //     // Leo los TextStyle de texto
        //     For Each c As Collection In Gcd.Drawing.cModel["TABLES"]["STYLE"]
        //         hlty = New TextStyle
        //
        //         hlty.Name = c[dxf.codName]
        //         hlty.handle = c[dxf.codHandle]
        //         hlty.IsDimStyle = false
        //         hlty.sFont_3 = c["3"]
        //
        //         hlty.FixedH_40 = Cdouble(c["40"])
        //
        //         // Esto no puede usarse asi, LastHeightUsed_2 es solo un dato de historial
        //         // If hlty.FixedH_40 = 0 Then hlty.FixedH_40 = Cdouble(c["42"])
        //
        //         If hlty.handle = "" Then sH2 = hlty.name Else sH2 = hlty.handle
        //         Drawing.oStyles.Add(hlty, sh2)
        //
        //     Next
        //     If Drawing.TextStyles.Count > 0 Then
        //
        //     Endif
        //
        //     // Leo lo TextStyle de dimensiones
        //     For Each c As Collection In Gcd.Drawing.cModel["TABLES"]["DIMSTYLE"]
        //         hlty = New TextStyle
        //
        //         hlty.Name = c[dxf.codName]
        //         If Isnull(c[dxf.codHandle]) Then
        //             hlty.handle = c["105"]
        //         Else
        //             hlty.handle = c[dxf.codHandle]
        //         Endif
        //
        //         hlty.IsDimStyle = true
        //
        //         Try hlty.fArrowSize_41 = Cdouble(c["41"])
        //         If hlty.fArrowSize_41 = 0 Then hlty.fArrowSize_41 = 1
        //
        //         Try hlty.fTxtHeight_140 = Cdouble(c["140"])
        //         If hlty.fTxtHeight_140 = 0 Then hlty.fTxtHeight_140 = 1
        //
        //         Try hlty.iRefStyleHandle_340 = c["340"]
        //
        //         If hlty.iRefStyleHandle_340 <> "" Then
        //             RefStyle = FindStyleByHandle(hlty.iRefStyleHandle_340)
        //             If Not Isnull(RefStyle) Then
        //                 If RefStyle.FixedH_40 > 0 Then hlty.fTxtHeight_140 = RefStyle.FixedH_40
        //                 hlty.sFont_3 = RefStyle.sFont_3
        //             Endif
        //         Endif
        //
        //         Drawing.oStyles.Add(hlty, hlty.handle)
        //
        //     Next
        //
        // End
        public static double GetLineWt(int iValue, Layer l)
        {

            // https://ezdxf.readthedocs.io/en/stable/concepts/lineweights.html
            // -1    LINEWEIGHT_BYLAYER
            // -2    LINEWEIGHT_BYBLOCK
            // -3    LINEWEIGHT_DEFAULT

            if (iValue > 1) return iValue / 100;
            if (l == null) l = Gcd.Drawing.CurrLayer;
            if (iValue == -1)
            {
                if (l.LineWt < 0) return Config.DefLineWt;
                return l.LineWt / 100;
            }
            else if (iValue == -3)
            {
                return Config.DefLineWt;
            }
            else if (iValue == -2)
            {
                if (Gcd.Drawing.CurrBlockLineWt < 0) return Config.DefLineWt;
                return Gcd.Drawing.CurrBlockLineWt / 100;
            }

            return Config.DefLineWt;

        }

        public static int GetGBColor(int CADcolor, Layer pLayer)
        {


            int iColor;
            // agrego la parte de PaperSpace

            if (pLayer == null)
            {
                iColor = CADcolor;
            }
            else
            {
                iColor = pLayer.Colour;
            }

            // color
            if (CADcolor == 256) // buscar color del layer
            {

                iColor = gColor[iColor];

            } // buscar color del objeto
            else if (CADcolor == 257)
            {

                iColor = gColor[iColor];

            }
            else if (CADcolor == 0)
            {

                iColor = gColor[iColor];
            }
            else
            {

                iColor = gColor[CADcolor];

            }

            if (Drawing.Sheet.IsModel)
            {
                // If icolor = 0 Then Stop
                return iColor;
            }
            else
            {

                if (iColor == Config.WhiteAndBlack) iColor = Color.Invert(icolor);

                if (Drawing.Sheet.pPrintStyle.ColorStyle == 2)
                {

                    return iColor;
                }
                else if (Drawing.Sheet.pPrintStyle.ColorStyle == 1)
                {
                    return Color.Desaturate(iColor);
                }
                else
                {
                    return Drawing.Sheet.WhiteAndBlack;
                }

            }

        }

        // DEvuelve una string supuestamente unica basada en Timer y Date

        public static string UniqueId()
        {


            string s;

            if (s == "")
            {

                s = CStr(Date) + CStr(Time) + Str(Gcd.Drawings.Count);

                s = Replace(s, "/", "");
                s = Replace(s, " ", "-");
                s = Replace(s, ":", "");
                s = Replace(s, ".", "-");
            }

            //s = "{" & Trim(s) & "}"
            s = string.Trim(s);
            return s;

        }

        // // Devuelve un handle disponible unico para la coleccion de datos.
        // // Basado en la variable de ambiente para el dibujo actual
        // public static Function GetNewHandle(c As Collection) As String
        //
        //   Dim i As Integer, s As String
        //
        //   i = c.Count
        //   Try i = CInt(c.Last)
        //   Do
        //     Inc i
        //     s = CStr(i) // esto podria ser Hex
        //     If Not c.ContainsKey(s) Then
        //       Return s
        //     Endif
        //
        //   Loop
        //
        // End

        // Returns the next available handle
        public static string NewId(Drawing d)
        {


            string hHex;

            if (!d) d = Drawing;
            d.HandSeed++; // esta es la ultima handle utilizada, le sumo 1
            hHex = Hex(D.HandSeed);
            d.Headers.HANDSEED = hHex;

            return hHex; // devuelvo la Hex del numero

        }

        // public static int iHeader(string VarName)
        //     {


        //     return Val("&H" + Drawing.cHeader[VarName][Drawing.cHeader[VarName].First]);

        // }

        // public static  Variant GetHeaderValue(string VarName, Variant DefValuer)
        //     {


        //     if ( Drawing.cHeader.ContainsKey(VarName) )
        //     {
        //         if ( Drawing.cHeader[VarName].count == 1 )
        //         {
        //             if ( Drawing.cHeader[VarName][Drawing.cHeader[VarName].First].key == "70" )
        //             {
        //                 return CInt(Drawing.cHeader[VarName][Drawing.cHeader[VarName].First]);
        //             }
        //             else if ( Drawing.cHeader[VarName][Drawing.cHeader[VarName].First].key == "40" )
        //             {
        //                 return Cdouble(Drawing.cHeader[VarName][Drawing.cHeader[VarName].First]);
        //             }
        //             else
        //             {
        //                 return Drawing.cHeader[VarName][Drawing.cHeader[VarName].First];
        //             }

        //         } // tiene mas de un elemento, Y LOS SUPONGO TODOS VARIANT
        //         else
        //         {
        //             New Variant[] vRet ;         
        //             foreach ( vVar As Variant in Drawing.cHeader[VarName])
        //             {
        //                 vRet.Add(vVar);
        //             }
        //             return vRet;

        //         }
        //     }

        // }

        public static string ODA_DWGtoDXF(string sDwgFile)
        {


            string str;
            string filebase;
            int Steps;

            filebase = utils.FileFromPath(sDwgFile);

            steps = 0; // elimino el archivo temporal que hubiese creado

            if (File.Exists(System.IO.Path.Combine(config.dirDwgIn, filebase))) File.Delete(System.IO.Path.Combine(config.dirDwgIn, filebase));

            Steps = 1; // hago una copia previa a la conversion
            Copy(sDwgFile, System.IO.Path.Combine(config.dirDwgIn, filebase));

            Steps = 2; // Calling the converter

            utils.Shell("ODAFileConverter; //" & config.dirDwgIn & "// //" & config.dirDxfIn & "// //ACAD2018// //DXF// 0 0", WaitTo: str);

            steps = 3;
            // vacio el directorio de entrada
            Kill(System.IO.Path.Combine(config.dirDwgIn, filebase));

            if (File.Exists(System.IO.Path.Combine(config.dirDxfIn, utils.FileWithoutExtension(filebase) + ".dxf")))
            {
                Gcd.debuginfo("Conversion to DXF correct", false, false, true);
                return System.IO.Path.Combine(config.dirDxfIn, utils.FileWithoutExtension(filebase) + ".dxf");
            }
            else
            {

                return "";
            }

            Catch;

            switch (Steps)
            {
                case 0:
                    // esto puede fallar por acceso denegado
                    Gcd.debuginfo("Acces denied to temp file", false, false, true);
                    return null;
                case 1:
                    // esto puede fallar por file corrupt
                    Gcd.debuginfo("File corrupt", false, false, true);
                    return null;
                case 2:
                    // esto puede fallar por diversas cuestiones
                    Gcd.debuginfo("Conversion failed", false, false, true);
                    return null;
                case 3:
                    // esto puede fallar por diversas cuestiones
                    Gcd.debuginfo("Could't empty temp dir", false, false, true);
                    return null;

            }

        }

        public static string ODA_DXFtoDWG(string sDxfFile)
        {


            string str;
            string filebase;
            int Steps;

            filebase = utils.FileFromPath(sDxfFile); // deberia estar en main.dirDxfIn

            steps = 0; // elimino el archivo temporal que hubiese creado
            if (File.Exists(System.IO.Path.Combine(config.dirDxfOut, filebase))) File.Delete(System.IO.Path.Combine(config.dirDxfOut, filebase));

            Steps = 1; // hago una copia previa a la conversion
            if (sDxfFile != (System.IO.Path.Combine(config.dirDxfOut, filebase))) File.Copy(sDxfFile, System.IO.Path.Combine(config.dirDxfOut, filebase));

            Steps = 2; // Calling the converter
            utils.Shell("ODAFileConverter; //" & config.dirDxfOut & "// //" & config.dirDwgOut & "// //ACAD2010// //DWG// 0 0", WaitTo: str);


            Gcd.debuginfo(str);

            steps = 3;
            // vacio el directorio de entrada
            //Kill main.dirDxfOut &/ filebase //FIXME: descomentar esto despues del debug

            return System.IO.Path.Combine(config.dirDwgOut, utils.FileWithoutExtension(filebase) + ".dwg");

            Catch;

            switch (Steps)
            {
                case 0:
                    // esto puede fallar por acceso denegado
                    Gcd.debuginfo("Acces denied to temp file");
                    return null;
                case 1:
                    // esto puede fallar por file corrupt
                    Gcd.debuginfo("File corrupt");
                    return null;
                case 2:
                    // esto puede fallar por diversas cuestiones
                    Gcd.debuginfo("Conversion failed");
                    return null;
                case 3:
                    // esto puede fallar por diversas cuestiones
                    Gcd.debuginfo("Could't empty temp dir");
                    return null;

            }

        }

        //  Lee entidades e intenta dibujarlas centrado en el contenedor: DrawingArea, Image, Image
        //  iColor: CAD color o -1 para usar como vienen en el DXF
        //  fLineWidth: -1=como vienen, 0=automatico, >0 = fijo en ese valor
        public static bool FitEntitiesToImage(Dictionary<string, Entity> cEntities, Object imgPreview, int iColor = 0, double fLineWIdth = 1, int iBackGround = -1)
        {


            double scaleX;
            double scale;
            double scaleY;


            double[] flxLimits;

            //depre clsEntities.BuildPoi(cEntities)
            if (cEntities.Count > 0)
            {
                flxLimits = clsEntities.ComputeLimits(cEntities); // computo el tamaño de la entidad, y luego determino la escala

                Paint.Begin(imgPreview);
                if (iBackGround >= 0) Paint.Background = iBackGround;

                Paint.Reset; // vuelvo escalas y traslados a cero
                Paint.Translate(Paint.W / 2, Paint.H / 2); // centro el dibujo
                if ((flxLimits[2] - flxLimits[0]) > 1e-10)
                    scaleX = Paint.w / (flxLimits[2] - flxLimits[0]);
                else
                    scaleX = 1e10;
                if ((flxLimits[3] - flxLimits[1]) > 1e-10)
                    scaleY = Paint.H / (flxLimits[3] - flxLimits[1]);
                else
                    scaleY = 1e10;
                if (scaleX < scaleY)
                    Scale = scaleX;
                else
                    Scale = scaleY;
                Paint.Scale(scale * 0.85, -scale * 0.85);

                PrintingScale = scale * 0.85;

                // centro el dibujo
                Paint.Translate(-(flxLimits[2] + flxLimits[0]) / 2, -(flxLimits[3] + flxLimits[1]) / 2);

                foreach (var entIdad in cEntities)
                {
                    if (entIdad.pLayer == null) entIdad.pLayer = Gcd.Drawing.CurrLayer;
                    if (iColor < 0)
                    {
                        Paint.Brush = Paint.Color(Gcd.gColor[entIdad.colour]);
                    }
                    else
                    {
                        Paint.Brush = Paint.Color(iColor);
                    }
                    if (fLineWIdth == 0)
                    {
                        if (entIdad.LineWIdth == 0) entIdad.LineWIdth = 1;

                        Paint.LineWIdth = GetLineWt(entIdad.LineWIdth) / scale;
                    }

                    Gcd.CCC[entIdad.gender].draw2(entIdad);
                }
                Paint.End;

            }

        }

        // Carga un DXF e intenta dibujarlo centrado en el contenedor: DrawingArea, Image, Image
        // iColor: CAD color o -1 para usar como vienen en el DXF
        // fLineWidth: -1=como vienen, 0=automatico, >0 = fijo en ese valor
        public static bool FitDxfToImage(string sDXFfile, Object imgPreview, int iColor = 0, double fLineWIdth = 1)
        {


            Drawing LastDrw;
            Drawing drwTemp;

            if (!Exist(sDxfFile)) return false;
            LastDrw = Gcd.drawing;

            drwTemp = Gcd.NewDrawing(sDxfFile);
            Gcd.Drawing = drwTemp;
            Dxf.LoadFile(sDxfFile, drwTemp);
            Gcd.Drawing = LastDrw;

            FitEntitiesToImage(drwTemp.Model.Entities, imgPreview, iColor, fLineWIdth);

        }

        public static bool IsPoint(string sVal)
        {


            Variant v;
            string[] slx;

            double f;

            sVal = Trim(sVal);
            if (sVal.StartsWith("@")) sVal = sVal.Substring(1);

            slx = Split(sval, ",");

            foreach (var s in slx)
            {
                try
                {
                    f = string.toDouble(s);
                }
                catch
                {
                    return false;
                }
            }
            if (slx.Count == 2 || slx.Count == 3)
                return true;
            else
                return false;


        }

        public static double Xreal(int ScreenX)
        {

            //Return Metros((screenx - Gcd.Drawing.Sheet.GlSheet.w / 2 - (Gcd.Drawing.Sheet.PanX + Gcd.Drawing.Sheet.PanBaseX)))
            if (Isnull(Gcd.Drawing.Sheet.GlSheet)) return 0;
            return Metros((ScreenX - Gcd.Drawing.Sheet.GlSheet.w / 2 - (Gcd.Drawing.Sheet.PanX))) + Gcd.Drawing.Sheet.PanBaseRealX;

        }

        public static double Yreal(int ScreenY)
        {

            //Return Metros((-ScreenY + Gcd.Drawing.Sheet.GlSheet.h / 2 - (Gcd.Drawing.Sheet.PanY + Gcd.Drawing.Sheet.PanBaseY)))
            if (Isnull(Gcd.Drawing.Sheet.GlSheet)) return 0;
            return Metros((-ScreenY + Gcd.Drawing.Sheet.GlSheet.h / 2 - (Gcd.Drawing.Sheet.PanY))) + Gcd.Drawing.Sheet.PanBaseRealY;

        }

        public static double XPix(double X)
        {

            return Pixels(X - Gcd.Drawing.Sheet.PanBaseRealX) + Gcd.Drawing.Sheet.GlSheet.w / 2 + Gcd.Drawing.Sheet.PanX; //+ Gcd.Drawing.Sheet.PanBaseX

        }

        public static double YPix(double Y)
        {

            //Return Metros((-ScreenY + glarea1.h / 2 - Gcd.Drawing.PanY))
            return -(Pixels(Y - Gcd.Drawing.Sheet.PanBaseRealY) - Gcd.Drawing.Sheet.GlSheet.h / 2 + Gcd.Drawing.Sheet.PanY); // + Gcd.Drawing.Sheet.PanBaseY)

        }

        // Returns the neares point to the grid, if active.
        public static double Near(double xyzReal)
        {
            // return the nearest point to the grid
            // this is a world to world points (not pixels)

            // Example:
            // if                    GridSpacing = 0.2
            // we pass               xyzReal = 1.35
            // function will give    NearReal = 1.40

            int n;
            double r;

            if (!Gcd.Drawing.GrIdActive) return xyzReal;

            r = xyzReal / Gcd.Drawing.GridCurentSpacing;
            n = Int(r);
            r = r - Int(r);

            if (r > 0.5) n += 1;

            return n * Gcd.Drawing.GridCurentSpacing;

        }

        public static int ScreenWIdth()
        {


            return Gcd.Drawing.Sheet.GlSheet.W;

        }

        public static int ScreenHeight()
        {


            return Gcd.Drawing.Sheet.GlSheet.h;

        }

        public static void redraw()
        {


            fmain.redraw;

        }

        public static void RefreshTexts()
        {


            fmain.tmrSlash_Timer();

        }

        // Regenera las listas de OpenGL
        public static void Regen()
        {


            // La parte de los VBO
            Gcd.debugInfo("Generating GL graphics", false, false, true);
            Gcd.StepsDone = 0;
            //ReEscalar(drawing.Sheet)
            //PanToOrigin()
            clsEntities.BuildGeometry;
            Gcd.Drawing.Sheet.ScaleZoomLast = Gcd.Drawing.Sheet.ScaleZoom;
            clsEntities.glGenDrawList;
            //clsEntities.glGenDrawList2
            clsEntities.glGenDrawListLAyers;
            // Gcd.debugInfo("Entities GL list generated",false,false,true, true)
            //clsEntities.glGenDrawListSel
            if (Gcd.Drawing.Has3dEntities)
            {
                Gcd.Drawing.Sheets["Model3D"].scene.models.Add(Gcd.Drawing.Sheets["Model3D"].Model3D, "model");
                Gcd.Drawing.Sheets["Model3D"].scene.placemodel(Gcd.Drawing.Sheets["Model3D"].Model3D);
                Gcd.Drawing.Sheets["Model3D"].scene.setscene();
            }
            Gcd.debugInfo("Layers compiled", false, false, true, true);
            redraw;

        }

        // Centra el grafico en una posicion
        public static void PanTo(double Xr, double Yr, Sheet s  = null)
        {


            double Xcentro;
            double Ycentro;
            if (!s) s = Gcd.Drawing.Sheet;

            Xcentro = Xreal(s.GlSheet.w / 2);
            Ycentro = Yreal(s.GlSheet.h / 2);

            // drw.Model.PanBaseRealX = -Xcentro
            // drw.Model.PanBaseRealY = -Ycentro
            //
            // drw.Model.PanBaseX = XPix(drw.Model.PanBaseRealX)
            // drw.Model.PanBaseY = YPix(drw.Model.PanBaseRealY)

            s.PanX -= Pixels(Xr - Xcentro); //drw.Model.PanBaseX
            s.PanY -= Pixels(Yr - Ycentro);

        }

        public static void PanToOrigin(Sheet s = null)
        {


            // muevo el grafico desde la posicion actual al 0,0
            double Xcentro;
            double Ycentro;
            if (!s) s = Gcd.Drawing.Sheet;

            Xcentro = Metros((int)-(Gcd.Drawing.Sheet.PanX));
            Ycentro = Metros((int)-(Gcd.Drawing.Sheet.PanY));

            // FIXME: despues de qe todo funcione bien, volvemos con esto
            //s.PanBaseRealX += Xcentro
            //s.PanBaseRealY += Ycentro

            s.PanX = 0;
            s.PanY = 0;

        }

        // Regenera los layers, sin regenerar cada entidad
        public static void RegenList()
        {


            // La parte de los VBO
            Gcd.debugInfo("Generating GL graphics", false, false, true, true);

            //clsEntities.glGenBuffers()
            //clsEntities.FillLayersWithEntities(Drawing)
            //clsEntities.CollectVisibleEntities
            //clsEntities.glGenDrawList
            //clsEntities.glGenDrawListSel
            clsEntities.glGenDrawListLAyers;
            redraw;

        }

        // public static  Sub ReEscalar(sToScale As Sheet)
        //
        //     Dim limits As New double[]
        //     Dim dX, dY As double         // DIMENSION DEL DIBUJO
        //     Dim gX, gY As double         // POSICION DEL CG
        //     Dim fX, fY As double          // relacion del tamaño con la posicion
        //
        //     limits = clsEntities.ComputeLimits(sToScale.Entities)
        //
        //     dX = limits[2] - limits[0]
        //     dy = limits[3] - limits[1]
        //
        //     GX = (limits[2] + limits[0]) / 2
        //     Gy = (limits[3] + limits[1]) / 2
        //
        //     If dX = 0 Or dY = 0 Then Return
        //
        //     fX = Abs(gX / dX)
        //     fY = Abs(gY / dY)
        //
        //     If fY > 1e5 Or fX > 1e5 Then // existe la posibilidad de artifacts
        //         Debug "ARTIFACTS EN", sToScale.Name
        //     End If
        //
        //     // de todas formas el rescalado tendria que ser
        //
        //     sToScale.PanBaseX = gx
        //     sToScale.PanBaseY = gy
        //
        // End

        public static void GetZoomExtents(Sheet s)
        {


            double[] limits = [];
            double SZx;
            // ahora calculo donde estaria el centro de este dibujo

            double cx;
            double cy;
            int px;          // coordenadas del punto medio
            int py;

            limits = clsEntities.ComputeLimits(s.Entities);

            cx = limits[2] - limits[0];
            cy = limits[3] - limits[1];

            if (cx == 0) cx = 0.00001;
            if (cy == 0) cy = 0.00001;
            s.ScaleZoom = s.GlSheet.h / cy * 0.9;

            szx = s.GlSheet.w / cx * 0.9;

            if (szx < s.ScaleZoom) s.ScaleZoom = szx;

            px = (limits[2] + limits[0]) / 2;
            py = (limits[3] + limits[1]) / 2;

            s.PanX = -Pixels(px);
            s.PanY = -Pixels(py);

            if (s.ScaleZoom > 1e6 || s.ScaleZoom < 1e-6)
            {
                s.ScaleZoom = 1;
                s.PanX = 0;
                s.PanY = 0;
            }

        }

        public static string LibreDWGtoDXF(string sDwgFile)
        {


            string str;
            string filebase;
            int Steps;

            filebase = utils.FileFromPath(sDwgFile);
            //filebase = sDwgFile
            steps = 0; // elimino el archivo temporal que hubiese creado

            if (File.Exists(System.IO.Path.Combine(main.dirDwgIn, filebase))) File.Delete(System.IO.Path.Combine(main.dirDwgIn, filebase));

            Steps = 1; // hago una copia previa a la conversion
            File.Copy(sDwgFile, System.IO.Path.Combine(main.dirDwgIn, filebase));

            Steps = 2; // Calling the converter
            Shell("dwgread -O DXF -o \"" + System.IO.Path.Combine(main.dirDxfOut, utils.FileWithoutExtension(filebase) + ".dxf") + "\" \"" + System.IO.Path.Combine(main.dirDwgIn, filebase) + "\"", out str);
            Debug str;
            steps = 3;
            // vacio el directorio de entrada

            return System.IO.Path.Combine(main.dirDxfOut, utils.FileWithoutExtension(filebase) + ".dxf");

            Catch;

            switch (Steps)
            {
                case 0:
                    // esto puede fallar por acceso denegado
                    Console.WriteLine(("Acces denied to temp file") + "\n");
                    return null;
                case 1:
                    // esto puede fallar por file corrupt
                    Console.WriteLine(("File corrupt") + "\n");
                    return null;
                case 2:
                    // esto puede fallar por diversas cuestiones
                    Console.WriteLine(("Conversion failed") + "\n");
                    return null;
                case 3:
                    // esto puede fallar por diversas cuestiones
                    Console.WriteLine(("Could't empty temp dir") + "\n");
                    return null;

            }

        }

        // Prepara un dibujo y lo devuelve listo para usarse
        public static Drawing NewDrawing(string sName)
        {


            Drawing d;

            d = new();

            d.Headers = new Headers();
            d.FileName = sname;
            Sheet s = new();
            s.Name = "Model";
            s.IsModel = true;
            //s.Entities = d.Entities
            // s.Handle = "2" // Le asigno el 2 porque el 1 es el block_record
            d.Sheets.Add(s, "Model");
            d.Sheet = s;
            d.Model = s;
            //D.Entities = d.Sheet.Entities

            // lo agrego a los bloques
            Block b = new();
            b.name = "*Model_Space";
            b.entities = Dictionary;
            d.Sheet.Entities = b.entities;
            b.IsAuxiliar = true;
            b.IsReciclable = false;

            // Asocio bloque y hoja
            s.Block = b;
            b.Sheet = s;

            d.Blocks.Add(b, b.name);

            Layer L = new();
            L.Name = "0";
            L.id = "20";
            d.Layers.Add(L, L.name); //Agrego un layer
            d.CurrLayer = L;

            LineType LT = new();
            LT.Name = "CONTINUOUS";
            LT.id = "21";
            d.LineTypes.Add(LT, LT.Name); // Agrego un LineType
            L.LineType = LT;
            d.CurrLineType = LT;

            DimStyle DS = new();
            DS.id = "22"; //Gcd.NewHandle(d)
            DS.name = "standard";


            d.CurrDimStyle = DS;
            d.DimStyles.Add(DS, DS.name); // Agrego un DimStyle

            // d.Tables.Add(d.Blocks, "1")
            // d.Tables.Add(d.Layers, "10")
            // d.Tables.Add(d.Viewports, "11")
            // d.Tables.Add(d.LineTypes, "12")
            // d.Tables.Add(d.TextStyles, "13")
            // d.Tables.Add(d.Views, "14")
            // d.Tables.Add(d.AppIDs, "15")
            // d.Tables.Add(d.DimStyles, "16")

            TextStyle ts = new();
            ts.FontName = "romans";
            ts.name = "standard";
            ts.sFont_3 = "romans";
            ts.FixedH_40 = 0.10;
            ts.Id = 23;
            d.TextStyles.Add(ts, ts.name);
            d.CurrTextStyle = ts;

            d.id = UniqueId();

            return d;

        }

        public static bool ImportPAT(string sFile)
        {

            // *ACAD_ISO14W100, dashed triplicate-dotted line
            // 0, 0,0, 0,5, 12,-3,.5,-3,.5,-6.5
            // 0, 0,0, 0,5, -22,.5,-3
            // *ACAD_ISO15W100, double-dashed triplicate-dotted line
            // 0, 0,0, 0,5, 12,-3,12,-3,.5,-10
            // 0, 0,0, 0,5, -33.5,.5,-3,.5,-3
            // ;;
            // ;; en

            File f;
            string s;
            string s2;
            Pattern p;
            HatchPattern hp;
            string[] sp;
            int i;

            f = File.Open(sfile, FileMode.Open, FileAccess.Read);

            do {
                f.ReadLine(s);

                s = Replace(s, "\r", "");

                s2 = Left(s, 1);
                if (s2 == ";") // es un comentario, se ignora
                {

                } // es un nuevo patron
                else if (s2 == "*")
                {

                    // si tenia un patron anterior, lo guardo
                    if (hp)
                    {
                        Gcd.HatchPatterns.Add(hp, hp.name);
                    }
                    s = Mid(s, 2);
                    hp = new HatchPattern();
                    sp = Split(s, ",");
                    hp.name = sp[0];
                    hp.description = sp[1];

                }
                else if (s != "")
                {
                    p = new Pattern();
                    s = Replace(s, " ", "");
                    sp = Split(s, ",");
                    p.AngleDeg = Cdouble(sp[0]);
                    p.BaseX = Cdouble(sp[1]);
                    p.BaseY = Cdouble(sp[2]);
                    p.OffsetX = Cdouble(sp[3]);
                    p.OffsetY = Cdouble(sp[4]);
                    for (i = 5; i <= sp.Length - 1; i + 1)
                    {
                        if (Left(sp[i], 1) == "-")
                        {
                            sp[i] = "-0" + Mid(sp[i], 2);
                        }
                        else
                        {
                            sp[i] = "0" + sp[i];
                        }
                        p.DashLength.Add(Cdouble(sp[i]));
                    }
                    hp.patterns.Add(p);

                }

            } while (!f.EOF);

        }

        // public static  Sub DigestInserts()
        //
        //     Dim s As Sheet
        //     Dim e, e2 As Entity
        //     Dim cParts As Dictionary
        //
        //     For Each s In Drawing.Sheets
        //         For Each e In s.Entities
        //             //Debug e.Gender
        //             If e.Gender = cadInsert.Gender Or InStr(e.Gender, "DIMENSION") > 0 Then
        //                 //Debug e.Gender
        //                 For Each e2 In cadInsert.InsertParts(e, InStr(e.Gender, "DIMENSION") > 0)
        //
        //                     cParts.Add(e2, e2.Id)
        //                 Next
        //             Endif
        //         Next
        //         For Each e2 In cParts
        //
        //             s.Entities.Add(e2, e2.Id)
        //         Next
        //         cParts.Clear
        //     Next
        //
        // End
        public static void ResetChronograph()
        {


            Chronos = DateTime.Now.Ticks;

        }

        // Debug hacia texarea con numero indefinido de parámetros
        public static void debugInfo(string txt, bool MismaLineaAnterior = false, bool forzar = false, bool LogToFile = false, bool debugtime = false)
        {


            // fmain.txtCmd.Text = ": " & txt
            // fmain.txtcmd.Tag = "true"

            if (debugtime)
            {
                txt = " : " + txt + " in " + string.Format("{0:0.00000}", (DateTime.Now.Ticks - Chronos)) + Environment.NewLine;
            }
            else
            {
                txt = ": " + txt + gb.CrLf;
            }

            if (LogToFile)
            {
                System.IO.File.AppendAllText(main.MyLog,
                    DateTime.Now.ToString("HH:mm:ss") + " -> " + txt + Environment.NewLine);
            }

    if (MismaLineaAnterior)
            {
                fdebug.txtDebug.Undo;

            } // no voy a loguear lineas repetidas
            else
            {
                //hlog(txt)
                Chronos = Timer;
            }

            fdebug.txtDebug.Insert(txt);
            fDebug.txtDebug.EnsureVisible;

            // txt = ": "
            // txtDebug.Insert(txt)
            //txtDebug.EnsureVisible

            //txtDebug.Insert(": ")
            //lastPos = txtDebug.Length
            // en caso que el ususario tenga cerrada esta ventana, le muestro la linea en
            //lblAyudaRapida.text = Left$(txt, -2)

            //itsTime = false
            if (forzar)
            {
                fdebug.txtDebug.Refresh;
                // Wait 0;
            }

            Utils.doevents;

        }

        // Regenera las Id de todo el grafico
        public static void RegenID(Drawing d)
        {


            d.HandSeed = 0;

            // Bloques
            foreach (var b in d.Blocks)
            {
                b.id = NewId();
            }

            foreach (var v in d.Viewports)
            {
                v.Id = NewId();
            }

        }
    }
}
