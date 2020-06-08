using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MessageBox = System.Windows.MessageBox;
using MouseEventHandler = System.Windows.Input.MouseEventHandler;
using Path = System.Windows.Shapes.Path;
using Point = System.Windows.Point;

namespace HaarImageCropper
{
    public partial class MainWindow : Window
    {
        bool isDragging = false;
        bool newSelection;
        Point startPosition;
        FileInfo[] sourceFiles;
        public MainWindow()
        {
            InitializeComponent();
            ImageCanvas.MouseLeftButtonDown += new MouseButtonEventHandler(SelectionStart);
            ImageCanvas.MouseMove += new MouseEventHandler(SelectionDraw);
            ImageCanvas.MouseLeftButtonUp += new MouseButtonEventHandler(SelectionStop);
            ImageCanvas.MouseRightButtonDown += new MouseButtonEventHandler(RemoveLastSelection);
        }
        
        private void SelectionStart(object sender, MouseButtonEventArgs e)
        {
            if (!isDragging)
            {
                isDragging = true;
                newSelection = true;
                startPosition = e.GetPosition(ImageCanvas);
            }
        }

        private void SelectionStop(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        private void SelectionDraw(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (isDragging)
            {
                Point currentPosition = e.GetPosition(ImageCanvas);

                if (newSelection)
                {
                    ImageCanvas.Children.Add(new Path
                    {
                        Data = new RectangleGeometry(new Rect(startPosition, currentPosition)),
                        Stroke = System.Windows.Media.Brushes.Red,
                        StrokeThickness = 1
                    });
                    newSelection = false;
                }
                else
                {
                    Path selection = ImageCanvas.Children.OfType<Path>().Last();
                    selection.Data = new RectangleGeometry(new Rect(startPosition, currentPosition));
                }
            }
        }

        private void RemoveLastSelection(object sender, MouseButtonEventArgs e)
        {
            if (ImageCanvas.Children.OfType<Path>().Count() > 0)
                ImageCanvas.Children.Remove(ImageCanvas.Children.OfType<Path>().Last());
        }

        private void OpenSourceDir_Click(object sender, RoutedEventArgs e)
        {
            using (var folder = new FolderBrowserDialog())
            {
                DialogResult result = folder.ShowDialog();
                folder.Description = "Choose folder";
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(folder.SelectedPath);
                    sourceFiles = dir.GetFiles();
                }
            }
            for (int i=0; i<sourceFiles.Length; i++)
            {
                SourceImagesDir.Items.Add(sourceFiles[i].Name);
            }
        }

