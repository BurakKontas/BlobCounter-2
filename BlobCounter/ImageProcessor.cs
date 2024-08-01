using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace BlobCounter
{
    public static class ImageProcessor
    {
        public static Bitmap GetMask(Bitmap image)
        {
            // Convert to grayscale
            Bitmap greyImage = ConvertToGrayscale(image);

            // Thresholding
            Bitmap mask = ApplyThreshold(greyImage, 120);

            return mask;
        }

        private static Bitmap ConvertToGrayscale(Bitmap image)
        {
            Bitmap greyImage = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixel = image.GetPixel(x, y);
                    int gray = (int)(0.3 * pixel.R + 0.59 * pixel.G + 0.11 * pixel.B);
                    greyImage.SetPixel(x, y, Color.FromArgb(gray, gray, gray));
                }
            }
            return greyImage;
        }

        private static Bitmap ApplyThreshold(Bitmap greyImage, int threshold)
        {
            Bitmap mask = new Bitmap(greyImage.Width, greyImage.Height);
            for (int y = 0; y < greyImage.Height; y++)
            {
                for (int x = 0; x < greyImage.Width; x++)
                {
                    Color pixel = greyImage.GetPixel(x, y);
                    int value = pixel.R;
                    Color newColor = value < threshold ? Color.White : Color.Black;
                    mask.SetPixel(x, y, newColor);
                }
            }
            return mask;
        }

        private static Bitmap GetNonWhitePixels(Bitmap image, Bitmap mask)
        {
            Bitmap nonWhitePixels = new Bitmap(image.Width, image.Height);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color imagePixel = image.GetPixel(x, y);
                    Color maskPixel = mask.GetPixel(x, y);
                    if (maskPixel.R == 255) // If white in mask
                    {
                        nonWhitePixels.SetPixel(x, y, imagePixel);
                    }
                    else
                    {
                        nonWhitePixels.SetPixel(x, y, Color.Black);
                    }
                }
            }
            return nonWhitePixels;
        }

        private static double[][] ConvertToData(Bitmap nonWhitePixels)
        {
            var width = nonWhitePixels.Width;
            var height = nonWhitePixels.Height;
            var data = new double[width * height][];
            int index = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color pixel = nonWhitePixels.GetPixel(x, y);
                    data[index] = new double[] { pixel.R }; // Assuming single channel (grayscale)
                    index++;
                }
            }

            return data;
        }

        private static Bitmap CreateClusteredImage(Bitmap image, KMeansCluster kmeans, Bitmap nonWhitePixels)
        {
            var width = image.Width;
            var height = image.Height;
            Bitmap clusteredImage = new Bitmap(width, height);

            int[] labels = kmeans.Labels;
            double[][] centers = kmeans.Centers;

            int index = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (nonWhitePixels.GetPixel(x, y).R == 255) // If white in nonWhitePixels
                    {
                        int clusterIdx = labels[index];
                        byte centerValue = (byte)centers[clusterIdx][0];
                        clusteredImage.SetPixel(x, y, Color.FromArgb(centerValue, centerValue, centerValue));
                    }
                    else
                    {
                        clusteredImage.SetPixel(x, y, Color.Black);
                    }
                    index++;
                }
            }

            return clusteredImage;
        }

        public static Bitmap ApplyMask(Bitmap mask, Bitmap originalImage)
        {
            return OverlayBitmaps(originalImage, mask);
        }

        private static Bitmap CreateMask(Bitmap clusteredImage)
        {
            Bitmap mask = new Bitmap(clusteredImage.Width, clusteredImage.Height);

            for (int i = 0; i < clusteredImage.Height; i++)
            {
                for (int j = 0; j < clusteredImage.Width; j++)
                {
                    Color pixel = clusteredImage.GetPixel(j, i);
                    // Maskede 0 ise, clusteredImage'de siyah
                    Color maskColor = pixel.R == 0 && pixel.G == 0 && pixel.B == 0 ? Color.FromArgb(0, 0, 0) : Color.FromArgb(255, 255, 255);
                    mask.SetPixel(j, i, maskColor);
                }
            }

            return mask;
        }

        public static Bitmap DilateMask(Bitmap mask, int kernelSize = 6, int iterations = 1)
        {
            Bitmap dilatedMask = new Bitmap(mask.Width, mask.Height);

            for (int iter = 0; iter < iterations; iter++)
            {
                Bitmap tempMask = new Bitmap(mask.Width, mask.Height);
                int halfKernel = kernelSize / 2;

                for (int y = 0; y < mask.Height; y++)
                {
                    for (int x = 0; x < mask.Width; x++)
                    {
                        if (mask.GetPixel(x, y).R == 255) // Assuming mask is binary (0 or 255)
                        {
                            for (int ky = -halfKernel; ky <= halfKernel; ky++)
                            {
                                for (int kx = -halfKernel; kx <= halfKernel; kx++)
                                {
                                    int nx = x + kx;
                                    int ny = y + ky;

                                    // Check bounds
                                    if (nx >= 0 && nx < mask.Width && ny >= 0 && ny < mask.Height)
                                    {
                                        tempMask.SetPixel(nx, ny, Color.FromArgb(255, 255, 255));
                                    }
                                }
                            }
                        }
                    }
                }

                mask = tempMask; // Update mask for the next iteration
            }

            return mask;
        }

        private static Bitmap OverlayBitmaps(Bitmap destinationBitmap, Bitmap sourceBitmap)
        {
            if (destinationBitmap.Width != sourceBitmap.Width || destinationBitmap.Height != sourceBitmap.Height)
                throw new ArgumentException("Source and destination bitmaps must have the same dimensions.");

            BitmapData destData = destinationBitmap.LockBits(new Rectangle(0, 0, destinationBitmap.Width, destinationBitmap.Height), ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            BitmapData srcData = sourceBitmap.LockBits(new Rectangle(0, 0, sourceBitmap.Width, sourceBitmap.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

            int bytesPerPixel = Image.GetPixelFormatSize(PixelFormat.Format32bppArgb) / 8;
            int height = destData.Height;
            int width = destData.Width;
            int stride = destData.Stride;
            IntPtr destPtr = destData.Scan0;
            IntPtr srcPtr = srcData.Scan0;

            unsafe
            {
                byte* dest = (byte*)destPtr;
                byte* src = (byte*)srcPtr;

                for (int y = 0; y < height; y++)
                {
                    byte* destRow = dest + (y * stride);
                    byte* srcRow = src + (y * stride);

                    for (int x = 0; x < width; x++)
                    {
                        // Source pixel indices
                        int srcIndex = x * bytesPerPixel;
                        byte blue = srcRow[srcIndex];
                        byte green = srcRow[srcIndex + 1];
                        byte red = srcRow[srcIndex + 2];
                        byte alpha = srcRow[srcIndex + 3];

                        // Check if the source pixel is white
                        if (red == 255 && green == 255 && blue == 255)
                        {
                            // Destination pixel indices
                            int destIndex = x * bytesPerPixel;
                            destRow[destIndex] = blue;       // Blue
                            destRow[destIndex + 1] = green;  // Green
                            destRow[destIndex + 2] = red;    // Red
                            destRow[destIndex + 3] = alpha;  // Alpha
                        }
                    }
                }
            }

            // Unlock the bits
            destinationBitmap.UnlockBits(destData);
            sourceBitmap.UnlockBits(srcData);

            return destinationBitmap;
        }

        public static byte[] ConvertBitmapToGrayscaleArray(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException(nameof(bitmap));

            int width = bitmap.Width;
            int height = bitmap.Height;
            byte[] grayscaleArray = new byte[width * height];

            BitmapData bmpData = bitmap.LockBits(new Rectangle(0, 0, width, height), ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb);
            IntPtr ptr = bmpData.Scan0;
            int bytes = bmpData.Stride * height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);
            bitmap.UnlockBits(bmpData);

            for (int i = 0; i < width * height; i++)
            {
                int index = i * 3;
                byte r = rgbValues[index];
                byte g = rgbValues[index + 1];
                byte b = rgbValues[index + 2];
                byte gray = (byte)((r + g + b) / 3);
                grayscaleArray[i] = gray;
            }

            return grayscaleArray;
        }

        public static byte[] CropImage(byte[] grey, int width, int height, int x, int y, int w, int h)
        {
            // Ensure bounding box is within image bounds
            if (x < 0) x = 0;
            if (y < 0) y = 0;
            if (x + w > width) w = width - x;
            if (y + h > height) h = height - y;

            // Create the cropped byte array
            byte[] cropArray = new byte[w * h];

            // Copy data from the original array to the cropped array
            for (int row = 0; row < h; row++)
            {
                for (int col = 0; col < w; col++)
                {
                    int srcIndex = (y + row) * width + (x + col);
                    int destIndex = row * w + col;
                    cropArray[destIndex] = grey[srcIndex];
                }
            }

            return cropArray;
        }

        public static (int, int, int, int) GetBoundingBox(List<(int, int)> contour, int width, int height)
        {
            int minX = width, minY = height, maxX = 0, maxY = 0;

            foreach (var (x, y) in contour)
            {
                if (x < minX) minX = x;
                if (x > maxX) maxX = x;
                if (y < minY) minY = y;
                if (y > maxY) maxY = y;
            }

            return (minX, minY, maxX - minX + 1, maxY - minY + 1);
        }

        public static unsafe Mat ConvertByteArrayToMat(byte[] greyArray, int width, int height)
        {
            if (greyArray.Length != width * height)
                throw new ArgumentException("Byte array size does not match the specified dimensions.");

            // Create a Mat with the correct dimensions and type
            Mat mat = new Mat(height, width, MatType.CV_8UC1);

            // Lock the Mat's data to get a pointer to it
            IntPtr matPtr = (IntPtr)mat.DataPointer;

            // Copy byte array data into the Mat
            Marshal.Copy(greyArray, 0, matPtr, greyArray.Length);

            return mat;
        }
    }
}