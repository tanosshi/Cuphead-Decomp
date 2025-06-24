using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace TMPro
{
	public static class TMP_MaterialManager
	{
		private class FallbackMaterial
		{
			public int baseID;

			public Material baseMaterial;

			public Material fallbackMaterial;

			public int count;
		}

		private class MaskingMaterial
		{
			public Material baseMaterial;

			public Material stencilMaterial;

			public int count;

			public int stencilID;
		}

		private static List<MaskingMaterial> m_materialList = new List<MaskingMaterial>();

		private static List<FallbackMaterial> m_fallbackMaterialList = new List<FallbackMaterial>();

		public static Material GetStencilMaterial(Material baseMaterial, int stencilID)
		{
			if (!baseMaterial.HasProperty(ShaderUtilities.ID_StencilID))
			{
				Debug.LogWarning("Selected Shader does not support Stencil Masking. Please select the Distance Field or Mobile Distance Field Shader.");
				return baseMaterial;
			}
			int instanceID = baseMaterial.GetInstanceID();
			for (int i = 0; i < m_materialList.Count; i++)
			{
				if (m_materialList[i].baseMaterial.GetInstanceID() == instanceID && m_materialList[i].stencilID == stencilID)
				{
					m_materialList[i].count++;
					return m_materialList[i].stencilMaterial;
				}
			}
			Material material = new Material(baseMaterial);
			material.hideFlags = HideFlags.HideAndDontSave;
			material.name = material.name + " Masking ID:" + stencilID;
			material.shaderKeywords = baseMaterial.shaderKeywords;
			ShaderUtilities.GetShaderPropertyIDs();
			material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
			material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			MaskingMaterial maskingMaterial = new MaskingMaterial();
			maskingMaterial.baseMaterial = baseMaterial;
			maskingMaterial.stencilMaterial = material;
			maskingMaterial.stencilID = stencilID;
			maskingMaterial.count = 1;
			m_materialList.Add(maskingMaterial);
			return material;
		}

		public static void ReleaseStencilMaterial(Material stencilMaterial)
		{
			int instanceID = stencilMaterial.GetInstanceID();
			for (int i = 0; i < m_materialList.Count; i++)
			{
				if (m_materialList[i].stencilMaterial.GetInstanceID() == instanceID)
				{
					if (m_materialList[i].count > 1)
					{
						m_materialList[i].count--;
						break;
					}
					Object.DestroyImmediate(m_materialList[i].stencilMaterial);
					m_materialList.RemoveAt(i);
					stencilMaterial = null;
					break;
				}
			}
		}

		public static Material GetBaseMaterial(Material stencilMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				return null;
			}
			return m_materialList[num].baseMaterial;
		}

		public static Material SetStencil(Material material, int stencilID)
		{
			material.SetFloat(ShaderUtilities.ID_StencilID, stencilID);
			if (stencilID == 0)
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 8f);
			}
			else
			{
				material.SetFloat(ShaderUtilities.ID_StencilComp, 4f);
			}
			return material;
		}

		public static void AddMaskingMaterial(Material baseMaterial, Material stencilMaterial, int stencilID)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num == -1)
			{
				MaskingMaterial maskingMaterial = new MaskingMaterial();
				maskingMaterial.baseMaterial = baseMaterial;
				maskingMaterial.stencilMaterial = stencilMaterial;
				maskingMaterial.stencilID = stencilID;
				maskingMaterial.count = 1;
				m_materialList.Add(maskingMaterial);
			}
			else
			{
				stencilMaterial = m_materialList[num].stencilMaterial;
				m_materialList[num].count++;
			}
		}

		public static void RemoveStencilMaterial(Material stencilMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.stencilMaterial == stencilMaterial);
			if (num != -1)
			{
				m_materialList.RemoveAt(num);
			}
		}

		public static void ReleaseBaseMaterial(Material baseMaterial)
		{
			int num = m_materialList.FindIndex((MaskingMaterial item) => item.baseMaterial == baseMaterial);
			if (num == -1)
			{
				Debug.Log("No Masking Material exists for " + baseMaterial.name);
			}
			else if (m_materialList[num].count > 1)
			{
				m_materialList[num].count--;
				Debug.Log("Removed (1) reference to " + m_materialList[num].stencilMaterial.name + ". There are " + m_materialList[num].count + " references left.");
			}
			else
			{
				Debug.Log("Removed last reference to " + m_materialList[num].stencilMaterial.name + " with ID " + m_materialList[num].stencilMaterial.GetInstanceID());
				Object.DestroyImmediate(m_materialList[num].stencilMaterial);
				m_materialList.RemoveAt(num);
			}
		}

		public static void ClearMaterials()
		{
			if (m_materialList.Count() == 0)
			{
				Debug.Log("Material List has already been cleared.");
				return;
			}
			for (int i = 0; i < m_materialList.Count(); i++)
			{
				Material stencilMaterial = m_materialList[i].stencilMaterial;
				Object.DestroyImmediate(stencilMaterial);
				m_materialList.RemoveAt(i);
			}
		}

		public static int GetStencilID(GameObject obj)
		{
			int num = 0;
			List<Mask> list = TMP_ListPool<Mask>.Get();
			obj.GetComponentsInParent(false, list);
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i].MaskEnabled())
				{
					num++;
				}
			}
			TMP_ListPool<Mask>.Release(list);
			return Mathf.Min((1 << num) - 1, 255);
		}

		public static Material GetFallbackMaterial(Material source, Texture mainTex)
		{
			int instanceID = source.GetInstanceID();
			int instanceID2 = mainTex.GetInstanceID();
			for (int i = 0; i < m_fallbackMaterialList.Count; i++)
			{
				if (m_fallbackMaterialList[i].fallbackMaterial == null)
				{
					m_fallbackMaterialList.RemoveAt(i);
				}
				else if (m_fallbackMaterialList[i].baseMaterial.GetInstanceID() == instanceID && m_fallbackMaterialList[i].fallbackMaterial.mainTexture.GetInstanceID() == instanceID2)
				{
					m_fallbackMaterialList[i].count++;
					return m_fallbackMaterialList[i].fallbackMaterial;
				}
			}
			Material material = new Material(source);
			material.name += " (Fallback Instance)";
			material.mainTexture = mainTex;
			FallbackMaterial fallbackMaterial = new FallbackMaterial();
			fallbackMaterial.baseID = instanceID;
			fallbackMaterial.baseMaterial = source;
			fallbackMaterial.fallbackMaterial = material;
			fallbackMaterial.count = 1;
			m_fallbackMaterialList.Add(fallbackMaterial);
			return material;
		}
	}
}
