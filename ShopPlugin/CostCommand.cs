using Rocket.API;
using Rocket.Unturned.Chat;
using System.Collections.Generic;

public class CostCommand : IRocketCommand
{
    public AllowedCaller AllowedCaller => AllowedCaller.Player;
    public string Name => "cost";
    public string Help => "Bir eşya veya araç maliyetini gösterir.";
    public string Syntax => "/cost <eşyaid|eşyaismi|v.araçid|v.araçismi>";
    public List<string> Aliases => new List<string>();
    public List<string> Permissions => new List<string> { "cost" };

    public void Execute(IRocketPlayer caller, string[] command)
    {
        if (command.Length != 1)
        {
            UnturnedChat.Say(caller, "Geçersiz kullanım! /cost <eşyaid|eşyaismi|v.araçid|v.araçismi>");
            return;
        }

        string input = command[0];
        ShopPlugins plugin = ShopPlugins.Instance;
        if (plugin == null)
        {
            UnturnedChat.Say(caller, "eklenti bulunamadı!");
            return;
        }

        decimal cost = -1;
        if (input.StartsWith("v."))
        {
            cost = plugin.GetVehicleCost(input.Substring(2));
            if (cost >= 0)
            {
                UnturnedChat.Say(caller, $"Araç fiyatı: {cost}");
            }
            else
            {
                UnturnedChat.Say(caller, "Araç bulunamadı.");
            }
        }
        else
        {
            cost = plugin.GetItemCost(input);
            if (cost >= 0)
            {
                UnturnedChat.Say(caller, $"Eşya fiyatı: {cost}");
            }
            else
            {
                UnturnedChat.Say(caller, "Eşya bulunamadı.");
            }
        }
    }
}
