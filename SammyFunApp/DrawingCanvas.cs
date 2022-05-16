using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace SammyFunApp
{
    public class DrawingCanvas : System.Windows.Controls.Canvas
    {
        private bool _leftMouseButtonClicked = false;

        public Color PenColor { get; set; }
        public int PenThickness { get; set; }

        public DrawingCanvas()
        {
            MouseLeftButtonDown += DrawingCanvas_MouseDown1;
            MouseLeftButtonUp += DrawingCanvas_MouseUp1;
            MouseMove += DrawingCanvas_MouseMove1;

            Background = new SolidColorBrush(Colors.White);
            PenColor = Colors.Black;
            PenThickness = 2;

        }

        private Point _lastPoint = new Point();
        private void DrawingCanvas_MouseMove1(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (!_leftMouseButtonClicked) return;

            var currentPoint = e.GetPosition(this);

            var myLine = new Line();
            myLine.Stroke = new SolidColorBrush(PenColor);
            myLine.X1 = _lastPoint.X;
            myLine.X2 = currentPoint.X;
            myLine.Y1 = _lastPoint.Y;
            myLine.Y2 = currentPoint.Y;
            myLine.VerticalAlignment = VerticalAlignment.Center;
            myLine.StrokeEndLineCap = PenLineCap.Round;
            myLine.StrokeStartLineCap = PenLineCap.Round;
            myLine.StrokeThickness = PenThickness;

            this.Children.Add(myLine);

            _lastPoint = currentPoint;
        }

        private void DrawingCanvas_MouseUp1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _leftMouseButtonClicked = false;
        }

        private void DrawingCanvas_MouseDown1(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            _lastPoint = e.GetPosition(this);
            _leftMouseButtonClicked = true;
        }    

        public void ClearDrawing()
        {
            this.Children.Clear();
        }

    }
}
