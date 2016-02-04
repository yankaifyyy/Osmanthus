using System;
using System.Collections.Generic;

namespace Osmanthus.MachineLearning.Clustering
{
    public interface IClustering
    {
        int[] Clustering(int n, Func<int, int, double> distFunc);
        int[] Clustering(double[,] distMatrix);
        int[] Clustering<T>(IList<T> data, Func<T, T, double> distFunc);
    }
}
