//using System;
//using System.Windows;
//using System.Windows.Controls;
//using System.Windows.Media;
//using System.Windows.Shapes;

//namespace SammyFunApp
//{
//    public class PeekabooBuddy : UIElement
//    {
//        //private System.Timers.Timer _timer = new System.Timers.Timer() { Interval = 10 };
//        //private System.Timers.Timer _carPausetimer = new System.Timers.Timer() { Interval = 5000 };

//        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer() { Interval = 10 };
//        private readonly System.Windows.Forms.Timer _carPausetimer = new System.Windows.Forms.Timer() { Interval = 5000 };
//        private readonly string[] _peekabooBuddyNames;
//        private int _peekabooBuddyIndex = 0;
//        private string _normalImageName;
//        private string _winkImageName;
//        private string _scaredImageName;

//        private Image _normalImage;
//        private Image NormalImage
//        {
//            get
//            {
//                if (_normalImage != null) return _normalImage;

//                _normalImage = (Bitmap)Images.ResourceManager.GetObject(_normalImageName);

//                return _normalImage;
//            }
//        }

//        private Image _winkImage;
//        private Image WinkImage
//        {
//            get
//            {
//                if (_winkImage != null) return _winkImage;

//                _winkImage = (Bitmap)Images.ResourceManager.GetObject(_winkImageName);

//                return _winkImage;
//            }
//        }

//        private Image _scaredImage;
//        private Image ScaredImage
//        {
//            get
//            {
//                if (_scaredImage != null) return _scaredImage;

//                _scaredImage = (Bitmap)Images.ResourceManager.GetObject(_scaredImageName);

//                return _scaredImage;
//            }
//        }

//        public PeekabooBuddy(string peekabooBuddyName) : this(new string[] { peekabooBuddyName })
//        { }
//        public PeekabooBuddy(params string[] peekabooBuddyNames) 
//        {
//            this.DoubleBuffered = true;
//            this.BackColor = Color.Transparent;

//            this.MouseEnter += PeekabooBuddy_MouseEnter;
//            this.MouseLeave += PeekabooBuddy_MouseLeave;
//            this.MouseClick += PeekabooBuddy_MouseClick;
//            this.ParentChanged += PeekabooBuddy_ParentChanged;
//            this.Paint += PeekabooBuddy_OnPaint;
//            _timer.Tick += _timer_Tick;
//            _carPausetimer.Tick += _carPausetimer_Tick;

//            _peekabooBuddyNames = peekabooBuddyNames;
//            InitializeBuddy();
//        }

//        private void PeekabooBuddy_OnPaint(object sender, PaintEventArgs e)
//        {
//            using (var graphics = e.Graphics)
//            {
//                graphics.DrawImage(_currentImage, 0, 0, _currentImage.Width, _currentImage.Height);
//            }
//        }

//        private void _carPausetimer_Tick(object sender, EventArgs e)
//        {
//            _carPausetimer.Stop();
//            SetVisibility(true);
//            _timer.Start();
//        }

//        private void PeekabooBuddy_ParentChanged(object sender, EventArgs e)
//        {
//            this.Parent.SizeChanged += Parent_SizeChanged;
//            _startYPos = this.Parent.Size.Height;
//            Location = new Point(0, this.Parent.Size.Height);
//        }

//        private void Parent_SizeChanged(object sender, EventArgs e)
//        {
//            _startYPos = this.Parent.Size.Height;
//            _origination = Location = new Point(0, this.Parent.Size.Height);
//            SetDestination(GetRandomOffScreenLocation());
//        }

//        private void SetDestination(Point point)
//        {
//            SetDestination(point.X, point.Y);
//        }

//        private int _startYPos;
//        private int? _lastYPos = null;
//        private double? _lastYPosD = null;
//        private int _lastXPos = 0;
//        private double _lastXPosD = 0.00D;
//        private int _midWayCnt = 0;
//        private int _defaultSpeed = 5;
//        private int _speed = 5;
//        private Point? _destination;
//        private Point? _origination;
//        private Point? _midWay;
//        private Rectangle _midWayRec;
//        private double? _angle;
//        private bool _blinked = false;
//        private bool _clicked = false;
//        private Random _coinFlip = new Random();
//        private void _timer_Tick(object sender, EventArgs e)
//        {
//            if (_lastYPos == null) _lastYPosD = _lastYPos = _startYPos;


