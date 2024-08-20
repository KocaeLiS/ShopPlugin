using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

public class BuyCommand : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "buy";
    public string Help => "Bir eşya veya araç satın al";
    public string Syntax => "/buy <eşyaid|eşyaismi|v.araçid|v.araçismi> [miktar]";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string> { "buy" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1 || command.Length > 2)
        {
            UnturnedChat.Say(caller, "Geçersiz kullanım! /buy <eşyaid|eşyaismi|v.araçid|v.araçismi> [miktar]");
            return;
        }

        UnturnedPlayer player = (UnturnedPlayer)caller;
        if (player == null)
        {
            UnturnedChat.Say(caller, "Oyuncu bulunamadı.");
            return;
        }

        string input = command[0];
        int quantity = 1;

        if (command.Length == 2 && !int.TryParse(command[1], out quantity))
        {
            UnturnedChat.Say(caller, "Geçersiz miktar.");
            return;
        }

        if (quantity < 1)
        {
            UnturnedChat.Say(caller, "Miktar en az 1 olmalı.");
            return;
        }

        ShopPlugins plugin = ShopPlugins.Instance;
        if (plugin == null)
        {
            UnturnedChat.Say(caller, "eklenti bulunamadı!");
            return;
        }

        decimal cost = -1;
        string idOrName = input;

        if (input.StartsWith("v."))
        {
            idOrName = input.Substring(2);
            cost = plugin.GetVehicleCost(idOrName);
            if (cost == 0)
            {
                UnturnedChat.Say(caller, "Bu araç satın alınamaz.");
                return;
            }
            if (cost >= 0)
            {
                string vehicleId = plugin.GetVehicleId(idOrName);
                if (!string.IsNullOrEmpty(vehicleId))
                 {
  

                    Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -cost);
                    GiveVehicle(player, vehicleId);
                    UnturnedChat.Say(caller, $"{idOrName} için {cost} harcadınız!");
                }
                else
                {
                    UnturnedChat.Say(caller, "Araç bulunamadı.");
                    return;
                }
            }
        }
        else
        {
            cost = plugin.GetItemCost(idOrName) * quantity;
            if (cost == 0)
            {
                UnturnedChat.Say(caller, "Bu eşya satın alınamaz.");
                return;
            }
            if (cost >= 0)
            {
                string itemId = plugin.GetItemId(idOrName);
                if (!string.IsNullOrEmpty(itemId))
                {
                    decimal balance = Uconomy.Instance.Database.GetBalance(player.CSteamID.ToString());
                    if (balance < cost)
                    {
                        UnturnedChat.Say(caller, "Yetersiz bakiye.");
                        return;
                    }

                    Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), -cost);
                    GiveItem(player, itemId, quantity);
                    UnturnedChat.Say(caller, $"{quantity} x {idOrName} için {cost} harcadınız!");
                }
                else
                {
                    UnturnedChat.Say(caller, "Eşya bulunamadı.");
                    return;
                }
            }
        }

        if (Uconomy.Instance == null || Uconomy.Instance.Database == null)
        {
            UnturnedChat.Say(caller, "Ekonomi sistemi kullanılamıyor.");
            return;
        }
    }

    private void GiveItem(UnturnedPlayer player, string itemId, int quantity)
    {
        if (ushort.TryParse(itemId, out ushort parsedItemId))
        {
            for (int i = 0; i < quantity; i++)
            {
                Item item = new Item(parsedItemId, true);
                player.Inventory.forceAddItem(item, true);
            }
        }
        else
        {
            UnturnedChat.Say(player, "Geçersiz eşya ID'si.");
        }
    }

    private void GiveVehicle(UnturnedPlayer player, string vehicleId)
    {
        if (ushort.TryParse(vehicleId, out ushort parsedVehicleId))
        {
            VehicleTool.giveVehicle(player.Player, parsedVehicleId);
        }
        else
        {
            UnturnedChat.Say(player, "Geçersiz araç ID'si.");
        }
    }
}
