using PBMultiplayerServer.Core.Messages.Impl;
using PBMultiplayerServer.Core.Messages.MessagePool;

namespace PBMultiplayerServer.Messages.MessagePool
{
    public interface IMessagePoolProvider
    {
        IMessagePool<IncomeMessage> GetIncomeMessagePool();
        IMessagePool<OutcomeMessage> GetOutcomeMessagePool();
    }
}