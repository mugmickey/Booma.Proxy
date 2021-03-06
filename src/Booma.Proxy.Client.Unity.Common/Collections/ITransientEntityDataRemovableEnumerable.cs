﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Booma.Proxy
{
	/// <summary>
	/// Just a simplified type of <see cref="IEnumerable{T}"/> where T is <see cref="IEntityCollectionRemovable"/>
	/// for transient data.
	/// </summary>
	public interface ITransientEntityDataRemovableEnumerable : IEnumerable<IEntityCollectionRemovable>
	{
		
	}
}
