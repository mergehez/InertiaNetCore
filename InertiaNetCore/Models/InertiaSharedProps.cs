namespace InertiaNetCore.Models;

internal class InertiaSharedProps
{
    private InertiaProps? Data { get; set; }

    public InertiaProps? GetData() => Data;

    public void Merge(InertiaProps with)
    {
        Data = (Data ?? []).Merge(with);
    }

    public void Set(string key, object? value)
    {
        Data ??= new InertiaProps();
        Data[key] = value;
    }
}
