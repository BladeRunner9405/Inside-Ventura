#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;

namespace InsideVentura.World.Editor
{
    public class RoomDataImporter : EditorWindow
    {
        [MenuItem("Tools/InsideVentura/Import RoomData from TXT")]
        public static void ShowWindow()
        {
            GetWindow<RoomDataImporter>("Import RoomData");
        }

        private string txtFilePath = "";
        private string outputFolder = "Assets/Resources/DungeonRooms";

        private void OnGUI()
        {
            GUILayout.Label("Import DungeonRoomData from text file", EditorStyles.boldLabel);

            EditorGUILayout.Space();

            // Выбор TXT файла
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("TXT File:", GUILayout.Width(60));
            EditorGUILayout.TextField(txtFilePath);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string path = EditorUtility.OpenFilePanel("Select level layout TXT", "", "txt");
                if (!string.IsNullOrEmpty(path))
                {
                    txtFilePath = path;
                }
            }
            EditorGUILayout.EndHorizontal();

            // Папка для сохранения Asset
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Output Folder:", GUILayout.Width(80));
            EditorGUILayout.TextField(outputFolder);
            if (GUILayout.Button("Browse", GUILayout.Width(80)))
            {
                string folder = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
                if (!string.IsNullOrEmpty(folder))
                {
                    // Преобразуем абсолютный путь в относительный (относительно проекта)
                    if (folder.StartsWith(Application.dataPath))
                    {
                        outputFolder = "Assets" + folder.Substring(Application.dataPath.Length);
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("Error", "Folder must be inside Assets folder", "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            if (GUILayout.Button("Create RoomData Asset"))
            {
                if (string.IsNullOrEmpty(txtFilePath))
                {
                    EditorUtility.DisplayDialog("Error", "Please select a TXT file", "OK");
                    return;
                }

                if (!File.Exists(txtFilePath))
                {
                    EditorUtility.DisplayDialog("Error", "TXT file does not exist", "OK");
                    return;
                }

                CreateRoomDataFromTxt(txtFilePath, outputFolder);
            }
        }

        private static void CreateRoomDataFromTxt(string txtPath, string outputFolderPath)
        {
            // Чтение всех строк файла
            string[] lines = File.ReadAllLines(txtPath, Encoding.UTF8);
            
            // Удаляем пустые строки
            var nonEmptyLines = System.Array.FindAll(lines, line => !string.IsNullOrWhiteSpace(line));
            if (nonEmptyLines.Length == 0)
            {
                EditorUtility.DisplayDialog("Error", "TXT file contains no data", "OK");
                return;
            }

            int height = nonEmptyLines.Length;
            int width = nonEmptyLines[0].Trim().Length;
            
            // Проверяем, что все строки одинаковой длины
            for (int i = 1; i < height; i++)
            {
                int len = nonEmptyLines[i].Trim().Length;
                if (len != width)
                {
                    EditorUtility.DisplayDialog("Error", 
                        $"Line {i + 1} has length {len}, but expected {width}. All lines must have equal length.", 
                        "OK");
                    return;
                }
            }

            // Создаём одномерный массив layout
            int totalCells = width * height;
            int[] layout = new int[totalCells];
            
            for (int y = 0; y < height; y++)
            {
                string line = nonEmptyLines[y].Trim();
                for (int x = 0; x < width; x++)
                {
                    char c = line[x];
                    if (!char.IsDigit(c))
                    {
                        EditorUtility.DisplayDialog("Error", 
                            $"Invalid character '{c}' at line {y + 1}, position {x + 1}. Only digits allowed.", 
                            "OK");
                        return;
                    }
                    int value = c - '0';
                    // Можно добавить проверку допустимого диапазона, если нужно
                    // if (value < 0 || value > 4) ...
                    layout[y * width + x] = value;
                }
            }

            // Создаём ScriptableObject
            DungeonRoomData roomData = ScriptableObject.CreateInstance<DungeonRoomData>();
            roomData.width = width;
            roomData.height = height;
            roomData.layout = layout;

            // Формируем имя файла из имени исходного txt файла
            string baseName = Path.GetFileNameWithoutExtension(txtPath);
            string assetName = baseName + ".asset";
            
            // Убедимся, что выходная папка существует
            if (!AssetDatabase.IsValidFolder(outputFolderPath))
            {
                Directory.CreateDirectory(outputFolderPath);
                AssetDatabase.Refresh();
            }

            string fullAssetPath = Path.Combine(outputFolderPath, assetName);
            // Убираем дублирование "Assets" если оно уже есть в outputFolderPath
            fullAssetPath = fullAssetPath.Replace("\\", "/");
            
            // Создаём Asset
            AssetDatabase.CreateAsset(roomData, fullAssetPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            
            EditorUtility.DisplayDialog("Success", $"RoomData created at:\n{fullAssetPath}", "OK");
            
            // Выделяем созданный ассет в Project окне
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = roomData;
        }
    }
}
#endif