using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPlot
{
    /// <summary>
    /// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
    ///
    /// Step 1a) Using this custom control in a XAML file that exists in the current project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPlot"
    ///
    ///
    /// Step 1b) Using this custom control in a XAML file that exists in a different project.
    /// Add this XmlNamespace attribute to the root element of the markup file where it is 
    /// to be used:
    ///
    ///     xmlns:MyNamespace="clr-namespace:WPlot;assembly=WPlot"
    ///
    /// You will also need to add a project reference from the project where the XAML file lives
    /// to this project and Rebuild to avoid compilation errors:
    ///
    ///     Right click on the target project in the Solution Explorer and
    ///     "Add Reference"->"Projects"->[Select this project]
    ///
    ///
    /// Step 2)
    /// Go ahead and use your control in the XAML file.
    ///
    ///     <MyNamespace:CustomControl1/>
    ///
    /// </summary>
    public class XYPlot : Control
    {
        static XYPlot()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(XYPlot), new FrameworkPropertyMetadata(typeof(XYPlot)));
        }

        private Path mainPath;
        private Grid mainGrid;

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.mainPath = (Path)GetTemplateChild("mainPath");
            this.mainGrid = (Grid)GetTemplateChild("mainGrid");
            UpdateGeometry();
            UpdateLinePen();
        }

        //private Pen linePen;

        public double XMin
        {
            get { return (double)GetValue(XMinProperty); }
            set { SetValue(XMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMinProperty =
            DependencyProperty.Register(nameof(XMin), typeof(double), typeof(XYPlot), new PropertyMetadata(0.0));

        public double XMax
        {
            get { return (double)GetValue(XMaxProperty); }
            set { SetValue(XMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for XMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty XMaxProperty =
            DependencyProperty.Register(nameof(XMax), typeof(double), typeof(XYPlot), new PropertyMetadata(0.0));

        public double YMin
        {
            get { return (double)GetValue(YMinProperty); }
            set { SetValue(YMinProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YMin.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMinProperty =
            DependencyProperty.Register(nameof(YMin), typeof(double), typeof(XYPlot), new PropertyMetadata(0.0));

        public double YMax
        {
            get { return (double)GetValue(YMaxProperty); }
            set { SetValue(YMaxProperty, value); }
        }

        // Using a DependencyProperty as the backing store for YMax.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty YMaxProperty =
            DependencyProperty.Register(nameof(YMax), typeof(double), typeof(XYPlot), new PropertyMetadata(0.0));

        public IXYPlotDataSource DataSource
        {
            get { return (IXYPlotDataSource)GetValue(DataSourceProperty); }
            set { SetValue(DataSourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for DataSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(nameof(DataSource), typeof(IXYPlotDataSource), typeof(XYPlot), new PropertyMetadata(null, OnDataSourceChanged));

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var plot = (XYPlot)d;
            plot.UpdateGeometry();
        }

        public Brush LineBrush
        {
            get { return (Brush)GetValue(LineBrushProperty); }
            set { SetValue(LineBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineBrushProperty =
            DependencyProperty.Register(nameof(LineBrush), typeof(Brush), typeof(XYPlot), new PropertyMetadata(null, OnLineBrushChanged));

        private static void OnLineBrushChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var plot = (XYPlot)d;
            plot.UpdateLinePen();
        }

        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LineThicknessProperty =
            DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(XYPlot), new PropertyMetadata(1.0, OnLineThicknessChanged));

        private static void OnLineThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var plot = (XYPlot)d;
            plot.UpdateLinePen();
        }

        private void UpdateLinePen()
        {
            var brush = LineBrush;
            var lineThickness = LineThickness;
            if (mainPath != null)
            {
                mainPath.Stroke = brush;
                mainPath.StrokeThickness = lineThickness;
            }
            //if (brush == null || lineThickness <= 0)
            //{
            //    linePen = null;
            //}
            //else
            //{
            //    linePen = new Pen(brush, lineThickness);
            //}
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            UpdateGeometry();
        }

        private void UpdateGeometry()
        {
            if (mainPath == null || mainGrid == null)
                return;

            var dataSource = DataSource;
            if (dataSource == null || dataSource.PointCount < 2)
                return;

            var width = mainGrid.ActualWidth;
            var height = mainGrid.ActualHeight;
            if (width <= 0 || height <= 0)
                return;

            //drawingContext.PushClip(new RectangleGeometry(new Rect(0, 0, width, height)));

            var bx = -this.XMin;
            var dxSource = this.XMax - this.XMin;
            if (dxSource <= 0)
                return;
            var ax = width / dxSource;
            bx *= ax;

            var by = -this.YMin;
            var dySource = this.YMax - this.YMin;
            if (dySource <= 0)
                return;
            var ay = -height / dySource;
            by = height + by * ay;

            var x0 = dataSource.GetX(0);
            var y0 = dataSource.GetY(0);
            var lineFigure = new PathFigure();
            //lineFigure.StartPoint = new Point(ax * x0 + bx, ay * y0 + by);
            lineFigure.StartPoint = new Point(x0, y0);
            for (int i = 1; i < dataSource.PointCount; i++)
            {
                var x = dataSource.GetX(i);
                var y = dataSource.GetY(i);
                //lineFigure.Segments.Add(new LineSegment(new Point(ax * x + bx, ay * y + by), true));
                lineFigure.Segments.Add(new LineSegment(new Point(x, y), true));
            }
            var geometry = new PathGeometry(new PathFigure[] { lineFigure });
            geometry.Transform = new MatrixTransform(ax, 0, 0, ay, bx, by);

            mainPath.Data = geometry;
            mainPath.Clip = new RectangleGeometry(new Rect(0, 0, width, height));
            mainPath.ClipToBounds = true;
        }

        //protected override void OnRender(DrawingContext drawingContext)
        //{
        //    base.OnRender(drawingContext);
        //    if (linePen == null)
        //        return;
        //    var dataSource = DataSource;
        //    if (dataSource == null || dataSource.PointCount < 2)
        //        return;

            

        //    drawingContext.DrawGeometry(Brushes.Transparent, linePen, geometry);

        //    drawingContext.Pop();
        //}


    }
}
