using Butterfly.Core;
using Butterfly.Net;
using ConnectionManager;
using System;

namespace Butterfly.Communication.WebSocket
{
    public class WebSocketManager
    {
        private readonly GameSocketManager _manager;

        public WebSocketManager(int port, int maxConnections)
        {
            this._manager = new GameSocketManager();
            this._manager.Init(port, maxConnections, new InitialPacketParser());

            this._manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.ConnectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            ButterflyEnvironment.GetGame().GetClientWebManager().CreateAndStartClient(connection.GetConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);
            information.ConnectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        private void CloseConnection(ConnectionInformation Connection)
        {
            try
            {
                ButterflyEnvironment.GetGame().GetClientWebManager().DisposeConnection(Connection.GetConnectionID());
                Connection.Dispose();
            }
            catch (Exception ex)
            {
                Logging.LogException((ex).ToString());
            }
        }

        public void Destroy()
        {
            this._manager.Destroy();
        }
    }
}
