using System;
using UnityEngine;

namespace TMPro
{
	public struct TMP_MeshInfo
	{
		private static readonly Color32 s_DefaultColor = new Color32(byte.MaxValue, byte.MaxValue, byte.MaxValue, byte.MaxValue);

		private static readonly Vector3 s_DefaultNormal = new Vector3(0f, 0f, -1f);

		private static readonly Vector4 s_DefaultTangent = new Vector4(-1f, 0f, 0f, 1f);

		public Mesh mesh;

		public int vertexCount;

		public Vector3[] vertices;

		public Vector3[] normals;

		public Vector4[] tangents;

		public Vector2[] uvs0;

		public Vector2[] uvs2;

		public Color32[] colors32;

		public int[] triangles;

		public TMP_MeshInfo(Mesh mesh, int size)
		{
			if (mesh == null)
			{
				mesh = new Mesh();
			}
			else
			{
				mesh.Clear();
			}
			this.mesh = mesh;
			int num = size * 4;
			int num2 = size * 6;
			vertexCount = 0;
			vertices = new Vector3[num];
			uvs0 = new Vector2[num];
			uvs2 = new Vector2[num];
			colors32 = new Color32[num];
			normals = new Vector3[num];
			tangents = new Vector4[num];
			triangles = new int[num2];
			int num3 = 0;
			int num4 = 0;
			while (num4 / 4 < size)
			{
				for (int i = 0; i < 4; i++)
				{
					vertices[num4 + i] = Vector3.zero;
					uvs0[num4 + i] = Vector2.zero;
					uvs2[num4 + i] = Vector2.zero;
					colors32[num4 + i] = s_DefaultColor;
					normals[num4 + i] = s_DefaultNormal;
					tangents[num4 + i] = s_DefaultTangent;
				}
				triangles[num3] = num4;
				triangles[num3 + 1] = num4 + 1;
				triangles[num3 + 2] = num4 + 2;
				triangles[num3 + 3] = num4 + 2;
				triangles[num3 + 4] = num4 + 3;
				triangles[num3 + 5] = num4;
				num4 += 4;
				num3 += 6;
			}
			this.mesh.vertices = vertices;
			this.mesh.normals = normals;
			this.mesh.tangents = tangents;
			this.mesh.triangles = triangles;
			this.mesh.bounds = new Bounds(Vector3.zero, new Vector3(3840f, 2160f, 0f));
		}

		public void ResizeMeshInfo(int size)
		{
			int newSize = size * 4;
			int newSize2 = size * 6;
			int num = vertices.Length / 4;
			Array.Resize(ref vertices, newSize);
			Array.Resize(ref normals, newSize);
			Array.Resize(ref tangents, newSize);
			Array.Resize(ref uvs0, newSize);
			Array.Resize(ref uvs2, newSize);
			Array.Resize(ref colors32, newSize);
			Array.Resize(ref triangles, newSize2);
			if (size > num)
			{
				for (int i = num; i < size; i++)
				{
					int num2 = i * 4;
					int num3 = i * 6;
					normals[num2] = s_DefaultNormal;
					normals[1 + num2] = s_DefaultNormal;
					normals[2 + num2] = s_DefaultNormal;
					normals[3 + num2] = s_DefaultNormal;
					tangents[num2] = s_DefaultTangent;
					tangents[1 + num2] = s_DefaultTangent;
					tangents[2 + num2] = s_DefaultTangent;
					tangents[3 + num2] = s_DefaultTangent;
					triangles[num3] = num2;
					triangles[1 + num3] = 1 + num2;
					triangles[2 + num3] = 2 + num2;
					triangles[3 + num3] = 2 + num2;
					triangles[4 + num3] = 3 + num2;
					triangles[5 + num3] = num2;
				}
				mesh.vertices = vertices;
				mesh.normals = normals;
				mesh.tangents = tangents;
				mesh.triangles = triangles;
			}
		}

		public void Clear()
		{
			if (vertices != null)
			{
				Array.Clear(vertices, 0, vertices.Length);
				vertexCount = 0;
				if (mesh != null)
				{
					mesh.vertices = vertices;
				}
			}
		}

		public void Clear(bool uploadChanges)
		{
			if (vertices != null)
			{
				Array.Clear(vertices, 0, vertices.Length);
				vertexCount = 0;
				if (uploadChanges && mesh != null)
				{
					mesh.vertices = vertices;
				}
			}
		}

		public void ClearUnusedVertices()
		{
			int num = vertices.Length - vertexCount;
			if (num > 0)
			{
				Array.Clear(vertices, vertexCount, num);
			}
		}

		public void ClearUnusedVertices(int startIndex)
		{
			int num = vertices.Length - startIndex;
			if (num > 0)
			{
				Array.Clear(vertices, startIndex, num);
			}
		}

		public void ClearUnusedVertices(int startIndex, bool updateMesh)
		{
			int num = vertices.Length - startIndex;
			if (num > 0)
			{
				Array.Clear(vertices, startIndex, num);
			}
			if (updateMesh && mesh != null)
			{
				mesh.vertices = vertices;
			}
		}
	}
}
