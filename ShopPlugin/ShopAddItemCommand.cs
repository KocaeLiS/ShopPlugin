using Rocket.API;
using Rocket.Unturned.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopPlugin
{
    public class ShopAddItemCommand : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "shopadditem";
        public string Help => "Mağazaya yeni bir eşya ekle";
        public string Syntax => "/shopadditem <itemid> <alışfiyatı> <satışfiyatı>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string> { "shop.add.item" };

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 3)
            {
                UnturnedChat.Say(caller, "Geçersiz kullanım! /shopadditem <itemid> <alışfiyatı> <satışfiyatı>");
                return;
            }

            string itemId = command[0];
            if (!decimal.TryParse(command[1], out decimal cost) || !decimal.TryParse(command[2], out decimal buyback))
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

            string itemName = plugin.GetItemName(itemId);

            if (plugin.AddItemToDatabase(itemId, itemName, cost, buyback))
            {
                UnturnedChat.Say(caller, $"{itemName} mağazaya eklendi. Alış: {cost}, Satış: {buyback}");
            }
            else
            {
                UnturnedChat.Say(caller, "Eşya eklenirken bir hata oluştu.");
            }
        }
    }

}
