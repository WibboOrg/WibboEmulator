using Butterfly.HabboHotel.GameClients;
            {
                return;
            }

            GameClient clientByUsername = ButterflyEnvironment.GetGame().GetClientManager().GetClientByUsername(Params[1]);
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (string.IsNullOrEmpty(clientByUsername.MachineId))
                {
                    Raison = CommandManager.MergeParams(Params, 2);
                }

                ButterflyEnvironment.GetGame().GetClientManager().BanUser(clientByUsername, Session.GetHabbo().Username, 788922000, Raison, true, true);