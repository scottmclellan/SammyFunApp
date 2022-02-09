using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Speech.Synthesis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using SammyFunApp.Utils;
using static SammyFunApp.Utils.ResourceHelper;

namespace SammyFunApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);
        }

        private FormBorderStyle onLoadBorderStyle;
        private Rectangle onLoadBounds;
        private ColourDatabase _colourDB;
        private List<PeekabooBuddy> _peekabooBuddies = new List<PeekabooBuddy>();
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Sammy's Paint Shop";          

            onLoadBorderStyle = this.FormBorderStyle;
            onLoadBounds = this.Bounds;
            this.WindowState = FormWindowState.Maximized;

            _colourDB = ColourDatabase.Populate(Path.Combine(Application.StartupPath, @"Resources\colourdb.json"));

            var cur = CursorHelper.GetColourCursor(Color.Black, CursorHelper.AppCursor.Pen);

            this.Cursor = cur;

            int day =  int.Parse(DateTime.Now.ToString("dd"));
            string suff = day == 1 ? "st" : day == 2 ? "nd" : day == 3 ? "rd" : "th";

            SpeechHelper.Speak($"Hey Sammy, it is {DateTime.Now.ToString("MMMMM")} {DateTime.Now.ToString("dd")}{suff}. Welcome to the paint shop");

            ToolStripButton[] buttons = {
                new PaintColourButton("Red",null, CustomToolStripButtonOnCLick,Color.Red),
                 new PaintColourButton("Blue",null, CustomToolStripButtonOnCLick,Color.Blue),
                 new PaintColourButton("Green",null, CustomToolStripButtonOnCLick,Color.Green),
                   new PaintColourButton("Yellow",null, CustomToolStripButtonOnCLick,Color.Yellow),
                    new PaintColourButton("Orange",null, CustomToolStripButtonOnCLick,Color.Orange),
                    new PaintColourButton("Purple",null, CustomToolStripButtonOnCLick,Color.Purple),
                      new PaintColourButton("Pink",null, CustomToolStripButtonOnCLick,Color.Pink),
                       new PaintColourButton("White",null, CustomToolStripButtonOnCLick,Color.White),
                       new PaintColourButton("Brown",null, CustomToolStripButtonOnCLick,Color.Brown),
                         new PaintColourButton("Black",null, CustomToolStripButtonOnCLick,Color.Black),
               new PaintBrushSizeButton( CustomToolStripButtonOnCLick, 2),
               new PaintBrushSizeButton(CustomToolStripButtonOnCLick,5),
               new PaintBrushSizeButton(CustomToolStripButtonOnCLick, 8),
               new PaintBrushSizeButton(CustomToolStripButtonOnCLick,13),
                     new PaintBrushSizeButton(CustomToolStripButtonOnCLick,21),
                        new PaintBrushSizeButton(CustomToolStripButtonOnCLick,34),
                           new PaintBrushSizeButton(CustomToolStripButtonOnCLick,55),
              new TrashBinButton( CustomToolStripButtonOnCLick)};

            this.toolStrip1.Items.AddRange(buttons);

            var bunnyPeekaBoo = new PeekabooBuddy("mcqueen", "mater", "doc","mcmissile","luigi","sally");
            this.pictureBox1.Controls.Add(bunnyPeekaBoo);
            this._peekabooBuddies.Add(bunnyPeekaBoo);
            bunnyPeekaBoo.Start();          

        }

        private Color _penColor = Color.Black;
        private int _penSize = 2;
        private Dictionary<string, DateTime> _colorsPicked = new Dictionary<string, DateTime>();
        private void CustomToolStripButtonOnCLick(object sender, EventArgs e)
        {
            if (sender is TrashBinButton)
            {
                ClearPictureBox();
                return;
            }

            if (sender is PaintBrushSizeButton)
            {
                var button = sender as PaintBrushSizeButton;
                _penSize = button.PenSize;
                this.Cursor = CursorHelper.GetColourCursor(_penColor, CursorHelper.AppCursor.Pen, CursorHelper.GetCursorSize(_penSize));
                return;
            }

            if (sender is PaintColourButton)
            {
                var button = sender as PaintColourButton;

                DateTime lastPicked;

                bool voicePrompt = false;

                if (_colorsPicked.TryGetValue(button.Text, out lastPicked))
                {
                    voicePrompt = DateTime.Now.Subtract(lastPicked).Seconds > 10;
                    _colorsPicked[button.Text] = DateTime.Now;

                }
                else
                {
                    voicePrompt = true;
                    _colorsPicked.Add(button.Text, DateTime.Now);
                }

                if (voicePrompt)
                {
                    this.toolStrip1.Enabled = false;

                    var colourObject = _colourDB.GetRandomObject(button.Text);

                    if (colourObject == null)
                        SpeechHelper.SpeakAsync($"{button.Text}", (t) => this.toolStrip1.Enabled = true);
                    else
                        SpeechHelper.SpeakAsync($"{button.Text}. You can draw a {colourObject.Name}! {colourObject.Description}.", (t) => toolStrip1.Enabled = true);
                }

                _penColor = button.BackColor;
                this.Cursor = CursorHelper.GetColourCursor(_penColor, CursorHelper.AppCursor.Pen, CursorHelper.GetCursorSize(_penSize));
                return;

            }

        }

        private bool _altPressed = false;
        private bool _fPressed = false;
        private bool _cPressed = false;
        private bool _isFullScreen = false;
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu) _altPressed = true;
            if (e.KeyCode == Keys.F) _fPressed = true;
            if (e.KeyCode == Keys.C) _cPressed = true;

            if (_altPressed && _fPressed)
            {
                SetScreenMode();
            }

            if (_altPressed && _cPressed)
            {
                ClearPictureBox();
            }
        }

        private void ClearPictureBox()
        {
            //_lastPictureBoxImage = null;
            pictureBox1.Image = null;
            pictureBox1.Invalidate();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu) _altPressed = false;
            if (e.KeyCode == Keys.F) _fPressed = false;
            if (e.KeyCode == Keys.C) _cPressed = false;
        }

        private List<Point> _drawPoints = new List<Point>();
        private Point _lastPoint = Point.Empty;
        private bool rightMouseButtonClicked = false;

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            _lastPoint = e.Location;
            rightMouseButtonClicked = true;
            
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            _lastPoint = Point.Empty;
            rightMouseButtonClicked = false;
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!rightMouseButtonClicked) return;

            if (_lastPoint == null) return;


            if (pictureBox1.Image == null)//if no available bitmap exists on the picturebox to draw on

            {
                //create a new bitmap
                Bitmap bmp = new Bitmap(pictureBox1.Width, pictureBox1.Height);

                pictureBox1.Image = bmp; //assign the picturebox.Image property to the bitmap created

            }

            using (Graphics g = Graphics.FromImage(pictureBox1.Image))

            {//we need to create a Graphics object to draw on the picture box, its our main tool

                //when making a Pen object, you can just give it color only or give it color and pen size

                g.DrawLine(new Pen(_penColor, _penSize), _lastPoint, e.Location);


                g.SmoothingMode = SmoothingMode.AntiAlias;
                //this is to give the drawing a more smoother, less sharper look

            }

            pictureBox1.Invalidate();//refreshes the picturebox

            _lastPoint = e.Location;//keep assigning the lastPoint to the current mouse position

        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SpeechHelper.Speak("Night night Sammy");
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        public void SetScreenMode()
        {
            if (!_isFullScreen)
            {                
                this.WindowState = FormWindowState.Normal;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                this.Bounds = Screen.PrimaryScreen.Bounds;
                this._isFullScreen = true;
            }
            else
            {               
                this.WindowState = FormWindowState.Maximized;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this._isFullScreen = false;
            }
        }
    }
}
