using UnityEngine;
using UnityEngine.Rendering;

namespace _Scripts.Truck.Skidmarks
{
    public class Skidmarks : MonoBehaviour
    {
        public Material _skidmarksMaterial;
        public float _skidmarkWidth = 0.5f;
        private const int MaxSkidMarks = 2048;
        private const float ContactOffset = 0.02f;
        private const float MinDistance = 0.25f;
        private const float MinDistanceSquare = MinDistance * MinDistance;
        private const float MaxOpacity = 1.0f;

        private class SkidMarkSection
        {
            public Vector3 Pos = Vector3.zero;
            public Vector3 Normal = Vector3.zero;
            public Vector4 Tangent = Vector4.zero;
            public Vector3 Posl = Vector3.zero;
            public Vector3 Posr = Vector3.zero;
            public Color32 Colour;
            public int LastIndex;
        };

        private int _markIndex;
        private SkidMarkSection[] _skidmarks;
        private Mesh _marksMesh;
        private MeshRenderer _mr;
        private MeshFilter _mf;

        private Vector3[] _vertices;
        private Vector3[] _normals;
        private Vector4[] _tangents;
        private Color32[] _colors;
        private Vector2[] _uvs;
        private int[] _triangles;

        private bool _meshUpdated;
        private bool _haveSetBounds;

        private Color32 _black = Color.black;

        private void Awake()
        {
            if (transform.position != Vector3.zero)
            {
                transform.position = Vector3.zero;
                transform.rotation = Quaternion.identity;
            }
        }

        protected void Start()
        {
            _skidmarks = new SkidMarkSection[MaxSkidMarks];

            for (int i = 0; i < MaxSkidMarks; i++)
            {
                _skidmarks[i] = new SkidMarkSection();
            }

            _mf = GetComponent<MeshFilter>();
            _mr = GetComponent<MeshRenderer>();

            if (_mr == null)
            {
                _mr = gameObject.AddComponent<MeshRenderer>();
            }

            _marksMesh = new Mesh();
            _marksMesh.MarkDynamic();

            if (_mf == null)
            {
                _mf = gameObject.AddComponent<MeshFilter>();
            }

            _mf.sharedMesh = _marksMesh;

            _vertices = new Vector3[MaxSkidMarks * 4];
            _normals = new Vector3[MaxSkidMarks * 4];
            _tangents = new Vector4[MaxSkidMarks * 4];
            _colors = new Color32[MaxSkidMarks * 4];
            _uvs = new Vector2[MaxSkidMarks * 4];
            _triangles = new int[MaxSkidMarks * 6];

            _mr.shadowCastingMode = ShadowCastingMode.Off;
            _mr.receiveShadows = false;
            _mr.material = _skidmarksMaterial;
            _mr.lightProbeUsage = LightProbeUsage.Off;
        }

        protected void LateUpdate()
        {
            if (!_meshUpdated) return;
            _meshUpdated = false;

            _marksMesh.vertices = _vertices;
            _marksMesh.normals = _normals;
            _marksMesh.tangents = _tangents;
            _marksMesh.triangles = _triangles;
            _marksMesh.colors32 = _colors;
            _marksMesh.uv = _uvs;

            if (!_haveSetBounds)
            {
                _marksMesh.bounds = new Bounds(new Vector3(0, 0, 0), new Vector3(10000, 10000, 10000));
                _haveSetBounds = true;
            }

            _mf.sharedMesh = _marksMesh;
        }

        public int AddSkidMark(Vector3 pos, Vector3 normal, float opacity, int lastIndex)
        {
            if (opacity > 1) opacity = 1.0f;
            else if (opacity < 0) return -1;

            _black.a = (byte) (opacity * 255);
            return AddSkidMark(pos, normal, _black, lastIndex);
        }

