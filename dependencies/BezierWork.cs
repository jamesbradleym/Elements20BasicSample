using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements20BasicSample;
using Newtonsoft.Json;

namespace Elements
{
    public class Bezierwork : GeometricElement
    {
        public Bezier Bezier { get; set; }
        public Polyline Polyline { get; set; }
        [JsonProperty("Add Id")]
        public string AddId { get; set; }

        public Bezierwork(BeziersOverrideAddition add)
        {
            this.Polyline = add.Value.Polyline;
            this.Bezier = new Bezier(add.Value.Polyline.Vertices.ToList());
            this.AddId = add.Id;

            SetMaterial();
        }

        public Bezierwork(Bezier bezier, Polyline bezierPolyline)
        {
            this.Polyline = bezierPolyline;
            this.Bezier = bezier;
            this.AddId = this.Id.ToString();
            SetMaterial();
        }

        public bool Match(BeziersIdentity identity)
        {
            return identity.AddId == this.AddId;
        }

        public Bezierwork Update(BeziersOverride edit)
        {
            this.Polyline = edit.Value.Polyline;
            this.Bezier = new Bezier(edit.Value.Polyline.Vertices.ToList());
            return this;
        }

        public void SetMaterial()
        {
            var materialName = this.Name + "_MAT";
            var materialColor = new Color(0.952941176, 0.360784314, 0.419607843, 1.0); // F15C6B with alpha 1
            var material = new Material(materialName);
            material.Color = materialColor;
            material.Unlit = true;
            this.Material = material;
        }

        public override void UpdateRepresentations()
        {
            var rep = new Representation();
            var solidRep = new Solid();

            // Define parameters for the extruded circle and spherical point
            var circleRadius = 0.1;
            var pointRadius = 0.2;
            var innerPointRadius = 0.05;

            // Create an extruded circle along each line segment of the polyline
            var circle = new Circle(circleRadius).ToPolygon();

            // Create an extruded circle along the line
            var sweep = new Sweep(circle, Bezier, 0, 0, 0, false);

            rep.SolidOperations.Add(sweep);

            var points = new List<Vector3>() { Bezier.Start, Bezier.End };
            points.InsertRange(1, Bezier.ControlPoints);
            // Add a spherical point at each vertex of the polyline
            for (int i = 0; i < points.Count; i++)
            {
                var vertex = points[i];
                var sphere = Mesh.Sphere((i == 0 || i == points.Count - 1) ? pointRadius : innerPointRadius, 10);

                HashSet<Geometry.Vertex> modifiedVertices = new HashSet<Geometry.Vertex>();
                // Translate the vertices of the mesh to center it at the origin
                foreach (var svertex in sphere.Vertices)
                {
                    if (!modifiedVertices.Contains(svertex))
                    {
                        svertex.Position += vertex;
                        modifiedVertices.Add(svertex);
                    }
                }

                foreach (var triangle in sphere.Triangles)
                {
                    // Create a Polygon from the triangle's vertices point
                    var polygon = new Polygon(triangle.Vertices.SelectMany(v => new List<Vector3> { v.Position }).ToList());
                    solidRep.AddFace(polygon);
                }
            }

            var consol = new ConstructedSolid(solidRep);
            rep.SolidOperations.Add(consol);

            this.Representation = rep;
        }
    }
}