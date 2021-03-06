﻿namespace NServiceBus.Transports.ActiveMQ.Tests.Encoders
{
    using ActiveMQ.Encoders;
    using Apache.NMS;
    using Moq;
    using NUnit.Framework;

    [TestFixture]
    public class ControlMessageEncoderTest
    {
        private ControlMessageEncoder testee;

        private Mock<ISession> session;

        [SetUp]
        public void SetUp()
        {
            session = new Mock<ISession> { DefaultValue = DefaultValue.Mock };

            SetupMessageCreation();

            testee = new ControlMessageEncoder();
        }

        [Test]
        public void Encode_WhenControlMessageWithEmptyBody_ReturnEmptyBinaryMessage()
        {
            var transportMessage = new TransportMessage();
            transportMessage.Headers.Add(Headers.ControlMessageHeader, null);

            var message = testee.Encode(transportMessage, session.Object);

            Assert.IsInstanceOf<IBytesMessage>(message);
            Assert.IsEmpty(((IBytesMessage)message).Content);
        }

        [Test]
        public void Encode_WhenControlMessageWithBody_ReturnFilledBinaryMessage()
        {
            var content = new byte[] { 2 };
            var transportMessage = new TransportMessage { Body = content };
            transportMessage.Headers.Add(Headers.ControlMessageHeader, null);

            var message = testee.Encode(transportMessage, session.Object);

            Assert.IsInstanceOf<IBytesMessage>(message);
            Assert.AreEqual(content, ((IBytesMessage)message).Content);
        }

        [Test]
        public void Encode_WhenNotControlMessage_ReturnNull()
        {
            var transportMessage = new TransportMessage();

            var message = testee.Encode(transportMessage, session.Object);

            Assert.IsNull(message);
        }

        private void SetupMessageCreation()
        {
            byte[] content = null;
            session.Setup(s => s.CreateBytesMessage(It.IsAny<byte[]>()))
                .Callback<byte[]>(c => content = c)
                .Returns(() => Mock.Of<IBytesMessage>(m => m.Content == content));
        }
    }
}