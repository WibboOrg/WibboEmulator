using Butterfly.Communication.Packets.Outgoing;
using Butterfly.Core;
using ConnectionManager;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Butterfly.Game.WebClients
{
    public class WebClientManager
    {
        public ConcurrentDictionary<int, WebClient> _clients;
        public ConcurrentDictionary<int, int> _userIDRegister;

        public int Count => this._clients.Count;

        public WebClientManager()
        {
            this._clients = new ConcurrentDictionary<int, WebClient>();
            this._userIDRegister = new ConcurrentDictionary<int, int>();
        }

        public WebClient GetClientByUserID(int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                if (!this.TryGetClient(this._userIDRegister[userID], out WebClient Client))
                {
                    return null;
                }

                return Client;
            }
            else
            {
                return null;
            }
        }

        public bool TryGetClient(int ClientId, out WebClient Client)
        {
            return this._clients.TryGetValue(ClientId, out Client);
        }

        public void SendMessage(IServerPacket Packet, Language Langue = Language.FRANCAIS, bool IsGameAlert = false)
        {
            foreach (WebClient Client in this._clients.Values.ToList())
            {
                if (Client == null || Client.Langue != Langue)
                {
                    continue;
                }

                if (IsGameAlert && !Client.ShowGameAlert)
                {
                    continue;
                }

                Client.SendPacket(Packet);
            }
        }

        public void CreateAndStartClient(int clientID, ConnectionInformation connection)
        {
            WebClient Client = new WebClient(clientID, connection);
            if (this._clients.TryAdd(Client.ConnectionID, Client))
            {
                Client.StartConnection();
            }
            else
            {
                connection.Dispose();
            }
        }

        public void DisposeConnection(int clientID)
        {
            if (!this.TryGetClient(clientID, out WebClient Client))
            {
                return;
            }

            if (Client != null)
            {
                Client.Dispose();
            }

            this.UnregisterClient(Client.UserId);
            this._clients.TryRemove(clientID, out Client);
        }

        public void LogClonesOut(int UserID)
        {
            WebClient clientByUserId = this.GetClientByUserID(UserID);
            if (clientByUserId == null)
            {
                return;
            }

            clientByUserId.Dispose();
        }

        public void RegisterClient(WebClient client, int userID)
        {
            if (this._userIDRegister.ContainsKey(userID))
            {
                this._userIDRegister[userID] = client.ConnectionID;
            }
            else
            {
                this._userIDRegister.TryAdd(userID, client.ConnectionID);
            }
        }

        public void UnregisterClient(int userid)
        {
            this._userIDRegister.TryRemove(userid, out int connectionId);
        }

        public void CloseAll()
        {
            try
            {
                foreach (WebClient client in this.GetClients.ToList())
                {

                    if (client == null || client.GetConnection() == null)
                    {
                        continue;
                    }

                    try
                    {
                        client.GetConnection().Dispose();
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogCriticalException((ex).ToString());
            }
            this._clients.Clear();
        }

        public ICollection<WebClient> GetClients => this._clients.Values;
    }
}
