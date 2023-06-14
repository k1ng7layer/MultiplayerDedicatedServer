using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Factories.Impl;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace PBMultiplayerServer.Core.Root.Impl
{
    public class DefaultServerCompositionRoot : IServerCompositionRoot
    {
        public ISocketProxyFactory SocketProxyFactory => new SocketProxyFactory();
        public IMessageProvider MessageProvider { get; }
        public IMessagePool<IncomeMessage> IncomeMessagePool { get; }
        public IMessagePool<OutcomeMessage> OutcomeMessagePool { get; }
    }
}