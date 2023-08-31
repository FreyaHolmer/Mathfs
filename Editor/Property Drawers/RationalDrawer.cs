using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Freya {

	[CustomPropertyDrawer( typeof(Rational) )]
	public class IngredientDrawer : PropertyDrawer {

		bool hasInitialized;
		
		// Draw the property inside the given rect
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) {
			// Using BeginProperty / EndProperty on the parent property means that
			// prefab override logic works on the entire property.
			EditorGUI.BeginProperty( rect, label, property );

			// Draw label
			rect = EditorGUI.PrefixLabel( rect, GUIUtility.GetControlID( FocusType.Passive ), label );

			// Don't make child fields be indented
			int indent = EditorGUI.indentLevel;
			EditorGUI.indentLevel = 0;

			// Calculate rects
			int slashWidth = 10;
			int inputBoxWidth = Mathfs.FloorToInt( ( rect.width - slashWidth ) / 2 ); 
			Rect rectNumerator = new Rect( rect.x, rect.y, inputBoxWidth, rect.height );
			Rect rectDivision = new Rect( rect.x + inputBoxWidth, rect.y, slashWidth, rect.height );
			Rect rectDenominator = new Rect( rect.x + inputBoxWidth + slashWidth, rect.y, inputBoxWidth, rect.height );

			SerializedProperty numerator = property.FindPropertyRelative( "n" );
			SerializedProperty denominator = property.FindPropertyRelative( "d" );
			
			using( var chChk = new EditorGUI.ChangeCheckScope() ) {
				EditorGUI.DelayedIntField( rectNumerator, numerator, GUIContent.none );
				GUI.Label( rectDivision, "/" );
				EditorGUI.DelayedIntField( rectDenominator, denominator, GUIContent.none );
				if( chChk.changed || hasInitialized == false ) {
					hasInitialized = true;
					// validation, make sure we're not dividing by 0
					if( denominator.intValue == 0 ) {
						denominator.intValue = 1;
						Debug.LogWarning( "Rational numbers cannot divide by 0. Setting denominator to 1", property.serializedObject.targetObject );
					}
				}
			}

			// Set indent back to what it was
			EditorGUI.indentLevel = indent;

			EditorGUI.EndProperty();
		}
	}

}