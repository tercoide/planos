// Gambas class file

// GambasCAD
// A simple CAD made in Gambas
//
// Copyright (C) Ing Martin P Cristia
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General public License as published by
// the Free Software Foundation; either version 3 of the License, or
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
using Gaucho;

public class Grip
{
    public Entity? AsociatedEntity= new();                //// its a reference
                                                    //public AsociatedNewEntity As Entity             //// una entidad clonada para poder ser editada
    public int AsociatedPoint;            //// el punto al que se asocia este grip, cada clase lo sabe porque es la que genera el grip
    public int AsociatedGrip;             //// otro grip que se ve afectado con este grip
    public bool DrawLineToAsociatedGrip = false;
    public float X;       //// punto del grip
    public float Y;
    public float Xr;      //// punto de referencia
    public float Yr;

    // public Xn As Float      //// punto nuevo
    // public Yn As Float

// FIXME
    // public double Tolerance = Gcd.Metros(8);
    public int Action;            //// 0-mover 1-rotar 2-estirar
    public string ToolTip = "";
// public Picture Icon;
public int Shape;             //// 0-Rectangulo 1-Rombo 2-Circulo 3-Triangulo N 4-Triangulo S 5-Triangulo W 6-Triangulo W
public System.Drawing.Color iColor = System.Drawing.Color.Blue;
public bool Filled = true;
public System.Drawing.Color iFillColor = System.Drawing.Color.LightCyan;
public System.Drawing.Color iFillColor2 = System.Drawing.Color.Blue;
public int glList;
public float Value;     // para los grips que asumen un valor
public int Group;     // para los RadioButton
                            // public Toggle As Boolean    // para los
public bool Rotate;
}