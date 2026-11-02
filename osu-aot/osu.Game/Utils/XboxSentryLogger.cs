// Xbox port: no Sentry on the console. Same public surface as Utils/SentryLogger.cs, minus the SDK.
#nullable enable

using System;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using osu.Framework.Bindables;
using osu.Framework.Platform;
using osu.Game.Online.API.Requests.Responses;

namespace osu.Game.Utils
{
    public class SentryLogger : IDisposable
    {
        public SentryLogger(OsuGame game, Storage? storage = null)
        {
        }

        public void AttachUser(IBindable<APIUser> user)
        {
        }

        public static bool IsLocalUserConnectivityException(Exception exception)
        {
            switch (exception)
            {
                case TimeoutException te:
                    return te.Message.Contains(@"elapsed without receiving a message from the server");

                case WebException we:
                    return we.Status == WebExceptionStatus.Timeout;

                case WebSocketException:
                case SocketException:
                    return true;
            }

            return false;
        }

        public void Dispose()
        {
        }
    }
}