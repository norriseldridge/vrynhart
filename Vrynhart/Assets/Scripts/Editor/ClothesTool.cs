using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class ClothesTool : EditorWindow
{
    static EditorCurveBinding _spriteCurveBinding = new EditorCurveBinding
    {
        type = typeof(SpriteRenderer),
        path = "",
        propertyName = "m_Sprite"
    };

    Sprite selectedSprite;
    string filenameNoExtension;

    [MenuItem("Blood/Clothes Creator")]
    static void Init()
    {
        // Get existing open window or if none, make a new one:
        ClothesTool window = GetWindow<ClothesTool>(typeof(ClothesTool));
        window.Show();
    }

    [System.Obsolete]
    void OnGUI()
    {
        // select a sprite to work with
        GUILayout.Label("Sprite");
        selectedSprite = EditorGUILayout.ObjectField(selectedSprite, typeof(Sprite)) as Sprite;

        if (GUILayout.Button("Create Clothes"))
        {
            if (selectedSprite == null)
                return;

            var path = AssetDatabase.GetAssetPath(selectedSprite);
            filenameNoExtension = Path.GetFileNameWithoutExtension(path);

            ConfigureTexture(path);
            CreateAnimations(path);
            selectedSprite = null;
        }
    }

    void ConfigureTexture(string path)
    {
        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        importer.textureType = TextureImporterType.Sprite;
        importer.spritePivot = Vector2.one * 0.5f;

        // set pixel per unit
        importer.spritePixelsPerUnit = 16;

        // image is sliced by 16x16
        importer.spriteImportMode = SpriteImportMode.Multiple;
        var columns = selectedSprite.texture.width / 16;
        var rectsList = new List<Rect>();

        for (var i = 0; i < columns; ++i)
        {
            var rect = new Rect() {
                x = i * 16,
                y = 0,
                width = 16,
                height = 16,
                center = new Vector2(i * 16 + 8, 8)
            };
            rectsList.Add(rect);
        }

        var metas = new List<SpriteMetaData>();
        int rectNum = 0;
        foreach (Rect rect in rectsList)
        {
            var meta = new SpriteMetaData();
            meta.pivot = Vector2.zero;
            meta.alignment = (int)SpriteAlignment.Center;
            meta.rect = rect;
            meta.name = filenameNoExtension + "_" + rectNum++;
            metas.Add(meta);
        }

        importer.spritesheet = metas.ToArray();

        // image compression is turned off
        importer.textureCompression = TextureImporterCompression.Uncompressed;

        // image filter is turned off (point)
        importer.filterMode = FilterMode.Point;

        // apply settings
        AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
    }

    void CreateAnimations(string path)
    {
        var fi = new FileInfo(path);
        var basePath = fi.Directory.ToString();
        var index = basePath.IndexOf("Assets/");
        basePath = basePath.Substring(index);

        CreateAnimation("Idle", basePath, 0, 1);
        CreateAnimation("Run", basePath, 2, 5);

        CreateAnimationController(basePath);
    }

    void CreateAnimation(string animationName, string basePath, int firstFrame, int lastFrame)
    {
        var animation = new AnimationClip();
        animation.name = $"{filenameNoExtension}_{animationName}";
        animation.frameRate = 12f;
        var settings = AnimationUtility.GetAnimationClipSettings(animation);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(animation, settings);

        var sprites = new List<Sprite>();
        AddFrames(ref sprites, $"{basePath}/{filenameNoExtension}", firstFrame, lastFrame);
        var keyFrames = new ObjectReferenceKeyframe[sprites.Count];
        for (int i = 0; i < sprites.Count; i++)
        {
            keyFrames[i] = new ObjectReferenceKeyframe
            {
                time = i / animation.frameRate,
                value = sprites[i]
            };
        }
        AnimationUtility.SetObjectReferenceCurve(animation, _spriteCurveBinding, keyFrames);

        AssetDatabase.CreateAsset(animation, $"{basePath}/{animation.name}.anim");
    }

    void AddFrames(ref List<Sprite> frames, string basePath, int firstFrame, int lastFrame)
    {
        var spritePath = $"{basePath}.png";
        var sprites = AssetDatabase.LoadAllAssetsAtPath(spritePath)
            .OfType<Sprite>();

        for (var i = firstFrame; i <= lastFrame; ++i)
        {
            var sprite = sprites.First(s => s.name.Contains($"_{i}"));
            frames.Add(sprite);
        }
    }

    void CreateAnimationController(string basePath)
    {
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath($"{basePath}/{filenameNoExtension}.controller");

        var animations = new string[] { "Idle", "Run" };
        var root = controller.layers[0].stateMachine;
        foreach (var animation in animations)
        {
            var state = root.AddState(animation);
            state.motion = AssetDatabase.LoadAssetAtPath<AnimationClip>($"{basePath}/{filenameNoExtension}_{animation}.anim");
            state.speed = 0.4f;
            if (animation == "Idle")
                root.defaultState = state;
        }

        AssetDatabase.SaveAssets();
    }
}
