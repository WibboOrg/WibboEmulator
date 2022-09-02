﻿using WibboEmulator.Communication.Packets.Outgoing.Inventory.Furni;
using WibboEmulator.Database.Daos;
using WibboEmulator.Database.Interfaces;
using WibboEmulator.Game.Catalog;
using WibboEmulator.Game.Clients;
using WibboEmulator.Game.Items;
using System.Data;
using WibboEmulator.Game.Rooms;
using System.Text;
using WibboEmulator.Game.Users;

namespace WibboEmulator.Game.Chat.Commands.Cmd
{
    internal class LootboxInfo : IChatCommand
    {
        public void Execute(Client Session, Room Room, RoomUser UserRoom, string[] Params)
        {
            int basicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(1);
            int communCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(2);
            int epicCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(3);
            int legendaryCount = WibboEnvironment.GetGame().GetLootManager().GetRarityCounter(4);

            StringBuilder stringBuilder = new StringBuilder();

            stringBuilder.Append("- Information sur le nombre de rare distribuer ce mois-ci -\r");
            stringBuilder.Append("Basique: " + basicCount + "\r");
            stringBuilder.Append("Commun: " + communCount + "\r");
            stringBuilder.Append("Epique: " + epicCount + "\r");
            stringBuilder.Append("Legendaire: " + legendaryCount + "\r");

            Session.SendNotification(stringBuilder.ToString());
        }
    }
}