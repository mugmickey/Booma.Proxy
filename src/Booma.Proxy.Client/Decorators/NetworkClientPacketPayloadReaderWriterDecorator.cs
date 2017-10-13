﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace Booma.Proxy
{
	/// <summary>
	/// Decorator that decorates the provided <see cref="NetworkClientBase"/> with functionality
	/// that allows you to write <see cref="TPayloadBaseType"/> directly into the stream/client.
	/// Overloads the usage of <see cref="Write"/> to accomplish this.
	/// </summary>
	/// <typeparam name="TClientType">The type of decorated client.</typeparam>
	/// <typeparam name="TPayloadBaseType">The payload base type.</typeparam>
	public sealed class NetworkClientPacketPayloadReaderWriterDecorator<TClientType, TReadPayloadBaseType, TWritePayloadBaseType> : NetworkClientBase,
		INetworkMessageClient<TReadPayloadBaseType, TWritePayloadBaseType>
		where TClientType : NetworkClientBase, IPacketHeaderReadable
		where TReadPayloadBaseType : class
		where TWritePayloadBaseType : class
	{
		/// <summary>
		/// The decorated client.
		/// </summary>
		private TClientType DecoratedClient { get; }

		/// <summary>
		/// The serializer service.
		/// </summary>
		private ISerializerService Serializer { get; }

		//TODO: Thread safety
		/// <summary>
		/// Thread specific buffer used to deserialize the packet header bytes into.
		/// </summary>
		private byte[] PacketPayloadBuffer { get; }

		public NetworkClientPacketPayloadReaderWriterDecorator([NotNull] TClientType decoratedClient, [NotNull] ISerializerService serializer)
		{
			if(decoratedClient == null) throw new ArgumentNullException(nameof(decoratedClient));
			if(serializer == null) throw new ArgumentNullException(nameof(serializer));

			DecoratedClient = decoratedClient;
			Serializer = serializer;

			//One of the lobby packets is 14,000 bytes. We may even need bigger.
			PacketPayloadBuffer = new byte[30000]; //TODO: Do we need a larger buffer for any packets?
		}

		/// <inheritdoc />
		public override async Task<bool> ConnectAsync(IPAddress address, int port)
		{
			return await DecoratedClient.ConnectAsync(address, port)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task DisconnectAsync(int delay)
		{
			await DecoratedClient.DisconnectAsync(delay)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public override async Task<byte[]> ReadAsync(byte[] buffer, int start, int count, int timeoutInMilliseconds)
		{
			return await DecoratedClient.ReadAsync(buffer, start, count, timeoutInMilliseconds)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public void Write(TWritePayloadBaseType payload)
		{
			//Write the outgoing message, it will internally create the header and it will be serialized
			WriteAsync(payload).Wait();
		}

		/// <inheritdoc />
		public override async Task WriteAsync(byte[] bytes, int offset, int count)
		{
			await DecoratedClient.WriteAsync(bytes, offset, count)
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public async Task WriteAsync(TWritePayloadBaseType payload)
		{
			//Write the outgoing message, it will internally create the header and it will be serialized
			await DecoratedClient.WriteAsync(Serializer.Serialize(new PSOBBNetworkOutgoingMessage(Serializer.Serialize(payload))))
				.ConfigureAwait(false);
		}

		/// <inheritdoc />
		public PSOBBNetworkIncomingMessage<TReadPayloadBaseType> Read()
		{
			return ReadAsync().Result;
		}

		/// <inheritdoc />
		public async Task<PSOBBNetworkIncomingMessage<TReadPayloadBaseType>> ReadAsync()
		{
			//Read the header first
			IPacketHeader header = await DecoratedClient.ReadHeaderAsync()
				.ConfigureAwait(false);

			//We need to read enough bytes to deserialize the payload
			await ReadAsync(PacketPayloadBuffer, 0, header.PayloadSize, 0)
				.ConfigureAwait(false);//TODO: Should we timeout?

			TReadPayloadBaseType payload = Serializer.Deserialize<TReadPayloadBaseType>(new FixedBufferWireReaderStrategy(PacketPayloadBuffer, header.PayloadSize));

			return new PSOBBNetworkIncomingMessage<TReadPayloadBaseType>(header, payload);
		}

		/// <inheritdoc />
		public override async Task<byte[]> ReadAsync(byte[] buffer, int start, int count, CancellationToken token)
		{
			return await DecoratedClient.ReadAsync(buffer, start, count, token)
				.ConfigureAwait(false);
		}

		public async Task<PSOBBNetworkIncomingMessage<TReadPayloadBaseType>> ReadAsync(CancellationToken token)
		{
			//Read the header first
			IPacketHeader header = await DecoratedClient.ReadHeaderAsync(token)
				.ConfigureAwait(false);

			//if was canceled the header reading probably returned null anyway
			if(token.IsCancellationRequested)
				return null;

			//We need to read enough bytes to deserialize the payload
			await ReadAsync(PacketPayloadBuffer, 0, header.PayloadSize, token)
				.ConfigureAwait(false);//TODO: Should we timeout?

			//If the token was canceled then the buffer isn't filled and we can't make a message
			if(token.IsCancellationRequested)
				return null;

			//Otherwise we're good
			TReadPayloadBaseType payload = Serializer.Deserialize<TReadPayloadBaseType>(new FixedBufferWireReaderStrategy(PacketPayloadBuffer, header.PayloadSize));

			return new PSOBBNetworkIncomingMessage<TReadPayloadBaseType>(header, payload);
		}
	}
}