//            if (_destination == null)
//            {
//                SetDestination(GetRandomOffScreenLocation());
//            }

//            var currentPositionRec = new Rectangle(new Point(_lastXPos, _lastYPos.Value), new Size(25, 25));

//            if (!_blinked && _speed == _defaultSpeed && currentPositionRec.IntersectsWith(_midWayRec))
//            {
//                _midWayCnt += 1;

//                if (_midWayCnt > 50)
//                {
//                    SetImage( NormalImage);
//                    _midWayCnt = 0;
//                    _blinked = true;
//                }
//                else
//                {
//                    if (_midWayCnt % 10 == 0)
//                        if (GetImage() == NormalImage)
//                            SetImage(WinkImage);
//                        else
//                            SetImage(NormalImage);
//                    return;
//                }
//            }

//            var xStep = Math.Cos(_angle.Value) * _speed * -1;
//            var yStep = Math.Sin(_angle.Value) * _speed * -1;

//            _lastXPosD += xStep;
//            _lastYPosD += yStep;

//            _lastXPos = (int)Math.Truncate(_lastXPosD);
//            _lastYPos = (int)Math.Truncate(_lastYPosD.Value);

//            if ((_lastXPos > _destination.Value.X && xStep > 0)
//                ||
//               (_lastXPos < _destination.Value.X && xStep < 0)
//               ||
//                (_lastYPos > _destination.Value.Y && yStep > 0)
//                ||
//                (_lastYPos < _destination.Value.Y && yStep < 0))
//            {
//                if (_speed != _defaultSpeed)
//                {  
//                    NextBuddy();
//                }
                
//                SetRandomSpeed();

//                SetDestination(GetRandomOffScreenLocation());

//                Stop();

//                SetVisibility(false);
//                _carPausetimer.Interval = _coinFlip.Next(1, 10) * 1000;
//                _carPausetimer.Start();
               
//                return;
//            }

//            System.Diagnostics.Debug.WriteLine($"Buddy moving x:{_lastXPos} y:{_lastXPos}");
//            SetLocation(new Point(_lastXPos, _lastYPos.Value));
//        }

//        //private delegate void SafeCallDelegateVisible(bool visible);
//        private void SetVisibility(bool visible)
//        {
//            this.Visible = visible;

//            //try
//            //{
//            //    if (this.InvokeRequired)
//            //    {
//            //        var d = new SafeCallDelegateVisible(SetVisibility);
//            //        this.Invoke(d, new object[] { visible });
//            //    }
//            //    else
//            //    {
                    
//            //    }
//            //}
//            //finally
//            //{ }
//        }

//        //private delegate void SafeCallDelegateLocation(Point point);
//        private void SetLocation(Point point)
//        {
//            this.Location = point;
//            //try
//            //{
//            //    if (this.InvokeRequired)
//            //    {
//            //        var d = new SafeCallDelegateLocation(SetLocation);
//            //        this.Invoke(d, new object[] { point });
//            //    }
//            //    else
//            //    {
                   
//            //    }
//            //}
//            //finally { }
//        }

//        private void SetDestination(int x, int y)
//        {
//            _origination = this.Location;
//            _destination = new Point(x, y);

//            _midWay = new Point((_origination.Value.X + _destination.Value.X) / 2, (_origination.Value.Y + _destination.Value.Y) / 2);
//            _midWayRec = new Rectangle(_midWay.Value, new Size(30, 30));

//            var dx = this.Location.X - _destination.Value.X;
//            var dy = this.Location.Y - _destination.Value.Y;

//            _blinked = _coinFlip.Next(1, 5) != 1;         

//            _angle = Math.Atan2(dy, dx);
//        }

//        private Image _preScareImage;
//        private void PeekabooBuddy_MouseLeave(object sender, EventArgs e)
//        {
//            if (_speed != _defaultSpeed) return; //already clicked the buddy

