﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booma.Proxy
{
	public interface ISub60CommandContainer
	{
		BaseSubCommand60 Command { get; }
	}
}
