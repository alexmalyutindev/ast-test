using Newtonsoft.Json;

namespace AST.Tests;

public class TokenConverter : JsonConverter<Token>
{
    public override void WriteJson(JsonWriter writer, Token? value, JsonSerializer serializer)
    {
        serializer.Serialize(writer, value?.ToString());
    }

    public override Token? ReadJson(
        JsonReader reader,
        Type objectType,
        Token? existingValue,
        bool hasExistingValue,
        JsonSerializer serializer
    )
    {
        throw new NotImplementedException();
    }

    public override bool CanRead => false;
}