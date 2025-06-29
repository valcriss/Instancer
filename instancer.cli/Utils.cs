using System;

namespace instancer.cli;

public static class Utils
{
    public static int LevenshteinDistance(string a, string b)
    {
        if (string.IsNullOrEmpty(a)) return b.Length;
        if (string.IsNullOrEmpty(b)) return a.Length;
        var d = new int[a.Length + 1, b.Length + 1];
        for (int i = 0; i <= a.Length; i++) d[i, 0] = i;
        for (int j = 0; j <= b.Length; j++) d[0, j] = j;
        for (int i = 1; i <= a.Length; i++)
        {
            for (int j = 1; j <= b.Length; j++)
            {
                int cost = a[i - 1] == b[j - 1] ? 0 : 1;
                d[i, j] = Math.Min(
                    Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1),
                    d[i - 1, j - 1] + cost);
            }
        }
        return d[a.Length, b.Length];
    }

    public static string? Suggest(string input, IEnumerable<string> options)
    {
        int min = int.MaxValue;
        string? closest = null;
        foreach (var option in options)
        {
            int dist = LevenshteinDistance(input, option);
            if (dist < min)
            {
                min = dist;
                closest = option;
            }
        }
        if (closest == null || min > input.Length) return null;
        return closest;
    }
}
