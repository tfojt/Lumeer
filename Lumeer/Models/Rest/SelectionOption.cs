namespace Lumeer.Models.Rest
{
    public class SelectionOption
    {
        public string Value { get; set; }
        public string DisplayValue { get; set; }
        public string Background { get; set; }

        public string EffectiveValue => GetValue();

        public string GetValue() => DisplayValue ?? Value;

        public override string ToString() => GetValue();
    }
}
