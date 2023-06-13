using MultiplayerDedicatedServer.PBMultiplayerServer.Core.Messages.MessagePool;
using PBMultiplayerServer.Core.Messages.Factory;
using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages.MessagePool.Impl
{
    public class IncomeMessagePool : MessagePoolBase<IncomeMessage>
    {
        public IncomeMessagePool(IMessageFactory<IncomeMessage> messageFactory) : base(messageFactory)
        {
        }

        protected override void OnCreated(IncomeMessage message)
        {
            
        }
    }
}