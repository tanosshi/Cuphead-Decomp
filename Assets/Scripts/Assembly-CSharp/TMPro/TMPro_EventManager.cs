using UnityEngine;

namespace TMPro
{
	public static class TMPro_EventManager
	{
		public static readonly FastAction<object, Compute_DT_EventArgs> COMPUTE_DT_EVENT = new FastAction<object, Compute_DT_EventArgs>();

		public static readonly FastAction<bool, Material> MATERIAL_PROPERTY_EVENT = new FastAction<bool, Material>();

		public static readonly FastAction<bool, TMP_FontAsset> FONT_PROPERTY_EVENT = new FastAction<bool, TMP_FontAsset>();

		public static readonly FastAction<bool, Object> SPRITE_ASSET_PROPERTY_EVENT = new FastAction<bool, Object>();

		public static readonly FastAction<bool, TextMeshPro> TEXTMESHPRO_PROPERTY_EVENT = new FastAction<bool, TextMeshPro>();

		public static readonly FastAction<GameObject, Material, Material> DRAG_AND_DROP_MATERIAL_EVENT = new FastAction<GameObject, Material, Material>();

		public static readonly FastAction<bool> TEXT_STYLE_PROPERTY_EVENT = new FastAction<bool>();

		public static readonly FastAction TMP_SETTINGS_PROPERTY_EVENT = new FastAction();

		public static readonly FastAction<bool, TextMeshProUGUI> TEXTMESHPRO_UGUI_PROPERTY_EVENT = new FastAction<bool, TextMeshProUGUI>();

		public static readonly FastAction<Material> BASE_MATERIAL_EVENT = new FastAction<Material>();

		public static readonly FastAction OnPreRenderObject_Event = new FastAction();

		public static readonly FastAction<Object> TEXT_CHANGED_EVENT = new FastAction<Object>();

		public static readonly FastAction WILL_RENDER_CANVASES = new FastAction();

		public static void ON_PRE_RENDER_OBJECT_CHANGED()
		{
			OnPreRenderObject_Event.Call();
		}

		public static void ON_MATERIAL_PROPERTY_CHANGED(bool isChanged, Material mat)
		{
			MATERIAL_PROPERTY_EVENT.Call(isChanged, mat);
		}

		public static void ON_FONT_PROPERTY_CHANGED(bool isChanged, TMP_FontAsset font)
		{
			FONT_PROPERTY_EVENT.Call(isChanged, font);
		}

		public static void ON_SPRITE_ASSET_PROPERTY_CHANGED(bool isChanged, Object obj)
		{
			SPRITE_ASSET_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static void ON_TEXTMESHPRO_PROPERTY_CHANGED(bool isChanged, TextMeshPro obj)
		{
			TEXTMESHPRO_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static void ON_DRAG_AND_DROP_MATERIAL_CHANGED(GameObject sender, Material currentMaterial, Material newMaterial)
		{
			DRAG_AND_DROP_MATERIAL_EVENT.Call(sender, currentMaterial, newMaterial);
		}

		public static void ON_TEXT_STYLE_PROPERTY_CHANGED(bool isChanged)
		{
			TEXT_STYLE_PROPERTY_EVENT.Call(isChanged);
		}

		public static void ON_TEXT_CHANGED(Object obj)
		{
			TEXT_CHANGED_EVENT.Call(obj);
		}

		public static void ON_TMP_SETTINGS_CHANGED()
		{
			TMP_SETTINGS_PROPERTY_EVENT.Call();
		}

		public static void ON_TEXTMESHPRO_UGUI_PROPERTY_CHANGED(bool isChanged, TextMeshProUGUI obj)
		{
			TEXTMESHPRO_UGUI_PROPERTY_EVENT.Call(isChanged, obj);
		}

		public static void ON_BASE_MATERIAL_CHANGED(Material mat)
		{
			BASE_MATERIAL_EVENT.Call(mat);
		}

		public static void ON_COMPUTE_DT_EVENT(object Sender, Compute_DT_EventArgs e)
		{
			COMPUTE_DT_EVENT.Call(Sender, e);
		}
	}
}
