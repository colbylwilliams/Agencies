using System;
using Square.SocketRocket;
namespace NomadCode.BotFramework.iOS
{
    public static class SocketExtensions
    {
        public static void Close (this WebSocket webSocket, long code, string reason) => webSocket.Close ((StatusCode)code, reason);
    }
}
