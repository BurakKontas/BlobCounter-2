using System;
using System.Collections.Generic;
using System.Drawing;

namespace BlobCounter
{
    public unsafe class SalatalikSapBulma
    {
        const byte BackgroundColor = 255;
        private int SalatalikThreshold;
        private int SapThreshold;
        private int[] parent;
        private int[] size;
        private int[][] components;
        private byte* image;
        private int width, height;

        // Bölge büyüklüğü eşiği
        private int MinimumAreaThreshold;

        public List<int[]> SapliSalataliklariBul(byte[] img, int w, int h, int salatalikThreshold, int alanEsigi)
        {
            MinimumAreaThreshold = alanEsigi;
            width = w;
            height = h;
            SalatalikThreshold = salatalikThreshold;

            fixed (byte* pImg = img)
            {
                image = pImg;
                InitializeUnionFind(width * height);

                // Initialize components as a jagged array
                components = new int[width * height][];
                for (int i = 0; i < width * height; i++)
                {
                    components[i] = new int[0];
                }

                // Union-Find ile tüm nesneleri bul
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        int idx = y * width + x;
                        if (image[idx] < BackgroundColor - SalatalikThreshold)
                        {
                            // Sağdaki piksel ile birleştir
                            if (x < width - 1 && image[idx + 1] < BackgroundColor - SalatalikThreshold)
                            {
                                Union(idx, idx + 1);
                            }
                            // Aşağıdaki piksel ile birleştir
                            if (y < height - 1 && image[idx + width] < BackgroundColor - SalatalikThreshold)
                            {
                                Union(idx, idx + width);
                            }
                        }
                    }
                }


                // Komponentleri oluştur
                var componentDict = new Dictionary<int, List<int>>();

                for (int i = 0; i < width * height; i++)
                {
                    if (image[i] < BackgroundColor - SalatalikThreshold)
                    {
                        int root = Find(i);
                        if (!componentDict.ContainsKey(root))
                        {
                            componentDict[root] = new List<int>();
                        }
                        componentDict[root].Add(i);
                    }
                }

                List<int[]> sapliSalatalikKomponentleri = new List<int[]>();

                foreach (var component in componentDict.Values)
                {
                    if (component.Count > MinimumAreaThreshold)
                    {
                        sapliSalatalikKomponentleri.Add(component.ToArray());
                    }
                }

                // Copy results to components array
                components = sapliSalatalikKomponentleri.ToArray();

                return sapliSalatalikKomponentleri;
            }
        }

        private void InitializeUnionFind(int n)
        {
            parent = new int[n];
            size = new int[n];
            for (int i = 0; i < n; i++)
            {
                parent[i] = i;
                size[i] = 1;
            }
        }

        private int Find(int x)
        {
            if (parent[x] != x)
            {
                parent[x] = Find(parent[x]);
            }
            return parent[x];
        }

        private void Union(int x, int y)
        {
            int rootX = Find(x);
            int rootY = Find(y);

            if (rootX != rootY)
            {
                if (size[rootX] < size[rootY])
                {
                    parent[rootX] = rootY;
                    size[rootY] += size[rootX];
                }
                else
                {
                    parent[rootY] = rootX;
                    size[rootX] += size[rootY];
                }
            }
        }

        public static void CemberCiz(Graphics g, int x, int y, int radius, Color color)
        {
            using (Pen pen = new Pen(color, 2))
            {
                g.DrawEllipse(pen, x - radius, y - radius, radius * 2, radius * 2);
            }
        }
    }
}
