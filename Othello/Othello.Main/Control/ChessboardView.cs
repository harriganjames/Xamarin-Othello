using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Othello.Main.Control
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChessboardView : ContentView
    {
        Grid _grid;

        public ChessboardView()
        {
            //InitializeComponent();
        }

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        public static readonly BindableProperty ColumnsProperty =
            BindableProperty.Create("Columns", typeof(int), typeof(ChessboardView), 0, propertyChanged: OnColumnsChanged);


        static void OnColumnsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;

            board.CreateBoard();
        }


        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        public static readonly BindableProperty RowsProperty =
            BindableProperty.Create("Rows", typeof(int), typeof(ChessboardView), 0, propertyChanged: OnRowsChanged);


        static void OnRowsChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;

            board.CreateBoard();
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

            board.CreateBoard();
        }

        public Color BoardBackgroundColor { get; set; }
        //public double BoardPadding { get; set; }

        public double CellSpacing
        {
            get { return (double)GetValue(CellSpacingProperty); }
            set { SetValue(CellSpacingProperty, value); }
        }

        public static readonly BindableProperty CellSpacingProperty =
            BindableProperty.Create("CellSpacing", typeof(double), typeof(ChessboardView), 0.0, propertyChanged: OnCellSpacingChanged);

        static void OnCellSpacingChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var board = (ChessboardView)bindable;

            board.CreateBoard();
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

            board.CreateBoard();
        }


        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (_grid != null)
            {
                var size = Math.Min(width, height);
                _grid.WidthRequest = size;
                _grid.HeightRequest = size;
            }
        }


        void CreateBoard()
        {
            if (Rows == 0 || Columns == 0 || ItemsSource == null || ItemTemplate == null || CellSpacing==0)
                return;

            _grid = new Grid() { RowSpacing = 0, ColumnSpacing = 0, HorizontalOptions = LayoutOptions.Center, VerticalOptions = LayoutOptions.Center };
            _grid.BackgroundColor = BoardBackgroundColor;
            _grid.Padding = CellSpacing/2;

            for (int row = 0; row < Rows; row++)
            {
                var rd = new RowDefinition();
                rd.Height = GridLength.Star;
                _grid.RowDefinitions.Add(rd);
            }

            for (int col = 0; col < Columns; col++)
            {
                var cd = new ColumnDefinition();
                cd.Width = GridLength.Star;
                _grid.ColumnDefinitions.Add(cd);
            }

            int gridRow = 0;
            int gridCol = 0;
            foreach (var item in ItemsSource)
            {
                var cell = ItemTemplate.CreateContent() as ViewCell;
                if (cell != null)
                {
                    cell.View.BindingContext = item;
                    cell.View.Margin = CellSpacing / 2;
                    cell.View.SetValue(Grid.ColumnProperty, gridCol);
                    cell.View.SetValue(Grid.RowProperty, gridRow);
                    _grid.Children.Add(cell.View);
                }
                gridCol++;
                if (gridCol >= Columns)
                {
                    gridCol = 0;
                    gridRow++;
                }
            }

            this.Content = _grid;
        }



    }
}