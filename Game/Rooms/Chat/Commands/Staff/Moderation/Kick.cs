using Butterfly.Game.GameClients;
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("input.usernotfound", Session.Langue));
            }
            else if (Session.GetHabbo().Rank <= clientByUsername.GetHabbo().Rank)
            {
                Session.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("action.notallowed", Session.Langue));
            }
            else if (clientByUsername.GetHabbo().CurrentRoomId < 1U)
                    {
                        return;
                    }

                    clientByUsername.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.withmessage", clientByUsername.Langue) + message);
                {
                    clientByUsername.SendNotification(ButterflyEnvironment.GetLanguageManager().TryGetValue("kick.nomessage", clientByUsername.Langue));
                }
            }