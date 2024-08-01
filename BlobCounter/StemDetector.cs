using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using BlobCounter;
using Point = OpenCvSharp.Point;
using Size = OpenCvSharp.Size;

public class StemDetector
{
    public static bool SapTespiti(byte[] imageData, int width, int height, double threshold)
    {
        // Mat nesnesini oluştur
        Mat img = ImageProcessor.ConvertByteArrayToMat(imageData, width, height);

        // Gürültü azaltma (Median filtresi)
        Mat imgBlurred = new Mat();
        Cv2.MedianBlur(img, imgBlurred, 5);

        // Otsu's thresholding ile eşikleme
        Mat thresh = new Mat();
        Cv2.Threshold(imgBlurred, thresh, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);

        // Morfolojik işlemler (erosion ve dilation)
        Mat kernel = Cv2.GetStructuringElement(MorphShapes.Rect, new Size(3, 3));
        Mat morphImg = new Mat();
        Cv2.Erode(thresh, morphImg, kernel, iterations: 1);
        Cv2.Dilate(morphImg, morphImg, kernel, iterations: 1);

        // Bağlantılı bileşen analizi
        Mat labels = new Mat();
        Mat stats = new Mat();
        Mat centroids = new Mat();
        int numLabels = Cv2.ConnectedComponentsWithStats(morphImg, labels, stats, centroids, PixelConnectivity.Connectivity4);

        // En büyük alanı kaplayan bileşeni bul
        int maxLabel = 1;
        int maxArea = (int)stats.At<int>(1, 4); // 4. sütun genellikle alanı temsil eder
        for (int i = 2; i < numLabels; i++)
        {
            int area = (int)stats.At<int>(i, 4); // 4. sütun genellikle alanı temsil eder
            if (area > maxArea)
            {
                maxLabel = i;
                maxArea = area;
            }
        }

        // Nesnenin merkezini bul
        double x = centroids.At<double>(maxLabel, 0);
        double y = centroids.At<double>(maxLabel, 1);

        // y değerini kontrol et ve görüntünün boyutlarına göre sınırla
        int imgHeight = img.Rows;
        y = Math.Min(y, imgHeight - 1);

        // Nesnenin üst kısmındaki ortalama yoğunluğu hesapla
        Rect upperHalfRect = new Rect(0, 0, img.Cols, (int)y);
        Mat upperHalf = new Mat(morphImg, upperHalfRect);
        Scalar avgUpper = Cv2.Mean(upperHalf);

        // Alt kısmındaki ortalama yoğunluğu hesapla
        Rect lowerHalfRect = new Rect(0, (int)y, img.Cols, imgHeight - (int)y);
        Mat lowerHalf = new Mat(morphImg, lowerHalfRect);
        Scalar avgLower = Cv2.Mean(lowerHalf);

        // Sap tespiti için basit bir eşikleme
        double thresholdRatio = threshold;  // Bu değer deneysel olarak ayarlanabilir
        double avgUpperValue = avgUpper.Val0;
        double avgLowerValue = avgLower.Val0;

        return avgUpperValue / avgLowerValue > thresholdRatio;
        
    }
    public bool IsStemPresent(byte[] imageData, int width, int height, int threshold, int threshold2, int threshold3, out List<Point> markedPoints)
    {
        // İşaretli noktaların listesini başlat
        markedPoints = new List<Point>();

        // Adım 1: Byte[] verisini 2D gri tonlamalı diziye dönüştür
        int[,] grayImage = new int[height, width];
        for (int i = 0; i < imageData.Length; i++)
        {
            int y = i / width;
            int x = i % width;
            grayImage[y, x] = imageData[i];
        }

        // Adım 2: Eşikleme ile binary görüntü oluştur
        bool[,] binaryImage = new bool[height, width];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                binaryImage[y, x] = grayImage[y, x] < threshold;
            }
        }

        // Adım 3: Bağlantı bileşenlerini bul
        int[,] labels = new int[height, width];
        int label = 1;
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (binaryImage[y, x] && labels[y, x] == 0)
                {
                    FloodFill(binaryImage, labels, x, y, label, markedPoints);
                    label++;
                }
            }
        }

        // Adım 4: Her bir bileşeni analiz et ve sapı tespit et
        for (int l = 1; l < label; l++)
        {
            int minX = width, minY = height, maxX = 0, maxY = 0;
            int sumGrayValue = 0;
            int pixelCount = 0;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (labels[y, x] == l)
                    {
                        minX = Math.Min(minX, x);
                        minY = Math.Min(minY, y);
                        maxX = Math.Max(maxX, x);
                        maxY = Math.Max(maxY, y);
                        sumGrayValue += grayImage[y, x];
                        pixelCount++;
                    }
                }
            }

            int avgGrayValue = sumGrayValue / pixelCount;

            if (avgGrayValue > threshold2 && (maxX - minX) < (width / 10) && (maxY - minY) > (height / 10))
            {
                return true; // Sap bulundu
            }
        }

        return false; // Sap bulunamadı
    }

    private void FloodFill(bool[,] binaryImage, int[,] labels, int x, int y, int label, List<Point> markedPoints)
    {
        int width = binaryImage.GetLength(1);
        int height = binaryImage.GetLength(0);
        if (x < 0 || x >= width || y < 0 || y >= height) return;
        if (!binaryImage[y, x] || labels[y, x] != 0) return;

        labels[y, x] = label;
        markedPoints.Add(new Point(x, y));

        FloodFill(binaryImage, labels, x + 1, y, label, markedPoints);
        FloodFill(binaryImage, labels, x - 1, y, label, markedPoints);
        FloodFill(binaryImage, labels, x, y + 1, label, markedPoints);
        FloodFill(binaryImage, labels, x, y - 1, label, markedPoints);
    }
}
