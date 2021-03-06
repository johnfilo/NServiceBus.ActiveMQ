﻿namespace NServiceBus.Transports.ActiveMQ
{
    using SessionFactories;

    public class MessageProducer : IMessageProducer
    {
        private readonly IActiveMqMessageMapper activeMqMessageMapper;
        private readonly ISessionFactory sessionFactory;
        private readonly IDestinationEvaluator destinationEvaluator;

        public MessageProducer(
            ISessionFactory sessionFactory, 
            IActiveMqMessageMapper activeMqMessageMapper,
            IDestinationEvaluator destinationEvaluator)
        {
            this.sessionFactory = sessionFactory;
            this.activeMqMessageMapper = activeMqMessageMapper;
            this.destinationEvaluator = destinationEvaluator;
        }

        public void SendMessage(TransportMessage message, string destination, string destinationPrefix)
        {
            var session = sessionFactory.GetSession();
            try
            {
                var jmsMessage = activeMqMessageMapper.CreateJmsMessage(message, session);

                using (var producer = session.CreateProducer())
                {
                    producer.Send(destinationEvaluator.GetDestination(session, destination, destinationPrefix), jmsMessage);
                }

                // We assign here the Id to the underlying id which was chosen by the broker.
                // TODO: Why do we need this daniel/remo?
                //message.Id = jmsMessage.NMSMessageId;
            }
            finally
            {
                sessionFactory.Release(session);
            }
        }        
    }
}