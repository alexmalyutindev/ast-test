using AST.Nodes;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace AST.Tests;

class AstContractResolver : DefaultContractResolver
{
    protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
    {
        var members = GetSerializableMembers(type);
        if (members == null)
        {
            throw new JsonSerializationException("Null collection of serializable members returned.");
        }

        var properties = new JsonPropertyCollection(type);

        foreach (var member in members)
        {
            var property = CreateProperty(member, memberSerialization);
            
            if (property.UnderlyingName == "Token")
            {
                properties.Insert(0, property);
            }
            else
            {
                properties.AddProperty(property);
            }
        }

        return properties;
    }
}