using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

namespace Flux
{

	public class FSequence : FObject
	{
		public static FSequence CreateSequence(string name)
		{
			return CreateSequence(new GameObject(name));
		}

		public static FSequence CreateSequence(GameObject gameObject)
		{
			FSequence sequence = gameObject.AddComponent<FSequence>();

			sequence._content = new GameObject("Content").transform;

			sequence._content.parent = sequence.transform;

			sequence.Add(FContainer.Create(FContainer.DEFAULT_COLOR, "Group 0"));

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
		private int _length = 1000;
		public int Length { get { return _length; } set { _length = value; } }


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

		/// Does the sequence have no events?
		public bool IsEmpty()
		{
			foreach (FContainer container in _containers)
			{
				if (!container.IsEmpty())
					return false;
			}

			return true;
		}

		protected virtual void Awake()
		{
		}

		protected virtual void Start()
		{
		}

		public void Rebuild()
		{
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

		private void UpdateContainerIds()
		{
			for (int i = 0; i != _containers.Count; ++i)
			{
				_containers[i].SetId(i);
			}
		}
		
	}

}

