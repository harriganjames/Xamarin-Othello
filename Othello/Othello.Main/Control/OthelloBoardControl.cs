using Othello.Main.Enum;
using Othello.Main.View;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Othello.Main.Control
{
    public class OthelloBoardControl : ContentView
    {
        AbsoluteLayout _layout;
        BoxView _gridBox;
        Dictionary<object, OthelloCell> _cells = new Dictionary<object, OthelloCell>();
        Dictionary<object, OthelloDisc> _discs = new Dictionary<object, OthelloDisc>();
        double _cellSize;
        Rectangle _gridRect = new Rectangle();
        double _gapRatio = 0.05;
        BlockingCollection<OthelloJob> _jobQueue = new BlockingCollection<OthelloJob>();
        TaskScheduler _taskScheduler;
        bool _debug = false;

        public OthelloBoardControl()
        {
            _layout = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
            _gridBox = new BoxView();
            _layout.BackgroundColor = Color.Linen;
            this.Content = _layout;

            _taskScheduler = TaskScheduler.FromCurrentSynchronizationContext();
            Task.Factory.StartNew(() => ProcessQueue());
        }


        public IEnumerable<object> CellItemsSource
        {
            get { return (IEnumerable<object>)GetValue(CellItemsSourceProperty); }
            set { SetValue(CellItemsSourceProperty, value); }
        }

        public static readonly BindableProperty CellItemsSourceProperty =
            BindableProperty.Create("CellItemsSource", typeof(IEnumerable<object>), typeof(OthelloBoardControl), propertyChanged: OnCellItemsSourceChanged);

        static void OnCellItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (OthelloBoardControl)bindable;
            board.BuildLayout();
        }


        public IEnumerable<object> DiscItemsSource
        {
            get { return (IEnumerable<object>)GetValue(DiscItemsSourceProperty); }
            set { SetValue(DiscItemsSourceProperty, value); }
        }

        public static readonly BindableProperty DiscItemsSourceProperty =
            BindableProperty.Create("DiscItemsSource", typeof(IEnumerable<object>), typeof(OthelloBoardControl), propertyChanged: OnDiscItemsSourceChanged);

        static void OnDiscItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (OthelloBoardControl)bindable;
            board.BuildLayout();
        }


        public Color BoardBackgroundColor
        {
            get { return (Color)GetValue(BoardBackgroundColorProperty); }
            set { SetValue(BoardBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoardBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BoardBackgroundColorProperty =
            BindableProperty.Create("BoardBackgroundColor", typeof(Color), typeof(OthelloBoardControl), Color.Transparent, propertyChanged: OnBoardBackgroundColorChanged);


        static void OnBoardBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (OthelloBoardControl)bindable;
            if (board.Content != null)
                board._gridBox.Color = (Color)newValue;
        }


        public Command CellTappedCommand
        {
            get { return (Command)GetValue(CellTappedCommandProperty); }
            set { SetValue(CellTappedCommandProperty, value); }
        }

        public static readonly BindableProperty CellTappedCommandProperty =
            BindableProperty.Create("CellTappedCommand", typeof(Command), typeof(OthelloBoardControl), null);



        public bool IsAnimating
        {
            get { return (bool)GetValue(IsAnimatingProperty); }
            set { SetValue(IsAnimatingProperty, value); }
        }

        public static readonly BindableProperty IsAnimatingProperty =
            BindableProperty.Create("IsAnimating", typeof(bool), typeof(OthelloBoardControl), false);


        int _busyCount=0;
        int BusyCount
        {
            get
            {
                return _busyCount;
            }
            set
            {
                if (value < 0)
                    value = 0;
                _busyCount = value;
                if (_busyCount == 0)
                    IsAnimating = false;
                if (_busyCount == 1)
                    IsAnimating = true;
            }
        }



        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 0 && height > 0)
            {
                if(_debug) Debug.WriteLine($"OnSizeAllocated {width} {height}");
                LayoutBoard();
            }
        }




        void BuildLayout()
        {
            if (CellItemsSource == null || DiscItemsSource==null)
                return;

            _layout.Children.Clear();
            _layout.Children.Add(_gridBox);
            foreach (var item in CellItemsSource)
            {
                CellView view = new CellView();
                if (view != null)
                {
                    view.BindingContext = item;
                    _layout.Children.Add(view);
                    var cell = new OthelloCell(view, ProcessCell);
                    cell.Item = item;
                    _cells.Add(item,cell);
                    view.PropertyChanged += CellView_PropertyChanged;
                    var tap = new TapGestureRecognizer();
                    tap.Command = new Command(v =>
                    {
                        CellTappedCommand?.Execute(v);
                        return;
                    });
                    tap.CommandParameter = item;
                    view.GestureRecognizers.Add(tap);
                }
            }
            foreach (var item in DiscItemsSource)
            {
                var view = new DiscView();
                view.BindingContext = item;
                _layout.Children.Add(view);
                var disc = new OthelloDisc(view,ProcessDisc);
                disc.Item = item;
                _discs.Add(item,disc);
                view.PropertyChanged += DiscView_PropertyChanged;
            }
        }

        bool _layoutBoardInProgress = false;
        bool _redoLayoutBoard = false;

        void LayoutBoard()
        {
            if(_debug) Debug.WriteLine($"LayoutBoard - Start");

            if (_layoutBoardInProgress)
            {
                _redoLayoutBoard = true;
                return;
            }

            _layoutBoardInProgress = true;

            double width = this.Width;
            double height = this.Height;

            int cells = 8;
            double maxSize = Math.Min(width, height);
            double gapSize = Math.Round(maxSize / cells * _gapRatio);
            double remaining = maxSize - (gapSize * (cells + 1));
            _cellSize = Math.Floor(remaining / cells);
            double newSize = _cellSize * cells + gapSize * (cells + 1);

            _gridRect.Width = _gridRect.Height = newSize;
            _gridRect.X = (_layout.Width - newSize) / 2;
            _gridRect.Y = (_layout.Height - newSize) / 2;

            AbsoluteLayout.SetLayoutBounds(_gridBox, _gridRect);

            int row = 0;
            int col = 0;
            foreach (var cell in _cells.Values)
            {
                var rec = new Rectangle(_gridRect.X + gapSize + col * (gapSize + _cellSize),
                                        _gridRect.Y + gapSize + row * (gapSize + _cellSize),
                                        _cellSize, _cellSize);

                AbsoluteLayout.SetLayoutBounds(cell.View, rec);
                col++;
                if (col >= cells)
                {
                    col = 0;
                    row++;
                }
            }

            int position = 0;
            foreach (var odisc in _discs.Values.Where(d => d.View.InitialColor == OthelloColor.White))
            {
                odisc.StackPosition = position++;
            }
            position = 0;
            foreach (var odisc in _discs.Values.Where(d => d.View.InitialColor == OthelloColor.Black))
            {
                odisc.StackPosition = position++;
            }

            ProcessAllDiscs();

            ProcessAllCells();

            _layoutBoardInProgress = false;

            if(_redoLayoutBoard)
            {
                _redoLayoutBoard = false;
                LayoutBoard();
            }
            if(_debug) Debug.WriteLine($"LayoutBoard - End");
        }



        private void DiscView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_layoutBoardInProgress)
                return;
            //if(_debug) Debug.WriteLine($"DiscView_PropertyChanged {e.PropertyName}");
            if (e.PropertyName == DiscView.DiscColorProperty.PropertyName
                || e.PropertyName == DiscView.InUseProperty.PropertyName)
            {
                var bo = sender as BindableObject;
                if (bo != null)
                {
                    OthelloDisc odisc;
                    _discs.TryGetValue(bo.BindingContext, out odisc);
                    if (odisc != null)
                    {
                        PushJob(new OthelloJob(odisc));
                    }
                }
            }
        }

        private void CellView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_layoutBoardInProgress)
                return;
            //if(_debug) Debug.WriteLine($"CellView_PropertyChanged {e.PropertyName}");
            if (e.PropertyName==CellView.DiscProperty.PropertyName)
            {
                var bo = sender as BindableObject;
                if(bo!=null)
                {
                    OthelloCell ocell;
                    _cells.TryGetValue(bo.BindingContext, out ocell);
                    if (ocell != null)
                    {
                        PushJob(new OthelloJob(ocell));
                    }
                }
            }
        }

        void ProcessQueue()
        {
            while (true)
            {
                var job = _jobQueue.Take();
                ProcessJob(job);
            }
        }


        void ProcessJob(OthelloJob job)
        {
            Task.Factory.StartNew(async () =>
            {
                if (job.OthelloItem.IsProcessing)
                    PushJob(job);
                else
                    await job.OthelloItem.Process();
            }, CancellationToken.None, TaskCreationOptions.None, _taskScheduler);
        }

        void PushJob(OthelloJob job)
        {
            _jobQueue.Add(job);
        }


        void ProcessAllCells()
        {
            if(_debug) Debug.WriteLine($"ProcessAllCells - Start");
            BusyCount++;
            foreach (var ocell in _cells.Values)
            {
                PushJob(new OthelloJob(ocell));
            }
            BusyCount--;
            if(_debug) Debug.WriteLine($"ProcessAllCells - End");
        }

        async Task ProcessCell(OthelloCell ocell)
        {
            if(_debug) Debug.WriteLine($"ProcessCell Start {ocell.Item}");
            ocell.IsProcessing = true;
            if(IsAnimating)
            {
                await Task.Delay(new Random().Next(200));
            }
            BusyCount++;
            if (ocell.View.Disc != null)
            {
                OthelloDisc odisc;
                if (_discs.TryGetValue(ocell.View.Disc, out odisc))
                {
                    if (ocell.OthelloDisc != odisc)
                    {
                        if(ocell.OthelloDisc!=null)
                            ocell.OthelloDisc.IsProcessing = true;
                        await MoveDiscFromCellToStack(ocell);
                        if(ocell.OthelloDisc!=null)
                            ocell.IsProcessing = false;
                    }
                    odisc.IsProcessing = true;
                    await MoveDiscToCell(odisc, ocell);
                    odisc.IsProcessing = false;
                }
            }
            // might need to remove ocell.InUse from here
            else if (ocell.View.Disc==null) // && ocell.InUse && ocell.OthelloDisc!=null)
            {
                await MoveDiscFromCellToStack(ocell);
            }
            BusyCount--;
            ocell.IsProcessing = false;
            if(_debug) Debug.WriteLine($"ProcessCell End {ocell.Item}");
        }

        void ProcessAllDiscs()
        {
            if(_debug) Debug.WriteLine($"ProcessAllDiscs - Start");
            BusyCount++;
            foreach (var odisc in _discs.Values)
            {
                PushJob(new OthelloJob(odisc));
            }
            BusyCount--;
            if(_debug) Debug.WriteLine($"ProcessAllDiscs - End");
        }

        async Task ProcessDisc(OthelloDisc odisc)
        {
            if(_debug) Debug.WriteLine($"ProcessDisc {odisc.Item} IsUse={odisc.InUse} Stack={odisc.StackPosition}");
            odisc.IsProcessing = true;
            BusyCount++;
            if(!odisc.InUse)
            {
                await MoveDiscToStack(odisc);
            }
            if (odisc.InUse && odisc.View.DiscColor != odisc.View.ActualColor)
            {
                await FlipDisc(odisc);
            }
            odisc.IsProcessing = false;
            BusyCount--;
        }


        async Task FlipDisc(OthelloDisc odisc)
        {
            if(_debug) Debug.WriteLine($"FlipDisc {odisc.Item}");
            bool cancelled = false;
            ViewExtensions.CancelAnimations(odisc.View);
            cancelled = await odisc.View.RotateYTo(90);
            if (!cancelled)
            {
                odisc.View.ActualColor = odisc.View.DiscColor;
                cancelled = await odisc.View.RotateYTo(0);
            }
        }

        async Task MoveDiscFromCellToStack(OthelloCell ocell)
        {
            if(_debug) Debug.WriteLine($"MoveDiscFromCellToStack Started Cell={ocell.Item} OthelloDisc={ocell.OthelloDisc}");
            if (ocell.OthelloDisc == null) return;
            ocell.InUse = false;
            await MoveDiscToStack(ocell.OthelloDisc);
            ocell.OthelloDisc = null;
            if(_debug) Debug.WriteLine($"MoveDiscFromCellToStack Finished Cell={ocell.Item} OthelloDisc={ocell.OthelloDisc}");
        }

        async Task MoveDiscToStack(OthelloDisc odisc)
        {
            if(_debug) Debug.WriteLine($"MoveDiscToStack Started OthelloDisc={odisc}");
            odisc.View.ActualColor = odisc.View.DiscColor;
            await PositionDiscToStack(odisc);
            if(_debug) Debug.WriteLine($"MoveDiscToStack Ended OthelloDisc={odisc}");
        }

        bool IsPortrait => Height > Width;

        async Task PositionDiscToStack(OthelloDisc odisc)
        {
            ViewExtensions.CancelAnimations(odisc.View);
            Point start = new Point();
            Rectangle rec;
            bool isDiscVertical;
            if (IsPortrait) 
            {
                var discHeight = _cellSize - 1;
                var discWidth = discHeight / 5;
                double x;
                if (odisc.View.InitialColor == OthelloColor.White)
                {
                    start.X = 1;
                    start.Y = 1;
                    x = start.X + (odisc.StackPosition * (discWidth + 2));
                }
                else
                {
                    start.X = Width - discWidth - 1;
                    start.Y = Height - discHeight - 1;
                    x = start.X - (odisc.StackPosition * (discWidth + 2));
                }
                rec = new Rectangle(x, start.Y, discWidth, discHeight);
                isDiscVertical = true;
            }
            else
            {
                var discWidth = _cellSize - 1;
                var discHeight = discWidth / 5;
                start.Y = Height - discHeight - 1;
                if (odisc.View.InitialColor == OthelloColor.White)
                {
                    start.X = 1;
                }
                else
                {
                    start.X = Width - discWidth - 1;
                }
                double y = start.Y - (odisc.StackPosition * (discHeight + 2));
                rec = new Rectangle(start.X, y, discWidth, discHeight);
                isDiscVertical = false;
            }

            _layout.RaiseChild(odisc.View);

            bool cancelled = false;
            if(odisc.InUse)
                cancelled = await odisc.View.LayoutTo(rec, 600, Easing.SpringIn);
            if (!cancelled)
            {
                AbsoluteLayout.SetLayoutBounds(odisc.View, rec);
                odisc.View.IsFlat = true;
                odisc.View.IsVertical = isDiscVertical;
                odisc.InUse = false;
            }
        }

        async Task MoveDiscToCell(OthelloDisc odisc, OthelloCell ocell)
        {
            if(_debug) Debug.WriteLine($"MoveDiscToCell Started Cell={ocell.Item} Disc={odisc.Item}");
            odisc.View.ActualColor = odisc.View.DiscColor;
            ocell.InUse = true;
            ocell.OthelloDisc = odisc;
            await PositionDiscToCell(odisc, ocell);
            if(_debug) Debug.WriteLine($"MoveDiscToCell Finished Cell={ocell.Item} Disc={odisc.Item}");
        }

        async Task PositionDiscToCell(OthelloDisc odisc, OthelloCell ocell)
        {
            ViewExtensions.CancelAnimations(odisc.View);
            double jumpHeight = 40;
            bool cancelled = false;
            var rect1 = odisc.View.Bounds;
            if (IsPortrait)
            {
                if (odisc.View.InitialColor == OthelloColor.White)
                    rect1.X += jumpHeight;
                else
                    rect1.X -= jumpHeight;
            }
            else
            {
                rect1.Y -= jumpHeight;
            }

            var finalRect = new Rectangle();
            finalRect.Left = ocell.View.Bounds.Left;
            finalRect.Top = ocell.View.Bounds.Top;
            finalRect.Width = ocell.View.Bounds.Width - 0;
            finalRect.Height = ocell.View.Bounds.Height - 0;

            if(_debug) Debug.WriteLine($"PositionDiscToCell disc={odisc.Item} cell={ocell.Item}");

            _layout.RaiseChild(odisc.View);

            if(!odisc.InUse)
                cancelled = await odisc.View.LayoutTo(rect1, 200, Easing.Linear);
            odisc.View.IsFlat = false;
            cancelled = await odisc.View.LayoutTo(finalRect,600,Easing.SpringOut);
            if (!cancelled)
            {
                AbsoluteLayout.SetLayoutBounds(odisc.View, finalRect);
                odisc.InUse = true;
            }
            return;
        }


        class OthelloJob
        {
            public OthelloJob(OthelloItem item)
            {
                OthelloItem = item;
            }
            public OthelloItem OthelloItem { get; set; }
        }

        abstract class OthelloItem
        {
            public object Item { get; set; }
            public virtual bool IsProcessing { get; set; }
            public bool InUse { get; set; }
            public abstract Task Process();
        }


        class OthelloCell : OthelloItem
        {
            Func<OthelloCell,Task> _processAction;
            public OthelloCell(CellView view, Func<OthelloCell, Task> processAction)
            {
                View = view;
                _processAction = processAction;
            }
            public CellView View { get; set; }
            public OthelloDisc OthelloDisc { get; set; }
            public override bool IsProcessing
            {
                get => base.IsProcessing || (OthelloDisc!=null && OthelloDisc.IsProcessing);
                set => base.IsProcessing = value;
            }

            public override async Task Process()
            {
                await _processAction(this);
            }
        }

        class OthelloDisc : OthelloItem
        {
            Func<OthelloDisc,Task> _processAction;
            public OthelloDisc(DiscView view, Func<OthelloDisc,Task> processAction)
            {
                View = view;
                _processAction = processAction;
            }
            public DiscView View { get; set; }
            public int StackPosition { get; set; }
            public override async Task Process()
            {
                await _processAction(this);
            }
            public override string ToString()
            {
                return $"OthelloDisc StackPosition={StackPosition}";
            }
        }
    }
}