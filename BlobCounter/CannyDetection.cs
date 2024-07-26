using System;

namespace BlobCounter
{
    public static class CannyEdgeDetection
    {
        private static byte[] SobelX = { unchecked((byte)-1), 0, 1, unchecked((byte)-2), 0, 2, unchecked((byte)-1), 0, 1 };
        private static byte[] SobelY = { unchecked((byte)-1), unchecked((byte)-2), unchecked((byte)-1), 0, 0, 0, 1, 2, 1 };

        public static byte[] ApplyCannyEdgeDetection(byte[] grayscaleArray, int width, int height, double lowThreshold, double highThreshold)
        {
            // Step 1: Gradyan hesaplama
            double[,] gradientX = new double[height, width];
            double[,] gradientY = new double[height, width];
            double[,] magnitude = new double[height, width];
            double[,] direction = new double[height, width];

            // Apply Sobel filters
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double gx = 0;
                    double gy = 0;

                    // Apply Sobel operators
                    for (int ky = -1; ky <= 1; ky++)
                    {
                        for (int kx = -1; kx <= 1; kx++)
                        {
                            int pixel = grayscaleArray[(y + ky) * width + (x + kx)];
                            gx += pixel * SobelX[(ky + 1) * 3 + (kx + 1)];
                            gy += pixel * SobelY[(ky + 1) * 3 + (kx + 1)];
                        }
                    }

                    gradientX[y, x] = gx;
                    gradientY[y, x] = gy;
                    magnitude[y, x] = Math.Sqrt(gx * gx + gy * gy);
                    direction[y, x] = Math.Atan2(gy, gx) * 180 / Math.PI;
                }
            }

            // Step 2: Non-Maximum Suppression
            byte[] edgeArray = new byte[width * height];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double angle = direction[y, x];
                    if (angle < 0)
                        angle += 180;

                    double mag = magnitude[y, x];
                    bool isEdge = false;

                    // Determine neighboring pixels based on the direction
                    if ((angle >= 0 && angle < 22.5) || (angle >= 157.5 && angle <= 180))
                    {
                        isEdge = mag > magnitude[y, x - 1] && mag > magnitude[y, x + 1];
                    }
                    else if (angle >= 22.5 && angle < 67.5)
                    {
                        isEdge = mag > magnitude[y - 1, x + 1] && mag > magnitude[y + 1, x - 1];
                    }
                    else if (angle >= 67.5 && angle < 112.5)
                    {
                        isEdge = mag > magnitude[y - 1, x] && mag > magnitude[y + 1, x];
                    }
                    else if (angle >= 112.5 && angle < 157.5)
                    {
                        isEdge = mag > magnitude[y - 1, x - 1] && mag > magnitude[y + 1, x + 1];
                    }

                    edgeArray[y * width + x] = isEdge ? (byte)255 : (byte)0;
                }
            }

            // Step 3: Histerezis eşikleme
            byte[] finalEdges = new byte[width * height];
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    double mag = magnitude[y, x];
                    if (edgeArray[y * width + x] == 255)
                    {
                        if (mag >= highThreshold)
                        {
                            finalEdges[y * width + x] = 255;
                        }
                        else if (mag >= lowThreshold)
                        {
                            // Check if it's connected to a strong edge
                            bool connectedToStrongEdge = false;
                            for (int ky = -1; ky <= 1; ky++)
                            {
                                for (int kx = -1; kx <= 1; kx++)
                                {
                                    if (edgeArray[(y + ky) * width + (x + kx)] == 255)
                                    {
                                        connectedToStrongEdge = true;
                                        break;
                                    }
                                }
                                if (connectedToStrongEdge) break;
                            }
                            finalEdges[y * width + x] = connectedToStrongEdge ? (byte)255 : (byte)0;
                        }
                        else
                        {
                            finalEdges[y * width + x] = 0;
                        }
                    }
                    else
                    {
                        finalEdges[y * width + x] = 0;
                    }
                }
            }

            return finalEdges;
        }
    }
}
