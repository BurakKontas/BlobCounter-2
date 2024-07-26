using System.Collections.Generic;

namespace BlobCounter
{
    public static class ContourDetection
    {
        private static readonly (int, int)[] Directions =
        {
            (0, 1), (1, 1), (1, 0), (1, -1),
            (0, -1), (-1, -1), (-1, 0), (-1, 1)
        };

        public static List<List<(int, int)>> FindContours(byte[] edgeArray, int width, int height)
        {
            var visited = new bool[width * height];
            var contours = new List<List<(int, int)>>();

            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    if (edgeArray[y * width + x] == 255 && !visited[y * width + x])
                    {
                        var contour = TraceContour(x, y, edgeArray, width, height, visited);
                        contours.Add(contour);
                    }
                }
            }

            return contours;
        }

        private static List<(int, int)> TraceContour(int startX, int startY, byte[] edgeArray, int width, int height, bool[] visited)
        {
            var contour = new List<(int, int)>();
            var stack = new Stack<(int, int)>();
            stack.Push((startX, startY));

            while (stack.Count > 0)
            {
                var (x, y) = stack.Pop();
                if (visited[y * width + x]) continue;

                visited[y * width + x] = true;
                contour.Add((x, y));

                foreach (var (dx, dy) in Directions)
                {
                    int newX = x + dx;
                    int newY = y + dy;

                    if (IsValid(newX, newY, width, height) &&
                        edgeArray[newY * width + newX] == 255 &&
                        !visited[newY * width + newX])
                    {
                        stack.Push((newX, newY));
                    }
                }
            }

            return contour;
        }

        private static bool IsValid(int x, int y, int width, int height)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }
    }
}