//            SetImage(_preScareImage);

//            Start();
//        }

//        private Image _currentImage;
//        private void SetImage(Image image)
//        {
//            _currentImage = image;
//            this.Invalidate();
//        }
        
//          private Image GetImage()
//        {
//            return _currentImage; 
//        }

//        private void PeekabooBuddy_MouseEnter(object sender, EventArgs e)
//        {
//            if (_clicked) return;

//            _preScareImage = GetImage();
//            SetImage(ScaredImage);

//            Stop();
//        }

      

//        private void PeekabooBuddy_MouseClick(object sender, MouseEventArgs e)
//        {
//            if (_clicked) return;

//            FartHelper.PlayRandom();

//            _speed = 30;
//            SetImage(_preScareImage);
//            SetDestination(_origination.Value.X, _origination.Value.Y);
//            _blinked = true;//don't stop to blink on the way back
//            _clicked = true;
//            Start();
//        }

//        public void Start()
//        {
//            _timer.Start();
//            _carPausetimer.Stop();
//        }

//        public void Stop()
//        {
//            _timer.Stop();
//        }

//        private void NextBuddy()
//        {
//            _clicked = false;

//            _timer.Stop();

//            _peekabooBuddyIndex++;

//            if (_peekabooBuddyIndex == _peekabooBuddyNames.Length)
//                _peekabooBuddyIndex = 0;

//            DisposeBuddy();
//            InitializeBuddy();

//            _timer.Start();

//        }

//        private void InitializeBuddy()
//        {
//            _normalImageName = _peekabooBuddyNames[_peekabooBuddyIndex];
//            _winkImageName = $"{_normalImageName}_wink";
//            _scaredImageName = $"{_normalImageName}_scared";

//            SetImage((Bitmap)Images.ResourceManager.GetObject(_normalImageName));
//            SetSize(new Size(GetImage().Size.Width, GetImage().Size.Height));
//        }

//       // private delegate void SafeCallDelegateSize(Size size);
//        private void SetSize(Size size)
//        {
//            this.Size = size;
//            //if (this.InvokeRequired)
//            //{
//            //    var d = new SafeCallDelegateSize(SetSize);
//            //    this.Invoke(d, new object[] { size });
//            //}
//            //else
//            //{
                
//            //}
//        }

//        private void DisposeBuddy()
//        {
//            if (_normalImage != null)
//            {
//                _normalImage.Dispose();
//                _normalImage = null;
//            }

//            if (_winkImage != null)
//            {
//                _winkImage.Dispose();
//                _winkImage = null;
//            }

//            if (_scaredImage != null)
//            {
//                _scaredImage.Dispose();
//                _scaredImage = null;
//            }
//        }
//        private int _lastRandomDirection = 0;
//        private Point GetRandomOffScreenLocation()
//        {
//            int dir, x = 0, y = 0;
//            var rnd = new Random();

//            dir = rnd.Next(1, 4);

//            while (dir == _lastRandomDirection)
//            {
//                dir = rnd.Next(1, 4);
//            }

//            _lastRandomDirection = dir;

//            var halfHeight = (this.Height / 2);
//            switch (dir)
//            {
//                case 1://left side of screen
//                    x = -this.Size.Width;
//                    y = rnd.Next(0, this.Parent.Size.Height-halfHeight);
//                    break;
//                case 2://bottom
//                    x = rnd.Next(0, this.Parent.Size.Width);
//                    y = this.Parent.Size.Height;
//                    break;
//                case 3://right
//                    x = this.Parent.Size.Width;
//                    y = rnd.Next(0, this.Parent.Size.Height-halfHeight);
//                    break;
//                case 4://top
//                    x = -this.Size.Height;
//                    y = rnd.Next(0, this.Parent.Size.Width);
//                    break;
//            }

//            return new Point(x, y);
//        }

//        private void SetRandomSpeed()
//        {
//            var speed = _coinFlip.Next(5, 15);
//            _speed = _defaultSpeed = speed;
//        }

//    }

//}
