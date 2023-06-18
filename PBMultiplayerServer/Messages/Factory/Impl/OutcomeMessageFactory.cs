using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages.Factory.Impl
{
    public class OutcomeMessageFactory : IMessageFactory<OutcomeMessage>
    {
        public OutcomeMessage Create(params object[] args)
        {
            return new OutcomeMessage();
        }
    }
}