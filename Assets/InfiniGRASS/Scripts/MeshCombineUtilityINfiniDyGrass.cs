using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Artngame.INfiniDy {

public class MeshCombineUtilityINfiniDyGrass {
	
	public struct MeshInstance
	{
		public Mesh      mesh;
		public int       subMeshIndex;            
		public Matrix4x4 transform;
	}

		public static ControlCombineChildrenINfiniDyGrass.Meshy CombineM (int ID,List<int> Has_mesh,bool thread_started, MeshInstance[] combines, bool generateStrips, int vertexCount, int triangleCount,List<int> Combine_Mesh_vertexCount,List<Vector3[]> Combine_Mesh_vertices,List<Vector3[]> Combine_Mesh_normals,List<Vector4[]> Combine_Mesh_tangets, List<Vector2[]> Combine_Mesh_uv,List<Vector2[]> Combine_Mesh_uv1,List<Color[]> Combine_Mesh_colors,List<int[]> Combine_Mesh_triangles)
		{		

			Vector3[] vertices = new Vector3[vertexCount] ;			
			Vector3[] normals = new Vector3[vertexCount] ;			
			Vector4[] tangents = new Vector4[vertexCount] ;			
			Vector2[] uv = new Vector2[vertexCount];			
			Vector2[] uv1 = new Vector2[vertexCount];			
			Color[] colors = new Color[vertexCount];	
			int[] triangles = new int[triangleCount];
			int offset;
			
			offset=0;

			int count = 0 ;
			foreach( MeshInstance combine in combines )				
			{			

				if(Has_mesh[count] == 1){
					Copy(Combine_Mesh_vertexCount[count], Combine_Mesh_vertices[count], vertices, ref offset, combine.transform);
				}

				count++;
			}
					
			
			offset=0;
			count = 0 ;
			foreach( MeshInstance combine in combines )				
			{			
				//if (combine.mesh)		
				if(Has_mesh[count] == 1)
				{					
					Matrix4x4 invTranspose = combine.transform;					
					invTranspose = invTranspose.inverse.transpose;					
					CopyNormal(Combine_Mesh_vertexCount[count], Combine_Mesh_normals[count], normals, ref offset, invTranspose);
					count++;
				}	
			}
			
			offset=0;
			count = 0 ;
			foreach( MeshInstance combine in combines )				
			{

				if(Has_mesh[count] == 1)
				{
					Matrix4x4 invTranspose = combine.transform;					
					invTranspose = invTranspose.inverse.transpose;					
					CopyTangents(Combine_Mesh_vertexCount[count], Combine_Mesh_tangets[count], tangents, ref offset, invTranspose);
					count++;
				}	
			}
			
			offset=0;
			count = 0 ;
			//foreach( MeshInstance combine in combines )
			for(int i =0;i< combines.Length;i++)
			{				

				if(Has_mesh[count] == 1){
					Copy(Combine_Mesh_vertexCount[count], Combine_Mesh_uv[count], uv, ref offset);
				}

				count++;
			}
			
			offset=0;
			count = 0 ;
			//foreach( MeshInstance combine in combines )
			for(int i =0;i< combines.Length;i++)
			{				

				if(Has_mesh[count] == 1){
					Copy(Combine_Mesh_vertexCount[count], Combine_Mesh_uv1[count], uv1, ref offset);
				}

				count++;
			}
			
			offset=0;
			count = 0 ;
			//foreach( MeshInstance combine in combines )	
			for(int i =0;i< combines.Length;i++)
			{				
					
				if(Has_mesh[count] == 1){
					CopyColors(Combine_Mesh_vertexCount[count], Combine_Mesh_colors[count], colors, ref offset);
				}

				count++;
			}
			
			int triangleOffset=0;			
			int vertexOffset=0;
			count = 0 ;
			//foreach( MeshInstance combine in combines )	
			for(int j =0;j< combines.Length;j++)
			{

				if(Has_mesh[count] == 1)
				{					
					int[]  inputtriangles = Combine_Mesh_triangles[count];
					
					for (int i=0;i<inputtriangles.Length;i++)						
					{						
						triangles[i+triangleOffset] = inputtriangles[i] + vertexOffset;						
					}
					
					triangleOffset += inputtriangles.Length;	
					
					vertexOffset += Combine_Mesh_vertexCount[count];
					count++;
				}				
			}		
			
			//Mesh mesh = new Mesh();
			ControlCombineChildrenINfiniDyGrass.Meshy mesh = new ControlCombineChildrenINfiniDyGrass.Meshy();
			mesh.name = "Combined Mesh";			
			mesh.vertices = vertices;			
			mesh.normals = normals;			
			mesh.colors = colors;			
			mesh.uv = uv;			
			mesh.uv1 = uv1;			
			mesh.tangents = tangents;			
			mesh.triangles = triangles;	
			//thread_started = false;
			mesh.thread_ended = true;

			//Debug.Log ("ID = "+ID);

			return mesh;			
		}

















	
	public static Mesh Combine (MeshInstance[] combines, bool generateStrips)
	{
		
		int vertexCount = 0;
		
		int triangleCount = 0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
			{
				
				vertexCount += combine.mesh.vertexCount;
				
			}
			
		}
		
		
		
		// Precompute how many triangles we need
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
			{
				
				triangleCount += combine.mesh.GetTriangles(combine.subMeshIndex).Length;
				
			}
			
		}
		
		
		
		Vector3[] vertices = new Vector3[vertexCount] ;
		
		Vector3[] normals = new Vector3[vertexCount] ;
		
		Vector4[] tangents = new Vector4[vertexCount] ;
		
		Vector2[] uv = new Vector2[vertexCount];
		
		Vector2[] uv1 = new Vector2[vertexCount];
		
		Color[] colors = new Color[vertexCount];
		
		
		
		int[] triangles = new int[triangleCount];
		
		
		
		int offset;
		
		
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
				Copy(combine.mesh.vertexCount, combine.mesh.vertices, vertices, ref offset, combine.transform);
			
		}
		
		
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
			{
				
				Matrix4x4 invTranspose = combine.transform;
				
				invTranspose = invTranspose.inverse.transpose;
				
				CopyNormal(combine.mesh.vertexCount, combine.mesh.normals, normals, ref offset, invTranspose);
				
			}
			
			
			
		}
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
			{
				
				Matrix4x4 invTranspose = combine.transform;
				
				invTranspose = invTranspose.inverse.transpose;
				
				CopyTangents(combine.mesh.vertexCount, combine.mesh.tangents, tangents, ref offset, invTranspose);
				
			}
			
			
			
		}
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
				Copy(combine.mesh.vertexCount, combine.mesh.uv, uv, ref offset);
			
		}
		
		
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
				Copy(combine.mesh.vertexCount, combine.mesh.uv2, uv1, ref offset);
			
		}
		
		
		
		offset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
				CopyColors(combine.mesh.vertexCount, combine.mesh.colors, colors, ref offset);
			
		}
		
		
		
		int triangleOffset=0;
		
		int vertexOffset=0;
		
		foreach( MeshInstance combine in combines )
			
		{
			
			if (combine.mesh)
				
			{
				
				int[]  inputtriangles = combine.mesh.GetTriangles(combine.subMeshIndex);
				
				for (int i=0;i<inputtriangles.Length;i++)
					
				{
					
					triangles[i+triangleOffset] = inputtriangles[i] + vertexOffset;
					
				}
				
				triangleOffset += inputtriangles.Length;
				
				
				
				vertexOffset += combine.mesh.vertexCount;
				
			}
			
		}
		
		
		
		Mesh mesh = new Mesh();
		
		mesh.name = "Combined Mesh";
		
		mesh.vertices = vertices;
		
		mesh.normals = normals;
		
		mesh.colors = colors;
		
		mesh.uv = uv;
		
		mesh.uv2 = uv1;
		
		mesh.tangents = tangents;
		
		mesh.triangles = triangles;
		
		
		
		return mesh;
		
	}

	
	static void Copy (int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i=0;i<src.Length;i++)
			dst[i+offset] = transform.MultiplyPoint(src[i]);
		offset += vertexcount;
	}

	static void CopyNormal (int vertexcount, Vector3[] src, Vector3[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i=0;i<src.Length;i++)
			dst[i+offset] = transform.MultiplyVector(src[i]).normalized;
		offset += vertexcount;
	}

	static void Copy (int vertexcount, Vector2[] src, Vector2[] dst, ref int offset)
	{
		for (int i=0;i<src.Length;i++)
			dst[i+offset] = src[i];
		offset += vertexcount;
	}

	static void CopyColors (int vertexcount, Color[] src, Color[] dst, ref int offset)
	{
		for (int i=0;i<src.Length;i++)
			dst[i+offset] = src[i];
		offset += vertexcount;
	}
	
	static void CopyTangents (int vertexcount, Vector4[] src, Vector4[] dst, ref int offset, Matrix4x4 transform)
	{
		for (int i=0;i<src.Length;i++)
		{
			Vector4 p4 = src[i];
			Vector3 p = new Vector3(p4.x, p4.y, p4.z);
			p = transform.MultiplyVector(p).normalized;
			dst[i+offset] = new Vector4(p.x, p.y, p.z, p4.w);
		}
			
		offset += vertexcount;
	}
}

}