using CargoApp.Utilities;

namespace CargoApp.ViewModels;

public class InputViewModel : InputViewModelBase
{
    public InputField[] Fields { get; }
    public List<string> Results => Fields.Select(v => v.DefaultValue).ToList();

    public InputViewModel(string title, bool canOK = false, bool canCancel = false, params InputField[] fields) 
        : base(title, canOK, canCancel)
    {
        Fields = fields;
    }
}