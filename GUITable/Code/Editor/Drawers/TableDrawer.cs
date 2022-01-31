using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace EditorGUITable
{

	/// <summary>
	/// Drawer for the Table Attribute.
	/// See the TableAttribute class documentation for the limitations of this attribute.
	/// </summary>
	[CustomPropertyDrawer(typeof(TableAttribute))]
	public class TableDrawer : PropertyDrawer
	{
		protected GUITableState tableState;

		Rect lastRect;

        bool isKeyDown = false;
        int RowCount = -1;
        int ColumCount = -1;
        int MinControlID = -1;
        int MaxControlID = -1;

        private Rect mPosition = Rect.zero;
        private int mPreIndex = 100;

		public override float GetPropertyHeight (SerializedProperty property, GUIContent label)
		{
			//Check that it is a collection
			Match match = Regex.Match(property.propertyPath, "^([a-zA-Z0-9_]*).Array.data\\[([0-9]*)\\]$");
			if (!match.Success)
			{
				return EditorGUIUtility.singleLineHeight;
			}

			// Check that it's the first element
			string index = match.Groups[2].Value;

			if (index != "0")
				return EditorGUIUtility.singleLineHeight + 2;
			
			return EditorGUIUtility.singleLineHeight + 2 + GetRequiredAdditionalHeight ();
		}

		protected virtual float GetRequiredAdditionalHeight ()
		{
			return 1f * EditorGUIUtility.singleLineHeight;
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            ProcessKeyInput();

			TableAttribute tableAttribute = (TableAttribute) attribute;

			//Check that it is a collection
			Match match = Regex.Match(property.propertyPath, "^([a-zA-Z0-9_]*).Array.data\\[([0-9]*)\\]$");
			if (!match.Success)
			{
				EditorGUI.LabelField(position, label.text, "Use the Table attribute with a collection.");
				return;
            }

            string collectionPath = match.Groups[1].Value;

            // Check that it's the first element
            string index = match.Groups[2].Value;

            int curIndex = int.Parse(index);

            if (curIndex == 0) mPosition = position; //테이블 초기 위치 설정

            if (mPreIndex > curIndex)
            {
                DrawTable(mPosition, property, label, collectionPath, tableAttribute);
            }

            mPreIndex = curIndex;
        }

        // 테이블을 그려줌
        private void DrawTable(Rect position, SerializedProperty property, GUIContent label, string collectionPath, TableAttribute tableAttribute)
        {
            // Sometimes GetLastRect returns 0, so we keep the last relevant value
            if (GUILayoutUtility.GetLastRect().width > 1f)
                lastRect = GUILayoutUtility.GetLastRect();

            SerializedProperty collectionProperty = property.serializedObject.FindProperty(collectionPath);

            EditorGUI.indentLevel = 0;

            Rect r = new Rect(position.x + 15f, position.y, position.width - 15f, lastRect.height);

            tableState = DrawTable(r, collectionProperty, label, tableAttribute);

            List<string> properties = SerializationHelpers.GetElementsSerializedFields(collectionProperty, out bool isObjectReferencesCollection);
            ColumCount = properties.Count;
            RowCount = collectionProperty.arraySize;

            if (MinControlID < 0)
            {
                MaxControlID = GUIUtility.GetControlID(FocusType.Passive) - 1;
                MinControlID = MaxControlID - (ColumCount * RowCount) + 1;
            }
        }

		protected virtual GUITableState DrawTable (Rect rect, SerializedProperty collectionProperty, GUIContent label, TableAttribute tableAttribute)
		{
			if (tableAttribute.properties == null && tableAttribute.widths == null)
				return GUITable.DrawTable(rect, tableState, collectionProperty, GUITableOption.AllowScrollView(false));
			else if (tableAttribute.widths == null)
				return GUITable.DrawTable(rect, tableState, collectionProperty, tableAttribute.properties.ToList(), GUITableOption.AllowScrollView(false));
			else
				return GUITable.DrawTable(rect, tableState, collectionProperty, GetPropertyColumns(tableAttribute), GUITableOption.AllowScrollView(false));
		}

		protected static List<SelectorColumn> GetPropertyColumns (TableAttribute tableAttribute)
		{
			List<SelectorColumn> res = new List<SelectorColumn>();
			for (int i = 0 ; i < tableAttribute.properties.Length ; i++)
			{
				if (i >= tableAttribute.widths.Length)
					res.Add(new SelectFromPropertyNameColumn(tableAttribute.properties[i], tableAttribute.properties[i]));
				else
					res.Add(new SelectFromPropertyNameColumn(tableAttribute.properties[i], tableAttribute.properties[i], TableColumn.Width(tableAttribute.widths[i])));
			}
			return res;
		}


        private void ProcessKeyInput()
        {
            Event e = Event.current;
            if (e != null)
            {
                if (e.type == EventType.KeyDown && !isKeyDown)
                {
                    switch (e.keyCode)
                    {
                        case KeyCode.UpArrow:
                            {
                                if (GUIUtility.keyboardControl - ColumCount >= MinControlID)
                                    GUIUtility.keyboardControl -= ColumCount;
                                break;
                            }
                        case KeyCode.DownArrow:
                            {
                                int maxCtrlID = Mathf.Min(MaxControlID, MinControlID + (ColumCount * RowCount) - 1);
                                if (GUIUtility.keyboardControl + ColumCount <= maxCtrlID)
                                    GUIUtility.keyboardControl += ColumCount;
                                break;
                            }
                    }
                }

                if (e.type == EventType.KeyDown)
                    isKeyDown = true;
                if (e.type == EventType.KeyUp)
                    isKeyDown = false;
            }

        }

	}

}