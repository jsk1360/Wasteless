using System.Collections.Generic;

namespace WastelessFunction.Functions.MenuApi;

public record Menu(string Name, IEnumerable<MenuItem> Items);