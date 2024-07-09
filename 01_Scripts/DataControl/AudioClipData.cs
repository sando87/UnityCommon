using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class AudioClipData : ScriptableObject
{
    public AudioClip[] clips = null; // 재생시킬 오디오 클립(다수면 랜덤재생)
    [Range(0, 1)]
    public float amplification = 1.0f; // 소리 크기 조절

    public string ID { get { return this.name; } }

#if UNITY_EDITOR
    [UnityEditor.MenuItem("Assets/MetalSuits/CreateAudioAsset")]
    public static void CreateNewAsset()
    {
        AudioClip obj = UnityEditor.Selection.activeObject as AudioClip;
        if(obj == null)
            return;

        string path = UnityEditor.AssetDatabase.GetAssetPath(obj.GetInstanceID());
        path = path.RemoveFileExtension();

        List<AudioClip> clips = new List<AudioClip>();
        foreach(Object clip in UnityEditor.Selection.objects)
            clips.Add(clip as AudioClip);

        CreateAudioDataAsset(path, clips.ToArray());
    }

    static void CreateAudioDataAsset(string path, AudioClip[] clips)
    {
        AudioClipData asset = ScriptableObject.CreateInstance<AudioClipData>();
        asset.clips = clips;

        string assetPathAndName = UnityEditor.AssetDatabase.GenerateUniqueAssetPath(path + ".asset");

        UnityEditor.AssetDatabase.CreateAsset(asset, assetPathAndName);

        UnityEditor.AssetDatabase.SaveAssets();
        UnityEditor.AssetDatabase.Refresh();
    }

    [UnityEditor.MenuItem("/Tools/Create Audio Clip NameList")]
    private static void CreateAudioClipList()
    {
        string COMMENT =
@"/// <summary>
/// AudioClip 리소스 리스트가 자동으로 생성되는 클래스.
/// AudioClip 접근시 string으로 바로 접근하는것이 아니라 class.clipname 과 같은 형식으로 편하게 접근하기 위함.
/// AudioClipData.cs 파일의 CreateAudioClipList()함수에 의해 자동 생성되는 파일 및 클래스
/// </summary>
";

        string FILE_FULLNAME = @"Assets\00_MetaSuit\01_Script\10_Data\AudioClipList.cs";

        string firstLine = @"public static class AudioClipList {";
        string lastLine = @"}";


        List<string> lines = new List<string>();
        lines.Add(COMMENT);
        lines.Add(firstLine);

        AudioClipData[] datas = Resources.LoadAll<AudioClipData>(Consts.PathAudioClipData);
        foreach (AudioClipData data in datas)
            lines.Add("\tpublic static string " + data.name + @" = """ + data.name + @""";");

        lines.Add(lastLine);

        System.IO.File.WriteAllLines(FILE_FULLNAME, lines);
        UnityEditor.AssetDatabase.Refresh();
    }
#endif
}
