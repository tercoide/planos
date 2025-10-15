// Gambas class file

//// Printing styles
//// Esto se guarda en el sistema, no con el archivo.
/// 
public class PrintStyle
{
    public const string Gender = "PRINTSTYLE";
    public string Name = "";                       //// El nombre de este estilo
    public string PrinterName = "";                //// Printer name
    public string ViewName = "";                       //// El nombre de este estilo

    public string handle = "";

    public int PrintArea = 0;                 //// El sector a ser impreso: 0=todo, 1=viewport, 2=window definida por lo siguiente
    public float PrintAreaX0 = 0;                //// graphics area to be printed
    public float PrintAreaY0 = 0;
    public float PrintAreaX1 = 0;
    public float PrintAreaY1 = 0;

    public int ColorStyle = 0;            //// 0=B/W  1=Gray 2=Color
    public float PrintScale = 1;              //// Draw Units/Paper mm
    public float ScalePaper = 1;
    public float ScaleDrawingUnit = 1;
    public int PrintOrientation = 0;      //// 0=LandScape  1=Portrait
    public bool PrintInvert = false;

    public float PrintOffsetX = 0;           //// mm from page left side
    public float PrintOffsetY = 0;           //// mm from top of page
    public int CenteringStyle = 0;            //// 0=Custom, 1=Top Left, 2=Centered

    public string PaperName = "";                  //// una descripcion or ej: A3
    public float PaperSizeW = 210;           //// mm
    public float PaperSizeH = 294;           //// mm

    public float MarginLeft = 0;             //// mm
    public float MarginRigth = 0;            //// mm
    public float MarginTop = 0;              //// mm
    public float MarginBottom = 0;           //// mm

// 0 Object name (PLOTSETTINGS)
//
// 5 Handle
//
// 102 Start of persistent reactors group; always “{ACAD_REACTORS”
//
// 330 Soft-pointer ID/handle to owner dictionary
//
// 102 End of persistent reactors group, always “}”
//
// 330 Soft-pointer ID/handle to owner object
//
// 100 Subclass marker (AcDbPlotSettings)
//
// 1 Page Setup name
//
// 2 Name of system printer or plot configuration file
//
// 4 Paper size
//
// 6 Plot view name
//
// 40 Size, in millimeters, of unprintable margin on left side of paper
//
// 41 Size, in millimeters, of unprintable margin on bottom of paper
//
// 42 Size, in millimeters, of unprintable margin on right side of paper
//
// 43 Size, in millimeters, of unprintable margin on top of paper
//
// 44 Plot paper size: physical paper width in millimeters
//
// 45 Plot paper size: physical paper height in millimeters
//
// 46 Plot origin: X value of origin offset in millimeters
//
// 47 Plot origin: Y value of origin offset in millimeters
//
// 48 Plot window area: X value of lower-left window corner                      OJO ACA PARECE HABER UN ERROR EN LA DOC
//
// 49 Plot window area: Y value of upper-right window corner
//
// 140 Plot window area: X value of lower-left window corner
//
// 141 Plot window area: Y value of upper-right window corner
//
// 142 Numerator of custom print scale: real world (paper) units
//
// 143 Denominator of custom print scale: drawing units
//
// 70 Plot layout flag:
//
// 1 = PlotViewportBorders
// 2 = ShowPlotStyles
// 4 = PlotCentered
// 8 = PlotHidden
// 16 = UseStandardScale
// 32 = PlotPlotStyles
// 64 = ScaleLineweights
// 128 = PrintLineweights
// 512 = DrawViewportsFirst
// 1024 = ModelType
// 2048 = UpdatePaper
// 4096 = ZoomToPaperOnUpdate
// 8192 = Initializing
// 16384 = PrevPlotInit
//
// 72 Plot paper units: 0 = Plot in inches 1 = Plot in millimeters 2 = Plot in pixels
//
// 73 Plot rotation: 0 = No rotation 1 = 90 degrees counterclockwise 2 = Upside-down 3 = 90 degrees clockwise
//
// 74 Plot type (portion of paper space to output to the media): 0 = Last screen display 1 = Drawing extents 2 = Drawing limits 3 = View specified by code 6 4 =window specified by codes 48, 49, 140, and 141 5 = Layout information
//
// 7 Current style sheet
//
// 75 Standard scale type:
//
// 0 = Scaled to Fit
// 1 = 1/128"=1//; 2 = 1/64"=1//; 3 = 1/32"=1//
// 4 = 1/16"=1//; 5 = 3/32"=1//; 6 = 1/8"=1//
// 7 = 3/16"=1//; 8 = 1/4"=1//; 9 = 3/8"=1//
// 10 = 1/2"=1//; 11 = 3/4"=1//; 12 = 1"=1//
// 13 = 3"=1//; 14 = 6"=1//; 15 = 1//=1//
// 16= 1:1 ; 17= 1:2; 18 = 1:4; 19 = 1:8; 20 = 1:10; 21= 1:16
// 22 = 1:20; 23 = 1:30; 24 = 1:40; 25 = 1:50; 26 = 1:100
// 27 = 2:1; 28 = 4:1; 29 = 8:1; 30 = 10:1; 31 = 100:1; 32 = 1000:1

// 76 ShadePlot mode: 0 = As Displayed 1 = Wireframe 2 = Hidden 3 = Rendered
//
// 77 ShadePlot resolution level: 0 = Draft 1 = Preview 2 = Normal 3 = Presentation 4 = Maximum 5 = Custom
//
// 78 ShadePlot custom DPI: Valid range: 100 to 32767
//
// Only applied when the ShadePlot resolution level is set to 5 (Custom)
//
// 147 A floating point scale factor that represents the standard scale value specified in code 75
//
// 148 Paper image origin: X value
//
// 149 Paper image origin: Y value
//
// 333 ShadePlot ID/Handle (optional)
}