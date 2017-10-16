﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FreecraftCore.Serializer;
using JetBrains.Annotations;

namespace Booma.Proxy
{
	//TODO: What is this?
	/// <summary>
	/// Command sent to set position and alert other clients
	/// when they finish warping.
	/// </summary>
	[WireDataContract]
	[SubCommand60(SubCommand60OperationCode.AlertFreshlyWarpedClients)]
	public sealed class Sub60FinishedWarpAckCommand : BaseSubCommand60
	{
		[WireMember(1)]
		public short ClientId { get; }

		/// <summary>
		/// The client that is moving.
		/// </summary>
		[WireMember(2)]
		public int ZoneId { get; }

		/// <summary>
		/// The position the client has moved to.
		/// </summary>
		[WireMember(3)]
		public Vector3<float> Position { get; } //server should set X and Z, ignoring y.

		//TODO: Soly said this is rotation so we should handle it 65536f / 360f
		[WireMember(4)]
		public int Rotation { get; }

		//Serializer ctor
		public Sub60FinishedWarpAckCommand(short clientId, int zoneId, [NotNull] Vector3<float> position)
			: this()
		{
			if(position == null) throw new ArgumentNullException(nameof(position));
			if(clientId < 0) throw new ArgumentOutOfRangeException(nameof(clientId));
			if(zoneId < 0) throw new ArgumentOutOfRangeException(nameof(zoneId));

			ClientId = clientId;
			ZoneId = zoneId;
			Position = position;
		}

		public Sub60FinishedWarpAckCommand()
		{
			//Calc static 32bit size
			CommandSize = 24 / 4;
		}
	}
}