using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
	[Serializable]
	public class FContainerEditor : FEditorList<FTimelineEditor> {

		public const int CONTAINER_HEIGHT = 25;

		public FContainer Container { get { return (FContainer)Obj; } }

		public void OnStop()
		{
			for( int i = 0; i != Editors.Count; ++i )
				Editors[i].OnStop();
		}

		public void UpdateTimelines( int frame, float time )
		{
			for( int i = 0; i != Editors.Count; ++i )
			{
				if( !Editors[i].Timeline.enabled )
					continue;
				Editors[i].UpdateTracks( frame, time );
			}
		}

		public override void Init( FObject obj, FEditor owner )
		{
			base.Init( obj, owner );

			Editors.Clear();

			List<FTimeline> timelines = Container.Timelines;
			
			for( int i = 0; i < timelines.Count; ++i )
			{
				FTimeline timeline = timelines[i];
				FTimelineEditor timelineEditor = SequenceEditor.GetEditor<FTimelineEditor>(timeline);
				timelineEditor.Init( timeline, this );
				Editors.Add( timelineEditor );
			}

			_icon = new GUIContent( FUtility.GetFluxTexture("Folder.png") );
		}

		public override FSequenceEditor SequenceEditor { get { return (FSequenceEditor)Owner; } }

		public override float HeaderHeight { get { return CONTAINER_HEIGHT; } }

		protected override string HeaderText { get { return Obj.name; } }

		protected override Color BackgroundColor { get { return Container.Color; } }

		protected override bool CanPaste( FObject obj )
		{
			// since Unity Objs can be "fake null"
			return obj != null && obj is FTimeline;
		}

		protected override void Paste( object obj )
		{
			if( !CanPaste(obj as FObject) )
				return;

			Undo.RecordObject( Container, string.Empty );

			FTimeline timeline = Instantiate<FTimeline>((FTimeline)obj);
			timeline.hideFlags = Container.hideFlags;
			Container.Add( timeline );
			Undo.RegisterCreatedObjectUndo( timeline.gameObject, "Paste Timeline " + ((FTimeline)obj).name );
		}

		protected override void Delete()
		{
			SequenceEditor.DestroyEditor( this );
		}

	}
}
