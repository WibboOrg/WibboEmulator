using Butterfly.Game.GameClients;
            {
                return;
            }

            if (!currentRoom.CheckRights(Session, true))
            {
                return;
            }

            try