using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Team08.UI
{
    // Vector geometry keeps the silhouette fixed while the shader animates its interior.
    [RequireComponent(typeof(CanvasRenderer))]
    public sealed class HealthFlameGraphic : MaskableGraphic
    {
        [SerializeField, Range(0, 1)] float fill;
        [SerializeField] float phase;
        public float Fill { get => fill; set { fill = Mathf.Clamp01(value); SetVerticesDirty(); } }
        public float Phase { get => phase; set { phase = value; SetVerticesDirty(); } }

        static readonly List<Vector2> Outline = BuildOutline();
        static readonly List<int> Triangles = Triangulate(Outline);

        static List<Vector2> BuildOutline()
        {
            var p = new List<Vector2>();
            Vector2 a = new Vector2(.50f, .04f);
            Curve(p, ref a, new Vector2(.12f,.04f), new Vector2(.03f,.28f), new Vector2(.24f,.52f));
            Curve(p, ref a, new Vector2(.22f,.36f), new Vector2(.36f,.34f), new Vector2(.36f,.52f));
            Curve(p, ref a, new Vector2(.34f,.72f), new Vector2(.59f,.78f), new Vector2(.59f,.98f));
            Curve(p, ref a, new Vector2(.89f,.73f), new Vector2(.51f,.60f), new Vector2(.72f,.45f));
            Curve(p, ref a, new Vector2(.78f,.40f), new Vector2(.83f,.51f), new Vector2(.80f,.59f));
            Curve(p, ref a, new Vector2(1.03f,.32f), new Vector2(.91f,.04f), new Vector2(.50f,.04f));
            return p;
        }

        static void Curve(List<Vector2> p, ref Vector2 a, Vector2 b, Vector2 c, Vector2 d)
        {
            for (int i = 0; i < 8; i++)
            {
                float t = i / 8f, s = 1 - t;
                p.Add(s*s*s*a + 3*s*s*t*b + 3*s*t*t*c + t*t*t*d);
            }
            a = d;
        }

        static float Cross(Vector2 a, Vector2 b) => a.x*b.y-a.y*b.x;
        static List<int> Triangulate(List<Vector2> p)
        {
            var remaining = new List<int>();
            float area = 0;
            for (int i=0;i<p.Count;i++) area += Cross(p[i],p[(i+1)%p.Count]);
            for (int i=0;i<p.Count;i++) remaining.Add(area>0 ? i : p.Count-1-i);
            var output = new List<int>();
            int guard = p.Count*p.Count;
            while (remaining.Count>2 && guard-->0)
            {
                bool found = false;
                for (int i=0;i<remaining.Count;i++)
                {
                    int a=remaining[(i+remaining.Count-1)%remaining.Count], b=remaining[i], c=remaining[(i+1)%remaining.Count];
                    if(Cross(p[b]-p[a],p[c]-p[b])<=0) continue;
                    bool occupied=false;
                    foreach(int j in remaining)
                    {
                        if(j==a||j==b||j==c) continue;
                        if(Cross(p[b]-p[a],p[j]-p[a])>=0 && Cross(p[c]-p[b],p[j]-p[b])>=0 && Cross(p[a]-p[c],p[j]-p[c])>=0) { occupied=true; break; }
                    }
                    if(occupied) continue;
                    output.Add(a);output.Add(b);output.Add(c);
                    remaining.RemoveAt(i);found=true;break;
                }
                if(!found) throw new System.InvalidOperationException("Invalid flame outline");
            }
            return output;
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();
            Rect r=GetPixelAdjustedRect();
            // Four translucent expanded silhouettes provide a local glow without
            // requiring camera bloom or changing the project's render settings.
            for(int layer=-4;layer<2;layer++)
            {
                int offset=vh.currentVertCount;
                foreach(Vector2 p in Outline)
                {
                    float scale=layer<0 ? 1f-layer*.045f : (layer==0 ? 1f : .94f);
                    Vector2 position=new Vector2(.5f,.46f)+(p-new Vector2(.5f,.46f))*scale;
                    var v=UIVertex.simpleVert;
                    v.position=new Vector3(r.x+position.x*r.width,r.y+position.y*r.height,0);
                    v.uv0=new Vector4((p.x-.13f)/.77f,p.y,fill,phase);
                    v.color=layer<0 ? new Color32(128,128,128,12) : (layer==0 ? new Color32(0,0,0,255) : new Color32(255,255,255,255));
                    vh.AddVert(v);
                }
                for(int i=0;i<Triangles.Count;i+=3) vh.AddTriangle(offset+Triangles[i],offset+Triangles[i+1],offset+Triangles[i+2]);
            }
        }
    }
}
