using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class BuildArenaScene
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";
    private const string MaterialFolder = "Assets/Materials";

    [MenuItem("Tools/GEA/Build Arena Ground")]
    public static void Build()
    {
        EnsureFolder();
        Scene scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Single);

        GameObject oldBuilder = GameObject.Find("Arena Environment");
        if (oldBuilder != null) Object.DestroyImmediate(oldBuilder);

        GameObject oldArena = GameObject.Find("Arena Ground");
        if (oldArena != null) Object.DestroyImmediate(oldArena);

        Material foundation = LoadOrCreate("Arena_Trim", new Color(0.055f, 0.09f, 0.13f), 0.18f);
        Material ground = LoadOrCreate("Arena_Ground", new Color(0.12f, 0.20f, 0.23f), 0.02f);
        Material tileA = LoadOrCreate("Arena_Tile_A", new Color(0.22f, 0.32f, 0.34f), 0.06f);
        Material tileB = LoadOrCreate("Arena_Tile_B", new Color(0.18f, 0.28f, 0.31f), 0.06f);
        Material accent = LoadOrCreate("Arena_Cyan", new Color(0.08f, 0.78f, 0.88f), 0.22f);

        GameObject root = new GameObject("Arena Ground");
        SceneManager.MoveGameObjectToScene(root, scene);
        CreateBlock(root.transform, "Foundation", Vector3.zero, new Vector3(24f, 0.5f, 24f), foundation);
        CreateBlock(root.transform, "Ground Surface", new Vector3(0f, 0.27f, 0f), new Vector3(23.4f, 0.08f, 23.4f), ground);

        for (int x = -5; x <= 5; x++)
        {
            for (int z = -5; z <= 5; z++)
            {
                Material tile = ((x + z) & 1) == 0 ? tileA : tileB;
                CreateBlock(root.transform, "Floor Tile", new Vector3(x * 2f, 0.34f, z * 2f),
                    new Vector3(1.94f, 0.06f, 1.94f), tile);
            }
        }

        CreateBlock(root.transform, "North Wall", new Vector3(0f, 0.75f, 11.8f), new Vector3(24f, 1.5f, 0.4f), foundation);
        CreateBlock(root.transform, "South Wall", new Vector3(0f, 0.75f, -11.8f), new Vector3(24f, 1.5f, 0.4f), foundation);
        CreateBlock(root.transform, "East Wall", new Vector3(11.8f, 0.75f, 0f), new Vector3(0.4f, 1.5f, 24f), foundation);
        CreateBlock(root.transform, "West Wall", new Vector3(-11.8f, 0.75f, 0f), new Vector3(0.4f, 1.5f, 24f), foundation);

        for (int x = -1; x <= 1; x += 2)
        {
            for (int z = -1; z <= 1; z += 2)
            {
                Vector3 corner = new Vector3(x * 10.5f, 0f, z * 10.5f);
                CreateBlock(root.transform, "Corner Pylon", corner + Vector3.up * 1.5f, new Vector3(0.9f, 3f, 0.9f), foundation);
                CreateBlock(root.transform, "Pylon Light", corner + Vector3.up * 3.1f, new Vector3(1.05f, 0.16f, 1.05f), accent);
            }
        }

        CreateBlock(root.transform, "Cover A", new Vector3(-5f, 0.75f, 2f), new Vector3(2.4f, 1.2f, 1.2f), foundation);
        CreateBlock(root.transform, "Cover B", new Vector3(5f, 0.75f, 4f), new Vector3(1.2f, 1.2f, 2.4f), foundation);
        CreateBlock(root.transform, "Cover C", new Vector3(-4f, 0.75f, -5f), new Vector3(2.4f, 1.2f, 1.2f), foundation);
        CreateBlock(root.transform, "Cover D", new Vector3(5f, 0.75f, -4f), new Vector3(1.2f, 1.2f, 2.4f), foundation);

        GameObject player = GameObject.Find("Player");
        if (player != null) player.transform.position = new Vector3(0f, 0.37f, 0f);

        Camera camera = Camera.main;
        if (camera != null)
        {
            camera.transform.position = new Vector3(0f, 7f, -13f);
            camera.transform.rotation = Quaternion.Euler(25f, 0f, 0f);
        }

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Arena ground and set dressing were added to SampleScene.");
    }

    private static void CreateBlock(Transform parent, string objectName, Vector3 position, Vector3 scale, Material material)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = objectName;
        block.transform.SetParent(parent, false);
        block.transform.localPosition = position;
        block.transform.localScale = scale;
        block.GetComponent<Renderer>().sharedMaterial = material;
    }

    private static Material LoadOrCreate(string name, Color color, float metallic)
    {
        string path = MaterialFolder + "/" + name + ".mat";
        Material material = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (material == null)
        {
            Shader shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) shader = Shader.Find("Standard");
            material = new Material(shader);
            AssetDatabase.CreateAsset(material, path);
        }
        material.color = color;
        if (material.HasProperty("_Metallic")) material.SetFloat("_Metallic", metallic);
        if (material.HasProperty("_Smoothness")) material.SetFloat("_Smoothness", 0.35f);
        EditorUtility.SetDirty(material);
        return material;
    }

    private static void EnsureFolder()
    {
        if (!AssetDatabase.IsValidFolder(MaterialFolder))
            AssetDatabase.CreateFolder("Assets", "Materials");
    }
}
