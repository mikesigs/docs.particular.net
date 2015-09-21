﻿namespace Snippets6.Sagas.Reply
{
    using System.Threading.Tasks;
    using NServiceBus;

    #region saga-with-reply

    public class MySaga : Saga<MySagaData>,
        IAmStartedByMessages<StartMessage>
    {

        public async Task Handle(StartMessage message)
        {
            Data.SomeID = message.SomeID;
            ReplyToOriginator(new AlmostDoneMessage
            {
                SomeID = Data.SomeID
            });
        }

        #endregion

        protected override void ConfigureHowToFindSaga(SagaPropertyMapper<MySagaData> mapper)
        {
        }
    }

}