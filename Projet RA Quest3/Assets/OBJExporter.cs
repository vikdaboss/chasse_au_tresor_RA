using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

public class OBJExporter : MonoBehaviour
{
    [SerializeField]
    MeshFilter mesh;

    [ContextMenu("Export")]
    public void Export()
    {
        Mesh mesh = Application.isPlaying ? this.mesh.mesh : this.mesh.sharedMesh;
        string objPath = Export(mesh, Application.dataPath + Path.DirectorySeparatorChar + mesh.name);
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
        UnityEditor.EditorGUIUtility.PingObject(UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(objPath.Substring(objPath.IndexOf("Assets"))));
#endif
    }

    public static string Export(Mesh mesh, string filepathWithoutExtension = "") => Export(new MeshConstruction(mesh), filepathWithoutExtension);

    public static string Export(GameObject g, string filepathWithoutExtension = "")
    {
        MeshConstruction meshConstruction = new MeshConstruction(g.GetComponent<MeshFilter>().mesh);
        meshConstruction.name = g.name;
        meshConstruction.transform = g.transform;
        meshConstruction.material = g.GetComponent<MeshRenderer>().material;
        return Export(meshConstruction, filepathWithoutExtension);
    }

    public static string Export(MeshConstruction construction, string filepathWithoutExtension = "")
    {
        // Get path.
        if (string.IsNullOrEmpty(filepathWithoutExtension))
            filepathWithoutExtension = Application.persistentDataPath + "/" + construction.name + " "
                + DateTime.Now.ToString("yyyyMMdd") + " " + DateTime.Now.ToString("T").Replace(':', '-');
        string safepath = GetSafePath(filepathWithoutExtension + ".obj");
        filepathWithoutExtension = Path.GetDirectoryName(safepath) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(safepath);

        Debug.Log("Saving obj at " + filepathWithoutExtension);
        if (Application.isPlaying)
        {
            try
            {
                SaveOBJService(construction, filepathWithoutExtension);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
        else
            SaveOBJService(construction, filepathWithoutExtension);

        if (construction.material)
            ExportMaterial(construction.material, filepathWithoutExtension);

        return safepath;
    }

    static void SaveOBJService(MeshConstruction mesh, string filenameWithoutExtension)
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("g " + mesh.name);

        // Vertices.
        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            Vector3 v = mesh.transform ? mesh.transform.TransformPoint(mesh.vertices[i]) : mesh.vertices[i];
            sb.Append($"v {ToStringInvariant(v.x)} {ToStringInvariant(v.y)} {ToStringInvariant(-v.z)}");
            if (mesh.colors.Length > 0)
                sb.Append($" {ToStringInvariant(mesh.colors[i].r)} { ToStringInvariant(mesh.colors[i].g)} {ToStringInvariant(mesh.colors[i].b)} 1");
            sb.AppendLine();
        }

        // Normals.
        if (mesh.normals.Length > 0)
            foreach (Vector3 n in mesh.normals)
                sb.AppendLine($"vn {ToStringInvariant(n.x)} {ToStringInvariant(n.y)} {ToStringInvariant(-n.z)}");

        // UVs.
        if (mesh.uv.Length > 0)
            foreach (Vector3 uv in mesh.uv)
                sb.AppendLine($"vt {ToStringInvariant(uv.x)} {ToStringInvariant(uv.y)}");

        // Triangles.
        int[] triangles = mesh.triangles;
        for (int j = 0; j < triangles.Length; j += 3)
            sb.AppendLine(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}", triangles[j + 2] + 1, triangles[j + 1] + 1, triangles[j] + 1));

        using (StreamWriter sw = new StreamWriter(filenameWithoutExtension + ".obj"))
        {
            sw.AutoFlush = true;
            sw.Write(sb.ToString());
        }
    }

    static void ExportMaterial(Material material, string filenameWithoutExtension)
    {
        StringBuilder sb = new StringBuilder();

        // Material name
        sb.Append("newmtl ");
        sb.Append(material.name);
        sb.Append("\n");

        // Color
        sb.Append("Kd 1.000000 1.000000 1.000000");
        sb.Append("\n");

        // Save the texture.
        if (material.mainTexture)
        {
            // Write to a png file.
            Texture2D texture = (Texture2D)material.mainTexture;
            byte[] bytes = texture.EncodeToPNG();
            string texturePath = filenameWithoutExtension + ".png";
            File.WriteAllBytes(texturePath, bytes);
            Debug.Log(bytes.Length / 1024 + "Kb was saved as: " + texturePath);

            // Write texture reference to mtl.
            sb.Append("map_Kd " + Path.GetFileName(texturePath));
        }

        // Write mtl to file.
        using (StreamWriter sw = new StreamWriter(filenameWithoutExtension + ".mtl"))
        {
            sw.AutoFlush = true;
            sw.Write(sb.ToString());
        }

        Debug.Log("MTL file saved " + Application.persistentDataPath + "/");
    }

    public class MeshConstruction
    {
        public string name;
        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public Color[] colors;
        public Vector2[] uv;
        public Material material;
        public Transform transform;

        public MeshConstruction(Mesh mesh)
        {
            name = mesh.name;
            vertices = mesh.vertices;
            normals = mesh.normals;
            uv = mesh.uv;
            colors = mesh.colors;
            triangles = mesh.triangles;
        }
        public MeshConstruction() { }
    }

    #region Utilies.
    static string ToStringInvariant(float number) => number.ToString("0.000000", CultureInfo.InvariantCulture);

    static string GetSafePath(string filePath)
    {
        FileInfo targetFile = new FileInfo(filePath);
        FileInfo file = new FileInfo(filePath);
        int fileIndex = 0;
        while (file.Exists)
        {
            fileIndex++;
            file = new FileInfo(targetFile.DirectoryName + "/" + targetFile.Name.Insert(targetFile.Name.Length - targetFile.Extension.Length, " (" + fileIndex + ")"));
        }
        return file.ToString();
    }
    #endregion
}

#if UNITY_EDITOR
[UnityEditor.CustomEditor(typeof(OBJExporter)), UnityEditor.CanEditMultipleObjects]
class OBJExporterComponentEditor : UnityEditor.Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        GUILayout.Space(5);

        OBJExporter script = (OBJExporter)target;

        if (GUILayout.Button("Export"))
            script.Export();
    }
}
#endif