using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Events;

namespace Flux
{
	[Serializable]

	public struct FrameRange
	{
		// start frame
		[SerializeField]
		private int _start;

		// end frame
		[SerializeField]
		private int _end;

		public int Start
		{ 
			get { return _start; } 
			set 
			{ 
				_start = value;
			}
		}

		public int End
		{ 
			get { return _end; } 
			set 
			{ 
				_end = value;
			}
		}


		public int Length { set{ End = _start + value; } get{ return _end - _start; } }

		public FrameRange( int start, int end )
		{
			this._start = start;
			this._end = end;
		}

		public int Cull( int i )
		{
			return Mathf.Clamp( i, _start, _end );
		}

		public bool Contains( int i )
		{
			return i >= _start && i <= _end;
		}

		public bool ContainsExclusive( int i )
		{
			return i > _start && i < _end;
		}

		public bool Collides( FrameRange range )
		{
			return _start < range._end && _end > range._start;
		}

		public bool Overlaps( FrameRange range )
		{
			return range.End >= _start && range.Start <= _end;
		}


		public FrameRangeOverlap GetOverlap( FrameRange range )
		{
			if( range._start >= _start )
			{
				// contains, left or none
				if( range._end <= _end )
				{
					return FrameRangeOverlap.ContainsFull;
				}
				else
				{
					if( range._start > _end )
					{
						return FrameRangeOverlap.MissOnRight;
					}
					return FrameRangeOverlap.ContainsStart;
				}
			}
			else
			{
				// contained, right or none
				if( range._end < _start )
				{
					return FrameRangeOverlap.MissOnLeft;
				}
				else
				{
					if( range._end > _end )
					{
						return FrameRangeOverlap.IsContained;
					}

					return FrameRangeOverlap.ContainsEnd;
				}
			}
		}

        public static bool operator ==( FrameRange a, FrameRange b )
        {
            return a._start == b._start && a._end == b._end;
        }

        public static bool operator !=( FrameRange a, FrameRange b )
        {
            return !(a == b);
        }

		public override bool Equals( object obj )
		{
			if( obj.GetType() != GetType() )
				return false;

			return (FrameRange)obj == this;
		}

		public override int GetHashCode()
		{
			return _start + _end;
		}

		public override string ToString()
		{
			return string.Format("[{0}; {1}]", _start, _end);
		}
	}

	/// @brief Types of range overlap
	public enum FrameRangeOverlap
	{
		MissOnLeft = -2,	/// @brief missed and is to the left of the range passed
		MissOnRight,		/// @brief missed and is to the right of the range passed
		IsContained,		/// @brief overlaps and is contained by the range passed
		ContainsFull,		/// @brief overlaps and contains the range passed
		ContainsStart,		/// @brief overlaps and contains the start of the range passed
		ContainsEnd			/// @brief overlaps and contains the end of the range passed
	}

	public class FEvent : FObject
	{
		public override Transform Owner { get { return _track.Owner; } }

		public override FSequence Sequence { get { return _track.Sequence; } }

		// track that owns this event
		[SerializeField]
		[HideInInspector]
		protected FTrack _track = null;

		public FTrack Track { get { return _track; } }

		[SerializeField]
		[HideInInspector]
		private bool _triggerOnSkip = true;
		/// @brief Should this event trigger if you skip it?
		public bool TriggerOnSkip { get { return _triggerOnSkip; } set { _triggerOnSkip = value; } }

		[SerializeField]
		[HideInInspector]
		private FrameRange _frameRange;
		/// @brief Range of the event.
		public FrameRange FrameRange { get { return _frameRange; } 
			set { 
				FrameRange oldFrameRange = _frameRange;
				_frameRange = value; OnFrameRangeChanged( oldFrameRange ); 
			} 
		}

		// has this event called Trigger already?
		private bool _hasTriggered = false;
		/// @brief Has Trigger been called already?
		public bool HasTriggered { get { return _hasTriggered; } }

		// has this event called Finish already?
		private bool _hasFinished = false;
		/// @brief Has Finish been called already?
		public bool HasFinished { get { return _hasFinished; } }

		public virtual string Text { get { return null; } set { } }