        public int AddSkidMark(Vector3 pos, Vector3 normal, Color32 colour, int lastIndex)
        {
            if (colour.a == 0) return -1;

            SkidMarkSection lastSection = null;
            Vector3 distAndDirection = Vector3.zero;
            Vector3 newPos = pos + normal * ContactOffset;
            if (lastIndex != -1)
            {
                lastSection = _skidmarks[lastIndex];
                distAndDirection = newPos - lastSection.Pos;
                if (distAndDirection.sqrMagnitude < MinDistanceSquare)
                {
                    return lastIndex;
                }

                if (distAndDirection.sqrMagnitude > MinDistanceSquare * 10)
                {
                    lastIndex = -1;
                    lastSection = null;
                }
            }

            colour.a = (byte) (colour.a * MaxOpacity);

            SkidMarkSection curSection = _skidmarks[_markIndex];

            curSection.Pos = newPos;
            curSection.Normal = normal;
            curSection.Colour = colour;
            curSection.LastIndex = lastIndex;

            if (lastSection != null)
            {
                Vector3 xDirection = Vector3.Cross(distAndDirection, normal).normalized;
                curSection.Posl = curSection.Pos + xDirection * _skidmarkWidth * 0.5f;
                curSection.Posr = curSection.Pos - xDirection * _skidmarkWidth * 0.5f;
                curSection.Tangent = new Vector4(xDirection.x, xDirection.y, xDirection.z, 1);

                if (lastSection.LastIndex == -1)
                {
                    lastSection.Tangent = curSection.Tangent;
                    lastSection.Posl = curSection.Pos + xDirection * _skidmarkWidth * 0.5f;
                    lastSection.Posr = curSection.Pos - xDirection * _skidmarkWidth * 0.5f;
                }
            }

            UpdateSkidmarksMesh();

            int curIndex = _markIndex;
            _markIndex = ++_markIndex % MaxSkidMarks;

            return curIndex;
        }

        private void UpdateSkidmarksMesh()
        {
            SkidMarkSection curr = _skidmarks[_markIndex];

            if (curr.LastIndex == -1) return;

            SkidMarkSection last = _skidmarks[curr.LastIndex];
            _vertices[_markIndex * 4 + 0] = last.Posl;
            _vertices[_markIndex * 4 + 1] = last.Posr;
            _vertices[_markIndex * 4 + 2] = curr.Posl;
            _vertices[_markIndex * 4 + 3] = curr.Posr;

            _normals[_markIndex * 4 + 0] = last.Normal;
            _normals[_markIndex * 4 + 1] = last.Normal;
            _normals[_markIndex * 4 + 2] = curr.Normal;
            _normals[_markIndex * 4 + 3] = curr.Normal;

            _tangents[_markIndex * 4 + 0] = last.Tangent;
            _tangents[_markIndex * 4 + 1] = last.Tangent;
            _tangents[_markIndex * 4 + 2] = curr.Tangent;
            _tangents[_markIndex * 4 + 3] = curr.Tangent;

            _colors[_markIndex * 4 + 0] = last.Colour;
            _colors[_markIndex * 4 + 1] = last.Colour;
            _colors[_markIndex * 4 + 2] = curr.Colour;
            _colors[_markIndex * 4 + 3] = curr.Colour;

            _uvs[_markIndex * 4 + 0] = new Vector2(0, 0);
            _uvs[_markIndex * 4 + 1] = new Vector2(1, 0);
            _uvs[_markIndex * 4 + 2] = new Vector2(0, 1);
            _uvs[_markIndex * 4 + 3] = new Vector2(1, 1);

            _triangles[_markIndex * 6 + 0] = _markIndex * 4 + 0;
            _triangles[_markIndex * 6 + 2] = _markIndex * 4 + 1;
            _triangles[_markIndex * 6 + 1] = _markIndex * 4 + 2;

            _triangles[_markIndex * 6 + 3] = _markIndex * 4 + 2;
            _triangles[_markIndex * 6 + 5] = _markIndex * 4 + 1;
            _triangles[_markIndex * 6 + 4] = _markIndex * 4 + 3;

            _meshUpdated = true;
        }
    }
}