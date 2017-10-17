﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booma.Proxy
{
	/// <summary>
	/// Command mesage context that contains both the reference to the <see cref="INetworkPlayer"/> that the message
	/// is about as well as a reference to the local network player.
	/// </summary>
	public interface INetworkPlayerFullCommandMessageContext : INetworkPlayerCommandMessageContext
	{
		/// <summary>
		/// The local player reference.
		/// </summary>
		INetworkPlayer LocalPlayer { get; }
	}
}
