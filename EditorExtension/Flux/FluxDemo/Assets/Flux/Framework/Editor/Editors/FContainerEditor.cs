using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

using Flux;

namespace FluxEditor
{
    [Serializable]
    public class FContainerEditor : FEditorList<FTrackEditor>
    {

        public const int CONTAINER_HEIGHT = 25;

        public FContainer Container { get { return (FContainer)Obj; } }

        public override void Init(FObject obj, FEditor owner)
        {
            base.Init(obj, owner);

            Editors.Clear();

            List<FTrack> tracks = Container.Tracks;

            for (int i = 0; i < tracks.Count; ++i)
            {
                FTrack track = tracks[i];
                FTrackEditor trackEditor = SequenceEditor.GetEditor<FTrackEditor>(track);
                trackEditor.Init(track, this);
                Editors.Add(trackEditor);
            }

            _icon = new GUIContent(FUtility.GetFluxTexture("Plus.png"));
        }

        public override FSequenceEditor SequenceEditor { get { return (FSequenceEditor)Owner; } }

        public override float HeaderHeight { get { return CONTAINER_HEIGHT; } }

        protected override string HeaderText { get { return Obj.name; } }

        protected override Color BackgroundColor { get { return Container.Color; } }

        protected override bool IconOnLeft
        {
            get
            {
                return false;
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

            Undo.RecordObject(Container, string.Empty);

            FTrack track = Instantiate<FTrack>((FTrack)obj);
            track.hideFlags = Container.hideFlags;
            Container.Add(track);
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

            Undo.RecordObjects(new UnityEngine.Object[] { Container, this }, "add Track");

            FTrack track = (FTrack)typeof(FContainer).GetMethod("Add", new Type[] { typeof(FrameRange) }).MakeGenericMethod(kvp.Key).Invoke(Container, new object[] { SequenceEditor.ViewRange });

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
    }
}
