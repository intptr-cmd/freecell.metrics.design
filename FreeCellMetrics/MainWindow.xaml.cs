using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FreeCellMetrics.Classes;

namespace FreeCellMetrics
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //new Game().Generate();
            //TODO

            this.Cards = new List<Card>();

        }

        public List<Card> Cards
        {
            get;
            set;
        }

        private static double total;
        private string CalculateStats(Card c)
        {
            StringBuilder resStr = new StringBuilder();

            this.Cards = TagReader.PopulateCards(this.grid.Children);
            
            var results = Cards.Where(s => s.Value == c.Value + 1
                && s.isRed == !c.isRed && s.isTop).ToList();

            resStr.Append(results.Count + " / 13 ");

            double stat = results.Count / 13.0;

            results = Cards.Where(s => s.Value == c.Value + 1
                && s.isRed == !c.isRed && s.isTopMinusOne).ToList();

            if (results.Count > 0)
            {
                resStr.Append(results.Count + " / 26 ");

                stat += results.Count / 26.0;
            }

            total += stat;
            resStr.Append(" " + stat.ToString("0.00") + " ");

            return resStr.ToString();
        }

        /// <summary>
        /// button drag drop
        /// </summary>
        List<double> rowsheight = new List<double>();
        List<double> columnswidth = new List<double>();
        Rect[,] rects = null;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            double rowstarno = 0;
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                if (grid.RowDefinitions[i].Height.IsStar)
                {
                    rowstarno += grid.RowDefinitions[i].Height.Value;
                }
            }
            for (int i = 0; i < grid.RowDefinitions.Count; i++)
            {
                GridLength length = grid.RowDefinitions[i].Height;
                if (length.IsAbsolute)
                {
                    rowsheight.Add(length.Value);
                }
                else if (length.IsStar)
                {
                    double rowheight = (grid.RowDefinitions[i].Height.Value / rowstarno) * grid.ActualHeight;
                    rowsheight.Add(rowheight);
                }
            }
            double columnstarno = 0;
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                if (grid.ColumnDefinitions[i].Width.IsStar)
                {
                    columnstarno += grid.ColumnDefinitions[i].Width.Value;
                }
            }
            for (int i = 0; i < grid.ColumnDefinitions.Count; i++)
            {
                GridLength length = grid.ColumnDefinitions[i].Width;
                if (length.IsAbsolute)
                {
                    columnswidth.Add(length.Value);
                }
                else if (length.IsStar)
                {
                    double columnheight = (grid.ColumnDefinitions[i].Width.Value / columnstarno) * grid.ActualWidth;
                    columnswidth.Add(columnheight);
                }
            }
            rects = new Rect[rowsheight.Count, columnswidth.Count];
            double yvalue = 0;
            for (int i = 0; i < rowsheight.Count; i++)
            {
                for (int j = 0; j < columnswidth.Count; j++)
                {
                    rects[i, j] = new Rect() { Y = yvalue, Height = rowsheight[i] };
                }
                yvalue += rowsheight[i];
            }
            double xvalue = 0;
            for (int j = 0; j < columnswidth.Count; j++)
            {
                for (int i = 0; i < rowsheight.Count; i++)
                {
                    rects[i, j].X = xvalue;
                    rects[i, j].Width = columnswidth[j];
                }
                xvalue += columnswidth[j];
            }

        }

        bool IsMouseDown = false, IsDraggingStarted = false, IsPopulated = false;
        DependencyObject ClickedElement = null;

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            IsPopulated = false;
            if (!e.Source.Equals(sender))
            {
                ClickedElement = e.Source as DependencyObject;
                IsMouseDown = true;
            }
        }

        private void Grid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (!IsDraggingStarted && IsMouseDown)
            {
                //
                Button clickedBtn = (Button)(ClickedElement); 
                this.TagText = clickedBtn.Tag.ToString();
                selectedBtn = TagReader.FindSelected(this.TagText, this.grid.Children);

                if (selectedBtn != null)
                {
                    selectedBtn.BorderBrush = new SolidColorBrush(Colors.Red);
                  //  ToolTip tooltip = new ToolTip { Content = "My Tooltip" };
                   // selectedBtn.ToolTip = tooltip;
                   // tooltip.IsOpen = true;
                }
                //
                DragDrop.DoDragDrop(sender as DependencyObject, ClickedElement, DragDropEffects.Move);
            }
        }
        Button selectedBtn; 
        private void grid_Drop(object sender, DragEventArgs e)
        {
            if (selectedBtn != null)
                selectedBtn.BorderBrush = new SolidColorBrush(Colors.Black);

            Point droppedpoint = e.GetPosition(sender as IInputElement);
            Grid grid = sender as Grid;

            bool cellfound = false;

            for (int i = 0; i < rects.GetLength(0); i++)
            {
                for (int j = 0; j < rects.GetLength(1); j++)
                {
                    if (rects[i, j].Contains(droppedpoint))
                    {
                        ClickedElement.SetValue(Grid.RowProperty, i);
                        ClickedElement.SetValue(Grid.ColumnProperty, j);
                        //dirty hack
                        Button clickedBtn;

                        clickedBtn = (Button)(ClickedElement); //todo

                        this.TagText = clickedBtn.Tag.ToString();

                        string newTag = string.Empty;
                        
                        newTag = TagReader.GenTag(clickedBtn.Tag.ToString(), i, j);
                          
                        Button button0 = new Button()
                        {
                            Content = clickedBtn.Content,
                            Background = clickedBtn.Background,
                            Margin = new Thickness(3),
                            Tag = (TagReader.GetRow(newTag.ToString())
                                    + TagReader.GetCol(newTag).ToString()
                                    + TagReader.GetColor(newTag)
                                    + TagReader.GetValue(newTag))
                        };

                        Grid.SetRow(button0, TagReader.GetRow(newTag.ToString()));
                        Grid.SetColumn(button0, TagReader.GetCol(newTag.ToString()));

                        this.grid.Children.Add(button0);

                        if (TagReader.GetRow(this.TagText) != 0)
                        {
                            //find tail 14S4
                            List<Button> tail = TagReader.FindTail(this.TagText, this.grid.Children);
                            i = TagReader.GetRow(newTag);
                            foreach (var el in tail)
                            {
                                Button button = new Button()
                                {
                                    Content = el.Content,
                                    Background = el.Background,
                                    Margin = new Thickness(3),
                                    Tag = ((++i).ToString()
                                            + TagReader.GetCol(newTag).ToString()
                                            + TagReader.GetColor(el.Tag.ToString())
                                            + TagReader.GetValue(el.Tag.ToString()))
                                };

                                Grid.SetRow(button, TagReader.GetRow(button.Tag.ToString()));
                                Grid.SetColumn(button, TagReader.GetCol(button.Tag.ToString()));
                                //button.Click += new RoutedEventHandler(button_Click);
                                this.grid.Children.Add(button);
                                this.grid.Children.Remove(el);
                            }
                        }


                        //
                        cellfound = true;

                        //set title
                        
                            var resStr = new StringBuilder();
                            total = 0;

                            foreach (Button item in grid.Children)
                            {
                                if (TagReader.GetRow(item.Tag.ToString()) == 0)
                                {
                                    if (TagReader.GetCol(item.Tag.ToString()) == 5
                                    || TagReader.GetCol(item.Tag.ToString()) == 6
                                    || TagReader.GetCol(item.Tag.ToString()) == 7
                                    || TagReader.GetCol(item.Tag.ToString()) == 8)
                                    {
                                        resStr.Append(CalculateStats(TagReader.Read(item.Tag.ToString())));
                                    }
                                }
                            }

                            this.Title = total.ToString(".00") + " " + resStr.ToString();
                            //todo if (TagReader.GetCol(newTag) == 1) +1
                            //
                        

                        if (!IsPopulated)
                        {
                            TagReader.Populate(TagText, grid);
                            IsPopulated = true;                           
                        }
                        //

                        break;
                    }
                }
                if (cellfound)
                {
                    break;
                }
            }

            
            IsDraggingStarted = false;
            IsMouseDown = false;

           
        }

        string tagText = string.Empty;
        public string TagText
        {
            get { return this.tagText; }
            set
            {
                this.tagText = value.ToString();
            }
        }
    }
}
