using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class EnumToStringConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : Enum
{
    public EnumToStringConverter() : base(
        v => v.ToString(),
        v => (TEnum)Enum.Parse(typeof(TEnum), v),
        new ConverterMappingHints(size: 50))
    {
    }
}