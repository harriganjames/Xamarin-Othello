using Othello.Main.Enum;
using Othello.Main.Model;
using Othello.Main.View;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > 0 && height > 0)// && (width!=_layout.Width || height!=_layout.Height))
            {
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


        void LayoutBoard()
        {
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

            ProcessAllCells();

            PositionAllDiscs();
        }



        private void DiscView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
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

        private void CellView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName==CellView.DiscProperty.PropertyName)
            {
                var bo = sender as BindableObject;
                if(bo!=null)
                {
                    OthelloCell ocell;
                    _cells.TryGetValue(bo.BindingContext, out ocell);
                    if (ocell != null)
                    {
                        ProcessCell(ocell);
                    }
                }
            }
        }

        bool _allCellsProcessed = false;

        void ProcessAllCells()
        {
            if (_allCellsProcessed) return;
            foreach (var ocell in _cells.Values)
            {
                ProcessCell(ocell);
            }
            _allCellsProcessed = true;
        }

        void ProcessCell(OthelloCell ocell)
        {
            if (ocell.IsBusy) return;

            if (ocell.View.Disc != null && !ocell.InUse)
            {
                OthelloDisc odisc;
                if (_discs.TryGetValue(ocell.View.Disc, out odisc))
                {
                    MoveDiscToCell(odisc, ocell);
                }
            }
            else if(ocell.View.Disc==null && ocell.InUse && ocell.OthelloDisc!=null)
            {
                MoveDiscFromCellToStack(ocell);
            }
        }

        void ProcessDisc(OthelloDisc odisc)
        {
            if (odisc.IsBusy) return;

            if (odisc.InUse && odisc.View.DiscColor != odisc.View.ActualColor)
            {
                FlipDisc(odisc);
            }
        }


        async void FlipDisc(OthelloDisc odisc)
        {
            odisc.IsBusy = true;
            await odisc.View.RotateYTo(90);
            odisc.View.ActualColor = odisc.View.DiscColor;
            await odisc.View.RotateYTo(0);
            odisc.DiscColor = odisc.View.DiscColor;
            odisc.IsBusy = false;
        }

        bool _discsStacked = false;

        void StackAllDiscs(IEnumerable<OthelloDisc> odiscs)
        {
            if (_discsStacked) return;

            foreach (var odisc in odiscs)
            {
                MoveDiscToStack(odisc);
            }
            _discsStacked = true;
        }

        void MoveDiscFromCellToStack(OthelloCell ocell)
        {
            if (ocell.OthelloDisc == null) return;
            ocell.IsBusy = true;
            MoveDiscToStack(ocell.OthelloDisc);
            ocell.OthelloDisc = null;
            ocell.InUse = false;
            ocell.IsBusy = false;
        }


        void MoveDiscToStack(OthelloDisc odisc)
        {
            odisc.View.ActualColor = odisc.View.DiscColor;
            odisc.DiscColor = odisc.View.DiscColor;
            PositionDiscToStack(odisc);
            odisc.InUse = false;
        }

        void PositionDiscToStack(OthelloDisc odisc)
        {
            Point start = new Point();
            Rectangle rec;
            bool isDiscVertical;
            if (Height > Width) // Portrait
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

            odisc.View.IsVertical = isDiscVertical;
            AbsoluteLayout.SetLayoutBounds(odisc.View, rec);

        }

        void MoveDiscToCell(OthelloDisc odisc, OthelloCell ocell)
        {
            ocell.IsBusy = true;

            odisc.InUse = true;
            odisc.View.State = odisc.View.DiscColor==OthelloColor.White ? CellStateEnum.White : CellStateEnum.Black;
            odisc.View.TransitionedState = odisc.View.State;
            ocell.InUse = true;
            ocell.OthelloDisc = odisc;
            odisc.View.ActualColor = odisc.View.DiscColor;
            odisc.DiscColor = odisc.View.DiscColor;

            PositionDiscToCell(odisc, ocell);
            ocell.IsBusy = false;
        }

        void PositionDiscToCell(OthelloDisc odisc, OthelloCell ocell)
        {
            var rec = new Rectangle();
            rec.Width = ocell.View.Bounds.Width - 0;
            rec.Height = ocell.View.Bounds.Height - 0;
            rec.Left = ocell.View.Bounds.Left;
            rec.Top = ocell.View.Bounds.Top;
            AbsoluteLayout.SetLayoutBounds(odisc.View, rec);
        }

        void PositionAllDiscs()
        {
            foreach (var odisc in _discs.Values)
            {
                if (!odisc.InUse)
                    PositionDiscToStack(odisc);
            }
            foreach (var ocell in _cells.Values)
            {
                if (ocell.InUse)
                    PositionDiscToCell(ocell.OthelloDisc, ocell);
            }
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
            public bool IsBusy { get; set; }
        }

        class OthelloDisc
        {
            public OthelloDisc(DiscView view)
            {
                View = view;
                DiscColor = OthelloColor.None;
            }
            public DiscView View { get; set; }
            public bool InUse { get; set; }
            public object Item { get; set; }
            public int StackPosition { get; set; }
            public OthelloColor DiscColor { get; set; }
            public bool IsBusy { get; set; }
        }
    }
}