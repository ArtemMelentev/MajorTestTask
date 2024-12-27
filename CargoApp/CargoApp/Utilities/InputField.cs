namespace CargoApp.Utilities;

public class InputField
{
    public string? Value { get; set; }
    public string? Name { get; set; }

    public InputField(string? value = null, string? name = null)
    {
        Value = value;
        Name = name;
    }
}