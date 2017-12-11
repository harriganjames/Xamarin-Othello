﻿using Othello.Main.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Othello.Main.View
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class BoardViewOld : ContentView
    {
        AbsoluteLayout _layout;
        BoxView _gridBox;
        List<Xamarin.Forms.View> _cells = new List<Xamarin.Forms.View>();
        List<DiscView> _whiteDiscs;
        List<DiscView> _blackDiscs;
        List<DiscView> _discs;

        public BoardViewOld()
        {
            _layout = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.Fill, VerticalOptions = LayoutOptions.Fill };
            _gridBox = new BoxView();
            _whiteDiscs = new List<DiscView>();
            _blackDiscs = new List<DiscView>();
            _layout.BackgroundColor = Color.Linen;
            this.Content = _layout;
        }


        public IEnumerable<object> CellItemsSource
        {
            get { return (IEnumerable<object>)GetValue(CellItemsSourceProperty); }
            set { SetValue(CellItemsSourceProperty, value); }
        }

        public static readonly BindableProperty CellItemsSourceProperty =
            BindableProperty.Create("CellItemsSource", typeof(IEnumerable<object>), typeof(BoardViewOld), propertyChanged: OnCellItemsSourceChanged);

        static void OnCellItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (BoardViewOld)bindable;
            board.BuildLayout();
        }


        public IEnumerable<object> DiscItemsSource
        {
            get { return (IEnumerable<object>)GetValue(DiscItemsSourceProperty); }
            set { SetValue(DiscItemsSourceProperty, value); }
        }

        public static readonly BindableProperty DiscItemsSourceProperty =
            BindableProperty.Create("DiscItemsSource", typeof(IEnumerable<object>), typeof(BoardViewOld), propertyChanged: OnDiscItemsSourceChanged);

        static void OnDiscItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (BoardViewOld)bindable;
            board.BuildLayout();
        }


        public Color BoardBackgroundColor
        {
            get { return (Color)GetValue(BoardBackgroundColorProperty); }
            set { SetValue(BoardBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoardBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BoardBackgroundColorProperty =
            BindableProperty.Create("BoardBackgroundColor", typeof(Color), typeof(BoardViewOld), Color.Transparent, propertyChanged: OnBoardBackgroundColorChanged);


        static void OnBoardBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (BoardViewOld)bindable;
            if(board.Content!=null)
                board._gridBox.Color = (Color)newValue;
        }


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(BoardViewOld), propertyChanged: OnItemTemplateChanged);

        static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (BoardViewOld)bindable;
            board.BuildLayout();
        }


        //public CellModel PlayCell
        //{
        //    get { return (Color)GetValue(PlayCellProperty); }
        //    set { SetValue(PlayCellProperty, value); }
        //}

        //// Using a DependencyProperty as the backing store for PlayCell.  This enables animation, styling, binding, etc...
        //public static readonly BindableProperty PlayCellProperty =
        //    BindableProperty.Create("PlayCell", typeof(Color), typeof(BoardView), Color.Transparent, propertyChanged: OnPlayCellChanged);


        //static void OnPlayCellChanged(BindableObject bindable, object oldValue, object newValue)
        //{
        //    var board = (BoardView)bindable;
        //}

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width>0 && height>0)// && (width!=_layout.Width || height!=_layout.Height))
            {
                LayoutBoard();
            }
        }

        double _gapRatio = 0.05;


        void BuildLayout()
        {
            if (CellItemsSource == null || ItemTemplate == null)// || DiscItemsSource==null)
                return;

            _layout.Children.Clear();
            _layout.Children.Add(_gridBox);
            _whiteDiscs.Clear();
            _blackDiscs.Clear();
            foreach (var item in CellItemsSource)
            {
                var cell = ItemTemplate?.CreateContent() as ViewCell;
                if (cell != null)
                {
                    cell.View.BindingContext = item;
                    _layout.Children.Add(cell.View);
                    _cells.Add(cell.View);
                }
            }
            //bool alternate = false;
            //foreach (var item in DiscItemsSource)
            //{
            //    var disc = new DiscView();
            //    disc.BindingContext = item;
            //    _layout.Children.Add(disc);
            //    _discs.Add(disc);
            //    if (alternate)
            //    {
            //        disc.State = Enum.DiscStateEnum.Stacked;
            //        _whiteDiscs.Add(disc);
            //    }
            //    else
            //    {
            //        disc.State = Enum.DiscStateEnum.Stacked;
            //        _blackDiscs.Add(disc);
            //    }
            //    alternate = !alternate;
            //}
        }

        double _cellSize;
        Rectangle _gridRect = new Rectangle();

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

            //_layout.WidthRequest = newSize;
            //_layout.HeightRequest = newSize;

            AbsoluteLayout.SetLayoutBounds(_gridBox, _gridRect);

            int row = 0;
            int col = 0;
            foreach (var view in _cells)
            {
                var rec = new Rectangle(_gridRect.X + gapSize + col * (gapSize + _cellSize),
                                        _gridRect.Y + gapSize + row * (gapSize + _cellSize),
                                        _cellSize, _cellSize);
                AbsoluteLayout.SetLayoutBounds(view, rec);
                row++;
                if (row >= cells)
                {
                    row = 0;
                    col++;
                }
            }

            //StackDiscs(_whiteDiscs, true);
            //StackDiscs(_blackDiscs, false);

            //StackDiscs(_discs.Where(d => d.State == Enum.DiscStateEnum.White && !d.InUse), true);
            //StackDiscs(_discs.Where(d => d.State == Enum.DiscStateEnum.Black && !d.InUse), false);
        }

        void StackDiscs(IEnumerable<DiscView> discs, bool left)
        {
            var discWidth = _cellSize - 1;
            var discHeight = discWidth / 5;
            Point start = new Point();
            start.Y = Height - discHeight - 1;
            if (left)
                start.X = 1;
            else
                start.X = Width - discWidth - 1;
            double y = start.Y;
            foreach (var disc in discs)
            {
                var rec = new Rectangle(start.X, y, discWidth, discHeight);
                y -= discHeight + 2;
                AbsoluteLayout.SetLayoutBounds(disc, rec);
            }

        }

    }
}