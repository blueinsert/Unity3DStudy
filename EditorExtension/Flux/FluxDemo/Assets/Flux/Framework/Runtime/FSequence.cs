using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Flux
{

	public class FSequence : FObject
	{
		public const int DEFAULT_FRAMES_PER_SECOND = 60;
		public const int DEFAULT_LENGTH = 10;

		public static FSequence CreateSequence()
		{
			return CreateSequence(new GameObject("FSequence"));
		}

		public static FSequence CreateSequence(GameObject gameObject)
		{
			FSequence sequence = gameObject.AddComponent<FSequence>();

			sequence._content = new GameObject("SequenceContent").transform;

			sequence._content.parent = sequence.transform;

			sequence.Add(FContainer.Create(FContainer.DEFAULT_COLOR));

			sequence.Version = FUtility.FLUX_VERSION;

			return sequence;
		}

		[SerializeField]
		private Transform _content = null;

		public Transform Content { get { return _content; } set { _content = value; _content.parent = transform; } }

		[SerializeField]
		private int id;
		public int ID { get { return id; } }

		[SerializeField]
		private List<FContainer> _containers = new List<FContainer>();

		public List<FContainer> Containers { get { return _containers; } }

		[SerializeField]
		[HideInInspector]
		private int _version = 0;
		public int Version { get { return _version; } set { _version = value; } }

		[SerializeField]
		private bool _loop = false;

		public bool Loop { get { return _loop; } set { _loop = value; } }

		[SerializeField]
		private int _length = DEFAULT_LENGTH * DEFAULT_FRAMES_PER_SECOND;
		public int Length { get { return _length; } set { _length = value; } }

		public float LengthTime { get { return (float)_length * _inverseFrameRate; } }

		[SerializeField]
		[HideInInspector]
		private int _frameRate = DEFAULT_FRAMES_PER_SECOND; // frame rate

		public int FrameRate { get { return _frameRate; } set { _frameRate = value; _inverseFrameRate = 1f / _frameRate; } }

		[SerializeField]
		[HideInInspector]
		private float _inverseFrameRate = 1f / DEFAULT_FRAMES_PER_SECOND;

		public float InverseFrameRate { get { return _inverseFrameRate; } }

		public override Transform Owner
		{
			get
			{
				return transform;
			}
		}

		public override FSequence Sequence
		{
			get
			{
				return this;
			}
		}

		public void Add(FContainer container)
		{
			int id = _containers.Count;
			_containers.Add(container);
			container.SetId(id);
			container.SetSequence(this);
		}

		public void Remove(FContainer container)
		{
			if (_containers.Remove(container))
			{
				container.SetSequence(null);

				UpdateContainerIds();
			}
		}

		/// @brief Does the sequence have no events?
		public bool IsEmpty()
		{
			foreach (FContainer container in _containers)
			{
				if (!container.IsEmpty())
					return false;
			}

			return true;
		}

		/// @brief Determines wether it has any timelines.
		public bool HasTimelines()
		{
			foreach (FContainer container in _containers)
			{
				if (container.Timelines.Count > 0)
					return true;
			}

			return false;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		public void Rebuild()
		{
#if FLUX_DEBUG
			Debug.Log("Rebuilding");
#endif
			_containers.Clear();

			Transform t = Content;

			for (int i = 0; i != t.childCount; ++i)
			{
				FContainer container = t.GetChild(i).GetComponent<FContainer>();

				if (container != null)
				{
					_containers.Add(container);
					container.SetSequence(this);
					container.Rebuild();
				}
			}

			UpdateContainerIds();
		}

		public FSequence Clone()
		{
			GameObject origin = this.gameObject;
			return GameObject.Instantiate<GameObject>(origin).GetComponent<FSequence>();
		}

		private void UpdateContainerIds()
		{
			for (int i = 0; i != _containers.Count; ++i)
			{
				_containers[i].SetId(i);
			}
		}
		
	}

}

