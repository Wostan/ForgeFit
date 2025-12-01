using System.Text.Json;
using System.Text.Json.Serialization;

namespace ForgeFit.Infrastructure.Services.FatSecret.Converters;

public class SingleOrArrayConverter<T> : JsonConverter<List<T>>
{
    public override List<T>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        switch (reader.TokenType)
        {
            case JsonTokenType.StartArray:
            {
                return JsonSerializer.Deserialize<List<T>>(ref reader, options);
            }
            case JsonTokenType.StartObject:
            {
                var item = JsonSerializer.Deserialize<T>(ref reader, options);
                return item is not null ? [item] : null;
            }
            default:
                return null;
        }
    }

    public override void Write(Utf8JsonWriter writer, List<T> value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, options);
    }
}
