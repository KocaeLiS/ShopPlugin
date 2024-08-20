using Rocket.API;
using Rocket.Core.Plugins;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using Steamworks;
using System.Data;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

public class ShopPlugins : RocketPlugin<Configuration>
{
    public static ShopPlugins Instance { get; private set; }
    private MySqlConnection connection;

    protected override void Load()
    {
        Instance = this;

        connection = new MySqlConnection(Configuration.Instance.MySqlConnectionString);
        try
        {
            connection.Open();
            Rocket.Core.Logging.Logger.Log("Shop Plugin loaded!");
        }
        catch (MySqlException ex)
        {
            Rocket.Core.Logging.Logger.LogError($"Failed to connect to database: {ex.Message}");
        }
    }

    protected override void Unload()
    {
        if (connection != null && connection.State == ConnectionState.Open)
        {
            connection.Close();
        }
        Rocket.Core.Logging.Logger.Log("Shop Plugin unloaded!");
    }

    public decimal GetItemCost(string itemNameOrId)
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
            return -1;
        }

        string query = $"SELECT cost FROM {Configuration.Instance.ItemTable} WHERE id = @id OR itemname LIKE @name LIMIT 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", itemNameOrId);
        cmd.Parameters.AddWithValue("@name", "%" + itemNameOrId + "%");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetDecimal("cost");
            }
        }
        return -1;
    }

    public bool AddItemToDatabase(string itemId, string itemName, decimal cost, decimal buyback)
    {
        try
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
                return false;
            }

            string query = $"INSERT INTO {Configuration.Instance.ItemTable} (id, itemname, cost, buyback) VALUES (@id, @itemname, @cost, @buyback)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", itemId);
            cmd.Parameters.AddWithValue("@itemname", itemName);
            cmd.Parameters.AddWithValue("@cost", cost);
            cmd.Parameters.AddWithValue("@buyback", buyback);

            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }
        catch (MySqlException ex)
        {
            Rocket.Core.Logging.Logger.LogError($"Failed to add item to database: {ex.Message}");
            return false;
        }
    }

    public bool AddVehicleToDatabase(string vehicleId, string vehicleName, decimal cost)
    {
        try
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
                return false;
            }

            string query = $"INSERT INTO {Configuration.Instance.VehicleTable} (id, vehiclename, cost) VALUES (@id, @vehiclename, @cost)";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.Parameters.AddWithValue("@id", vehicleId);
            cmd.Parameters.AddWithValue("@vehiclename", vehicleName);
            cmd.Parameters.AddWithValue("@cost", cost);

            int result = cmd.ExecuteNonQuery();
            return result > 0;
        }
        catch (MySqlException ex)
        {
            Rocket.Core.Logging.Logger.LogError($"Failed to add vehicle to database: {ex.Message}");
            return false;
        }
    }

    public string GetItemName(string itemId)
    {
        if (ushort.TryParse(itemId, out ushort parsedItemId))
        {
            ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, parsedItemId);
            return itemAsset?.itemName ?? "Bilinmeyen Eşya";
        }
        return "Bilinmeyen Eşya";
    }

    public string GetVehicleName(string vehicleId)
    {
        if (ushort.TryParse(vehicleId, out ushort parsedVehicleId))
        {
            VehicleAsset vehicleAsset = (VehicleAsset)Assets.find(EAssetType.VEHICLE, parsedVehicleId);
            return vehicleAsset?.vehicleName ?? "Bilinmeyen Araç";
        }
        return "Bilinmeyen Araç";
    }
    public string GetItemId(string itemNameOrId)
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
            return null;
        }

        string query = $"SELECT id FROM {Configuration.Instance.ItemTable} WHERE id = @id OR itemname LIKE @name LIMIT 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", itemNameOrId);
        cmd.Parameters.AddWithValue("@name", "%" + itemNameOrId + "%");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetString("id");
            }
        }
        return null;
    }

    public decimal GetVehicleCost(string vehicleNameOrId)
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
            return -1;
        }

        string query = $"SELECT cost FROM {Configuration.Instance.VehicleTable} WHERE id = @id OR vehiclename LIKE @name LIMIT 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", vehicleNameOrId);
        cmd.Parameters.AddWithValue("@name", "%" + vehicleNameOrId + "%");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetDecimal("cost");
            }
        }
        return -1;
    }

    public decimal GetItemBuybackPrice(string itemNameOrId)
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
            return -1;
        }

        string query = $"SELECT buyback FROM {Configuration.Instance.ItemTable} WHERE id = @id OR itemname LIKE @name LIMIT 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", itemNameOrId);
        cmd.Parameters.AddWithValue("@name", "%" + itemNameOrId + "%");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetDecimal("buyback");
            }
        }
        return -1;
    }


    public string GetVehicleId(string vehicleNameOrId)
    {
        if (connection == null || connection.State != ConnectionState.Open)
        {
            Rocket.Core.Logging.Logger.LogError("Database connection is not open.");
            return null;
        }

        string query = $"SELECT id FROM {Configuration.Instance.VehicleTable} WHERE id = @id OR vehiclename LIKE @name LIMIT 1";
        MySqlCommand cmd = new MySqlCommand(query, connection);
        cmd.Parameters.AddWithValue("@id", vehicleNameOrId);
        cmd.Parameters.AddWithValue("@name", "%" + vehicleNameOrId + "%");

        using (MySqlDataReader reader = cmd.ExecuteReader())
        {
            if (reader.Read())
            {
                return reader.GetString("id");
            }
        }
        return null;
    }
}
