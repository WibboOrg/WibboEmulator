namespace WibboEmulator.Database;

public class DatabaseConfiguration
{
    public string Hostname { get; set; }
    public string Name { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public uint Port { get; set; }
    public uint MinimumPoolSize { get; set; }
    public uint MaximumPoolSize { get; set; }
}
