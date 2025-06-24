using System;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxPropertiesData : ScriptableObject
{
	[Serializable]
	public class ThemeProperties
	{
		[Serializable]
		public class LayerGroup
		{
			public Layer[] layers;

			[NonSerialized]
			public bool zEditor_expanded;

			public LayerGroup()
			{
				layers = new Layer[20];
				for (int i = 0; i < layers.Length; i++)
				{
					layers[i] = new Layer();
					layers[i].speed = 0.05f * (float)(i + 1);
					layers[i].sortingOrder = 100 * (i + 1);
				}
			}

			public void InvertSpeed()
			{
				Layer[] array = layers;
				foreach (Layer layer in array)
				{
					layer.speed *= -1f;
				}
			}

			public void InvertSortingOrder()
			{
				Layer[] array = layers;
				foreach (Layer layer in array)
				{
					layer.sortingOrder *= -1;
				}
			}

			public Layer GetLayer(int layer)
			{
				return layers[layer];
			}
		}

		[Serializable]
		public class Layer
		{
			public float speed;

			public int sortingOrder;
		}

		public PlatformingLevel.Theme theme;

		public LayerGroup background;

		public LayerGroup foreground;

		[NonSerialized]
		public bool zEditor_expanded;

		public ThemeProperties()
		{
			background = new LayerGroup();
			background.InvertSortingOrder();
			foreground = new LayerGroup();
			foreground.InvertSpeed();
		}

		public ThemeProperties(PlatformingLevel.Theme theme)
		{
			this.theme = theme;
			background = new LayerGroup();
			background.InvertSortingOrder();
			foreground = new LayerGroup();
			foreground.InvertSpeed();
		}
	}

	private const string PATH = "Parallax/data";

	public const int LAYER_COUNT = 20;

	private static ParallaxPropertiesData _instance;

	[SerializeField]
	private List<ThemeProperties> _properties;

	public static ParallaxPropertiesData Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = Resources.Load<ParallaxPropertiesData>("Parallax/data");
			}
			return _instance;
		}
	}

	public List<ThemeProperties> Properties
	{
		get
		{
			return _properties;
		}
	}

	public ThemeProperties.Layer GetProperty(PlatformingLevel.Theme theme, int layer, PlatformingLevelParallax.Sides side)
	{
		if (side == PlatformingLevelParallax.Sides.Background || side != PlatformingLevelParallax.Sides.Foreground)
		{
			return GetTheme(theme).background.GetLayer(layer);
		}
		return GetTheme(theme).foreground.GetLayer(layer);
	}

	private ThemeProperties GetTheme(PlatformingLevel.Theme theme)
	{
		foreach (ThemeProperties property in _properties)
		{
			if (property.theme == theme)
			{
				return property;
			}
		}
		return null;
	}
}
