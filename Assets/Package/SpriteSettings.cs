using UnityEngine;

[ExecuteAlways]
public class SpriteSettings : MonoBehaviour
{
    public enum AssetType { Sprite, Texture }
    public enum ShaderMode { URP_Lit, Legacy_Standard }

    [SerializeField, HideInInspector] public Texture _texture;
    [SerializeField, HideInInspector] public Sprite _sprite;
    [SerializeField, HideInInspector] public AssetType _assetType = AssetType.Sprite;
    [SerializeField, HideInInspector] public ShaderMode _shaderMode = ShaderMode.URP_Lit; 
    [SerializeField, HideInInspector] public Color _color = Color.white;
    [SerializeField, HideInInspector] public MeshRenderer _meshRenderer;
    [SerializeField, HideInInspector] public MeshFilter _meshFilter;
    [SerializeField, HideInInspector] public Material _material;
    [SerializeField, HideInInspector] public float _meshScale = 1f;

    private static MaterialPropertyBlock _propBlock;
    
    private static readonly int BaseMapURP = Shader.PropertyToID("_BaseMap");
    private static readonly int BaseColorURP = Shader.PropertyToID("_BaseColor");
    private static readonly int MainTexLegacy = Shader.PropertyToID("_MainTex");
    private static readonly int ColorLegacy = Shader.PropertyToID("_Color");

    private void OnEnable()
    {
        Refresh();
    }

    public void Refresh()
    {
        if (!_meshRenderer || !_meshFilter) return;

        if (_material != null && _meshRenderer.sharedMaterial != _material)
        {
            _meshRenderer.sharedMaterial = _material;
        }

        ApplyVisualProperties();

        if ((_assetType == AssetType.Sprite && _sprite != null) || (_assetType == AssetType.Texture && _texture != null))
        {
            UpdateMesh();
        }
    }

    private void ApplyVisualProperties()
    {
        _propBlock ??= new MaterialPropertyBlock();
        _meshRenderer.GetPropertyBlock(_propBlock);

        Texture targetTex = (_assetType == AssetType.Sprite && _sprite != null) ? _sprite.texture : _texture;
        
        int texID = (_shaderMode == ShaderMode.URP_Lit) ? BaseMapURP : MainTexLegacy;
        int colorID = (_shaderMode == ShaderMode.URP_Lit) ? BaseColorURP : ColorLegacy;

        if (targetTex != null) _propBlock.SetTexture(texID, targetTex);
        _propBlock.SetColor(colorID, _color);
        
        _meshRenderer.SetPropertyBlock(_propBlock);
    }

    private void UpdateMesh()
    {
        Rect rect;
        float ppu, texW, texH;

        if (_assetType == AssetType.Sprite && _sprite != null)
        {
            rect = _sprite.textureRect;
            ppu = _sprite.pixelsPerUnit;
            texW = _sprite.texture.width;
            texH = _sprite.texture.height;
        }
        else
        {
            rect = new Rect(0, 0, _texture.width, _texture.height);
            ppu = 100f;
            texW = _texture.width;
            texH = _texture.height;
        }

        Mesh mesh = _meshFilter.sharedMesh;
        if (mesh == null || mesh.name != "SpriteMesh_Instance")
        {
            mesh = new Mesh { name = "SpriteMesh_Instance", hideFlags = HideFlags.DontSave };
        }

        float aspect = rect.width / rect.height;
        float worldHeight = (rect.height / ppu) * _meshScale;
        float worldWidth = worldHeight * aspect;
        float hW = worldWidth * 0.5f;
        float hH = worldHeight * 0.5f;

        mesh.vertices = new Vector3[] {
            new (-hW, -hH, 0), new (hW, -hH, 0),
            new (-hW, hH, 0),  new (hW, hH, 0)
        };

        mesh.uv = new Vector2[] {
            new (rect.x / texW, rect.y / texH),
            new (rect.xMax / texW, rect.y / texH),
            new (rect.x / texW, rect.yMax / texH),
            new (rect.xMax / texW, rect.yMax / texH)
        };

        mesh.triangles = new int[] { 0, 2, 1, 1, 2, 3 };
        mesh.RecalculateBounds();
        
        _meshFilter.sharedMesh = mesh;
    }

    public void ChangeSprite(Sprite newSprite) { _sprite = newSprite; Refresh(); }
    public void ChangeColor(Color newColor) { _color = newColor; Refresh(); }
}