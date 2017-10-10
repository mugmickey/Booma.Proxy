﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Booma.Proxy
{
	/// <summary>
	/// Default client message context.
	/// Implements <see cref="IClientMessageContext{TPayloadBaseType}"/>.
	/// </summary>
	/// <typeparam name="TPayloadBaseType">The type of the base payload.</typeparam>
	public sealed class DefaultClientMessageContext<TPayloadBaseType> : IClientMessageContext<TPayloadBaseType>
		where TPayloadBaseType : class
	{
		/// <inheritdoc />
		public IConnectionService ConnectionService { get; }

		/// <inheritdoc />
		public IClientPayloadSendService<TPayloadBaseType> PayloadSendService { get; }

		/// <inheritdoc />
		public IClientRequestSendService<TPayloadBaseType> RequestSendService { get; }

		/// <inheritdoc />
		public DefaultClientMessageContext([NotNull] IConnectionService connectionService, [NotNull] IClientPayloadSendService<TPayloadBaseType> payloadSendService, [NotNull] IClientRequestSendService<TPayloadBaseType> requestSendService)
		{
			if(connectionService == null) throw new ArgumentNullException(nameof(connectionService));
			if(payloadSendService == null) throw new ArgumentNullException(nameof(payloadSendService));
			if(requestSendService == null) throw new ArgumentNullException(nameof(requestSendService));

			ConnectionService = connectionService;
			PayloadSendService = payloadSendService;
			RequestSendService = requestSendService;
		}
	}
}
