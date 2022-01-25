using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditorInternal;

using System;
using System.Collections.Generic;

using Flux;

/*
namespace FluxEditor
{
    public class FTimelineEditor : FEditorList<FTrackEditor>
    {
        public const int HEADER_HEIGHT = 25;

        public FContainerEditor ContainerEditor { get { return (FContainerEditor)Owner; } }

        public FTimeline Timeline { get { return (FTimeline)Obj; } }

        public bool _showHeader = true;
        public bool _showTracks = true;


        public override void Init(FObject obj, FEditor owner)
        {
            base.Init(obj, owner);

            Editors.Clear();

            List<FTrack> tracks = Timeline.Tracks;

            for (int i = 0; i < tracks.Count; ++i)
            {
                FTrack track = tracks[i];
                FTrackEditor trackEditor = ContainerEditor.SequenceEditor.GetEditor<FTrackEditor>(track);
                trackEditor.Init(track, this);
                Editors.Add(trackEditor);
            }

            _icon = new GUIContent(FUtility.GetFluxTexture("Plus.png"));
        }

        public override float Height
        {
            get
            {
                float headerHeight = _showHeader ? HEADER_HEIGHT : 0;
                float tracksHeight = 0;
                foreach (FTrackEditor trackEditor in Editors)
                    tracksHeight += trackEditor.Height;
                tracksHeight *= ShowPercentage;
                return headerHeight + tracksHeight;
            }
        }

        public override float HeaderHeight
        {
            get
            {
                return HEADER_HEIGHT;
            }
        }

        protected override string HeaderText
        {
            get
            {
                return Timeline.transform.name;
            }
        }

        protected override bool IconOnLeft
        {
            get
            {
                return false;
            }
        }

        protected override Color BackgroundColor
        {
            get
            {
                return FGUI.GetTimelineColor();
            }
        }

        protected override bool CanPaste(FObject obj)
        {
            // since Unity Objs can be "fake null"
            return obj != null && obj is FTrack;
        }

        protected override void Paste(object obj)
        {
            if (!CanPaste(obj as FObject))
                return;

            Undo.RecordObject(Timeline, string.Empty);

            FTrack track = Instantiate<FTrack>((FTrack)obj);
            track.hideFlags = Timeline.hideFlags;
            Timeline.Add(track);
            Undo.RegisterCreatedObjectUndo(track.gameObject, "Paste Track " + ((FTrack)obj).name);
        }

        protected override void Delete()
        {
            SequenceEditor.DestroyEditor(this);
        }

        protected override void OnHeaderInput(Rect labelRect, Rect iconRect)
        { 
            base.OnHeaderInput(labelRect, iconRect);

            if (Event.current.type == EventType.MouseDown && iconRect.Contains(Event.current.mousePosition))
            {
                ShowAddTrackMenu();
            }
        }

        private void ShowAddTrackMenu()
        {
            Event.current.Use();

            GenericMenu menu = new GenericMenu();

            System.Reflection.Assembly fluxAssembly = typeof(FEvent).Assembly;
            //獲取FEvent類所在程序集的所有類型
            Type[] types = typeof(FEvent).Assembly.GetTypes();


            if (fluxAssembly.GetName().Name != "Assembly-CSharp")
            {
                // if we are in the flux trial, basically allow to get the types in the project assembly
                ArrayUtility.AddRange<Type>(ref types, System.Reflection.Assembly.Load("Assembly-CSharp").GetTypes());
            }

            List<KeyValuePair<Type, FEventAttribute>> validTypeList = new List<KeyValuePair<Type, FEventAttribute>>();

            foreach (Type t in types)
            {
                //Fish：typeof(FEvent).IsAssignableFrom(t) 類型t是否間接或直接實現了FEvent（t是不是FEvent的後代或者後代的後代）

                if (!typeof(FEvent).IsAssignableFrom(t))
                    continue;
                object[] attributes = t.GetCustomAttributes(typeof(FEventAttribute), false);
                if (attributes.Length == 0 || ((FEventAttribute)attributes[0]).menu == null)
                    continue;

                validTypeList.Add(new KeyValuePair<Type, FEventAttribute>(t, (FEventAttribute)attributes[0]));
            }

            validTypeList.Sort(delegate (KeyValuePair<Type, FEventAttribute> x, KeyValuePair<Type, FEventAttribute> y)
                              {
                                  return x.Value.menu.CompareTo(y.Value.menu);
                              });

            foreach (KeyValuePair<Type, FEventAttribute> kvp in validTypeList)
            {
                menu.AddItem(new GUIContent(kvp.Value.menu), false, AddTrackMenu, kvp);
            }

            menu.ShowAsContext();
        }


        protected override void PopulateContextMenu(GenericMenu menu)
        {
            base.PopulateContextMenu(menu);
            
            if (CanPaste(FSequenceEditor.CopyObject))
            {
                menu.AddItem(new GUIContent("Paste " + FSequenceEditor.CopyObject.name), false, Paste, FSequenceEditor.CopyObject);
            }

            menu.AddSeparator(null);
        }

        void AddTrackMenu(object param)
        {
            KeyValuePair<Type, FEventAttribute> kvp = (KeyValuePair<Type, FEventAttribute>)param;

            Undo.RecordObjects(new UnityEngine.Object[] { Timeline, this }, "add Track");

            FTrack track = (FTrack)typeof(FTimeline).GetMethod("Add", new Type[] { typeof(FrameRange) }).MakeGenericMethod(kvp.Key).Invoke(Timeline, new object[] { SequenceEditor.ViewRange });

            string evtName = track.gameObject.name;

            int nameStart = 0;
            int nameEnd = evtName.Length;
            if (nameEnd > 2 && evtName[0] == 'F' && char.IsUpper(evtName[1]))
                nameStart = 1;
            if (evtName.EndsWith("Event"))
                nameEnd = evtName.Length - "Event".Length;
            evtName = evtName.Substring(nameStart, nameEnd - nameStart);

            track.gameObject.name = ObjectNames.NicifyVariableName(evtName);

            SequenceEditor.Refresh();

            Undo.RegisterCreatedObjectUndo(track.gameObject, string.Empty);

            SequenceEditor.SelectExclusive(SequenceEditor.GetEditor<FEventEditor>(track.GetEvent(0)));
        }

        public override FSequenceEditor SequenceEditor { get { return ContainerEditor != null ? ContainerEditor.SequenceEditor : null; } }

    }
}
*/