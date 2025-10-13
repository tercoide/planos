using Gtk;
using System;
using System.Collections.Generic;

namespace Gaucho
{
    public class MainWindow : Gtk.Window
    {
        public MainWindow() : base()
        {
            // Set window properties
            Title = "Planos - GTK4 Application";
            SetDefaultSize(800, 600);
            
            // Connect the close request to quit the application
            OnCloseRequest += HandleCloseRequest;

            // Create a vertical box container
            var vbox = Box.New(Orientation.Vertical, 5);
            vbox.SetMarginTop(10);
            vbox.SetMarginBottom(10);
            vbox.SetMarginStart(10);
            vbox.SetMarginEnd(10);

            // Create a simple label
            var label = Label.New("Welcome to Planos CAD Application");
            vbox.Append(label);

            // Create buttons for CAD operations
            var buttonBox = Box.New(Orientation.Horizontal, 5);
            
            var lineButton = Button.NewWithLabel("Draw Line");
            lineButton.OnClicked += (sender, e) => DrawEntity("LINE");
            buttonBox.Append(lineButton);

            var circleButton = Button.NewWithLabel("Draw Circle");
            circleButton.OnClicked += (sender, e) => DrawEntity("CIRCLE");
            buttonBox.Append(circleButton);

            var polygonButton = Button.NewWithLabel("Draw Polygon");
            polygonButton.OnClicked += (sender, e) => DrawEntity("POLYGON");
            buttonBox.Append(polygonButton);

            vbox.Append(buttonBox);

            // Create a text view for output
            var scrolledWindow = ScrolledWindow.New();
            var textView = TextView.New();
            textView.Buffer.Text = "Click buttons to draw entities...\n";
            scrolledWindow.SetChild(textView);
            vbox.Append(scrolledWindow);

            // Set the container as the window content
            SetChild(vbox);

            // Show the window
            Show();
        }

        private void DrawEntity(string entityType)
        {
            Console.WriteLine($"GTK: Drawing {entityType}");
            
            // Here you can integrate with your existing drawing logic
            try
            {
                Dictionary<string, IEntity> cad = new Dictionary<string, IEntity>();
                cad.Add("LINE", new cadLine());
                cad.Add("CIRCLE", new cadCircle());

                if (cad.ContainsKey(entityType))
                {
                    cad[entityType].Draw();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error drawing {entityType}: {ex.Message}");
            }
        }

        private bool HandleCloseRequest()
        {
            Application.Quit();
            return false; // Allow the window to close
        }
    }
}