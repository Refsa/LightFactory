using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Rendering;

public class LineDrawer : System.IDisposable
{
    public struct LineData
    {
        public Vector3 Pos1;
        public Vector3 Pos2;
        public Vector3 Color;
        public float Width;
    }

    static readonly Material LineMaterial;
    static readonly Bounds DefaultBounds;

    List<LineData> lineDatas;

    ComputeBuffer lineDataBuffer;

    uint[] args;
    ComputeBuffer argsBuffer;

    MaterialPropertyBlock materialPropertyBlock;

    public List<LineData> LineDatas => lineDatas;

    static LineDrawer()
    {
        LineMaterial = new Material(Shader.Find("Hidden/Line"));
        LineMaterial.enableInstancing = true;
        DefaultBounds = new Bounds(Vector3.zero, Vector3.one * 10_000f);
    }

    public LineDrawer()
    {
        lineDatas = new List<LineData>();
        materialPropertyBlock = new MaterialPropertyBlock();

        args = new uint[5];
        args[1] = 1;
        argsBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
    }

    public void Render(Camera camera)
    {
        if (lineDataBuffer == null || lineDataBuffer.count < lineDatas.Count)
        {
            lineDataBuffer?.Dispose();
            lineDataBuffer = new ComputeBuffer(lineDatas.Count, Marshal.SizeOf<LineData>());
        }

        args[0] = (uint)lineDatas.Count;
        argsBuffer.SetData(args);

        lineDataBuffer.SetData(lineDatas);
        materialPropertyBlock.SetBuffer("_Properties", lineDataBuffer);

        Graphics.DrawProceduralIndirect(
            LineMaterial,
            DefaultBounds,
            MeshTopology.Points,
            argsBuffer,
            properties: materialPropertyBlock
        );
    }

    public void Add(Vector3 p1, Vector3 p2, Color color, float width = 1f)
    {
        lineDatas.Add(new LineData
        {
            Pos1 = p1,
            Pos2 = p2,
            Color = new Vector3(color.r, color.g, color.b),
            Width = width,
        });
    }

    public void Clear()
    {
        lineDatas.Clear();
    }

    public void Dispose()
    {
        lineDataBuffer?.Dispose();
        argsBuffer?.Dispose();
    }
}