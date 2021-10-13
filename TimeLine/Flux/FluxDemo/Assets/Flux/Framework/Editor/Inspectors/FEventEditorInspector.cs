using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
	[Serializable]
	public class FEventEditorInspector : FEditorInspector<FEventEditor,FEvent> {

		public override string Title {
			get {
				if( _editors.Count == 1 )
					return "Event:";
				return "Events:";
			}
		}

	}
}
