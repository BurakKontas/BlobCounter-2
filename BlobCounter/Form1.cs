using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using MathNet.Numerics.LinearAlgebra;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using OpenCvSharp.ML;
using OpenCvSharp.WpfExtensions;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

namespace BlobCounter
{
    public partial class Form1 : Form
    {
        private Bitmap _defaultImage;

        public Form1()
        {
            InitializeComponent();
            _defaultImage = (Bitmap)pictureBox1.Image;

        }

        private void Stage1_Button_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)_defaultImage.Clone();

            byte[] grey = ImageProcessor.ConvertBitmapToGrayscaleArray(bitmap);

            int width = 1900;
            int height = 128;

            Mat image = ImageProcessor.ConvertByteArrayToMat(grey, width, height);

            Mat edges = new Mat();
            Cv2.Canny(image, edges, 1, 500);

            Cv2.FindContours(edges, out var contours, out _, RetrievalModes.External, ContourApproximationModes.ApproxSimple);

            Parallel.For(0, contours.Length, i =>
            {
                Rect bbox = Cv2.BoundingRect(contours[i]);

                Bitmap crop = new Mat(image, bbox).ToBitmap();
                Bitmap mask = ImageProcessor.GetMask(crop);
                Bitmap dilatedMask = ImageProcessor.DilateMask(mask, (int)threshold_1.Value, (int)threshold_2.Value);
                Bitmap appliedMask = ImageProcessor.ApplyMask(dilatedMask, crop); //return this values
            });


            //int bboxCounter = 1;
            //for (int i = 0; i < contours.Length; i++)
            //{
            //    Rect bbox = Cv2.BoundingRect(contours[i]);

            //    Bitmap crop = new Mat(image, bbox).ToBitmap();

            //    Bitmap clone = (Bitmap)crop.Clone();
            //    Bitmap mask = ImageProcessor.GetMask(crop);
            //    Bitmap dilatedMask = ImageProcessor.DilateMask(mask, (int)threshold_1.Value, (int)threshold_2.Value);
            //    Bitmap appliedMask = ImageProcessor.ApplyMask(dilatedMask, crop);

            //    appliedMask = ApplyRedToNonWhitePixels(appliedMask);

            //    appliedMask.Save($"C:\\Users\\abura\\Desktop\\Staj\\BBoxes\\{bboxCounter}_masked.png");
            //    clone.Save($"C:\\Users\\abura\\Desktop\\Staj\\BBoxes\\{bboxCounter}_crop.png");
            //    bboxCounter++;
            //}


