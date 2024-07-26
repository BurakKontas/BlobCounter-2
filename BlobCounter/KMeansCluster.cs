using System;
using System.Linq;

namespace BlobCounter
{
    public class KMeansCluster
    {
        public int K { get; set; }
        public double[][] Centers { get; private set; }
        public int[] Labels { get; private set; }
        private Random random = new Random();

        public KMeansCluster(int k)
        {
            K = k;
        }

        public void Fit(double[][] data)
        {
            int numPoints = data.Length;
            int numFeatures = data[0].Length;

            // Initialize centers using K-Means++
            Centers = InitializeCenters(data, numFeatures);

            bool converged = false;
            int maxIterations = 100;
            int iteration = 0;

            while (!converged && iteration < maxIterations)
            {
                // Assign labels
                Labels = AssignLabels(data);

                // Compute new centers
                double[][] newCenters = ComputeCenters(data);

                // Check convergence
                converged = Centers.Select((center, i) => center.SequenceEqual(newCenters[i])).All(equal => equal);

                Centers = newCenters;
                iteration++;
            }
        }

        private double[][] InitializeCenters(double[][] data, int numFeatures)
        {
            double[][] centers = new double[K][];
            bool[] chosen = new bool[data.Length];

            // Choose the first center randomly
            int firstIndex = random.Next(data.Length);
            centers[0] = data[firstIndex];
            chosen[firstIndex] = true;

            for (int i = 1; i < K; i++)
            {
                // Compute distances from data points to the nearest center
                double[] minDistances = new double[data.Length];
                for (int j = 0; j < data.Length; j++)
                {
                    minDistances[j] = data.Select(center => ComputeDistance(data[j], center)).Min();
                }

                // Choose the next center based on weighted probability
                double totalDistance = minDistances.Sum();
                double[] probabilities = minDistances.Select(d => d / totalDistance).ToArray();
                int nextIndex = ChooseIndex(probabilities);
                centers[i] = data[nextIndex];
                chosen[nextIndex] = true;
            }

            return centers;
        }

        private int ChooseIndex(double[] probabilities)
        {
            double rand = random.NextDouble();
            double cumulative = 0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                cumulative += probabilities[i];
                if (rand < cumulative)
                {
                    return i;
                }
            }
            return probabilities.Length - 1;
        }

        private int[] AssignLabels(double[][] data)
        {
            int[] labels = new int[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                double minDistance = double.MaxValue;
                int label = 0;

                for (int j = 0; j < K; j++)
                {
                    double distance = ComputeDistance(data[i], Centers[j]);
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        label = j;
                    }
                }
                labels[i] = label;
            }
            return labels;
        }

        private double[][] ComputeCenters(double[][] data)
        {
            double[][] newCenters = new double[K][];
            int[] counts = new int[K];

            for (int i = 0; i < K; i++)
            {
                newCenters[i] = new double[data[0].Length];
            }

            for (int i = 0; i < data.Length; i++)
            {
                int label = Labels[i];
                for (int j = 0; j < data[i].Length; j++)
                {
                    newCenters[label][j] += data[i][j];
                }
                counts[label]++;
            }

            for (int i = 0; i < K; i++)
            {
                if (counts[i] > 0)
                {
                    for (int j = 0; j < newCenters[i].Length; j++)
                    {
                        newCenters[i][j] /= counts[i];
                    }
                }
            }

            return newCenters;
        }

        private double ComputeDistance(double[] point1, double[] point2)
        {
            double sum = 0;
            for (int i = 0; i < point1.Length; i++)
            {
                double diff = point1[i] - point2[i];
                sum += diff * diff;
            }
            return Math.Sqrt(sum);
        }
    }

}