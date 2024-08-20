using fr34kyn01535.Uconomy;
using Rocket.API;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

public class SellCommand : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "sell";
    public string Help => "Envanterinizdeki bir eşyayı veya mermiyi geri satın.";
    public string Syntax => "/sell <eşyaid|mermiid> [miktar]";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string> { "sell" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length < 1 || command.Length > 2)
        {
            UnturnedChat.Say(caller, "Geçersiz kullanım! /sell <eşyaid|mermiid> [miktar]");
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
            UnturnedChat.Say(caller, "Eklenti bulunamadı!");
            return;
        }

        string itemId = input;
        decimal buybackPricePerItem = plugin.GetItemBuybackPrice(itemId);
        if (buybackPricePerItem < 0)
        {
            UnturnedChat.Say(caller, "Eşya alım fiyatı bulunamadı.");
            return;
        }

        int ammoCapacity = GetAmmoCapacity(itemId);
        decimal unitPrice = buybackPricePerItem;

        if (ammoCapacity > 0)
        {
            unitPrice = buybackPricePerItem / ammoCapacity;
        }

        if (HasItem(player, itemId, quantity))
        {
            RemoveItems(player, itemId, quantity, ammoCapacity);
            decimal totalBuybackPrice = unitPrice * quantity;
            Uconomy.Instance.Database.IncreaseBalance(player.CSteamID.ToString(), totalBuybackPrice);
            UnturnedChat.Say(caller, $"{quantity} x {itemId} için {totalBuybackPrice:F2} kazandınız!");
        }
        else
        {
            UnturnedChat.Say(caller, "Envanterinizde yeterli miktarda eşya yok.");
        }
    }

    private bool HasItem(UnturnedPlayer player, string itemId, int quantity)
    {
        int count = 0;
        List<InventorySearch> items = player.Inventory.search(ushort.Parse(itemId), true, true);
        foreach (var item in items)
        {
            count += item.jar.item.amount > 0 ? item.jar.item.amount : 1; 
        }
        return count >= quantity;
    }

    private void RemoveItems(UnturnedPlayer player, string itemId, int quantity, int ammoCapacity)
    {
        List<InventorySearch> items = player.Inventory.search(ushort.Parse(itemId), true, true);
        foreach (var item in items)
        {
            if (quantity <= 0) break;

            if (ammoCapacity > 0 && item.jar.item.amount > 0)
            {
                if (item.jar.item.amount <= quantity)
                {
                    quantity -= item.jar.item.amount;
                    player.Inventory.removeItem(item.page, player.Inventory.getIndex(item.page, item.jar.x, item.jar.y));
                }
                else
                {
                    byte newAmount = (byte)(item.jar.item.amount - quantity);
                    player.Inventory.sendUpdateAmount(item.page, item.jar.x, item.jar.y, newAmount);
                    quantity = 0;
                }
            }
            else
            {
                player.Inventory.removeItem(item.page, player.Inventory.getIndex(item.page, item.jar.x, item.jar.y));
                quantity--;
            }
        }
    }

    private int GetAmmoCapacity(string itemId)
    {
        ushort id = ushort.Parse(itemId);
        ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, id);
        return itemAsset != null ? itemAsset.amount : 0;
    }
}