            bitmap.Dispose();
        } // 290 ms


        private static Bitmap ApplyRedToNonWhitePixels(Bitmap image)
        {
            Bitmap newBitmap = new Bitmap(image.Width, image.Height);

            // Lock the bitmap's bits.
            Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
            BitmapData bitmapData = image.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bitmapData.Scan0;
            int bytes = Math.Abs(bitmapData.Stride) * image.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            image.UnlockBits(bitmapData);

            // Process the image data
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int index = (y * bitmapData.Stride) + (x * 3);

                    byte blue = rgbValues[index];
                    byte green = rgbValues[index + 1];
                    byte red = rgbValues[index + 2];

                    if (red == 255 && green == 255 && blue == 255)
                    {
                        // Pixel is white, no change
                        continue;
                    }
                    else
                    {
                        // Set pixel to red
                        rgbValues[index] = 0;      // Blue
                        rgbValues[index + 1] = 0;  // Green
                        rgbValues[index + 2] = 255; // Red
                    }
                }
            }

            // Create a new Bitmap and set the modified pixel data
            Bitmap newBitmapWithRed = new Bitmap(image.Width, image.Height, PixelFormat.Format24bppRgb);
            BitmapData newBitmapData = newBitmapWithRed.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            IntPtr newPtr = newBitmapData.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, newPtr, bytes);
            newBitmapWithRed.UnlockBits(newBitmapData);

            return newBitmapWithRed;
        }
        private bool IsOne(Bitmap bitmap, double thresholdPercentage = 15.25)
        {
            // Bitmap'i OpenCV Mat formatına dönüştür
            Mat mat = bitmap.ToMat();
            if (mat.Empty())
                throw new ArgumentException("Invalid image.");

            // Görüntüyü gri tonlamaya dönüştür
            Mat gray = new Mat();
            Cv2.CvtColor(mat, gray, ColorConversionCodes.BGR2GRAY);

            // Beyaz olmayan piksellerin konumlarını bul
            Mat mask = new Mat();
            Cv2.InRange(gray, new Scalar(0), new Scalar(254), mask);

            // Beyaz olmayan piksellerin konumlarını al
            Point[] nonWhitePoints;
            using (var output = new Mat())
            {
                Cv2.FindNonZero(mask, output);
                nonWhitePoints = ConvertMatToPoints(output);
            }

            if (nonWhitePoints.Length == 0)
            {
                return false;
            }

            // Koordinatları ayır
            double[] xCoords = nonWhitePoints.Select(p => (double)p.X).ToArray();
            double[] yCoords = nonWhitePoints.Select(p => (double)p.Y).ToArray();

            // Lineer regresyon için matrisleri oluştur
            var X = Matrix<double>.Build.DenseOfRowArrays(xCoords.Select(x => new[] { x, 1.0 }).ToArray());
            var Y = Matrix<double>.Build.DenseOfColumnArrays(yCoords.Select(y => new[] { y }).ToArray());

            // Lineer regresyon hesaplamaları
            (double m, double c) = FitLine(xCoords, yCoords);

            // Fotoğrafın boyutlarını al
            int width = mat.Width;
            int height = mat.Height;

            // Doğrunun üzerindeki beyaz pikselleri say
            int numWhiteOnLine = 0;
            int totalOnLine = 0;

            for (int x = 0; x < width; x++)
            {
                int y = (int)(m * x + c);
                if (y >= 0 && y < height)
                {
                    if (gray.At<byte>(y, x) == 255)
                    {
                        numWhiteOnLine++;
                    }
                    totalOnLine++;
                }
            }

            // Beyaz piksel yüzdesini hesapla
            double whitePixelPercentage = totalOnLine > 0 ? (numWhiteOnLine / (double)totalOnLine) * 100 : 0;

            // Eşik değerine göre boolean döndür
            return whitePixelPercentage <= thresholdPercentage;
        }

        private (double m, double c) FitLine(double[] xCoords, double[] yCoords)
        {
            if (xCoords.Length != yCoords.Length || xCoords.Length == 0)
                throw new ArgumentException("xCoords and yCoords must have the same non-zero length.");

            int n = xCoords.Length;
            double xMean = xCoords.Average();
            double yMean = yCoords.Average();

            double xySum = xCoords.Zip(yCoords, (x, y) => (x - xMean) * (y - yMean)).Sum();
            double xxSum = xCoords.Select(x => (x - xMean) * (x - xMean)).Sum();

            double m = xySum / xxSum;
            double c = yMean - m * xMean;

            return (m, c);
        }

        private Point[] ConvertMatToPoints(Mat mat)
        {
            var points = new Point[mat.Rows];
            for (int i = 0; i < mat.Rows; i++)
            {
                points[i] = new Point(mat.At<Vec2i>(i)[0], mat.At<Vec2i>(i)[1]);
            }
            return points;
        }

        private void Upload_Button_Click(object sender, EventArgs e)
        {
            UploadImage(pictureBox1);
            _defaultImage = (Bitmap)pictureBox1.Image;
        }

        private void UploadImage(PictureBox pictureBox, bool resizeBox = false)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg, *.jpeg, *.png, *.gif, *.bmp)|*.jpg; *.jpeg; *.png; *.gif; *.bmp|All files (*.*)|*.*",
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                var selectedFile = openFileDialog.FileName;

                pictureBox.Image = Image.FromFile(selectedFile);

                if (resizeBox) pictureBox.SizeMode = PictureBoxSizeMode.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Resim yüklenirken bir hata oluştu: " + ex.Message);
            }
        }

    }
}
