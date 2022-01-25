using UnityEngine;
using System;
using System.Collections.Generic;
using System.Text;
/*
namespace Flux
{

public class FTimeline : FObject
{
	[SerializeField]
	private FContainer _container = null;
	public FContainer Container { get { return _container; } }
	public override FSequence Sequence { get { return _container.Sequence; } }
	[SerializeField]
	private List<FTrack> _tracks = new List<FTrack>();
	public List<FTrack> Tracks { get { return _tracks; } }

	internal void SetContainer( FContainer container )
	{
		_container = container;
		if( _container )
			transform.parent = container.transform;
		else
			transform.parent = null;
		OnValidate();
	}


public static FTimeline Create()
		{
			GameObject go = new GameObject("Timeline");
			FTimeline timeline = go.AddComponent<FTimeline>();
		
			return timeline;
		}


		/// Returns if the timeline doesn't have any events.
		public bool IsEmpty()
		{
			foreach( FTrack track in _tracks )
			{
				if( !track.IsEmpty() )
					return false;
			}

			return true;
		}


		//被反射调用
		public FTrack Add<T>( FrameRange range ) where T : FEvent
		{
			FTrack track = FTrack.Create<T>();

			Add( track );

			FEvent evt = FEvent.Create<T>( range );

			track.Add( evt );

			return track;
		}

		public void Add( FTrack track )
		{
			int id = _tracks.Count;

			_tracks.Add( track );

			track.SetTimeline( this );
			track.SetId( id );

		}

		public void Remove( FTrack track )
		{
			if( _tracks.Remove( track ) )
			{
				track.SetTimeline( null );

				UpdateTrackIds();
			}
		}

		public void Rebuild()
		{
			Transform t = transform;
			_tracks.Clear();

			for( int i = 0; i != t.childCount; ++i )
			{
				FTrack track = t.GetChild(i).GetComponent<FTrack>();
				if( track )
				{
					_tracks.Add( track );
					track.SetTimeline( this );
					track.Rebuild();
				}
			}

			UpdateTrackIds();
		}

		// updates the track ids
		private void UpdateTrackIds()
		{
			for( int i = 0; i != _tracks.Count; ++i )
				_tracks[i].SetId( i );
		}

		protected virtual void OnValidate()
		{
			
		}

    }

}
*/
