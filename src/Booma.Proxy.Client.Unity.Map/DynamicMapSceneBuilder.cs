﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Booma.Proxy;
using SceneJect.Common;
using UnityEngine;

namespace Booma
{
	[Injectee]
	public class DynamicMapSceneBuilder
	{
		private IUnitScalerStrategy Scaler { get; }

		public DynamicMapSceneBuilder(IUnitScalerStrategy scaler)
		{
			if(scaler == null) throw new ArgumentNullException(nameof(scaler));

			Scaler = scaler;
		}

		public void AddObjectsToScene(IEnumerable<MapDataFormatObjectEntry> objects, Dictionary<int, NRelSectionModel> sectionData)
		{
			int index = 0;

			Dictionary<int, GameObject> SectionRootObject = new Dictionary<int, GameObject>();

			foreach(var entry in objects)
			{
				GameObject o = GameObject.CreatePrimitive(PrimitiveType.Cube) as GameObject;

				//TODO: This is for testing purposes
				o.AddComponent<MapObjectTestComponent>().MapObject = entry;

				o.name = $"{index}_{entry.ObjectType.ToString()}_{entry.Identifier}";

				o.transform.position = Scaler.Scale(entry.Position.ToUnityVector3());

				//TODO: Is this right?
				o.transform.rotation = Quaternion.AngleAxis(Scaler.ScaleYRotation(entry.Rotation.Y), Vector3.up);

				if(!SectionRootObject.ContainsKey(entry.Section))
					SectionRootObject[entry.Section] = new GameObject($"Section_{entry.Section}_Root");

				o.transform.parent = SectionRootObject[entry.Section].transform;
				index++;
			}

			//This will step through each section root and move it to the offset that the section
			//data dictionary contains.
			foreach(var sectionRootKvp in SectionRootObject)
			{
				if(!sectionData.ContainsKey(sectionRootKvp.Key))
					throw new InvalidOperationException($"SectionData does not contain data for id: {sectionRootKvp.Key}");

				sectionRootKvp.Value.transform.position = Scaler.Scale(sectionData[sectionRootKvp.Key].Position.ToUnityVector3());
			}
		}
	}
}
