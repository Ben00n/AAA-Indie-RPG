// Based off of work developed by Tom Kail at Inkle
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditorInternal;
using UnityEditor;
#endif

namespace MalbersAnimations
{
#if UNITY_EDITOR

	/// <summary>
	///  This class contains extension methods for the SerializedProperty
	///  class.  Specifically, methods for dealing with object arrays.
	/// <see cref="SerializedProperty"/> extensions.
	/// </summary>
	public static class SerializedPropertyExtensions
    {
        // Use this to add an object to an object array represented by a SerializedProperty.
        public static void AddToObjectArray<T>(this SerializedProperty arrayProperty, T elementToAdd)
            where T : UnityEngine.Object
        {
            // If the SerializedProperty this is being called from is not an array, throw an exception.
            if (!arrayProperty.isArray)
                throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

            // Pull all the information from the target of the serializedObject.
            arrayProperty.serializedObject.Update();

            // Add a null array element to the start of the array then populate it with the object parameter.
            arrayProperty.InsertArrayElementAtIndex(0);
            arrayProperty.GetArrayElementAtIndex(0).objectReferenceValue = elementToAdd;

            //// Add a null array element to the end of the array then populate it with the object parameter.
            //arrayProperty.InsertArrayElementAtIndex(arrayProperty.arraySize);
            //arrayProperty.GetArrayElementAtIndex(arrayProperty.arraySize - 1).objectReferenceValue = elementToAdd;

            // Push all the information on the serializedObject back to the target.
            arrayProperty.serializedObject.ApplyModifiedProperties();
        }


        // Use this to remove the object at an index from an object array represented by a SerializedProperty.
        public static void RemoveFromObjectArrayAt(this SerializedProperty arrayProperty, int index)
        {
            // If the index is not appropriate or the serializedProperty this is being called from is not an array, throw an exception.
            if (index < 0)
                throw new UnityException("SerializedProperty " + arrayProperty.name + " cannot have negative elements removed.");

            if (!arrayProperty.isArray)
                throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

            if (index > arrayProperty.arraySize - 1)
                throw new UnityException("SerializedProperty " + arrayProperty.name + " has only " + arrayProperty.arraySize + " elements so element " + index + " cannot be removed.");

            // Pull all the information from the target of the serializedObject.
            arrayProperty.serializedObject.Update();

            // If there is a non-null element at the index, null it.
            if (arrayProperty.GetArrayElementAtIndex(index).objectReferenceValue)
                arrayProperty.DeleteArrayElementAtIndex(index);

            // Delete the null element from the array at the index.
            arrayProperty.DeleteArrayElementAtIndex(index);

            // Push all the information on the serializedObject back to the target.
            arrayProperty.serializedObject.ApplyModifiedProperties();
        }


        // Use this to remove an object from an object array represented by a SerializedProperty.
        public static void RemoveFromObjectArray<T>(this SerializedProperty arrayProperty, T elementToRemove)
            where T : UnityEngine.Object
        {
            // If either the serializedProperty doesn't represent an array or the element is null, throw an exception.
            if (!arrayProperty.isArray)
                throw new UnityException("SerializedProperty " + arrayProperty.name + " is not an array.");

            if (!elementToRemove)
                throw new UnityException("Removing a null element is not supported using this method.");

            // Pull all the information from the target of the serializedObject.
            arrayProperty.serializedObject.Update();

            // Go through all the elements in the serializedProperty's array...
            for (int i = 0; i < arrayProperty.arraySize; i++)
            {
                SerializedProperty elementProperty = arrayProperty.GetArrayElementAtIndex(i);

                // ... until the element matches the parameter...
                if (elementProperty.objectReferenceValue == elementToRemove)
                {
                    // ... then remove it.
                    arrayProperty.RemoveFromObjectArrayAt(i);
                    return;
                }
            }

            throw new UnityException("Element " + elementToRemove.name + "was not found in property " + arrayProperty.name);
        }

    }

     
	/// <summary>
	/// <see cref="SerializedObject"/> extensions.
	/// </summary>
	public static class SerializedObjectExtensions
	{
        /// <summary>
        /// Finds and draws the properties given under a foldout and within a GUI.Box if <paramref name="foldout"/> is true.
        /// </summary>
        /// <param name="serializedObject">The serializedProperty with the given fields</param>
        /// <param name="title">The title of the drawn box</param>
        /// <param name="fields">The fields to draw</param>
        /// <param name="foldout">The value of the drawn foldout</param>
        /// <param name="beginning">Action fired at the start of the GUI.Box</param>
        /// <param name="end">Action fired at the end of the GUI.Box</param>
        public static void DrawFieldsUnderFoldout(this SerializedObject serializedObject, string title, string[] fields,
												  ref bool foldout, Action beginning = null, Action end = null)
		{
			foldout = EditorGUILayout.Foldout(foldout, title);
			if (foldout)
			{
				EditorGUI.indentLevel++;
				if (beginning != null)
				{
					beginning();
				}
				foreach (var propertyPath in fields)
				{
					SerializedProperty property = serializedObject.FindProperty(propertyPath);
					if (property != null)
					{
						EditorGUILayout.PropertyField(property, true);
					}
					else
					{
						Debug.LogErrorFormat("Property: {0} not found in {1}", propertyPath,
											 serializedObject.targetObject);
					}
				}
				if (end != null)
				{
					end();
				}
				EditorGUI.indentLevel--;
			}

			if (GUI.changed)
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		
	}

#endif
}