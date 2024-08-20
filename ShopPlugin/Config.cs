using Rocket.API;

public class Configuration : IRocketPluginConfiguration
{
    public string MySqlConnectionString;
    public string ItemTable;
    public string VehicleTable;

    public void LoadDefaults()
    {
        MySqlConnectionString = "server=localhost;port=3306;database=unturned;uid=root;password=;";
        ItemTable = "uconomyitemshop";
        VehicleTable = "uconomyvehicleshop";
    }
}
