using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Othello.Main.Control
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChessboardView : ContentView
    {
        AbsoluteLayout _layout;

        public ChessboardView()
        {
            _layout = new AbsoluteLayout() { HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            this.Content = _layout;
        }


        public IEnumerable<object> ItemsSource
        {
            get { return (IEnumerable<object>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.Create("ItemsSource", typeof(IEnumerable<object>), typeof(ChessboardView), propertyChanged: OnItemsSourceChanged);

        static void OnItemsSourceChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;
            board.BuildLayout();

            //board.CreateBoard();
        }

        public Color BoardBackgroundColor
        {
            get { return (Color)GetValue(BoardBackgroundColorProperty); }
            set { SetValue(BoardBackgroundColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BoardBackgroundColor.  This enables animation, styling, binding, etc...
        public static readonly BindableProperty BoardBackgroundColorProperty =
            BindableProperty.Create("BoardBackgroundColor", typeof(Color), typeof(ChessboardView), Color.Transparent, propertyChanged: OnBoardBackgroundColorChanged);


        static void OnBoardBackgroundColorChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;
            if(board.Content!=null)
                board.Content.BackgroundColor = (Color)newValue;
        }


        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)GetValue(ItemTemplateProperty); }
            set { SetValue(ItemTemplateProperty, value); }
        }

        public static readonly BindableProperty ItemTemplateProperty =
            BindableProperty.Create("ItemTemplate", typeof(DataTemplate), typeof(ChessboardView), propertyChanged: OnItemTemplateChanged);

        static void OnItemTemplateChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;
            board.BuildLayout();
        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width>0 && height>0)
            {
                LayoutBoard();
            }
        }

        double _gapRatio = 0.05;


        void BuildLayout()
        {
            if (ItemsSource == null || ItemTemplate == null)
                return;

            _layout.Children.Clear();
            foreach (var item in ItemsSource)
            {
                var cell = ItemTemplate?.CreateContent() as ViewCell;
                if (cell != null)
                {
                    cell.View.BindingContext = item;
                    _layout.Children.Add(cell.View);
                }
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
            double cellSize = Math.Floor(remaining / cells);
            double newSize = cellSize * cells + gapSize * (cells + 1);
            _layout.WidthRequest = newSize;
            _layout.HeightRequest = newSize;

            int row = 0;
            int col = 0;
            foreach (var view in _layout.Children)
            {
                var rec = new Rectangle(gapSize + col * (gapSize + cellSize),
                                        gapSize + row * (gapSize + cellSize),
                                        cellSize, cellSize);
                AbsoluteLayout.SetLayoutBounds(view, rec);
                row++;
                if (row >= cells)
                {
                    row = 0;
                    col++;
                }
            }


        }



    }
}