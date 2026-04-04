#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text;
using System.Collections.Generic;

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
      GUILayout.Label("Import DungeonRoomData (Space-separated 2-digit)", EditorStyles.boldLabel);

      EditorGUILayout.Space();

      // Выбор TXT файла
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("TXT File:", GUILayout.Width(60));
      txtFilePath = EditorGUILayout.TextField(txtFilePath);
      if (GUILayout.Button("Browse", GUILayout.Width(80)))
      {
        string path = EditorUtility.OpenFilePanel("Select level layout TXT", "", "txt");
        if (!string.IsNullOrEmpty(path))
          txtFilePath = path;
      }
      EditorGUILayout.EndHorizontal();

      // Папка для сохранения
      EditorGUILayout.BeginHorizontal();
      EditorGUILayout.LabelField("Output Folder:", GUILayout.Width(80));
      outputFolder = EditorGUILayout.TextField(outputFolder);
      if (GUILayout.Button("Browse", GUILayout.Width(80)))
      {
        string folder = EditorUtility.OpenFolderPanel("Select output folder", "Assets", "");
        if (!string.IsNullOrEmpty(folder) && folder.StartsWith(Application.dataPath))
        {
          outputFolder = "Assets" + folder.Substring(Application.dataPath.Length);
        }
      }
      EditorGUILayout.EndHorizontal();

      EditorGUILayout.Space();

      if (GUILayout.Button("Create RoomData Asset"))
      {
        if (string.IsNullOrEmpty(txtFilePath) || !File.Exists(txtFilePath))
        {
          EditorUtility.DisplayDialog("Error", "Invalid TXT file path", "OK");
          return;
        }
        CreateRoomDataFromTxt(txtFilePath, outputFolder);
      }
    }

    private static void CreateRoomDataFromTxt(string txtPath, string outputFolderPath)
    {
      string[] lines = File.ReadAllLines(txtPath, Encoding.UTF8);

      // Фильтруем пустые строки
      List<string> nonEmptyLines = new List<string>();
      foreach (var line in lines)
      {
        if (!string.IsNullOrWhiteSpace(line))
          nonEmptyLines.Add(line.Trim());
      }

      if (nonEmptyLines.Count == 0)
      {
        EditorUtility.DisplayDialog("Error", "TXT file is empty", "OK");
        return;
      }

      int height = nonEmptyLines.Count;
      // Определяем ширину по первой строке (считаем количество чисел через пробел)
      string[] firstRowElements = nonEmptyLines[0]
        .Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
      int width = firstRowElements.Length;

      int[] layout = new int[width * height];

      for (int y = 0; y < height; y++)
      {
        // Разбиваем строку по пробелам, игнорируя лишние пробелы между числами
        string[] elements = nonEmptyLines[y]
          .Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);

        if (elements.Length != width)
        {
          EditorUtility.DisplayDialog(
            "Error",
            $"Row {y + 1} has {elements.Length} elements, expected {width}.",
            "OK"
          );
          return;
        }

        for (int x = 0; x < width; x++)
        {
          if (int.TryParse(elements[x], out int value))
          {
            layout[y * width + x] = value;
          }
          else
          {
            EditorUtility.DisplayDialog(
              "Error",
              $"Invalid number '{elements[x]}' at row {y + 1}, col {x + 1}",
              "OK"
            );
            return;
          }
        }
      }

      // Создание ассета
      DungeonRoomData roomData = ScriptableObject.CreateInstance<DungeonRoomData>();
      roomData.width = width;
      roomData.height = height;
      roomData.layout = layout;

      if (!AssetDatabase.IsValidFolder(outputFolderPath))
      {
        Directory.CreateDirectory(outputFolderPath);
        AssetDatabase.Refresh();
      }

      string baseName = Path.GetFileNameWithoutExtension(txtPath);
      string fullPath = Path.Combine(outputFolderPath, baseName + ".asset").Replace("\\", "/");

      AssetDatabase.CreateAsset(roomData, fullPath);
      AssetDatabase.SaveAssets();
      AssetDatabase.Refresh();

      EditorUtility.FocusProjectWindow();
      Selection.activeObject = roomData;

      Debug.Log(
        $"<color=green>[Importer]</color> Room '{baseName}' imported. Size: {width}x{height}"
      );
    }
  }
}
#endif
