using System;
using UnityEngine;

namespace TMPro
{
	[ExecuteInEditMode]
	[RequireComponent(typeof(MeshRenderer))]
	[RequireComponent(typeof(MeshFilter))]
	public class TMP_SubMesh : MonoBehaviour
	{
		[SerializeField]
		private TMP_FontAsset m_fontAsset;

		[SerializeField]
		private TMP_SpriteAsset m_spriteAsset;

		[SerializeField]
		private Material m_material;

		[SerializeField]
		private Material m_sharedMaterial;

		[SerializeField]
		private bool m_isDefaultMaterial;

		[SerializeField]
		private float m_padding;

		[SerializeField]
		private Renderer m_renderer;

		[SerializeField]
		private MeshFilter m_meshFilter;

		private Mesh m_mesh;

		[SerializeField]
		private TextMeshPro m_TextComponent;

		[NonSerialized]
		private bool m_isRegisteredForEvents;

		public TMP_FontAsset fontAsset
		{
			get
			{
				return m_fontAsset;
			}
			set
			{
				m_fontAsset = value;
			}
		}

		public TMP_SpriteAsset spriteAsset
		{
			get
			{
				return m_spriteAsset;
			}
			set
			{
				m_spriteAsset = value;
			}
		}

		public Material material
		{
			get
			{
				return GetMaterial(m_sharedMaterial);
			}
			set
			{
				if (m_sharedMaterial.GetInstanceID() != value.GetInstanceID())
				{
					m_sharedMaterial = value;
					m_padding = GetPaddingForMaterial();
					SetVerticesDirty();
					SetMaterialDirty();
				}
			}
		}

		public Material sharedMaterial
		{
			get
			{
				return GetSharedMaterial();
			}
			set
			{
				SetSharedMaterial(value);
			}
		}

		public bool isDefaultMaterial
		{
			get
			{
				return m_isDefaultMaterial;
			}
			set
			{
				m_isDefaultMaterial = value;
			}
		}

		public float padding
		{
			get
			{
				return m_padding;
			}
			set
			{
				m_padding = value;
			}
		}

		public Renderer renderer
		{
			get
			{
				if (m_renderer == null)
				{
					m_renderer = GetComponent<Renderer>();
				}
				return m_renderer;
			}
		}

		public MeshFilter meshFilter
		{
			get
			{
				if (m_meshFilter == null)
				{
					m_meshFilter = GetComponent<MeshFilter>();
				}
				return m_meshFilter;
			}
		}

		public Mesh mesh
		{
			get
			{
				if (m_mesh == null)
				{
					m_mesh = new Mesh();
					m_mesh.hideFlags = HideFlags.HideAndDontSave;
					meshFilter.mesh = m_mesh;
				}
				return m_mesh;
			}
			set
			{
				m_mesh = value;
			}
		}

		private void OnEnable()
		{
			if (!m_isRegisteredForEvents)
			{
				m_isRegisteredForEvents = true;
			}
			if (m_sharedMaterial != null)
			{
				m_sharedMaterial.SetVector(ShaderUtilities.ID_ClipRect, new Vector4(-10000f, -10000f, 10000f, 10000f));
			}
		}

		private void OnDestroy()
		{
			if (m_mesh != null)
			{
				UnityEngine.Object.DestroyImmediate(m_mesh);
			}
			m_isRegisteredForEvents = false;
		}

		public static TMP_SubMesh AddSubTextObject(TextMeshPro textComponent, MaterialReference materialReference)
		{
			GameObject gameObject = new GameObject("TMP SubMesh [" + materialReference.material.name + "]");
			TMP_SubMesh tMP_SubMesh = gameObject.AddComponent<TMP_SubMesh>();
			gameObject.transform.SetParent(textComponent.transform, false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.layer = textComponent.gameObject.layer;
			tMP_SubMesh.m_meshFilter = gameObject.GetComponent<MeshFilter>();
			tMP_SubMesh.m_TextComponent = textComponent;
			tMP_SubMesh.m_fontAsset = materialReference.fontAsset;
			tMP_SubMesh.m_spriteAsset = materialReference.spriteAsset;
			tMP_SubMesh.m_isDefaultMaterial = materialReference.isDefaultMaterial;
			tMP_SubMesh.SetSharedMaterial(materialReference.material);
			tMP_SubMesh.renderer.sortingLayerID = textComponent.renderer.sortingLayerID;
			tMP_SubMesh.renderer.sortingOrder = textComponent.renderer.sortingOrder;
			return tMP_SubMesh;
		}

		public void DestroySelf()
		{
			UnityEngine.Object.Destroy(base.gameObject, 1f);
		}

		private Material GetMaterial(Material mat)
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			if (m_material == null || m_material.GetInstanceID() != mat.GetInstanceID())
			{
				m_material = CreateMaterialInstance(mat);
			}
			m_sharedMaterial = m_material;
			m_padding = GetPaddingForMaterial();
			SetVerticesDirty();
			SetMaterialDirty();
			return m_sharedMaterial;
		}

		private Material CreateMaterialInstance(Material source)
		{
			Material material = new Material(source);
			material.shaderKeywords = source.shaderKeywords;
			material.name += " (Instance)";
			return material;
		}

		private Material GetSharedMaterial()
		{
			if (m_renderer == null)
			{
				m_renderer = GetComponent<Renderer>();
			}
			return m_renderer.sharedMaterial;
		}

		private void SetSharedMaterial(Material mat)
		{
			m_sharedMaterial = mat;
			m_padding = GetPaddingForMaterial();
			SetMaterialDirty();
		}

		public float GetPaddingForMaterial()
		{
			return ShaderUtilities.GetPadding(m_sharedMaterial, m_TextComponent.extraPadding, m_TextComponent.isUsingBold);
		}

		public void UpdateMeshPadding(bool isExtraPadding, bool isUsingBold)
		{
			m_padding = ShaderUtilities.GetPadding(m_sharedMaterial, isExtraPadding, isUsingBold);
		}

		public void SetVerticesDirty()
		{
			if (base.enabled && m_TextComponent != null)
			{
				m_TextComponent.havePropertiesChanged = true;
				m_TextComponent.SetVerticesDirty();
			}
		}

		public void SetMaterialDirty()
		{
			UpdateMaterial();
		}

		protected void UpdateMaterial()
		{
			if (m_renderer == null)
			{
				m_renderer = renderer;
			}
			m_renderer.sharedMaterial = m_sharedMaterial;
		}
	}
}
