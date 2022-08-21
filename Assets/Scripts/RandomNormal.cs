using System;

public class RandomNormal {
    private static readonly Random rand = new();

    public static double Random(double std) {
        double u1 = 1.0 - rand.NextDouble();
        double u2 = 1.0 - rand.NextDouble();
        double normal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);
        return normal * std;
    }
}
