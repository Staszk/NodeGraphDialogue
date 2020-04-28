using UnityEngine;
using System.Collections;

[System.Serializable]
public class DialogueNode : BaseNode
{
    public static readonly string title = "Dialogue Node";
    public static readonly int height = 100;
    public static readonly int width = 200;

    private string charName = "Name";
    private Sprite charSprite = null;
    private string dialogue = "Dialogue...";

    public override void DrawNode()
    {
        GUILayout.BeginVertical();
        charName = GUILayout.TextField(charName);
        charSprite = (Sprite)UnityEditor.EditorGUILayout.ObjectField(charSprite, typeof(Sprite), false);
        GUILayout.FlexibleSpace();
        dialogue = GUILayout.TextArea(dialogue, GUILayout.MaxHeight(150));
        GUILayout.EndVertical();

        //base.DrawNode();
    }

    public override void DrawCurve()
    {
        base.DrawCurve();
    }
}
