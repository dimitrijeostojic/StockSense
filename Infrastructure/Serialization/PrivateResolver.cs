using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Infrastructure.Serialization;

public sealed class PrivateResolver : DefaultContractResolver
{
    protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
    {
        JsonProperty jsonProperty = base.CreateProperty(member, memberSerialization);
        if (!jsonProperty.Writable)
        {
            var property = member as PropertyInfo;
            bool hasPrivateSetter = property?.GetSetMethod(true) != null;
            jsonProperty.Writable = hasPrivateSetter;
        }
        return jsonProperty;
    }

}