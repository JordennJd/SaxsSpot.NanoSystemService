namespace SaxsSpot.NanoSystemService.Contracts.ExternalDto;

public class PlotRequest
{
    public string title { get; set; } = string.Empty;
    public string x_label { get; set; } = string.Empty;
    public string y_label { get; set; } = string.Empty;
    public bool x_log_scale { get; set; }
    public bool y_log_scale { get; set; }
    public Dataset[] datasets { get; set; } = [];
}

public class Dataset
{
    public string id { get; set; } = string.Empty;
    public double[] x { get; set; } = [];
    public double[] y { get; set; } = [];

    public void SortByX()
    {
        if (x.Length != y.Length)
        {
            throw new InvalidOperationException("Length of x and y arrays must be equal.");
        }

        var n = x.Length;
        var indices = Enumerable.Range(0, n).ToArray();
        Array.Sort(indices, (a, b) => x[a].CompareTo(x[b]));

        var xSorted = indices.Select(i => x[i]).ToArray();
        var ySorted = indices.Select(i => y[i]).ToArray();
        Array.Copy(xSorted, x, n);
        Array.Copy(ySorted, y, n);
    }
}