		/**
		 * @brief Create an event. Should be used to create events since it also 
		 * calls SetDefaultValues.
		 * @param range Range of the event.
		 */
		public static T Create<T>( FrameRange range ) where T : FEvent
		{
			GameObject go = new GameObject( typeof(T).ToString() );

			T evt = go.AddComponent<T>();

			evt._frameRange = new FrameRange( range.Start, range.End );

			evt.SetDefaultValues();

			return evt;
		}

		/// @overload
		public static FEvent Create( Type evtType, FrameRange range )
		{
			GameObject go = new GameObject( evtType.ToString() );
			
			FEvent evt = (FEvent)go.AddComponent(evtType);
			
			evt._frameRange = new FrameRange( range.Start, range.End );

			evt.SetDefaultValues();
	
			return evt;
		}

		// sets the track this event belongs to, to be called only by FTrack
		internal void SetTrack( FTrack track )
		{
			_track = track;
			if( _track )
			{
				transform.parent = _track.transform;
			}
			else
			{
				transform.parent = null;
			}
		}

		/// @brief Use this function to setup default values for when events get created
		protected virtual void SetDefaultValues()
		{
		}

		/// @brief Use this function if you want to do something to the event when the frame range
		/// changed, e.g. adjust some variables to the new event size.
		/// @param oldFrameRange Previous FrameRange, the current one is set on the event.
		protected virtual void OnFrameRangeChanged( FrameRange oldFrameRange )
		{
		}

		/// @brief Returns \e true if it is the first event of the track it belongs to.
		public bool IsFirstEvent { get { return GetId() == 0; } }

		/// @brief Returns \e true if it is the last event of the track it belongs to.
		public bool IsLastEvent { get { return GetId() == _track.Events.Count-1; } }

		/// @brief Shortcut to FrameRange.Start
		public int Start
		{
			get{ return _frameRange.Start; }
			set{ _frameRange.Start = value; }
		}

		/// @brief Shortcut to FrameRange.End
		public int End
		{
			get { return _frameRange.End; }
			set{ _frameRange.End = value; }
		}

		/// @brief Shortcut to FrameRange.Length
		public int Length
		{
			get{ return _frameRange.Length; } 
			set{ _frameRange.Length = value; }
		}

		/// @brief What this the event starts.
		/// @note This value isn't cached.
        public float StartTime
        {
            get { return _frameRange.Start * Sequence.InverseFrameRate; }
        }

		/// @brief What this the event ends.
		/// @note This value isn't cached.
        public float EndTime
        {
            get { return _frameRange.End * Sequence.InverseFrameRate; }
        }

		/// @brief Length of the event in seconds.
		/// @note This value isn't cached.
        public float LengthTime
        {
            get { return _frameRange.Length * Sequence.InverseFrameRate; }
        }

		/// @brief What's the minimum length this event can have?
		/// @warning Events cannot be smaller than 1 frame.
		public virtual int GetMinLength()
		{
			return 1;
		}

		/// @brief What's the maximum length this event can have?
		public virtual int GetMaxLength()
		{
			return int.MaxValue;
		}

		/// @brief Does the Event collides the \e e?
		public bool Collides( FEvent e )
		{
			return _frameRange.Collides( e.FrameRange );
		}

		/// @brief Returns the biggest frame range this event can have
		public FrameRange GetMaxFrameRange()
		{
			FrameRange range = new FrameRange(0, 0);

			int id = GetId();

			if( id == 0 )
			{
				range.Start = 0;
			}
			else
			{
				range.Start = _track.Events[id-1].End;
			}

			if( id == _track.Events.Count-1 ) // last one?
			{
				range.End = _track.Timeline.Sequence.Length;
			}
			else
			{
				range.End = _track.Events[id+1].Start;
			}

			return range;
		}

		/// @brief Compares events based on their start frame, basically used to order them.
		/// @param e1 Event
		/// @param e2 Event
		public static int Compare( FEvent e1, FEvent e2 )
		{
			return e1.Start.CompareTo( e2.Start );
		}
	}


    /**
	 * @brief Attribute that adds an Event to the add event menu.
	 */
    public class FEventAttribute : System.Attribute
	{
		// menu path
		public string menu;

		// type of track to be used
		public Type trackType;

//		public object _color = null;


		public FEventAttribute( string menu )
			:this( menu, typeof(FTrack) )
		{
		}

		public FEventAttribute( string menu, Type trackType )
		{
			this.menu = menu;
			this.trackType = trackType;
		}

	}
}
