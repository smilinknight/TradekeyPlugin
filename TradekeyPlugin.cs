using Terraria;
using System;
using TerrariaApi.Server;
using TShockAPI;
using On.Terraria.ID;
using Microsoft.Xna.Framework;
using Terraria.Localization;
using Terraria.GameContent.Bestiary;
using TShockAPI.Hooks;
using Microsoft.Xna.Framework.Input;
using NuGet.Packaging.Signing;
using ReLogic.Peripherals.RGB;

namespace TradekeyPlugin
{
    [ApiVersion(2, 1)]
    public class TradekeyPlugin : TerrariaPlugin
    {
        public override string Name => "Tradekey";

        public override Version Version => new Version(1, 0);

        public override string Author => "Elly";

        public override string Description => "This plugin allows players to exchange their biome keys for specific biome chest weapons";

        
        public Dictionary<int, int> _keyItemMappings = new Dictionary<int, int>
        
        {
            { 1536, 1260 }, // hallow
            { 1535, 1569 }, // crimson 
            { 1533, 1156 }, // jungle
            { 1534, 1571 }, // corrupt
            { 1537, 1572 }, // snow
            { 4714, 4607  }, // desert
        };

        public TradekeyPlugin(Main game) : base(game)
        {
        }

        public override void Initialize()
        {

            
            ServerApi.Hooks.GameInitialize.Register(this, WolrdLoaded);
            
        }

        private void WolrdLoaded(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("qol.tradekey", Tradekey, "tradekey"));
            
        }

        public void Tradekey(CommandArgs args)

            
        {
            TSPlayer player = args.Player;

                if (player == null)
                return;

            int heldItemID = player.SelectedItem.type;
            bool isBossDefeated = NPC.downedPlantBoss; 

            if (!isBossDefeated)
            {
                args.Player.SendErrorMessage("The command is not available at the current stage of the game. Defeat the Plantera first!");
               // RU: // args.Player.SendErrorMessage("Команда не доступна на данном этапе игры. Плантера должна быть побеждена!");
                return;
            }


            if (heldItemID <= 0)
            {
                args.Player.SendMessage("The biome key was not found. Put the key in your chosen hotbar slot or click on it in the inventory, and then enter the command again.", Color.Red);
                // RU: // args.Player.SendMessage("Биомный ключ не обнаружен. Положите ключ в быстрый слот или щелкните по нему в инвентаре, а затем введите команду еще раз.", Color.Red);
                return;
            }



            if (_keyItemMappings.TryGetValue(heldItemID, out int rewardItemID))
            {
                if (player.SelectedItem.stack > 1)
                 {
                    args.Player.SendMessage("Multiple keys in stack, please, exchange keys by one at a time.", Color.Red);
                    // RU: // args.Player.SendMessage("Обнаружено несколько ключей в слоте, пожалуйста, обменивайте ключи по одному.", Color.Red);
                    return;

                }

                if (player.TPlayer.inventory[player.TPlayer.selectedItem].type != 0)
                {
                   
                    int heldItemIndex = player.TPlayer.selectedItem;

                    args.Player.SendMessage($"You have succesfully exchanged [i:{heldItemID}] for [i:{rewardItemID}] !", Color.Green);
                    // RU: // args.Player.SendMessage($"Вы успешно обменяли [i:{heldItemID}] на [i:{rewardItemID}] !", Color.Green);

                    player.GiveItem(rewardItemID, 1, 0);                 
                    player.TPlayer.inventory[heldItemIndex] = new Item(); // or Item.Empty

                    NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, null, player.Index, heldItemIndex);


                }

            }
            else
            {
                args.Player.SendMessage("The biome key was not found. Put the key in your chosen hotbar slot or click on it in the inventory, and then enter the command again.", Color.Red);
                // RU: // args.Player.SendMessage("Биомный ключ не обнаружен. Положите ключ в быстрый слот или щелкните по нему в инвентаре, а затем введите команду еще раз.", Color.Red);
                return;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameInitialize.Deregister(this, WolrdLoaded);
                
            }
            base.Dispose(disposing);
        }
    }
}


