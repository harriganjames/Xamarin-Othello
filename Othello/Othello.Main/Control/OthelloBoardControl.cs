using Othello.Main.Enum;
using Othello.Main.Model;
using Othello.Main.View;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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


        public OthelloBoardControl()
        {
            _layout = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
            _gridBox = new BoxView();
            _layout.BackgroundColor = Color.Linen;
            this.Content = _layout;
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
                Debug.WriteLine($"OnSizeAllocated {width} {height}");
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
                    var cell = new OthelloCell(view);
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
                var disc = new OthelloDisc(view);
                disc.Item = item;
                _discs.Add(item,disc);
                view.PropertyChanged += DiscView_PropertyChanged;
            }
        }

        bool _layoutBoardInProgress = false;
        bool _redoLayoutBoard = false;

        async void LayoutBoard()
        {
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

            StackAllDiscs(_discs.Values);

            Debug.WriteLine($"LayoutBoard - b4 ProcessAllCells");
            await ProcessAllCells();
            Debug.WriteLine($"LayoutBoard - b4 ProcessAllDiscs");

            Debug.WriteLine($"LayoutBoard - end");

            _layoutBoardInProgress = false;

            if(_redoLayoutBoard)
            {
                _redoLayoutBoard = false;
                LayoutBoard();
            }
        }



        private void DiscView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_layoutBoardInProgress)
                return;
            if (e.PropertyName == DiscView.DiscColorProperty.PropertyName)
            {
                var bo = sender as BindableObject;
                if (bo != null)
                {
                    OthelloDisc odisc;
                    _discs.TryGetValue(bo.BindingContext, out odisc);
                    if (odisc != null)
                    {
                        ProcessDisc(odisc);
                    }
                }
            }
        }

        private async void CellView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_layoutBoardInProgress)
                return;
            if (e.PropertyName==CellView.DiscProperty.PropertyName)
            {
                var bo = sender as BindableObject;
                if(bo!=null)
                {
                    OthelloCell ocell;
                    _cells.TryGetValue(bo.BindingContext, out ocell);
                    if (ocell != null)
                    {
                        await ProcessCell(ocell);
                    }
                }
            }
        }

        async Task ProcessAllCells()
        {
            BusyCount++;
            var tasks = new List<Task>();
            foreach (var ocell in _cells.Values)
            {
                tasks.Add(ProcessCell(ocell));
            }
            await Task.WhenAll(tasks.ToArray());
            BusyCount--;
        }

        async Task ProcessCell(OthelloCell ocell)
        {
            BusyCount++;
            if (ocell.View.Disc != null)
            {
                OthelloDisc odisc;
                if (_discs.TryGetValue(ocell.View.Disc, out odisc))
                {
                    await MoveDiscToCell(odisc, ocell);
                }
            }
            // might need to remove ocell.InUse from here
            else if (ocell.View.Disc==null && ocell.InUse && ocell.OthelloDisc!=null)
            {
                await MoveDiscFromCellToStack(ocell);
            }
            BusyCount--;
        }

        void ProcessDisc(OthelloDisc odisc)
        {
            BusyCount++;
            if (odisc.InUse && odisc.View.DiscColor != odisc.View.ActualColor)
            {
                FlipDisc(odisc);
            }
            BusyCount--;
        }


        async void FlipDisc(OthelloDisc odisc)
        {
            bool cancelled = false;
            ViewExtensions.CancelAnimations(odisc.View);
            cancelled = await odisc.View.RotateYTo(90);
            if (!cancelled)
            {
                odisc.View.ActualColor = odisc.View.DiscColor;
                cancelled = await odisc.View.RotateYTo(0);
            }
        }

        void StackAllDiscs(IEnumerable<OthelloDisc> odiscs)
        {
            BusyCount++;
            foreach (var odisc in odiscs)
            {
                MoveDiscToStack(odisc);
            }
            BusyCount--;
        }

        async Task MoveDiscFromCellToStack(OthelloCell ocell)
        {
            if (ocell.OthelloDisc == null) return;
            //ocell.OthelloDisc.InUse = false;
            await MoveDiscToStack(ocell.OthelloDisc);
            ocell.InUse = false;
            ocell.OthelloDisc = null;
        }


        async Task MoveDiscToStack(OthelloDisc odisc)
        {
            odisc.View.ActualColor = odisc.View.DiscColor;
            await PositionDiscToStack(odisc);
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
            ocell.InUse = true;
            ocell.OthelloDisc = odisc;
            odisc.View.ActualColor = odisc.View.DiscColor;
            //odisc.DiscColor = odisc.View.DiscColor;
            await PositionDiscToCell(odisc, ocell);
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

            Debug.WriteLine($"PositionDiscToCell disc={odisc.Item} cell={ocell.Item}");

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

        class OthelloCell
        {
            public OthelloCell(CellView view)
            {
                View = view;
            }
            public CellView View { get; set; }
            public bool InUse { get; set; }
            public object Item { get; set; }
            public OthelloDisc OthelloDisc { get; set; }
        }

        class OthelloDisc
        {
            public OthelloDisc(DiscView view)
            {
                View = view;
                //DiscColor = OthelloColor.None;
            }
            public DiscView View { get; set; }
            public bool InUse { get; set; }
            public object Item { get; set; }
            public int StackPosition { get; set; }
            //public OthelloColor DiscColor { get; set; }
        }
    }
}