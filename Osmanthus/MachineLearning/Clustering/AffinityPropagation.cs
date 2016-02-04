using System;
using System.Collections.Generic;
using System.Linq;

namespace Osmanthus.MachineLearning.Clustering
{
    public class AffinityPropagation : IClustering
    {
        public enum PreferenceChoice
        {
            Median, Min, Max, Average, Constant
        }
        public class APSettings
        {
            public int MaxIterations = 100;
            public double DampingFactor = 0.9;
            public PreferenceChoice Preference = PreferenceChoice.Median;

            /// <summary>
            /// If preference choice method is constant, this field should be set
            /// </summary>
            public double ConstantPreference = -1;

            /// <summary>
            /// Add random noise to prevent `distance` = 0
            /// </summary>
            public bool RandomNoise = false;

            /// <summary>
            /// Max random noise
            /// </summary>
            public double NoiseScale = 1e-8;

            /// <summary>
            /// Use Time-dependent random seed if RandomSeed %lt; 0,
            /// or use a certain random seed so that the effect of each run is consistent
            /// </summary>
            public int RandomSeed = -1;
        }

        public APSettings Settings { get; set; } = new APSettings();

        public int[] Clustering(int n, Func<int, int, double> distFunc)
        {
            int[] labels = new int[n];

            Random rand = null;
            bool noise = Settings.RandomNoise;

            double df = Settings.DampingFactor;
            int maxIters = Settings.MaxIterations;

            if (noise)
            {
                if (Settings.RandomSeed >= 0)
                    rand = new Random(Settings.RandomSeed);
                else
                    rand = new Random();
            }

            double[,] S = new double[n, n],
                R = new double[n, n],
                A = new double[n, n],
                E = new double[n, n];
            double[] SR = new double[n];

            List<double> vals = new List<double>();

            for (int i = 0; i < n; ++i)
                for (int j = 0; j < n; ++j)
                {
                    double s = -distFunc(i, j);
                    if (noise)
                        s -= rand.NextDouble() * Settings.NoiseScale;
                    S[i, j] = s;
                    vals.Add(s);
                }

            vals.Sort();
            double preference = 0;
            switch (Settings.Preference)
            {
                case PreferenceChoice.Constant:
                    preference = Settings.ConstantPreference;
                    break;
                case PreferenceChoice.Min:
                    preference = vals[0];
                    break;
                case PreferenceChoice.Max:
                    preference = vals.Last();
                    break;
                case PreferenceChoice.Median:
                    if ((vals.Count & 1) == 0)
                        preference = 0.5 * (vals[vals.Count >> 1] + vals[(vals.Count - 1) >> 1]);
                    else
                        preference = vals[(vals.Count >> 1)];
                    break;
                case PreferenceChoice.Average:
                    preference = vals.Average();
                    break;
                default:
                    throw new ArgumentException("Error preference choice");
            }


            for (int i = 0; i < n; ++i)
                S[i, i] = preference;

            for (int iter = 0; iter < maxIters; ++iter)
            {
                #region Update Information
                for (int i = 0; i < n; ++i)
                {
                    double max = double.NegativeInfinity, max2 = double.NegativeInfinity;
                    int maxId = 0;

                    for (int k = 0; k < n; ++k)
                    {
                        double asx = A[i, k] + S[i, k];
                        if (asx > max)
                        {
                            max2 = max;
                            max = asx;
                            maxId = k;
                        }
                        else if (asx > max2)
                        {
                            max2 = asx;
                        }
                    }
                    for (int k = 0; k < n; ++k)
                    {
                        double m = (k == maxId ? max2 : max);
                        R[i, k] = df * R[i, k] + (1 - df) * (S[i, k] - m);
                    }
                }

                for (int k = 0; k < n; ++k)
                {
                    double sum = 0;
                    for (int i = 0; i < n; ++i)
                        if (R[i, k] > 0)
                            sum += R[i, k];
                    SR[k] = sum;
                }

                for (int i = 0; i < n; ++i)
                {
                    for (int k = 0; k < n; ++k)
                    {
                        double aik = 0;
                        double rkk = Math.Max(0, R[k, k]);
                        if (i != k)
                        {
                            double rik = Math.Max(0, R[i, k]);
                            aik = Math.Min(0, R[k, k] + SR[k] - rik - rkk);
                        }
                        else
                        {
                            aik = SR[k] - rkk;
                        }
                        A[i, k] = df * A[i, k] + (1 - df) * aik;
                    }
                }
                #endregion
            }

            #region Select Exemplars
            List<int> centers = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                if (R[i, i] + A[i, i] > 0)
                    centers.Add(i);
            }

            for (int i = 0; i < n; ++i)
            {
                double maxS = double.NegativeInfinity;
                int lb = 0;
                foreach (var c in centers)
                {
                    double s = S[i, c];
                    if (s > maxS)
                    {
                        maxS = s;
                        lb = c;
                    }
                }
                labels[i] = lb;
            }
            #endregion

            return labels;
        }

        public int[] Clustering(double[,] distMatrix)
        {
            int nr = distMatrix.GetLength(0);
            Func<int, int, double> distFunc = (i, j) =>
            {
                return distMatrix[i, j];
            };

            return Clustering(nr, distFunc);
        }

        public int[] Clustering<T>(IList<T> data, Func<T, T, double> distFunc)
        {
            int nr = data.Count;
            Func<int, int, double> func = (i, j) =>
            {
                return distFunc(data[i], data[j]);
            };

            return Clustering(nr, func);
        }
    }
}
