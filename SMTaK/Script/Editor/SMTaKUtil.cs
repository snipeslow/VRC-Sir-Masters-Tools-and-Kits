using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
namespace SirMasters {

    public class SMTaKOptionsWindow : EditorWindow
    {
        [MenuItem("SMTaK/SMTaK Options")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SMTaKOptionsWindow), false, "SMTaK Options");

        }
        void OnGUI()
        {
            EditorPrefs.SetBool("SMTaKForceDefaultBlendShapeNormalsToImport", EditorGUILayout.Toggle("Enable Force Default BlendShape Import to Import", EditorPrefs.GetBool("SMTaKForceDefaultBlendShapeNormalsToImport", true)));
            EditorPrefs.SetInt("SMTaKFloatTrunication", EditorGUILayout.IntSlider("Float Trunication Decimals", EditorPrefs.GetInt("SMTaKFloatTrunication", 3),2,5));
        }
    }

    public static class SMTaK
    {
        const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
        public static bool IsAprilFools()
        {
            if (DateTime.Now.Day == 1 && DateTime.Now.Month == 4)
            {
                return true;
            }
            return false;
        }
        public static string Randstring(int minCharAmount, int maxCharAmount)
		{
			string myString = "";
			int charAmount = UnityEngine.Random.Range(minCharAmount, maxCharAmount); //set those to the minimum and maximum length of your string
			for(int i=0; i<charAmount; i++)
			{
			 myString += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
			}
			return myString;
        }
        public static void DrawExperimentalWarning()
        {
            SMTaK.DrawHorizontalLine();
            EditorGUILayout.LabelField("WARNING: Experimental, please make backups before using.");
            SMTaK.DrawHorizontalLine();

        }
        public static void DrawHorizontalLine()
        {
            Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(1 + 10));
            r.height = 1;
            r.y += 10 / 2;
            EditorGUI.DrawRect(r, Color.gray);
        }
        public static float TrunicateFloat(float value, int decimalPoints)
        {
            
            return Mathf.Floor(value * Mathf.Pow(10, decimalPoints)) / Mathf.Pow(10, decimalPoints);
        }
    }
}