// Gambas class file

// Tool maintained by Terco
//
// Copyright (C) Ing Martin P Cristia
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General public License as published by`
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General public License for more details.
//
// You should have received a copy of the GNU General public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor,
// Boston, MA  02110-1301  USA

//===============================DOCUMENT==============================================
// datos obtenidos de la lectura de los archivos DWG, DXF y de dibujar
// ver http://entercad.ru/acadauto.en/
// namespace Gaucho;
public class Drawing
{

    public Headers Headers = new Headers();
    // public CadClasses As New CadClass[]               //// de objetos classes, como no tienen handle no es una coleccion

    // Tables
    // public Tables As New Collection // aqui estaran las siguientes tablas

    // public AppIDs As New Collection
    public Dictionary<string, DimStyle> DimStyles = new Dictionary<string, DimStyle>();
    public Dictionary<string, MLineStyle> MLineStyles = new Dictionary<string, MLineStyle>();
    public Dictionary<string, TextStyle> TextStyles = new Dictionary<string, TextStyle>();
    public Dictionary<string, View> Views = new Dictionary<string, View>();
    public Dictionary<string, Viewport> Viewports = new Dictionary<string, Viewport>();
    public Dictionary<string, UCS> UCSs = new Dictionary<string, UCS>();
    public Dictionary<string, LineType> LineTypes = new Dictionary<string, LineType>();
    public Dictionary<string, Layer> Layers = new Dictionary<string, Layer>();
    //// de objetos Layer

    // Blocks of entities
    //public Block_Record As New Collection          //// bloques reutilizables en este grafico
    //public Blocks As New Collection                //// bloques reutilizables en este grafico
    public Dictionary<string, Block> Blocks = [];                //// bloques reutilizables en este grafico
                                                                 // public Dictionary<string, Insert> Inserts = new Dictionary<string, Insert>();               //// bloques insertados en este grafico, incluyen los de las Dimensiones
                                                                 // public Dictionary<string, Hatch> Hatchs = new Dictionary<string, Hatch>();                //// Datos de Hatch usados por HatchBuilder
                                                                 // public Dictionary<string, Dim> Dims = new Dictionary<string, Dim>();                   //// Bloques de dimensiones

    // Entidades
    public Dictionary<string, DictEntry> Dictionaries = new();
    //Objetos
    // public Objects As New Collection

    public Dictionary<string, Sheet> Sheets = []; // of handle, Sheet

    // //================================================================================================================
    // public IDs As New Collection    // Nuevo 2023: la idea de esta coleccion es acumular IDs aca a medida que se crean
    // //================================================================================================================

    // USAGE VARS
    public bool FirstTime = true;
    public bool DrawingReady = false;
    public bool DrawNeedsUpdate = false;                   //// true on mouse, key or tool events
    public int TimerTic = 0;                      //// this reaches 1024 and then zeroes, at 33Hz
    public int nDraws = 0;
    public int nDrawsRequired = 0;
    public string id = "";
    public Entity? LastEntity;
    public int Lastid;
    public int LastHatch;
    public string LastHatchFile = "";
    public string LastHatchPattern = "";
    public double LastScale = 1.0F;
    public double LastAngle = 0.0F;
    public Layer? LastLayer; // in model
    public double Xmenor = 1E100D;
    public double Xmayor = -1E100D;
    public double Ymenor = 1E100D;
    public double Ymayor = -1E100D;
    public string FileName = "";

    public bool RequiresFileRename = true;   // para dibujos nuevo o formatos no soportados para save
    public bool RequiresSaving = false;      // cuando haya cambios en el dibujo q guardar
    public bool Has3dEntities = false;
    // Current drawing vars
    //depre public Entities As Collection                   //// apunta a la lista de entidades del objeto actual
    public List<Entity> EntitiesVisibles = [];

    public Sheet Sheet;                           //// o sea Model, Paper1, Paper2, etc
    public Sheet? Model;
    public Layer? CommonLayer;
    public Layer? CurrLayer;
    public int CurrColor;
    public LineType? CurrLineType;
    public double CurrLineWt;                               //// in mm
    public int CurrBlockLineWt = -3;                   //// config.deflinewt
    public DimStyle? CurrDimStyle;
    public TextStyle? CurrTextStyle;
    public PrintStyle? CurrPrintStyle;
    public int HandSeed = 128 * 128;                   //// Last available handle for this drawing

    //=======================cosas temporales que no se guardan con el grafico=================
    // public Undo uUndo = new Undo();

    // interaccion del mouse en pantalla
    // public LastPoint[] LastPoint = new LastPoint[];                 //// ultimo punto marcado o null si no existe
    public double[] iEntity = [0, 0, 0];                  //// las coordenadas del punto encontrado y el tipo de punto
    public Entity? HoveredEntity;                 //// La entidad quee esta debajo del mouse
    public Entity? HoveredInsert;                  //// Si la entidad pertenece a un inserto, es este
    public List<Entity> HoveredEntities = [];          //// Las entidades que estan debajo del mouse
    public Entity? HoveredEntityPrevious;          //// La entidad que estaba debajo del mouse antes de la ultima detectada

    public Entity? eLastEntity;                       //// ultima entidad encontrada
    public Entity? LastDimension;                      //// guardo este parametro para hacer las dimensiones continuadas

    public bool flgShowPOIinfo = true;

    public int EntitySearchMode = 0;  // 0 = no search; 1 = search entity ; 2 = search POI

    // Grid
    public bool GridActive;
    public int GridStyle = 0;                 //// 0=Dots 1=Lines 2=Dots with major 3=Lines with major
    public double GridMinorSpacing = 1;
    public double GridMidSpacing = 5;
    public double GridMajorSpacing = 10;
    public double GridCurentSpacing = 1;

    // OpenGl stuff
    // public GlAreaInuse As GLArea
    //depre public GlListAllEntities As Integer
    public int GlListEntitiesSelected;
    public int GlListGrid;

    public Drawing()
    {
        // inicializo las tablas basicas
        Sheet = new Sheet();

        return;

    }


}