        private void SourceImagesDir_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue != null)
            {
                FileInfo file = sourceFiles.Where(x => x.Name == e.NewValue.ToString()).Single();
                ImageSource img = new BitmapImage(new Uri(file.FullName));
                Image.Source = img;
                ImageCanvas.Children.Clear();
            }
        }

        private void CloseSourceDir_Click(object sender, RoutedEventArgs e)
        {
            SourceImagesDir.Items.Clear();
            sourceFiles = new FileInfo[0];
        }

        private void ClearSourceDir_Click(object sender, RoutedEventArgs e)
        {
            if (sourceFiles.Length > 0)
            {
                foreach (FileInfo file in sourceFiles)
                {
                    File.Delete(file.FullName);
                }
                SourceImagesDir.Items.Clear();
                sourceFiles = new FileInfo[0];
            }
        }

        private void OpenSaveSamplesDir_Click(object sender, RoutedEventArgs e)
        {
            using (var folder = new FolderBrowserDialog())
            {
                DialogResult result = folder.ShowDialog();
                folder.Description = "Choose folder";
                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folder.SelectedPath))
                {
                    DirectoryInfo dir = new DirectoryInfo(folder.SelectedPath);
                    SaveSamplesDir.Text = dir.FullName;
                }
                try
                {
                    if (!Directory.Exists($@"{SaveSamplesDir.Text}\Good"))
                        Directory.CreateDirectory($@"{SaveSamplesDir.Text}\Good");
                    if (!Directory.Exists($@"{SaveSamplesDir.Text}\Bad"))
                        Directory.CreateDirectory($@"{SaveSamplesDir.Text}\Bad");
                    DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Good");
                    GoodSamplesCount.Content = dir.GetFiles().Length;
                    dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Bad");
                    BadSamplesCount.Content = dir.GetFiles().Length;
                }
                catch (Exception ex)
                {
                   MessageBox.Show($@"Creating samples directories error: {ex.Message}", "Error");
                    return;
                }
            }
        }

        private void SaveSample_Click(object sender, RoutedEventArgs e)
        {
            if (ImageCanvas.Children.OfType<Path>().Count() == 0)
                MessageBox.Show("First crop a sample from image!");
            else if (!IsGoodSample.IsChecked.HasValue || !IsBadSample.IsChecked.HasValue)
                MessageBox.Show("Select the type of the sample!");
            else if (string.IsNullOrEmpty(SaveSamplesDir.Text))
                MessageBox.Show("Select a saving directory for the samples!");
            else
            {
                SaveSamplesDir.Text = SaveSamplesDir.Text.Trim(' ');
                foreach (Path sample in ImageCanvas.Children.OfType<Path>())
                {
                    Rectangle sampleRect = new Rectangle((int)sample.Data.Bounds.X, (int)sample.Data.Bounds.Y, (int)sample.Data.Bounds.Width, (int)sample.Data.Bounds.Height);
                    MemoryStream ms = new MemoryStream();
                    var encoder = new BmpBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(Image.Source as BitmapSource));
                    encoder.Save(ms);
                    ms.Flush();
                    Bitmap bmpImage = new Bitmap(ms);
                    Bitmap bmpCrop = bmpImage.Clone(sampleRect, bmpImage.PixelFormat);
                    if (IsInGrayscale.IsChecked.HasValue && IsInGrayscale.IsChecked.Value)
                    {
                        bmpCrop = Extensions.CopyAsGrayscale(bmpCrop);
                    }
                    if (IsGoodSample.IsChecked.Value)
                    {
                        DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Good");
                        int cropNumber = dir.GetFiles().Length;
                        bmpCrop.Save($@"{SaveSamplesDir.Text}\Good\{cropNumber}.bmp", ImageFormat.Bmp);
                        GoodSamplesCount.Content = cropNumber + 1;
                        if (IsMarkupCreating.IsChecked.HasValue && IsMarkupCreating.IsChecked.Value)
                        {
                            using (StreamWriter sw = new StreamWriter($@"{SaveSamplesDir.Text}\Good.dat", true))
                            {
                                sw.WriteLine($@"Good\{cropNumber}.bmp 1 0 0 {bmpCrop.Width} {bmpCrop.Height}");
                            }
                        }
                    }
                    else if (IsBadSample.IsChecked.Value)
                    {
                        DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Bad");
                        int cropNumber = dir.GetFiles().Length;
                        bmpCrop.Save($@"{SaveSamplesDir.Text}\Bad\{cropNumber}.bmp", ImageFormat.Bmp);
                        BadSamplesCount.Content = cropNumber + 1;
                        if (IsMarkupCreating.IsChecked.HasValue && IsMarkupCreating.IsChecked.Value)
                        {
                            using (StreamWriter sw = new StreamWriter($@"{SaveSamplesDir.Text}\Bad.dat", true))
                            {
                                sw.WriteLine($@"{SaveSamplesDir.Text}\Bad\{cropNumber}.bmp");
                            }
                        }
                    }
                    ms.Dispose();
                }
                ImageCanvas.Children.Clear();
                MessageBox.Show("All samples saved!");
            }
        }

        private void ConvertExistingGrayscale_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoodSample.IsChecked.HasValue || !IsBadSample.IsChecked.HasValue)
                MessageBox.Show("Select the type of the sample!");
            else if (string.IsNullOrEmpty(SaveSamplesDir.Text))
                MessageBox.Show("Select a saving directory for the samples!");
            else
            {
                Directory.CreateDirectory($@"{SaveSamplesDir.Text}\Temp");
                if (IsGoodSample.IsChecked.Value)
                {
                    DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Good");
                    List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                    for (int i=0; i<samples.Count; i++)
                    {
                        Bitmap source = new Bitmap(samples[i].FullName);
                        Bitmap result = Extensions.CopyAsGrayscale(source);
                        source.Dispose();
                        result.Save($@"{SaveSamplesDir.Text}\Temp\{samples[i].Name}", ImageFormat.Bmp);
                        result.Dispose();
                    }
                    samples.Clear();
                    dir.Delete(true);
                    Directory.Move($@"{SaveSamplesDir.Text}\Temp", $@"{SaveSamplesDir.Text}\Good");
                }
                else if (IsBadSample.IsChecked.Value)
                {
                    DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Bad");
                    List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                    for (int i = 0; i < samples.Count; i++)
                    {
                        Bitmap source = new Bitmap(samples[i].FullName);
                        Bitmap result = Extensions.CopyAsGrayscale(source);
                        source.Dispose();
                        result.Save($@"{SaveSamplesDir.Text}\Temp\{samples[i].Name}", ImageFormat.Bmp);
                        result.Dispose();
                    }
                    samples.Clear();
                    dir.Delete(true);
                    Directory.Move($@"{SaveSamplesDir.Text}\Temp", $@"{SaveSamplesDir.Text}\Bad");
                }
                MessageBox.Show("All samples converted!");
            }
        }

        private void RenameSamples_Click(object sender, RoutedEventArgs e)
        {
            if (!IsGoodSample.IsChecked.HasValue || !IsBadSample.IsChecked.HasValue)
                MessageBox.Show("Select the type of the sample!");
            else if (string.IsNullOrEmpty(SaveSamplesDir.Text))
                MessageBox.Show("Select a saving directory for the samples!");
            else
            {
                Directory.CreateDirectory($@"{SaveSamplesDir.Text}\Temp");
                if (IsGoodSample.IsChecked.Value)
                {
                    DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Good");
                    List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                    for (int i = 0; i < samples.Count; i++)
                    {
                        samples[i].CopyTo($@"{SaveSamplesDir.Text}\Temp\{i}.bmp");
                    }
                    samples.Clear();
                    dir.Delete(true);
                    Directory.Move($@"{SaveSamplesDir.Text}\Temp", $@"{SaveSamplesDir.Text}\Good");
                }
                else if (IsBadSample.IsChecked.Value)
                {
                    DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Bad");
                    List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                    for (int i = 0; i < samples.Count; i++)
                    {
                        samples[i].CopyTo($@"{SaveSamplesDir.Text}\Temp\{i}.bmp");
                    }
                    samples.Clear();
                    dir.Delete(true);
                    Directory.Move($@"{SaveSamplesDir.Text}\Temp", $@"{SaveSamplesDir.Text}\Bad");
                }
                MessageBox.Show("All samples renamed by the order!");
            }
        }

        private void RewriteMarkup_Click(object sender, RoutedEventArgs e)
        {
            if (IsGoodSample.IsChecked.Value)
            {
                DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Good");
                List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                if (File.Exists($@"{SaveSamplesDir.Text}\Good.dat"))
                    File.Delete($@"{SaveSamplesDir.Text}\Good.dat");
                for (int i = 0; i < samples.Count; i++)
                {
                    Bitmap image = new Bitmap(samples[i].FullName);
                    using (StreamWriter sw = new StreamWriter($@"{SaveSamplesDir.Text}\Good.dat", true))
                    {
                        sw.WriteLine($@"Good\{samples[i].Name} 1 0 0 {image.Width} {image.Height}");
                    }
                    image.Dispose();
                }
                samples.Clear();
            }
            else if (IsBadSample.IsChecked.Value)
            {
                DirectoryInfo dir = new DirectoryInfo($@"{SaveSamplesDir.Text}\Bad");
                List<FileInfo> samples = new List<FileInfo>(dir.GetFiles());
                if (File.Exists($@"{SaveSamplesDir.Text}\Bad.dat"))
                    File.Delete($@"{SaveSamplesDir.Text}\Bad.dat");
                for (int i = 0; i < samples.Count; i++)
                {
                    using (StreamWriter sw = new StreamWriter($@"{SaveSamplesDir.Text}\Bad.dat", true))
                    {
                        sw.WriteLine($@"{SaveSamplesDir.Text}\Bad\{samples[i].Name}");
                    }
                }
                samples.Clear();
            }
            MessageBox.Show("Markup file was rewritten!");
        }
    }
}