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
using System.Windows.Navigation;
using System.Windows.Threading;
using Microsoft.Win32.SafeHandles;
using SammyFunApp.Utils;
using static SammyFunApp.Utils.ResourceHelper;

namespace SammyFunApp
{
    public partial class MainForm : Form
    {
        private DrawingCanvas _drawingCanvas;

        public MainForm()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.UserPaint, true);
            this.SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            this.SetStyle(ControlStyles.AllPaintingInWmPaint, true);

            _drawingCanvas = new DrawingCanvas();

            this.elementHost1.Child = _drawingCanvas;
            _timer = new DispatcherTimer();
            _timer.Interval = new TimeSpan(0,0,0,0,33);
            _timer.Tick += _timer_Tick;

        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            Buddy buddy = new Buddy("mater");

            this._drawingCanvas.Children.Add(buddy);
        }

        private FormBorderStyle onLoadBorderStyle;
        private Rectangle onLoadBounds;
        private ColourDatabase _colourDB;
        private readonly DispatcherTimer _timer;
        private void Form1_Load(object sender, EventArgs e)
        {
            Text = "Sammy's Paint Shop";

            string[] all = System.Reflection.Assembly.GetEntryAssembly().
  GetManifestResourceNames().Where(x=> x.EndsWith("ico")).ToArray();

            MessageBox.Show(string.Join(", ", all));
            //foreach (string one in all)
            //{
            //    MessageBox.Show(one);
            //}

            onLoadBorderStyle = this.FormBorderStyle;
            onLoadBounds = this.Bounds;
            this.WindowState = FormWindowState.Maximized;

            _colourDB = ColourDatabase.Populate(Path.Combine(Application.StartupPath, @"Resources\colourdb.json"));

            var cur = CursorHelper.GetColourCursor( Color.Black, CursorHelper.AppCursor.Pen);

            SetCursor(cur);

            string dayText = TranslateIntToWords(DateTime.Now.Day);

            PlayGreeting();

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

            //var bunnyPeekaBoo = new PeekabooBuddy("mcqueen", "mater", "doc", "mcmissile", "luigi", "sally");
            //this.drawingPanel.Controls.Add(bunnyPeekaBoo);
            // this._peekabooBuddies.Add(bunnyPeekaBoo);
            //bunnyPeekaBoo.Start();


            _timer.Start();

        }

        private void SetCursor(Cursor cur)
        {
            SafeFileHandle panHandle = new SafeFileHandle(cur.Handle, false);
            elementHost1.Cursor = this.Cursor = cur;
            _drawingCanvas.Cursor = System.Windows.Interop.CursorInteropHelper.Create(panHandle);

        }

        private void PlayGreeting()
        {
            string greeting = string.Empty;
            int currentHour = DateTime.Now.Hour;

            if (currentHour < 12)
                greeting = "Good morning";
            else if (currentHour >= 12 && currentHour <= 18)
                greeting = "Good afternoon";
            else
                greeting = "Good evening";

            SpeechHelper.Instance.Speak(greeting, "Sammy", "it is", DateTime.Now.ToString("MMMMM"), TranslateIntToWords(DateTime.Now.Day), "Welcome to the paint shop");

        }

        private string TranslateIntToWords(int dayNumber)
        {
            switch (dayNumber.ToString().Reverse().First())
            {
                case '1':
                    return $"{dayNumber}st";
                case '2':
                    return $"{dayNumber}nd";
                case '3':
                    return $"{dayNumber}rd";
                default:
                    return $"{dayNumber}th";
            }
        }

        private Color _penColor = Color.Black;
        private int _previousSize, _penSize = 2;
        private Dictionary<string, DateTime> _colorsPicked = new Dictionary<string, DateTime>();
        private void CustomToolStripButtonOnCLick(object sender, EventArgs e)
        {
            if (sender is TrashBinButton)
            {
                SpeechHelper.Instance.Speak("toilet");
                ClearPictureBox();
                return;
            }

            if (sender is PaintBrushSizeButton)
            {
                var button = sender as PaintBrushSizeButton;

                this.toolStrip1.Enabled = false;

                if (_previousSize < button.PenSize)
                    SpeechHelper.Instance.SpeakAsync(() => this.toolStrip1.Enabled = true, "up");
                else
                    SpeechHelper.Instance.SpeakAsync(() => this.toolStrip1.Enabled = true, "down");

                _previousSize = _penSize = _drawingCanvas.PenThickness = button.PenSize;

                SetCursor(CursorHelper.GetColourCursor(_penColor, CursorHelper.AppCursor.Pen, CursorHelper.GetCursorSize(button.PenSize)));
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
                        SpeechHelper.Instance.SpeakAsync(() => this.toolStrip1.Enabled = true, button.Text);
                    else
                        SpeechHelper.Instance.SpeakAsync(() => toolStrip1.Enabled = true, button.Text, $"You can draw a {colourObject.Name}. {colourObject.Description}");
                }
                _penColor = button.BackColor;

                _drawingCanvas.PenColor = System.Windows.Media.Color.FromArgb(button.BackColor.A, button.BackColor.R, button.BackColor.G, button.BackColor.B);
                SetCursor(CursorHelper.GetColourCursor(button.BackColor, CursorHelper.AppCursor.Pen, CursorHelper.GetCursorSize(_penSize)));
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
            _drawingCanvas.ClearDrawing();
        }

        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Menu) _altPressed = false;
            if (e.KeyCode == Keys.F) _fPressed = false;
            if (e.KeyCode == Keys.C) _cPressed = false;
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SpeechHelper.Instance.Speak("bye bye Sammy");
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
