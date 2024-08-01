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

        public override string Description => "Fuck you.";

        
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
            // World initialized; you can add any world-specific setup here if needed.
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
                args.Player.SendErrorMessage("Команда не доступна на данном этапе игры!");
                return;
            }


            if (heldItemID <= 0)
            {
                args.Player.SendMessage("Биомный ключ не обнаружен. Положите ключ в быстрый слот или щелкните по нему в инвентаре, а затем введите команду еще раз.", Color.Red);
                return;
            }



            if (_keyItemMappings.TryGetValue(heldItemID, out int rewardItemID))
            {
                if (player.SelectedItem.stack > 1)
                 {
                    args.Player.SendMessage("Обнаружено несколько ключей в слоте, пожалуйста, обменивайте ключи по одному.", Color.Red);
                    return;

                }

                if (player.TPlayer.inventory[player.TPlayer.selectedItem].type != 0)
                {
                   
                    int heldItemIndex = player.TPlayer.selectedItem;

                    player.GiveItem(rewardItemID, 1, 0);


                    
                    player.TPlayer.inventory[heldItemIndex] = new Item(); // or Item.Empty

                    
                    NetMessage.SendData((int)PacketTypes.PlayerSlot, player.Index, -1, null, player.Index, heldItemIndex);
                }

            }
            else
            {
                args.Player.SendMessage("Биомный ключ не обнаружен. Положите ключ в быстрый слот или щелкните по нему в инвентаре, а затем введите команду еще раз.", Color.Red);
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


