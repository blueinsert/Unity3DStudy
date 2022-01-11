using UnityEngine;
using System.Collections;

namespace Flux
{
	public abstract class FObject : MonoBehaviour
	{

		[SerializeField]
		[HideInInspector]
		private int _id = -1;

		public int GetId(){ return _id; }

		internal void SetId( int id ) { _id = id; }

		public abstract FSequence Sequence { get; }

		public abstract Transform Owner { get; }


	}
}
