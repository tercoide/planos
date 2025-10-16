using Gaucho;
public class Headers
{
 // Gambas class file

 // Adapted from:
 // © 2021 Autodesk Inc. All rights reserved
 // Except where otherwise noted, this work is licensed under a Creative Commons Attribution-NonCommercial-ShareAlike 3.0 Unported License. Please see the Autodesk Creative Commons FAQ for more information.

public int ACADMAINTVER = 0;  //Maintenance version number (should be ignored)
public string ACADVER = "AC1024";   //The AutoCAD drawing database version number:  AC1006 = R10  AC1009 = R11 and R12  AC1012 = R13  AC1014 = R14  AC1015 = AutoCAD 2000  AC1018 = AutoCAD 2004  AC1021 = AutoCAD 2007  AC1024 = AutoCAD 2010  AC1027 = AutoCAD 2013  AC1032 = AutoCAD 2018
public double ANGBASE = 0;  //Angle 0 direction
public int ANGDIR = 0;  //1 = Clockwise angles  0 = Counterclockwise angles
public int ATTMODE = 1;  //Attribute visibility:  0 = None  1 = Normal  2 = All
public int AUNITS = 0; //Units format for angles
public int AUPREC = 0;    //Units precision for angles
public double CECOLOR = 256;  //Current entity color number:  0 = BYBLOCK; 256 = BYLAYER
public double CELTSCALE = 1;   //Current entity linetype scale
public string CELTYPE = "ByLayer";  //Entity linetype name, or BYBLOCK or BYLAYER
public int CELWEIGHT = -1;   //Lineweight of new objects
public int CEPSNID = 0;  //Plotstyle handle of new objects; if CEPSNTYPE is 3, then this value indicates the handle
public int CEPSNTYPE = 0;   //Plot style type of new objects:  0 = Plot style by layer  1 = Plot style by block  2 = Plot style by dictionary default  3 = Plot style by object ID/handle
public double CHAMFERA = 10;  //First chamfer distance
public double CHAMFERB = 10;   //Second chamfer distance
public double CHAMFERC = 20;   //Chamfer length
public double CHAMFERD = 0;  //Chamfer angle
public string CLAYER = "0";  //Current layer name
public int CMLJUST = 0;  //Current multiline justification:  0 = Top; 1 = Middle; 2 = Bottom
public double CMLSCALE = 1;  //Current multiline scale
public string CMLSTYLE = "standard";  //Current multiline style name
public int CSHADOW = 0;   //Shadow mode for a 3D object:  0 = Casts and receives shadows  1 = Casts shadows  2 = Receives shadows  3 = Ignores shadows  Note: Starting with AutoCAD 2016-based products, this variable is obsolete but still supported for backwards compatibility.
public int DIMADEC = 0;   //Number of precision places displayed in angular dimensions
public int DIMALT = 0;  //Alternate unit dimensioning performed if nonzero
public int DIMALTD = 2;  //Alternate unit decimal places
public double DIMALTF = 1;  //Alternate unit scale factor
public double DIMALTRND = 0;  //Determines rounding of alternate units
public int DIMALTTD = 2;   //Number of decimal places for tolerance values of an alternate units dimension
public int DIMALTTZ = 0;   //Controls suppression of zeros for alternate tolerance values:  0 = Suppresses zero feet and precisely zero inches  1 = Includes zero feet and precisely zero inches  2 = Includes zero feet and suppresses zero inches  3 = Includes zero inches and suppresses zero feet  To suppress leading or trailing zeros, add the following values to one of the preceding values:  4 = Suppresses leading zeros  8 = Suppresses trailing zeros
public int DIMALTU = 2;   //Units format for alternate units of all dimension style family members except angular:  1 = Scientific  2 = Decimal  3 = Engineering  4 = Architectural (stacked)  5 = Fractional (stacked)  6 = Architectural  7 = Fractional  8 = Operating system defines the decimal separator and number grouping symbols
public int DIMALTZ = 0;   //Controls suppression of zeros for alternate unit dimension values:  0 = Suppresses zero feet and precisely zero inches  1 = Includes zero feet and precisely zero inches  2 = Includes zero feet and suppresses zero inches  3 = Includes zero inches and suppresses zero feet  4 = Suppresses leading zeros in decimal dimensions  8 = Suppresses trailing zeros in decimal dimensions  12 = Suppresses both leading and trailing zeros
public string DIMAPOST = "";  //Alternate dimensioning suffix
public int DIMASO = 1;  //1 = Create associative dimensioning  0 = Draw individual entities  Note: Obsolete; see $DIMASSOC.
public int DIMASSOC = 2;   //Controls the associativity of dimension objects  0 = Creates exploded dimensions; there is no association between elements of the dimension, and the lines, arcs, arrowheads, and text of a dimension are drawn as separate objects  1 = Creates non-associative dimension objects; the elements of the dimension are formed into a single object, and if the definition point on the object moves, then the dimension value is updated  2 = Creates associative dimension objects; the elements of the dimension are formed into a single object and one or more definition points of the dimension are coupled with association points on geometric objects
public double DIMASZ = 1;  //Dimensioning arrow size
public int DIMATFIT = 2;   //Controls dimension text and arrow placement when space is not sufficient to place both within the extension lines:  0 = Places both text and arrows outside extension lines  1 = Moves arrows first, then text  2 = Moves text first, then arrows  3 = Moves either text or arrows, whichever fits best  AutoCAD adds a leader to moved dimension text when DIMTMOVE is set to 1
public int DIMAUNIT = 0;   //Angle format for angular dimensions:  0 = Decimal degrees  1 = Degrees/minutes/seconds;  2 = Gradians  3 = Radians  4 = Surveyor//s units
public int DIMAZIN = 0;  //Controls suppression of zeros for angular dimensions:  0 = Displays all leading and trailing zeros  1 = Suppresses leading zeros in decimal dimensions  2 = Suppresses trailing zeros in decimal dimensions  3 = Suppresses leading and trailing zeros
public string DIMBLK = "OBLIQUE";  //Arrow block name
public string DIMBLK1 = "";   //First arrow block name
public string DIMBLK2 = "";  //Second arrow block name
public double DIMCEN = 0.1d;  //Size of center mark/lines
public int DIMCLRD = 0;  //Dimension line color:  range is 0 = BYBLOCK; 256 = BYLAYER
public int DIMCLRE = 0;   //Dimension extension line color:  range is 0 = BYBLOCK; 256 = BYLAYER
public int DIMCLRT = 0;   //Dimension text color:  range is 0 = BYBLOCK; 256 = BYLAYER
public int DIMDEC = 2;  //Number of decimal places for the tolerance values of a primary units dimension
public double DIMDLE = 0;   //Dimension line extension
public double DIMDLI = 0.001d;  //Dimension line increment
public int DIMDSEP = 44;   //Single-character decimal separator used when creating dimensions whose unit format is decimal
public double DIMEXE = 0.0005d;  //Extension line extension
public double DIMEXO = 0.0001d;  //Extension line offset
public double DIMFRAC = 0d;  //Scale factor used to calculate the height of text for dimension fractions and tolerances. AutoCAD multiplies DIMTXT by DIMTFAC to set the fractional or tolerance text height.
public double DIMGAP = 0d;  //Dimension line gap
public int DIMJUST = 0;   //Horizontal dimension text position:  0 = Above dimension line and center-justified between extension lines  1 = Above dimension line and next to first extension line  2 = Above dimension line and next to second extension line  3 = Above and center-justified to first extension line  4 = Above and center-justified to second extension line
public string DIMLDRBLK = "ARROW";   //Arrow block name for leaders
public double DIMLFAC = 1d;  //Linear measurements scale factor
public int DIMLIM = 0;   //Dimension limits generated if nonzero
public int DIMLUNIT = 1;   //Sets units for all dimension types except Angular:  1 = Scientific  2 = Decimal  3 = Engineering  4 = Architectural  5 = Fractional  6 = Operating system
public int DIMLWD = -3;   //Dimension line lineweight:  -3 = Standard  -2 = ByLayer  -1 = ByBlock  0-211 = an integer representing 100th of mm
public int DIMLWE = -3;   //Extension line lineweight:  -3 = Standard  -2 = ByLayer  -1 = ByBlock  0-211 = an integer representing 100th of mm
public string DIMPOST = "";   //General dimensioning suffix
public double DIMRND = 0;  //Rounding value for dimension distances
public int DIMSAH = 0;   //Use separate arrow blocks if nonzero
public double DIMSCALE = 1;   //Overall dimensioning scale factor
public int DIMSD1 = 0;   //Suppression of first extension line:  0 = Not suppressed  1 = Suppressed
public int DIMSD2 = 0;  //Suppression of second extension line:  0 = Not suppressed  1 = Suppressed
public int DIMSE1 = 0;   //First extension line suppressed if nonzero
public int DIMSE2 = 0;  //Second extension line suppressed if nonzero
public int DIMSHO = 1;  //1 = Recompute dimensions while dragging  0 = Drag original image
public int DIMSOXD = 0;   //Suppress outside-extensions dimension lines if nonzero
public string DIMSTYLE = "standard";   //Dimension style name
public int DIMTAD = 1;  //Text above dimension line if nonzero
public int DIMTDEC = 2;   //Number of decimal places to display the tolerance values
public double DIMTFAC = 1;  //Dimension tolerance display scale factor
public int DIMTIH = 0;  //Text inside horizontal if nonzero
public int DIMTIX = 0;  //Force text inside extensions if nonzero
public double DIMTM = 0;  //Minus tolerance
public int DIMTMOVE = 0;   //Dimension text movement rules:  0 = Moves the dimension line with dimension text  1 = Adds a leader when dimension text is moved  2 = Allows text to be moved freely without a leader
public int DIMTOFL = 0;   //If text is outside the extension lines, dimension lines are forced between the extension lines if nonzero
public int DIMTOH = 0;  //Text outside horizontal if nonzero
public int DIMTOL = 0;  //Dimension tolerances generated if nonzero
public int DIMTOLJ = 1;   //Vertical justification for tolerance values:  0 = Top  1 = Middle  2 = Bottom
public double DIMTP = 0;  //Plus tolerance
public double DIMTSZ = 0;  //Dimensioning tick size:  0 = Draws arrowheads  >0 = Draws oblique strokes instead of arrowheads
public double DIMTVP = 0;  //Text vertical position
public string DIMTXSTY = "standard";  //Dimension text style
public double DIMTXT = 2;  //Dimensioning text height
public int DIMTZIN = 0;   //Controls suppression of zeros for tolerance values:  0 = Suppresses zero feet and precisely zero inches  1 = Includes zero feet and precisely zero inches  2 = Includes zero feet and suppresses zero inches  3 = Includes zero inches and suppresses zero feet  4 = Suppresses leading zeros in decimal dimensions  8 = Suppresses trailing zeros in decimal dimensions  12 = Suppresses both leading and trailing zeros
public int DIMUPT = 0;  //Cursor functionality for user-positioned text:  0 = Controls only the dimension line location  1 = Controls the text position as well as the dimension line location
public int DIMZIN = 0;  //Controls suppression of zeros for primary unit values:  0 = Suppresses zero feet and precisely zero inches  1 = Includes zero feet and precisely zero inches  2 = Includes zero feet and suppresses zero inches  3 = Includes zero inches and suppresses zero feet  4 = Suppresses leading zeros in decimal dimensions  8 = Suppresses trailing zeros in decimal dimensions  12 = Suppresses both leading and trailing zeros
public int DISPSILH = 0;  //Controls the display of silhouette curves of body objects in Wireframe mode:  0 = Off  1 = On
public int DRAGVS = 0;  //Hard-pointer ID to visual style while creating 3D solid primitives. The default value is NULL
public string DWGCODEPAGE = "ANSI_1252";   //Drawing code page; set to the system code page when a new drawing is created, but not otherwise maintained by AutoCAD
public double ELEVATION = 0;   //Current elevation set by ELEV command
public int ENDCAPS = 0;  //Lineweight endcaps setting for new objects:  0 = None  1 = Round  2 = Angle  3 = Square
public double[] EXTMAX =[];          //X, Y, and Z drawing extents upper-right corner (in WCS)

public double[] EXTMIN =[];          //X, Y, and Z drawing extents lower-left corner (in WCS)

public int EXTNAMES = 1;    //Controls symbol table naming:  0 = Release 14 compatibility. Limits names to 31 characters in length. Names can include the letters A to Z, the numerals 0 to 9, and the special characters dollar sign ($), underscore (_), and hyphen (-).  1 = AutoCAD 2000. Names can be up to 255 characters in length, and can include the letters A to Z, the numerals 0 to 9, spaces, and any special characters not used for other purposes by Microsoft Windows and AutoCAD
public double FILLETRAD = 0;   //Fillet radius
public int FILLMODE = 1;   //Fill mode on if nonzero
public string FINGERPRINTGUID = "";          //Set at creation time, uniquely identifies a particular drawing
public int HALOGAP = 0;    //Specifies a gap to be displayed where an object is hidden by another object; the value is specified as a percent of one unit and is independent of the zoom level. A haloed line is shortened at the point where it is hidden when HIDE or the Hidden option of SHADEMODE is used
public string HANDSEED = "10";  //Next available handle
public int HIDETEXT = 1;  //Specifies HIDETEXT system variable:  0 = HIDE ignores text objects when producing the hidden view  1 = HIDE does not ignore text objects
public string HYPERLINKBASE = "";   //Path for all relative hyperlinks in the drawing. If null, the drawing path is used
public int INDEXCTL = 0;   //Controls whether layer and spatial indexes are created and saved in drawing files:  0 = No indexes are created  1 = Layer index is created  2 = Spatial index is created  3 = Layer and spatial indexes are created
public float[] INSBASE = [];          //Insertion base set by BASE command (in WCS)

public int INSUNITS = 6;   //Default drawing units for AutoCAD DesignCenter blocks:  0 = Unitless  1 = Inches  2 = Feet  3 = Miles  4 = Millimeters  5 = Centimeters  6 = Meters  7 = Kilometers  8 = Microinches  9 = Mils  10 = Yards  11 = Angstroms  12 = Nanometers  13 = Microns  14 = Decimeters  15 = Decameters  16 = Hectometers  17 = Gigameters  18 = Astronomical units  19 = Light years  20 = Parsecs  21 = US Survey Feet  22 = US Survey Inch  23 = US Survey Yard  24 = US Survey Mile
public double INTERFERECOLOR = 256;  //Represents the ACI color index of the "interference objects" created during the INTERFERE command. Default value is 1
public int INTERFEREOBJVS = 0;  //Hard-pointer ID to the visual style for interference objects. Default visual style is Conceptual.
public int INTERFEREVPVS = 0;  //Hard-pointer ID to the visual style for the viewport during interference checking. Default visual style is 3d Wireframe.
public int INTERSECTIONCOLOR = 257;   //Specifies the entity color of intersection polylines:  Values 1-255 designate an AutoCAD color index (ACI)  0 = Color BYBLOCK  256 = Color BYLAYER  257 = Color BYENTITY
public int INTERSECTIONDISPLAY = 0;   //Specifies the display of intersection polylines:  0 = Turns off the display of intersection polylines  1 = Turns on the display of intersection polylines
public int JOINSTYLE = 0;  //Lineweight joint setting for new objects:  0=None  1= Round  2 = Angle  3 = Flat
public int LIMCHECK = 0;  //Nonzero if limits checking is on
public double[]    LIMMAX =[];          //XY drawing limits upper-right corner (in WCS)

public double[]    LIMMIN=[] ;          //XY drawing limits lower-left corner (in WCS)

public double LTSCALE = 0.1;   //Global linetype scale
public int LUNITS = 2;  //Units format for coordinates and distances
public int LUPREC = 0;  //Units precision for coordinates and distances
public int LWDISPLAY = 0;  //Controls the display of lineweights on the Model or Layout tab:  0 = Lineweight is not displayed  1 = Lineweight is displayed
public int MAXACTVP = 64;   //Sets maximum number of viewports to be regenerated
public int MEASUREMENT = 1;  //Sets drawing units:  0 = English  1 = Metric
public string MENU = ".";   //Name of menu file
public int MIRRTEXT = 1;   //Mirror text if nonzero
public int OBSCOLOR = 257;  //Specifies the color of obscured lines. An obscured line is a hidden line made visible by changing its color and linetype and is visible only when the HIDE or SHADEMODE command is used. The OBSCUREDCOLOR setting is visible only if the OBSCUREDLTYPE is turned ON by setting it to a value other than 0.  0 and 256 = Entity color  1-255 = An AutoCAD color index (ACI)
public int OBSLTYPE = 0;  //Specifies the linetype of obscured lines. Obscured linetypes are independent of zoom level, unlike regular AutoCAD linetypes. Value 0 turns off display of obscured lines and is the default. Linetype values are defined as follows:  0 = Off  1 = Solid  2 = Dashed  3 = Dotted  4 = Short Dash  5 = Medium Dash  6 = Long Dash  7 = Double Short Dash  8 = Double Medium Dash  9 = Double Long Dash  10 = Medium Long Dash  11 = Sparse Dot
public int ORTHOMODE = 0;   //Ortho mode on if nonzero
public int PDMODE = 0;  //Point display mode
public double PDSIZE = 0;  //Point display size
public double PELEVATION = 0;   //Current paper space elevation
public double[] PEXTMAX =[];          //Maximum X, Y, and Z extents for paper space

public double[] PEXTMIN =[];          //Minimum X, Y, and Z extents for paper space

public double[] PINSBASE =[];          //Paper space insertion base point

public int PLIMCHECK = 0;  //Limits checking in paper space when nonzero
public double[] PLIMMAX =[];          //Maximum X and Y limits in paper space

public double[] PLIMMIN =[];          //Minimum X and Y limits in paper space

public int PLINEGEN = 0;   //Governs the generation of linetype patterns around the vertices of a 2D polyline:  1 = Linetype is generated in a continuous pattern around vertices of the polyline  0 = Each segment of the polyline starts and ends with a dash
public double PLINEWID = 0;  //Default polyline width
public string PROJECTNAME = "";   //Assigns a project name to the current drawing. Used when an external reference or image is not found on its original path. The project name points to a section in the registry that can contain one or more search paths for each project name defined. Project names and their search directories are created from the Files tab of the Options dialog box
public int PROXYGRAPHICS = 0;   //Controls the saving of proxy object images
public int PSLTSCALE = 1;  //Controls paper space linetype scaling:  1 = No special linetype scaling  0 = Viewport scaling governs linetype scaling
public int PSTYLEMODE = 1;   //Indicates whether the current drawing is in a Color-Dependent or Named Plot Style mode:  0 = Uses named plot style tables in the current drawing  1 = Uses color-dependent plot style tables in the current drawing
public double PSVPSCALE = 0;  //View scale factor for new viewports:  0 = Scaled to fit  >0 = Scale factor (a positive real value)
public string PUCSBASE = "";  //Name of the UCS that defines the origin and orientation of orthographic UCS settings (paper space only)
public string PUCSNAME = "";  //Current paper space UCS name
public double[] PUCSORG =[];          //Current paper space UCS origin
public double[] PUCSORGBACK =[];          //Point which becomes the new UCS origin after changing paper space UCS to BACK when PUCSBASE is set to WORLD
public double[] PUCSORGBOTTOM =[];          //Point which becomes the new UCS origin after changing paper space UCS to BOTTOM when PUCSBASE is set to WORLD
public double[] PUCSORGFRONT =[];          //Point which becomes the new UCS origin after changing paper space UCS to FRONT when PUCSBASE is set to WORLD
public double[] PUCSORGLEFT =[];          //Point which becomes the new UCS origin after changing paper space UCS to LEFT when PUCSBASE is set to WORLD
public double[] PUCSORGRIGHT =[];          //Point which becomes the new UCS origin after changing paper space UCS to RIGHT when PUCSBASE is set to WORLD
public double[] PUCSORGTOP =[];          //Point which becomes the new UCS origin after changing paper space UCS to TOP when PUCSBASE is set to WORLD
public string PUCSORTHOREF = "";  //If paper space UCS is orthographic (PUCSORTHOVIEW not equal to 0), this is the name of the UCS that the orthographic UCS is relative to. If blank, UCS is relative to WORLD
public int PUCSORTHOVIEW = 0;   //Orthographic view type of paper space UCS:  0 = UCS is not orthographic  1 = Top  2 = Bottom  3 = Front  4 = Back  5 = Left  6 = Right
public double[] PUCSXDIR =[];          //Current paper space UCS X axis
public double[] PUCSYDIR =[];          //Current paper space UCS Y axis
public int QTEXTMODE = 0;   //Quick Text mode on if nonzero
public int REGENMODE = 0;    //REGENAUTO mode on if nonzero
public int SHADEDGE = 3;  //Controls the shading of edges:  0 = Faces shaded, edges not highlighted  1 = Faces shaded, edges highlighted in black  2 = Faces not filled, edges in entity color  3 = Faces in entity color, edges in black
public int SHADEDIF = 70;   //Percent ambient/diffuse light; range 1-100; default 70
public double SHADOWPLANELOCATION = 0;  //Location of the ground shadow plane. This is a Z axis ordinate.
public double SKETCHINC = 1;   //Sketch record increment
public int SKPOLY = 0;  //Determines the object type created by the SKETCH command:  0 = Generates lines  1 = Generates polylines  2 = Generates splines
public int SORTENTS = 0;   //Controls the object sorting methods; accessible from the Options dialog box User Preferences tab. SORTENTS uses the following bitcodes:  0 = Disables SORTENTS  1 = Sorts for object selection  2 = Sorts for object snap  4 = Sorts for redraws; obsolete  8 = Sorts for MSLIDE command slide creation; obsolete  16 = Sorts for REGEN commands  32 = Sorts for plotting  64 = Sorts for PostScript output; obsolete
public int SPLINESEGS = 8;  //Number of line segments per spline patch
public int SPLINETYPE = 6;   //SPLINE curve type for PEDIT SPLINE
public int SURFTAB1 = 6; //Number of mesh tabulations in first direction
public int SURFTAB2 = 6;  //Number of mesh tabulations in second direction
public int SURFTYPE = 6;  //Surface type for PEDIT Smooth
public int SURFU = 6;  //Surface density (for PEDIT Smooth) in M direction
public int SURFV = 6;  //Surface density (for PEDIT Smooth) in N direction
public double TDCREATE = 2453292;   //Local date/time of drawing creation (see Special Handling of Date/Time Variables)
public double TDINDWG = 0.7775557D;  //Cumulative editing time for this drawing (see Special Handling of Date/Time Variables)
public double TDUCREATE = 2453292;   //Universal date/time the drawing was created (see Special Handling of Date/Time Variables)
public double TDUPDATE = 2459858;  //Local date/time of last drawing update (see Special Handling of Date/Time Variables)
public double TDUSRTIMER = 2458240;   //User-elapsed timer
public double TDUUPDATE = 2459858;   //Universal date/time of the last update/save (see Special Handling of Date/Time Variables)
public double TEXTSIZE = 10;   //Default text height
public string TEXTSTYLE = "romans";  //Current text style name
public double THICKNESS = 0;  //Current thickness set by ELEV command
public int TILEMODE = 1;   //1 for previous release compatibility mode; 0 otherwise
public double TRACEWID = 1;   //Default trace width
public int TREEDEPTH = 3020;   //Specifies the maximum depth of the spatial index
public string UCSBASE = "";   //Name of the UCS that defines the origin and orientation of orthographic UCS settings
public string UCSNAME = "";  //Name of current UCS
public double[] UCSORG =[];          //Origin of current UCS (in WCS)
public double[] UCSORGBACK =[];          //Point which becomes the new UCS origin after changing model space UCS to BACK when UCSBASE is set to WORLD
public double[] UCSORGBOTTOM =[];          //Point which becomes the new UCS origin after changing model space UCS to BOTTOM when UCSBASE is set to WORLD
public double[] UCSORGFRONT =[];          //Point which becomes the new UCS origin after changing model space UCS to FRONT when UCSBASE is set to WORLD
public double[] UCSORGLEFT =[];          //Point which becomes the new UCS origin after changing model space UCS to LEFT when UCSBASE is set to WORLD
public double[] UCSORGRIGHT =[];          //Point which becomes the new UCS origin after changing model space UCS to RIGHT when UCSBASE is set to WORLD
public double[] UCSORGTOP =[];          //Point which becomes the new UCS origin after changing model space UCS to TOP when UCSBASE is set to WORLD
public string UCSORTHOREF = "";   //If model space UCS is orthographic (UCSORTHOVIEW not equal to 0), this is the name of the UCS that the orthographic UCS is relative to. If blank, UCS is relative to WORLD
public int UCSORTHOVIEW = 0;  //Orthographic view type of model space UCS:  0 = UCS is not orthographic  1 = Top  2 = Bottom  3 = Front  4 = Back  5 = Left  6 = Right
public double[] UCSXDIR =[];          //Direction of the current UCS X axis (in WCS)
public double[] UCSYDIR =[];          //Direction of the current UCS Y axis (in WCS)
public int UNITMODE = 0;   //Low bit set = Display fractions, feet-and-inches, and surveyor//s angles in input format
 // Public USERI1 As New Integer[]   //Five integer variables intended for use by third-party developers
 // Public USERR1 As New Single[]   //Five real variables intended for use by third-party developers
public int USRTIMER = 1;   //Controls the user timer for the drawing:  0 = Timer off  1 = Timer on
public string VERSIONGUID = "";   //Uniquely identifies a particular version of a drawing. Updated when the drawing is modified
public int VISRETAIN = 0;   //Controls the properties of xref-dependent layers:  0 = Don//t retain xref-dependent visibility settings  1 = Retain xref-dependent visibility settings
public int WORLDVIEW = 1;  //Determines whether input for the DVIEW and VPOINT command evaluated as relative to the WCS or current UCS:  0 = Don//t change UCS  1 = Set UCS to WCS during DVIEW/VPOINT
public int XCLIPFRAME = 1;  //Controls the visibility of xref clipping boundaries:  0 = Clipping boundary is not visible  1 = Clipping boundary is visible
public int XEDIT = 1;   //Controls whether the current drawing can be edited in-place when being referenced by another drawing:  0 = Can//t use in-place reference editing  1 = Can use in-place reference editing
public double LASTSAVEDBY = 0;
public double DIMFXL = 1;
public double DIMFXLON = 0;
public double DIMJOGANG = 0.785398163;
public double DIMTFILL = 0;
public double DIMTFILLCLR = 0;
public double DIMARCSYM = 0;
public double DIMLTYPE = 0;
public double DIMLTEX1 = 0;
public double DIMLTEX2 = 0;
public double DIMTXTDIRECTION = 0;
public double SPLFRAME = 0;
public double USERI1 = 0;
public double USERI2 = 0;
public double USERI3 = 0;
public double USERI4 = 0;
public double USERI5 = 0;
public double USERR1 = 0;
public double USERR2 = 0;
public double USERR3 = 0;
public double USERR4 = 0;
public double USERR5 = 0;
public double STYLESHEET = 0;
public double OLESTARTUP = 0;
public double CAMERADISPLAY = 0;
public double LENSLENGTH = 50;
public double CAMERAHEIGHT = 0;
public double STEPSPERSEC = 2;
public double STEPSIZE = 6;
public double __3DDWFPREC ;         
public double PSOLWIDTH = 0.25;
public double PSOLHEIGHT = 4;
public double LOFTANG1 = 1.570796326749;
public double LOFTANG2 = 1.570796326749;
public double LOFTMAG1 = 0;
public double LOFTMAG2 = 0;
public double LOFTPARAM = 7;
public double LOFTNORMALS = 1;
public double LATITUDE = 37.795;
public double LONGITUDE = -122.394;
public double NORTHDIRECTION = 0;
public double TIMEZONE = -8000;
public double LIGHTGLYPHDISPLAY = 0;
public double TILEMODELIGHTSYNCH = 0;
public double CMATERIAL = 0;
public double SOLIDHIST = 0;
public double SHOWHIST = 0;
public double DWFFRAME = 0;
public double DGNFRAME = 0;
public double REALWORLDSCALE = 1;

public int _LASTSAVEDBY = 70;
public int _DIMFXL = 70;
public int _DIMFXLON = 70;
public int _DIMJOGANG = 70;
public int _DIMTFILL = 70;
public int _DIMTFILLCLR = 70;
public int _DIMARCSYM = 70;
public int _DIMLTYPE = 70;
public int _DIMLTEX1 = 70;
public int _DIMLTEX2 = 70;
public int _DIMTXTDIRECTION = 70;
public int _SPLFRAME = 70;
public int _USERI1 = 70;
public int _USERI2 = 70;
public int _USERI3 = 70;
public int _USERI4 = 70;
public int _USERI5 = 70;
public int _USERR1 = 70;
public int _USERR2 = 70;
public int _USERR3 = 70;
public int _USERR4 = 70;
public int _USERR5 = 70;
public int _STYLESHEET = 70;
public int _OLESTARTUP = 70;
public int _CAMERADISPLAY = 70;
public int _LENSLENGTH = 70;
public int _CAMERAHEIGHT = 70;
public int _STEPSPERSEC = 70;
public int _STEPSIZE = 70;
public int _3DDWFPREC = 70;
public int _PSOLWIDTH = 70;
public int _PSOLHEIGHT = 70;
public int _LOFTANG1 = 70;
public int _LOFTANG2 = 70;
public int _LOFTMAG1 = 70;
public int _LOFTMAG2 = 70;
public int _LOFTPARAM = 70;
public int _LOFTNORMALS = 70;
public int _LATITUDE = 70;
public int _LONGITUDE = 70;
public int _NORTHDIRECTION = 70;
public int _TIMEZONE = 70;
public int _LIGHTGLYPHDISPLAY = 70;
public int _TILEMODELIGHTSYNCH = 70;
public int _CMATERIAL = 70;
public int _SOLIDHIST = 70;
public int _SHOWHIST = 70;
public int _DWFFRAME = 70;
public int _DGNFRAME = 70;
public int _REALWORLDSCALE = 70;

public int _ACADMAINTVER = 70;
public int _ACADVER = 1;
public int _ANGBASE = 50;
public int _ANGDIR = 70;
public int _ATTMODE = 70;
public int _AUNITS = 70;
public int _AUPREC = 70;
public int _CECOLOR = 62;
public int _CELTSCALE = 40;
public int _CELTYPE = 6;
public int _CELWEIGHT = 370;
public int _CEPSNID = 390;
public int _CEPSNTYPE = 380;
public int _CHAMFERA = 40;
public int _CHAMFERB = 40;
public int _CHAMFERC = 40;
public int _CHAMFERD = 40;
public int _CLAYER = 8;
public int _CMLJUST = 70;
public int _CMLSCALE = 40;
public int _CMLSTYLE = 2;
public int _CSHADOW = 280;
public int _DIMADEC = 70;
public int _DIMALT = 70;
public int _DIMALTD = 70;
public int _DIMALTF = 40;
public int _DIMALTRND = 40;
public int _DIMALTTD = 70;
public int _DIMALTTZ = 70;
public int _DIMALTU = 70;
public int _DIMALTZ = 70;
public int _DIMAPOST = 1;
public int _DIMASO = 70;
public int _DIMASSOC = 280;
public int _DIMASZ = 40;
public int _DIMATFIT = 70;
public int _DIMAUNIT = 70;
public int _DIMAZIN = 70;
public int _DIMBLK = 1;
public int _DIMBLK1 = 1;
public int _DIMBLK2 = 1;
public int _DIMCEN = 40;
public int _DIMCLRD = 70;
public int _DIMCLRE = 70;
public int _DIMCLRT = 70;
public int _DIMDEC = 70;
public int _DIMDLE = 40;
public int _DIMDLI = 40;
public int _DIMDSEP = 70;
public int _DIMEXE = 40;
public int _DIMEXO = 40;
public int _DIMFRAC = 40;
public int _DIMGAP = 40;
public int _DIMJUST = 70;
public int _DIMLDRBLK = 1;
public int _DIMLFAC = 40;
public int _DIMLIM = 70;
public int _DIMLUNIT = 70;
public int _DIMLWD = 70;
public int _DIMLWE = 70;
public int _DIMPOST = 1;
public int _DIMRND = 40;
public int _DIMSAH = 70;
public int _DIMSCALE = 40;
public int _DIMSD1 = 70;
public int _DIMSD2 = 70;
public int _DIMSE1 = 70;
public int _DIMSE2 = 70;
public int _DIMSHO = 70;
public int _DIMSOXD = 70;
public int _DIMSTYLE = 2;
public int _DIMTAD = 70;
public int _DIMTDEC = 70;
public int _DIMTFAC = 40;
public int _DIMTIH = 70;
public int _DIMTIX = 70;
public int _DIMTM = 40;
public int _DIMTMOVE = 70;
public int _DIMTOFL = 70;
public int _DIMTOH = 70;
public int _DIMTOL = 70;
public int _DIMTOLJ = 70;
public int _DIMTP = 40;
public int _DIMTSZ = 40;
public int _DIMTVP = 40;
public int _DIMTXSTY = 7;
public int _DIMTXT = 40;
public int _DIMTZIN = 70;
public int _DIMUPT = 70;
public int _DIMZIN = 70;
public int _DISPSILH = 70;
public int _DRAGVS = 349;
public int _DWGCODEPAGE = 3;
public int _ELEVATION = 40;
public int _ENDCAPS = 280;
public int _EXTMAX = 10;
public int _EXTMIN = 10;
public int _EXTNAMES = 290;
public int _FILLETRAD = 40;
public int _FILLMODE = 70;
public int _FINGERPRINTGUID = 2;
public int _HALOGAP = 280;
public int _HANDSEED = 5;
public int _HIDETEXT = 290;
public int _HYPERLINKBASE = 1;
public int _INDEXCTL = 280;
public int _INSBASE = 10;
public int _INSUNITS = 70;
public int _INTERFERECOLOR = 62;
public int _INTERFEREOBJVS = 345;
public int _INTERFEREVPVS = 346;
public int _INTERSECTIONCOLOR = 70;
public int _INTERSECTIONDISPLAY = 290;
public int _JOINSTYLE = 280;
public int _LIMCHECK = 70;
public int _LIMMAX = 10;
public int _LIMMIN = 10;
public int _LTSCALE = 40;
public int _LUNITS = 70;
public int _LUPREC = 70;
public int _LWDISPLAY = 290;
public int _MAXACTVP = 70;
public int _MEASUREMENT = 70;
public int _MENU = 1;
public int _MIRRTEXT = 70;
public int _OBSCOLOR = 70;
public int _OBSLTYPE = 280;
public int _ORTHOMODE = 70;
public int _PDMODE = 70;
public int _PDSIZE = 40;
public int _PELEVATION = 40;
public int _PEXTMAX = 10;
public int _PEXTMIN = 10;
public int _PINSBASE = 10;
public int _PLIMCHECK = 70;
public int _PLIMMAX = 10;
public int _PLIMMIN = 10;
public int _PLINEGEN = 70;
public int _PLINEWID = 40;
public int _PROJECTNAME = 1;
public int _PROXYGRAPHICS = 70;
public int _PSLTSCALE = 70;
public int _PSTYLEMODE = 290;
public int _PSVPSCALE = 40;
public int _PUCSBASE = 2;
public int _PUCSNAME = 2;
public int _PUCSORG = 10;
public int _PUCSORGBACK = 10;
public int _PUCSORGBOTTOM = 10;
public int _PUCSORGFRONT = 10;
public int _PUCSORGLEFT = 10;
public int _PUCSORGRIGHT = 10;
public int _PUCSORGTOP = 10;
public int _PUCSORTHOREF = 2;
public int _PUCSORTHOVIEW = 70;
public int _PUCSXDIR = 10;
public int _PUCSYDIR = 10;
public int _QTEXTMODE = 70;
public int _REGENMODE = 70;
public int _SHADEDGE = 70;
public int _SHADEDIF = 70;
public int _SHADOWPLANELOCATION = 40;
public int _SKETCHINC = 40;
public int _SKPOLY = 70;
public int _SORTENTS = 280;
public int _SPLINESEGS = 70;
public int _SPLINETYPE = 70;
public int _SURFTAB1 = 70;
public int _SURFTAB2 = 70;
public int _SURFTYPE = 70;
public int _SURFU = 70;
public int _SURFV = 70;
public int _TDCREATE = 40;
public int _TDINDWG = 40;
public int _TDUCREATE = 40;
public int _TDUPDATE = 40;
public int _TDUSRTIMER = 40;
public int _TDUUPDATE = 40;
public int _TEXTSIZE = 40;
public int _TEXTSTYLE = 7;
public int _THICKNESS = 40;
public int _TILEMODE = 70;
public int _TRACEWID = 40;
public int _TREEDEPTH = 70;
public int _UCSBASE = 2;
public int _UCSNAME = 2;
public int _UCSORG = 10;
public int _UCSORGBACK = 10;
public int _UCSORGBOTTOM = 10;
public int _UCSORGFRONT = 10;
public int _UCSORGLEFT = 10;
public int _UCSORGRIGHT = 10;
public int _UCSORGTOP = 10;
public int _UCSORTHOREF = 2;
public int _UCSORTHOVIEW = 70;
public int _UCSXDIR = 10;
public int _UCSYDIR = 10;
public int _UNITMODE = 70;

public int _USRTIMER = 70;
public int _VERSIONGUID = 2;
public int _VISRETAIN = 70;
public int _WORLDVIEW = 70;
public int _XCLIPFRAME = 290;
public int _XEDIT = 290;


}