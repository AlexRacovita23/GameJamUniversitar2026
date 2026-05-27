using UnityEngine;
using System.Collections.Generic;

public static class ProceduralRockMesh
{
    public static Mesh Generate(int seed = 0, float roughness = 0.25f,
                                float squash = 1f, int subdivisions = 0)
    {
        Mesh mesh = BuildIcosphere(subdivisions);
        Vector3[] verts = mesh.vertices;

        for (int i = 0; i < verts.Length; i++)
        {
            float noise = Mathf.PerlinNoise(verts[i].x * 3.1f + seed,
                                                   verts[i].z * 3.1f + seed * 0.7f);
            float displacement = Mathf.Lerp(-roughness, roughness, noise);
            verts[i] += verts[i].normalized * displacement;
            verts[i].y *= squash;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        return mesh;
    }

    private static Mesh BuildIcosphere(int subdivisions)
    {
        float t = (1f + Mathf.Sqrt(5f)) / 2f;

        var verts = new List<Vector3>
        {
            new Vector3(-1,  t,  0).normalized, new Vector3( 1,  t,  0).normalized,
            new Vector3(-1, -t,  0).normalized, new Vector3( 1, -t,  0).normalized,
            new Vector3( 0, -1,  t).normalized, new Vector3( 0,  1,  t).normalized,
            new Vector3( 0, -1, -t).normalized, new Vector3( 0,  1, -t).normalized,
            new Vector3( t,  0, -1).normalized, new Vector3( t,  0,  1).normalized,
            new Vector3(-t,  0, -1).normalized, new Vector3(-t,  0,  1).normalized,
        };

        var tris = new List<int>
        {
            0,11,5,  0,5,1,  0,1,7,  0,7,10,  0,10,11,
            1,5,9,   5,11,4, 11,10,2, 10,7,6,  7,1,8,
            3,9,4,   3,4,2,  3,2,6,  3,6,8,   3,8,9,
            4,9,5,   2,4,11, 6,2,10, 8,6,7,   9,8,1
        };

        for (int s = 0; s < subdivisions; s++)
        {
            var newTris = new List<int>();
            var midCache = new Dictionary<long, int>();

            for (int i = 0; i < tris.Count; i += 3)
            {
                int a = tris[i], b = tris[i + 1], c = tris[i + 2];
                int ab = Midpoint(a, b, verts, midCache);
                int bc = Midpoint(b, c, verts, midCache);
                int ca = Midpoint(c, a, verts, midCache);
                newTris.AddRange(new[] { a, ab, ca, b, bc, ab, c, ca, bc, ab, bc, ca });
            }
            tris = newTris;
        }

        var uvs = new Vector2[verts.Count];
        for (int i = 0; i < verts.Count; i++)
        {
            Vector3 n = verts[i].normalized;
            uvs[i] = new Vector2(
                0.5f + Mathf.Atan2(n.z, n.x) / (2f * Mathf.PI),
                0.5f - Mathf.Asin(n.y) / Mathf.PI
            );
        }

        var mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }

    private static int Midpoint(int a, int b, List<Vector3> verts, Dictionary<long, int> cache)
    {
        long key = (long)Mathf.Min(a, b) * 1_000_000 + Mathf.Max(a, b);
        if (cache.TryGetValue(key, out int idx)) return idx;

        verts.Add(((verts[a] + verts[b]) / 2f).normalized);
        idx = verts.Count - 1;
        cache[key] = idx;
        return idx;
    }
}