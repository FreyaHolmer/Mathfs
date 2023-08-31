namespace Freya {

	using UnityEngine;
	using UnityEditor;

	public class ClampedIntegerDrawer : PropertyDrawer {
		protected bool hasInitialized; // this flag is used to ensure it's valid when the inspector reveals it
	}

	[CustomPropertyDrawer( typeof(PositiveIntegerAttribute) )] public class PositiveIntegerDrawer : ClampedIntegerDrawer {
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) => IntegerDrawerUtils.OnGUI( ref hasInitialized, rect, property, label, false, false, true );
	}

	[CustomPropertyDrawer( typeof(NegativeIntegerAttribute) )] public class NegativeIntegerDrawer : ClampedIntegerDrawer {
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) => IntegerDrawerUtils.OnGUI( ref hasInitialized, rect, property, label, true, false, false );
	}

	[CustomPropertyDrawer( typeof(NonNegativeIntegerAttribute) )] public class NonNegativeIntegerDrawer : ClampedIntegerDrawer {
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) => IntegerDrawerUtils.OnGUI( ref hasInitialized, rect, property, label, false, true, true );
	}

	[CustomPropertyDrawer( typeof(NonPositiveIntegerAttribute) )] public class NonPositiveIntegerDrawer : ClampedIntegerDrawer {
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) => IntegerDrawerUtils.OnGUI( ref hasInitialized, rect, property, label, true, true, false );
	}

	[CustomPropertyDrawer( typeof(NonZeroIntegerAttribute) )] public class NonZeroIntegerDrawer : ClampedIntegerDrawer {
		public override void OnGUI( Rect rect, SerializedProperty property, GUIContent label ) => IntegerDrawerUtils.OnGUI( ref hasInitialized, rect, property, label, true, false, true );
	}

	static class IntegerDrawerUtils {

		public static void OnGUI( ref bool hasInitialized, Rect rect, SerializedProperty property, GUIContent label, bool allowNegative, bool allowZero, bool allowPositive ) {
			EditorGUI.BeginProperty( rect, label, property );

			if( property.propertyType == SerializedPropertyType.Integer ) {
				// PositiveIntegerAttribute range = attribute as PositiveIntegerAttribute;
				using( EditorGUI.ChangeCheckScope chChk = new() ) {
					int prevValue = property.intValue;

					EditorGUI.PropertyField( rect, property, label );
					if( chChk.changed || hasInitialized == false ) {
						hasInitialized = true;
						int newValue = property.intValue;
						// special case, make it "skip" 0 when scrubbing
						if( allowNegative && allowZero == false && allowPositive && newValue == 0 ) {
							property.intValue = prevValue > 0 ? -1 : 1;
						} else {
							// other cases can clamp
							int min = GetRangeMin( allowNegative, allowZero, allowPositive );
							int max = GetRangeMax( allowNegative, allowZero, allowPositive );
							if( newValue < min || newValue > max )
								property.intValue = Mathf.Clamp( newValue, min, max );
						}
					}
				}
			} else {
				EditorGUI.LabelField( rect, label.text, $"PositiveInteger only works on integer fields. Field is: {property.propertyType.ToString()}" );
			}

			EditorGUI.EndProperty();
		}

		static int GetRangeMin( bool allowNegative, bool allowZero, bool allowPositive ) {
			if( allowNegative )
				return int.MinValue;
			if( allowZero )
				return 0;
			return 1;
		}

		static int GetRangeMax( bool allowNegative, bool allowZero, bool allowPositive ) {
			if( allowPositive )
				return int.MaxValue;
			if( allowZero )
				return 0;
			return -1;
		}

	}

}