using UnityEngine;
using UnityEditor;
using System.Collections;

namespace bluebean
{
    public class DraggableIcon
    {

        public static bool Draw(bool selected, int id, ref Vector2 position, Color color)
        {

            Texture texture = Resources.Load<Texture2D>("Dot");

            int controlID = GUIUtility.GetControlID(id, FocusType.Passive);

            // select vertex on mouse click:
            switch (Event.current.GetTypeForControl(controlID))
            {

                case EventType.MouseDown:

                    Rect area = new Rect(position.x - 5, position.y - 5, 10, 10);

                    if (area.Contains(Event.current.mousePosition))
                    {
                        selected = true;
                        GUIUtility.hotControl = controlID;
                        Event.current.Use();
                    }
                    else if ((Event.current.modifiers & EventModifiers.Shift) == 0 && (Event.current.modifiers & EventModifiers.Command) == 0)
                    {

                        selected = false;

                    }

                    break;

                case EventType.MouseDrag:

                    if (GUIUtility.hotControl == controlID)
                    {

                        position = Event.current.mousePosition;
                        GUI.changed = true;

                        Event.current.Use();

                    }

                    break;

                case EventType.MouseUp:

                    if (GUIUtility.hotControl == controlID)
                    {

                        GUIUtility.hotControl = 0;
                        Event.current.Use();

                    }

                    break;

                case EventType.Repaint:

                    Color oldColor = GUI.color;
                    GUI.color = selected ? new Color(0.6f, 0.2f, 0.1f) : new Color(0.3f, 0.3f, 0.3f);
                    Rect rect = new Rect(position.x - 2, position.y - 2, 4, 4);
                    GUI.Box(rect, EditorGUIUtility.whiteTexture);
                    GUI.color = oldColor;

                    break;

            }

            return selected;
        }
    }
}
