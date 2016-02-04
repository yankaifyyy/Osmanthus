using Osmanthus.MachineLearning.Clustering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsmanthusTestConsole
{
    class Point
    {
        public double X;
        public double Y;

        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        static double Sqr(double x)
        {
            return x * x;
        }

        public static double Distance(Point a, Point b)
        {
            return Math.Sqrt(Sqr(a.X - b.X) + Sqr(a.Y - b.Y));
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var apcluster = new AffinityPropagation();

            apcluster.Settings.RandomNoise = true;
            apcluster.Settings.DampingFactor = 0.9;
            apcluster.Settings.Preference = AffinityPropagation.PreferenceChoice.Median;
            apcluster.Settings.MaxIterations = 1000;

            List<Point> plist = new List<Point>();

            plist.Add(new Point(0, 0));
            plist.Add(new Point(0, 1));
            plist.Add(new Point(1, 0));
            plist.Add(new Point(5, 0));
            plist.Add(new Point(5, 7.1));
            plist.Add(new Point(100, 0));
            plist.Add(new Point(100, 5));

            var labels = apcluster.Clustering(plist, Point.Distance);
            for (int i = 0; i < labels.Length; ++i)
                Console.WriteLine(labels[i]);
        }
    }
}
