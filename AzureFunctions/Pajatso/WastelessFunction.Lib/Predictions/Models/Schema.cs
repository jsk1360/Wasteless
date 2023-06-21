using System.Collections.Generic;

namespace WastelessFunction.Functions.Predictions.Models;

public class Schema
{
    public List<Field> Fields { get; set; }
    public string PandasVersion { get; set; }
}