using Gaucho;
class cadZoomW
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
const string Gender = "ZOOMW";

public static bool Start(Variant ElemToBuild, int _Mode= 0)
    {

     // Modes:
     //       0 = Move, all points in the element must be selected, or click on it.
     //       1 = Stretch, selection may be partial, each element is called to see if the support stretching

    this.Mode = _Mode;
    this.PoiChecking = false;
    gcd.flgSearchingAllowed = false;

}

public static void MouseDown()
    {


    if ( Mouse.Left )
    {
        this.SelStartX = mouse.X;
        this.SelStartY = mouse.Y;
        this.SelEndX = this.SelStartX;
        this.SelEndy = this.SelStartY;

        this.SelStartXr = gcd.Xreal(Me.SelStartX);
        this.SelStartYr = gcd.Yreal(Me.SelStartY);

        this.Active = true;
    }

}

public static void MouseUp()
    {


    this.SelEndX = mouse.x;
    this.SelEndy = mouse.Y;
    this.Active = false;

     // corrijo para start<end
    if ( Me.SelStartX > Me.SelEndX ) Swap this.SelStartX, this.SelEndX;
    if ( Me.SelStartY < Me.SelEndy ) Swap this.SelStartY, this.SelEndy; // this is FLIPPED

     // Paso a coordenadas reales
    this.SelStartXr = gcd.Xreal(Me.SelStartX);
    this.SelStartYr = gcd.Yreal(Me.SelStartY);
    this.SelEndXr = gcd.Xreal(Me.SelEndX);
    this.SelEndyr = gcd.Yreal(Me.SelEndy);

     // veo si el rectangulo es suficientemente grande como para representar una seleccion por rectangulo
    if ( (Me.SelEndX - Me.SelStartX + (-Me.SelEndy + Me.SelStartY)) < 10 ) // es un rectangulo minusculo
    {

        DrawingAIds.ErrorMessage = ("Window is too small");

    }
         // engañamos a estas vars

        gcd.Drawing.Xmayor = this.SelEndXr;
        gcd.Drawing.Xmenor = this.SelStartXr;

        gcd.Drawing.Ymayor = this.SelEndYr;
        gcd.Drawing.Ymenor = this.SelStartYr;

        cadZoomE.Start(, 1);
        this.Finish;

    }

}

public static void MouseMove()
    {


    this.SelEndX = mouse.x;

    this.SelEndy = mouse.Y;

    this.SelEndXr = gcd.Xreal(Me.SelEndX);
    this.SelEndYr = gcd.Yreal(Me.SelEndY);

    gcd.Redraw;

}

public static void Draw() // esta rutina es llamada por FCAD en el evento DrawingArea_Draw
    {

     // por ultimo, y para que se vea arriba, la seleccion

    if ( ! Me.Active ) return;

    double[] xyStart ;         
    double[] xyEnd ;         

    glx.Rectangle2D(Me.SelStartXr, this.SelStartYr, this.SelEndXr - this.SelStartXr, this.SelEndYr - this.SelStartYr,,,,, Color.DarkBlue, 1,, 2);

}

public static void Finish()
    {


    gcd.clsJob = gcd.clsJobPrevious;
    gcd.clsJobPrevious = this;

    DrawingAIds.CleanTexts;

    this.Active = false;

    gcd.flgPosition = true;
    gcd.flgSearchingAllowed = true;

}

}