using UnityEditor;
using UnityEngine;

namespace LOMN.Menu
{
    public class LOMNMenu : MonoBehaviour
    {
        [MenuItem("LOMN/Import")]
        static void OpenImport()
        {
            EditorWindow.GetWindow(typeof(FileImporter),false, "LOMN Import");
        }
    }
}