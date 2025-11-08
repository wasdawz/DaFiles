using System;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DaFiles.Helpers;

/// <summary>
/// <para>Serializes <see cref="SecureString"/> as <see langword="bool"/> indicating
/// whether the string was non-empty or not.</para>
/// <para>Deserializes <see langword="true"/> to a new instance of <see cref="SecureString"/>,
/// and <see langword="false"/> to <see langword="null"/>.</para>
/// </summary>
public class SecretJsonConverter : JsonConverter<SecureString?>
{
    public override SecureString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetBoolean() ? new() : null;
    }

    public override void Write(Utf8JsonWriter writer, SecureString? value, JsonSerializerOptions options)
    {
        writer.WriteBooleanValue((value?.Length) > 0);
    }
}
