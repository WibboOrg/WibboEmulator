/*using Butterfly;
using Butterfly.Core;
using Butterfly.Net;
using System;

namespace Butterfly.Communication.ConnectionManager
{
    public class ConnectionHandeling
    {
        private readonly GameSocketManager _manager;

        public ConnectionHandeling(int port, int maxConnections, int connectionsPerIP)
        {
            this._manager = new GameSocketManager();
            this._manager.Init(port, maxConnections, new InitialPacketParser());

            this._manager.connectionEvent += new GameSocketManager.ConnectionEvent(this.ConnectionEvent);
        }

        private void ConnectionEvent(ConnectionInformation connection)
        {
            connection.ConnectionClose += new ConnectionInformation.ConnectionChange(this.ConnectionChanged);

            ButterflyEnvironment.GetGame().GetClientManager().CreateAndStartClient(connection.GetConnectionID(), connection);
        }

        private void ConnectionChanged(ConnectionInformation information)
        {
            this.CloseConnection(information);

            information.ConnectionClose -= new ConnectionInformation.ConnectionChange(this.ConnectionChanged);
        }

        public void CloseConnection(ConnectionInformation connection)
        {
            try
            {
                ButterflyEnvironment.GetGame().GetClientManager().DisposeConnection(connection.GetConnectionID());

                connection.Dispose();
            }
            catch (Exception ex)
            {
                ExceptionLogger.LogException((ex).ToString());
            }
        }


        public void Destroy()
        {
            this._manager.Destroy();
        }
    }
}
*/