using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScreenShotTaker : MonoBehaviour
{
    private Camera cam;
    public string pathFolder;

    public List<GameObject> sceneObjects;
    public List<InventoryItemData> dataObjects;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    [ContextMenu("ScreenShot")]
    private void ProcessScreenshots()
    {
            
    }

    private IEnumerator Screenshot()
    {
        for (int i = 0; i < sceneObjects.Count; i++)
        {
            GameObject obj = sceneObjects[i];
            InventoryItemData data = dataObjects[i];
            
            obj.gameObject.SetActive(true);
            yield return null;

            TakeShot($"{Application.dataPath}/{pathFolder}/{data.id}CustomIcon.png");

            yield return null;
            obj.gameObject.SetActive(false);

            Sprite sprite = AssetDatabase.LoadAssetAtPath<Sprite>($"Assets/{pathFolder}/{data.id}CustomIcon.png");
            if (sprite != null)
            {
                data.icon = sprite;
                EditorUtility.SetDirty(data);
            }
            yield return null;
        }
    }

    public void TakeShot(string fullPath)
    {
        RenderTexture renderTexture = new RenderTexture(256, 256, 24);
        cam.targetTexture = renderTexture;
        Texture2D screenShot = new Texture2D(256, 256, TextureFormat.RGBA32, false);
        cam.Render();
        RenderTexture.active = renderTexture;
        
        screenShot.ReadPixels(new Rect(0,0,256,256), 0,0);
        cam.targetTexture = null;
        RenderTexture.active = null;

        if (Application.isEditor)
        {
            DestroyImmediate(renderTexture);
        }
        else
        {
            Destroy(renderTexture);
        }

        byte[] bytes = screenShot.EncodeToPNG();
        System.IO.File.WriteAllBytes(fullPath, bytes);
#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif        
    }
}
