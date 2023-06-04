using PBMultiplayerServer.Core.Messages.Impl;

namespace PBMultiplayerServer.Core.Messages.Factory.Impl
{
    public class IncomeMessageFactory : IMessageFactory<IncomeMessage>
    {
        public IncomeMessage Create(params object[] args)
        {
            return new IncomeMessage();
        }
    }
}