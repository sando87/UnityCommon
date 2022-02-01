using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace EditorGUITable
{

	/// <summary>
	/// This cell class draws a button which, when clicked, will trigger the
	/// action given in the constructor.
	/// </summary>
	public class ActionCell : TableCell
	{
		string name;
		System.Action<int> action;

        public override void DrawCell (Rect rect, int idxRow, int idxColumn)
		{
            GUI.SetNextControlName(idxRow + "_" + idxColumn);
			
            if (GUI.Button (rect, name))
			{
				if (action != null)
					action.Invoke (idxRow);
			}
		}

		public override string comparingValue
		{
			get
			{
				return name;
			}
		}

		public ActionCell (string name, System.Action<int> action)
		{
			this.name = name;
			this.action = action;
		}
	}

}
