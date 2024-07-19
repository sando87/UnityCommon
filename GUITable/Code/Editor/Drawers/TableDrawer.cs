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

        private Rect mPosition = Rect.zero;
        private int mPreIndex = 100;

        private int mNewLineIndex = -1;
        private int mDelLineIndex = -1;

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
            if (GUI.Button(new Rect(70, 4, 70, 20), "ToCSV"))
            {
                property.serializedObject.targetObject.ExInvokePrivateMethod("ToCSV", null);
            }
            if (GUI.Button(new Rect(150, 4, 70, 20), "FromCSV"))
            {
                property.serializedObject.targetObject.ExInvokePrivateMethod("FromCSV", null);
            }

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

            if(mNewLineIndex >= 0)
            {
                collectionProperty.InsertArrayElementAtIndex(mNewLineIndex);
                mNewLineIndex = -1;
            }

            if (mDelLineIndex >= 0)
            {
                collectionProperty.DeleteArrayElementAtIndex(mDelLineIndex);
                mDelLineIndex = -1;
            }

            EditorGUI.indentLevel = 0;

            Rect r = new Rect(position.x + 15f, position.y, position.width - 15f, lastRect.height);

            tableState = DrawTable(r, collectionProperty, label, tableAttribute);

            List<string> properties = SerializationHelpers.GetElementsSerializedFields(collectionProperty, out bool isObjectReferencesCollection);
            ColumCount = properties.Count;
            RowCount = collectionProperty.arraySize;
        }

		protected virtual GUITableState DrawTable (Rect rect, SerializedProperty collectionProperty, GUIContent label, TableAttribute tableAttribute)
		{
			if (tableAttribute.properties == null && tableAttribute.widths == null)
            {
                return GUITable.DrawTable(rect, tableState, collectionProperty, GetMyFunctionalPropertyColumns(collectionProperty), GUITableOption.AllowScrollView(false));
                //return GUITable.DrawTable(rect, tableState, collectionProperty, GUITableOption.AllowScrollView(false));
            }
			else if (tableAttribute.widths == null)
				return GUITable.DrawTable(rect, tableState, collectionProperty, tableAttribute.properties.ToList(), GUITableOption.AllowScrollView(false));
			else
				return GUITable.DrawTable(rect, tableState, collectionProperty, GetPropertyColumns(tableAttribute), GUITableOption.AllowScrollView(false));
        }

        protected List<SelectorColumn> GetMyFunctionalPropertyColumns(SerializedProperty collectionProperty)
        {
            List<string> properties = SerializationHelpers.GetElementsSerializedFields(collectionProperty, out bool isObjectReferencesCollection);

            List<SelectorColumn> columns = new List<SelectorColumn>();
            
            columns.Add(new SelectFromFunctionColumn(sp =>
            {
                return new ActionCell("+", (idx) => mNewLineIndex = idx);
            },
            "New",
            TableColumn.Width(20f),
            TableColumn.Optional(true)));
            
            columns.Add(new SelectFromFunctionColumn(sp =>
            {
                return new ActionCell("-", (idx) => mDelLineIndex = idx);
            },
            "Del",
            TableColumn.Width(20f),
            TableColumn.Optional(true)));

            columns.AddRange(properties.Select(prop => (SelectorColumn)new SelectFromPropertyNameColumn(prop, ObjectNames.NicifyVariableName(prop))));

            return columns;
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
                                string focusID = GUI.GetNameOfFocusedControl();
                                string[] pieces = focusID.Split('_');
                                if(pieces.Length == 2)
                                {
                                    int rowIdx = int.Parse(pieces[0]);
                                    int colIdx = int.Parse(pieces[1]);
                                    if (rowIdx > 0)
                                    {
                                        string nextFocusID = (rowIdx - 1) + "_" + colIdx;
                                        GUI.FocusControl(nextFocusID);
                                    }
                                }
                                break;
                            }
                        case KeyCode.DownArrow:
                            {
                                string focusID = GUI.GetNameOfFocusedControl();
                                string[] pieces = focusID.Split('_');
                                if (pieces.Length == 2)
                                {
                                    int rowIdx = int.Parse(pieces[0]);
                                    int colIdx = int.Parse(pieces[1]);
                                    if(rowIdx < RowCount - 1)
                                    {
                                        string nextFocusID = (rowIdx + 1) + "_" + colIdx;
                                        GUI.FocusControl(nextFocusID);
                                    }
                                }
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