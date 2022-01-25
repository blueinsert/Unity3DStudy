using UnityEngine;
using System.Collections.Generic;

namespace Flux
{
	public class FContainer : FObject {

		public static readonly Color DEFAULT_COLOR = new Color(0.14f, 0.14f, 0.14f, 0.7f);

		[SerializeField]
		private FSequence _sequence = null;

		[SerializeField]
		private Color _color;
		public Color Color { get { return _color; } set { _color = value; } }

		[SerializeField]
		private List<FTrack> _tracks = new List<FTrack>();
		public List<FTrack> Tracks { get { return _tracks; } }

		public override FSequence Sequence { get { return _sequence; } }

		public static FContainer Create( Color color, string name)
		{
			GameObject go = new GameObject(name);
			FContainer container = go.AddComponent<FContainer>();
			container.Color = color;

			return container;
		}

		internal void SetSequence( FSequence sequence )
		{
			_sequence = sequence;
			if( _sequence )
				transform.parent = _sequence.Content;
			else
				transform.parent = null;
		}

		public bool IsEmpty()
		{
			foreach( FTrack track in _tracks)
			{
				if( !track.IsEmpty() )
				{
					return false;
				}
			}

			return true;
		}

		//被反射调用
		public FTrack Add<T>(FrameRange range) where T : FEvent
		{
			FTrack track = FTrack.Create<T>();

			Add(track);

			FEvent evt = FEvent.Create<T>(range);

			track.Add(evt);

			return track;
		}

		public void Add(FTrack track)
		{
			int id = _tracks.Count;

			_tracks.Add(track);

			track.SetContainer(this);
			track.SetId(id);

		}

		public void Remove(FTrack track)
		{
			if (_tracks.Remove(track))
			{
				track.SetContainer(null);

				UpdateTrackIds();
			}
		}

		public void Rebuild()
		{
			Transform t = transform;
			_tracks.Clear();

			for (int i = 0; i != t.childCount; ++i)
			{
				FTrack track = t.GetChild(i).GetComponent<FTrack>();
				if (track)
				{
					_tracks.Add(track);
					track.SetContainer(this);
					track.Rebuild();
				}
			}

			UpdateTrackIds();
		}

		private void UpdateTrackIds()
		{
			for (int i = 0; i != _tracks.Count; ++i)
				_tracks[i].SetId(i);
		}
	}
}
