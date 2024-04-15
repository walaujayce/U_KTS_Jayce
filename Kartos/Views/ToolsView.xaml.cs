using Kartos.ViewModels;
using OpenCvSharp;
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
using Point = System.Windows.Point;
using OpenCvSharp.WpfExtensions;
using Microsoft.Win32;
using System.Collections;
using ControlzEx.Standard;


namespace Kartos.Views
{
    /// <summary>
    /// ToolsView.xaml 的互動邏輯
    /// </summary>
    public partial class ToolsView : UserControl
    {

        ToolsViewModel toolsViewModel;

        bool _isMouseDown = false;

        Point _mouseDownPosition;

        Thickness _mouseDownMargin;
        private Image _currentImage;
        private Canvas _overlayCanvas;
        private Mat mat;


        public ToolsView()
        {
            InitializeComponent();
            toolsViewModel = new ToolsViewModel();
            this.DataContext = toolsViewModel;

        }

        private void Image_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMouseDown == true && isDrawingLine == false && isDrawingPolygon == false)
            {
                var c = sender as Image;
                var pos = e.GetPosition(this);
                var dp = pos - _mouseDownPosition;
                c.Margin = new Thickness(
                    _mouseDownMargin.Left + dp.X,
                    _mouseDownMargin.Top + dp.Y,
                    _mouseDownMargin.Right - dp.X,
                    _mouseDownMargin.Bottom - dp.Y
                );
            }
        }

        private void Image_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Image;
            _isMouseDown = true;
            _mouseDownPosition = e.GetPosition(this);
            _mouseDownMargin = c.Margin;
            c.CaptureMouse();
        }

        private void Image_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            var c = sender as Image;
            _isMouseDown = false;
            c.ReleaseMouseCapture();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            Mat mat = new Mat(100, 100, MatType.CV_8UC3, Scalar.Gray);
            mat.PutText("New CV Mat", new OpenCvSharp.Point(10, 50), HersheyFonts.Italic, 0.3, Scalar.Black);


            Image img = new Image()
            {
                Width = 100,
                Height = 120,
            };

            img.AddHandler(Button.MouseDownEvent, new MouseButtonEventHandler(Image_PreviewMouseDown), true);
            img.AddHandler(Button.MouseMoveEvent, new MouseEventHandler(Image_PreviewMouseMove), true);
            img.AddHandler(Button.MouseUpEvent, new MouseButtonEventHandler(Image_PreviewMouseUp), true);
            img.Source = mat.ToBitmapSource();
            MainWindow.GloableConsole.Children.Add(img);
            Canvas.SetLeft(img, 100);
            Canvas.SetTop(img, 100);

        }

        private void FolderOpen_Click(object sender, RoutedEventArgs e)
        {

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png)|*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;

            bool? result = openFileDialog.ShowDialog();

            if (result == true)
            {
                // Get the selected file name and display it
                string selectedFileName = openFileDialog.FileName;

                // Read the image using OpenCVSharp
                mat = Cv2.ImRead(selectedFileName);

                Image img = new Image()
                {
                    Width = 500,
                    Height = 500,
                    Source = mat.ToBitmapSource()
                };
                _currentImage = img;
                img.AddHandler(Button.MouseDownEvent, new MouseButtonEventHandler(Image_PreviewMouseDown), true);
                img.AddHandler(Button.MouseMoveEvent, new MouseEventHandler(Image_PreviewMouseMove), true);
                img.AddHandler(Button.MouseUpEvent, new MouseButtonEventHandler(Image_PreviewMouseUp), true);

                // Attach additional handlers for drawing lines
                img.AddHandler(Button.MouseDownEvent, new MouseButtonEventHandler(Image_PointMouseDown), true);
                img.AddHandler(Button.MouseMoveEvent, new MouseEventHandler(Image_PointMouseMove), true);
                img.AddHandler(Button.MouseUpEvent, new MouseButtonEventHandler(Image_PointMouseUp), true);

                img.AddHandler(Button.MouseDownEvent, new MouseButtonEventHandler(ImageControl_MouseLeftButtonDown), true);

                MainWindow.GloableConsole.Children.Add(img);

            }
        }


        private bool isDrawingLine = false;

        private void RayStartEnd_Click(object sender, RoutedEventArgs e)
        {
            if (isDrawingLine == false)
            {
                isDrawingLine = true;
            }
            else
                isDrawingLine = false;

            _overlayCanvas = new Canvas();
            MainWindow.GloableConsole.Children.Add(_overlayCanvas);

        }
        private Line _currentLine;

        private void Image_PointMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingLine)
            {
                if (e.ChangedButton == MouseButton.Left)
                {
                    _mouseDownPosition = e.GetPosition(this);
                    _currentLine = new Line()
                    {
                        X1 = _mouseDownPosition.X,
                        Y1 = _mouseDownPosition.Y,
                        X2 = _mouseDownPosition.X,
                        Y2 = _mouseDownPosition.Y,
                        Stroke = System.Windows.Media.Brushes.Red,
                        StrokeThickness = 2
                    };
                    _overlayCanvas.Children.Add(_currentLine);
                }
            }
        }

        private void Image_PointMouseMove(object sender, MouseEventArgs e)
        {
            if (isDrawingLine)
            {
                if (_currentLine != null && e.LeftButton == MouseButtonState.Pressed)
                {
                    var pos = e.GetPosition(this);
                    _currentLine.X2 = pos.X;
                    _currentLine.Y2 = pos.Y;
                }
            }
        }

        private void Image_PointMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingLine)
            {
                _currentLine = null;
            }
        }
        private void CropSquare_Click(object sender, RoutedEventArgs e)
        {
            //Mat mat = new Mat(512,512,MatType.CV_8UC3,new Scalar(0,0,0));
            if (isDrawingLine == false)
            {
                isDrawingLine = true;

            }
            else
                isDrawingLine = false;
            Cv2.Rectangle(mat, new OpenCvSharp.Point(50, 50), new OpenCvSharp.Point(100, 100), Scalar.Red, 2);

            ShowImage(mat);
        }
        private void ShowImage(Mat image)
        {
            // Display the image
            Cv2.ImShow("Image", image);
            Cv2.WaitKey(0);
            Cv2.DestroyAllWindows();
        }

        private List<Point> polygonPoints = new List<Point>();
        private bool isDrawingPolygon = false;
        private void PolygonButton_Click(object sender, RoutedEventArgs e)
        {
            if (isDrawingPolygon == false)
            {
                isDrawingPolygon = true;
                polygonPoints.Clear();
            }
            else
                isDrawingPolygon = false;

            _overlayCanvas = new Canvas();
            MainWindow.GloableConsole.Children.Add(_overlayCanvas);
        }
        private void ImageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isDrawingPolygon)
            {
                polygonPoints.Add(_mouseDownPosition);

                if (polygonPoints.Count > 1)
                {
                    DrawPolygon();
                }
                
            }
        }

        private void DrawPolygon()
        {
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                Pen pen = new Pen(Brushes.Red, 2);

                // Draw lines between each pair of consecutive points
                for (int i = 0; i < polygonPoints.Count - 1; i++)
                {
                    dc.DrawLine(pen, polygonPoints[i], polygonPoints[i + 1]);
                }

                // Draw line between the last and first point to close the polygon
                dc.DrawLine(pen, polygonPoints[polygonPoints.Count - 1], polygonPoints[0]);
            }

            // Render the visual to the image control
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)_overlayCanvas.ActualWidth, (int)_overlayCanvas.ActualHeight, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(visual);
            _currentImage.Source = rtb;
        }
    }
} 

