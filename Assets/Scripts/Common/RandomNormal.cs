using System;

public class RandomNormal {
    private static readonly Random _rand = new();

    /// <summary>
    /// Generates a random normal with mu = 0, and standard deviation <c>std</c>
    /// </summary>
    /// <param name="std">Standard distribution.</param>
    public static double Random(double std) {
        double u1 = 1.0 - _rand.NextDouble();
        double u2 = 1.0 - _rand.NextDouble();
        double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

        return normal * std;
    }
}