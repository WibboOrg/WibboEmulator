using Butterfly.HabboHotel.GameClients;
            {
                return;
            }

            string username = Params[1];
            {
                return;
            }

            string Message = CommandManager.MergeParams(Params, 2);
            {
                return;
            }

            Bot.OnChat(Message, (Bot.IsPet) ? 0 : 2, false);