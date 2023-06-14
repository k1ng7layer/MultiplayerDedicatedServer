using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace PBMultiplayerServer.Core.Root
{
    public interface IServerCompositionRoot
    {
        ISocketProxyFactory SocketProxyFactory { get; }
        IMessageProvider MessageProvider { get; }
        IMessagePool<IncomeMessage> IncomeMessagePool { get; }
        IMessagePool<OutcomeMessage> OutcomeMessagePool { get; }
    }
}