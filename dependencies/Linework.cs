using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements20BasicSample;
using Newtonsoft.Json;

namespace Elements
{
    public class Linework : GeometricElement
    {
        public Line Line { get; set; }
        [JsonProperty("Add Id")]
        public string AddId { get; set; }

        public Linework(LinesOverrideAddition add)
        {
            this.Line = add.Value.Line;
            this.AddId = add.Id;

            SetMaterial();
        }

        public Linework(string id, Line line)
        {
            Line = line;
            this.AddId = id;
            SetMaterial();
        }

        public bool Match(LinesIdentity identity)
        {
            return identity.AddId == this.AddId;
        }

        public Linework Update(LinesOverride edit)
        {
            this.Line = edit.Value.Line;
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

            var lineVertices = new List<Vector3>() { Line.Start, Line.End };

            // Create an extruded circle along each line segment of the polyline
            var direction = Line.Direction();
            var length = Line.Length();

            var circle = new Elements.Geometry.Circle(Vector3.Origin, circleRadius).ToPolygon(10);
            circle.Transform(new Transform(new Plane(Line.Start, direction)));

            // Create an extruded circle along the line segment
            var extrusion = new Extrude(circle, length, direction, false);

            rep.SolidOperations.Add(extrusion);

            // Add a spherical point at each vertex of the polyline
            foreach (var vertex in lineVertices)
            {
                var sphere = Mesh.Sphere(pointRadius, 10);

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