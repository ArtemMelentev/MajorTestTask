namespace CargoApp.Utilities;

public class InputField
{
    public string DefaultValue { get; set; }
    public string? Name { get; set; }

    public InputField(string name = "", string defaultValue = "")
    {
        DefaultValue = defaultValue;
        Name = name;
    }
}