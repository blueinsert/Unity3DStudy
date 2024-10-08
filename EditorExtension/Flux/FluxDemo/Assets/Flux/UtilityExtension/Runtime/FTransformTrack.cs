﻿using UnityEngine;
using System.Collections.Generic;

namespace Flux
{
	public class FTransformTrack : FTrack {
		
		private static Dictionary<FSequence, Dictionary<Transform, TransformSnapshot>> _snapshots = new Dictionary<FSequence, Dictionary<Transform, TransformSnapshot>>();

		public static TransformSnapshot GetSnapshot( FSequence sequence, Transform transform )
		{
			if( transform == null )
				return null;
			
			Dictionary<Transform, TransformSnapshot> sequenceSnapshots = null;
			if( !_snapshots.TryGetValue(sequence, out sequenceSnapshots) )
			{
				sequenceSnapshots = new Dictionary<Transform, TransformSnapshot>();
				_snapshots.Add( sequence, sequenceSnapshots );
			}

			TransformSnapshot result = null;
			if( !sequenceSnapshots.TryGetValue(transform, out result) )
			{
				result = new TransformSnapshot(transform);
				sequenceSnapshots.Add( transform, result );
			}
			return result;
		}

		protected TransformSnapshot _snapshot = null;
		public TransformSnapshot Snapshot { get { return _snapshot; } }

	}

	public class TransformSnapshot
	{
		public Transform Transform { get; private set; }
		public Transform Parent { get; private set; }
		public Vector3 LocalPosition { get; private set; }
		public Quaternion LocalRotation { get; private set; }
		public Vector3 LocalScale { get; private set; }

		public TransformSnapshot[] ChildrenSnapshots = null;

		public TransformSnapshot( Transform transform, bool recursive = false )
		{
			Transform = transform;
			Parent = Transform.parent;
			LocalPosition = Transform.localPosition;
			LocalRotation = Transform.localRotation;
			LocalScale = Transform.localScale;

			if( recursive )
			{
				TakeChildSnapshots();
			}
		}

		public void TakeChildSnapshots()
		{
			if( ChildrenSnapshots != null )
				return;
			ChildrenSnapshots = new TransformSnapshot[Transform.childCount];
			for( int i = 0; i != ChildrenSnapshots.Length; ++i )
			{
				ChildrenSnapshots[i] = new TransformSnapshot( Transform.GetChild(i), true );
			}
		}

		public void Restore()
		{
            if (Parent != Transform.parent)
                Transform.SetParent(Parent);
            Transform.localPosition = LocalPosition;
            Transform.localRotation = LocalRotation;
            Transform.localScale = LocalScale;

            if (ChildrenSnapshots != null)
            {
                for (int i = 0; i != ChildrenSnapshots.Length; ++i)
                    ChildrenSnapshots[i].Restore();
            }
        }
	}
}
