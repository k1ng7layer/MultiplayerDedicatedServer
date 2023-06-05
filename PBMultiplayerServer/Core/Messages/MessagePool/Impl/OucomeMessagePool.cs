using MultiplayerDedicatedServer.PBMultiplayerServer.Core.Messages.MessagePool;
using PBMultiplayerServer.Core.Messages.Factory;
using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages.MessagePool.Impl
{
    public class OutcomeMessagePool : MessagePoolBase<OutcomeMessage>
    {
        public OutcomeMessagePool(IMessageFactory<OutcomeMessage> messageFactory) : base(messageFactory)
        {
        }

        protected override void OnCreated(OutcomeMessage message)
        {
            message.OnRetrieved();
        }
    }
}