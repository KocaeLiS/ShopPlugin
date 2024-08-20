using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace ShopPlugin
{
    public class ShopAddVehicleCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "shopaddvehicle";
        public string Help => "Mağazaya yeni bir araç ekle";
        public string Syntax => "/shopaddvehicle <vehicleid> <fiyat>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "shop.add.vehicle" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 2)
            {
                UnturnedChat.Say(caller, "Geçersiz kullanım! /shopaddvehicle <vehicleid> <fiyat>");
                return;
            }

            string vehicleId = command[0];
            if (!decimal.TryParse(command[1], out decimal cost))
            {
                UnturnedChat.Say(caller, "Geçersiz fiyat.");
                return;
            }

            ShopPlugins plugin = ShopPlugins.Instance;
            if (plugin == null)
            {
                UnturnedChat.Say(caller, "Plugin bulunamadı!");
                return;
            }

            string vehicleName = plugin.GetVehicleName(vehicleId);

            if (plugin.AddVehicleToDatabase(vehicleId, vehicleName, cost))
            {
                UnturnedChat.Say(caller, $"{vehicleName} mağazaya eklendi. Fiyat: {cost}");
            }
            else
            {
                UnturnedChat.Say(caller, "Araç eklenirken bir hata oluştu.");
            }
        }
    }

}
