using System;
using Square.OkHttp3.WS;
namespace NomadCode.BotFramework.Droid
{
    public static class SocketExtensions
    {
        public static void Close (this IWebSocket webSocket, long code, string reason) => webSocket.Close ((int)code, reason);
    }
}
