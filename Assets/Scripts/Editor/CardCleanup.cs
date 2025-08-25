using UnityEditor;
using UnityEngine;

public class CardCleanup
{
    [MenuItem("Tools/Cleanup Card SubAssets")]
    public static void Cleanup()
    {
        var cards = AssetDatabase.FindAssets("t:Card");
        foreach (var guid in cards)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var card = AssetDatabase.LoadAssetAtPath<Card>(path);
            var allSubAssets = AssetDatabase.LoadAllAssetsAtPath(path);

            foreach (var sub in allSubAssets)
            {
                if (sub is Action action && !card.actions.Contains(action))
                {
                    Debug.Log($"Deleting orphaned action {sub.name} from {card.name}");
                    Object.DestroyImmediate(sub, true);
                }
            }
            AssetDatabase.SaveAssets();
        }
    }
}