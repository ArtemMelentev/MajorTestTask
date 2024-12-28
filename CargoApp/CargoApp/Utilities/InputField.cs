namespace CargoApp.Utilities;

public class InputField
{
    public string? DefaultValue { get; set; }
    public string? Name { get; set; }

    public InputField(string? defaultValue = null, string? name = null)
    {
        DefaultValue = defaultValue;
        Name = name;
    }
}