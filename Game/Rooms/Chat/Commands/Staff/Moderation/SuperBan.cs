using Butterfly.Game.GameClients;
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            {
                return;
            }
                if (Params.Length == 3)
                {
                    int.TryParse(Params[2], out num);
                }

                if (num <= 600)
                {
                    Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("ban.toolesstime", Session.Langue));
                }
                else