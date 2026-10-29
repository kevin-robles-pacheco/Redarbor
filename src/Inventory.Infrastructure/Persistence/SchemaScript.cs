namespace Inventory.Infrastructure.Persistence;
public static class SchemaScript
{
    public static string Read()
    {
        var assembly = typeof(SchemaScript).Assembly;
        var resourceName = assembly.GetManifestResourceNames().Single(name => name.EndsWith("schema.sql", StringComparison.Ordinal));
        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream is null)
        {
            throw new InvalidOperationException("Schema script was not embedded.");
        }
        using var reader = new StreamReader(stream);

        return reader.ReadToEnd();
    }
}
