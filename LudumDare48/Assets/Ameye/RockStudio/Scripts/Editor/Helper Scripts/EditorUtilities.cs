using UnityEditor;
using UnityEngine;

namespace RockStudio 
{
	public class EditorUtilities 
	{
		public static void DrawUILine(Color aColor, int aThickness = 2, int aPadding = 10) 
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(aPadding+aThickness));
			r.height = aThickness;
			r.y+=aPadding/2;
			r.x-=2;
			r.width +=6;
			EditorGUI.DrawRect(r, aColor);
		}
	}
}