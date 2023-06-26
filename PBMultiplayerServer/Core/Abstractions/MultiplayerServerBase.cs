using PBMultiplayerServer.Core.Factories;
using PBMultiplayerServer.Core.Factories.Impl;
using PBMultiplayerServer.Core.Messages;
using PBMultiplayerServer.Core.Messages.Factory.Impl;
using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;
using PBMultiplayerServer.Core.Messages.MessagePool.Impl;

namespace PBMultiplayerServer.Core.Abstractions
{
    public abstract class MultiplayerServerBase
    {
        private ISocketProxyFactory _socketProxyFactory;
        private IMessageProvider _messageProvider;
        private IMessagePool<IncomeMessage> _incomeMessagePool;
        private IMessagePool<OutcomeMessage> _outcomeMessagePool;

        protected IMessagePool<IncomeMessage> IncomeMessagePool
        {
            get => _incomeMessagePool ??= new IncomeMessagePool(new IncomeMessageFactory());
            set => _incomeMessagePool = value;
        }

        protected IMessagePool<OutcomeMessage> OutcomeMessagePool
        {
            get => _outcomeMessagePool ??= new OutcomeMessagePool(new OutcomeMessageFactory());
            set => _outcomeMessagePool = value;
        }

        public ISocketProxyFactory SocketProxyFactory
        {
            get => _socketProxyFactory ??= new SocketProxyFactory();
            set => _socketProxyFactory = value;
        }

        public IMessageProvider MessageProvider
        {
            get => _messageProvider ??= new MessageProvider(IncomeMessagePool, OutcomeMessagePool);
            set => _messageProvider = value;
        }
    }
}