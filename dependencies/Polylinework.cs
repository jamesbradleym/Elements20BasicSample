using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements20BasicSample;
using Newtonsoft.Json;

namespace Elements
{
    public class Polylinework : GeometricElement
    {

        public Polyline Polyline { get; set; }
        [JsonProperty("Add Id")]
        public string AddId { get; set; }
        public bool ShowPoints { get; set; }

        public Polylinework(PolylinesOverrideAddition add, bool showpoints = true)
        {
            this.Polyline = add.Value.Polyline;
            this.AddId = add.Id;

            this.ShowPoints = showpoints;
            SetMaterial();
        }
        public Polylinework(Polyline polyline, bool showpoints = true)
        {
            Polyline = polyline;

            this.ShowPoints = showpoints;
            SetMaterial();
        }
        public bool Match(PolylinesIdentity identity)
        {
            return identity.AddId == this.AddId;
        }

        public Polylinework Update(PolylinesOverride edit)
        {
            this.Polyline = edit.Value.Polyline;
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

            // Create an extruded circle along each line segment of the polyline
            for (int i = 0; i < Polyline.Vertices.Count - 1; i++)
            {
                var start = Polyline.Vertices[i];
                var end = Polyline.Vertices[i + 1];
                var direction = Polyline.Segments()[i].Direction();
                var length = Polyline.Segments()[i].Length();

                var circle = Polygon.Circle(circleRadius, 10);
                circle.Transform(new Transform(new Plane(start, direction)));

                // Create an extruded circle along the line segment
                var extrusion = new Extrude(circle, length, direction, false);

                rep.SolidOperations.Add(extrusion);
            }

            // Add a spherical point at each vertex of the polyline
            if (ShowPoints)
            {
                foreach (var vertex in Polyline.Vertices)
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

                    // List<Polygon> polygons = new List<Polygon>();

                    foreach (var triangle in sphere.Triangles)
                    {
                        var vertices = new List<Vector3>();

                        foreach (var tvertex in triangle.Vertices)
                        {
                            // Convert Vector3D to Vector3
                            var vector3 = new Vector3(tvertex.Position.X, tvertex.Position.Y, tvertex.Position.Z);
                            vertices.Add(vector3);
                        }

                        // Create a Polygon from the triangle's vertices
                        var polygon = new Polygon(vertices);

                        solidRep.AddFace(polygon);
                    }
                }
            }

            var consol = new ConstructedSolid(solidRep);
            rep.SolidOperations.Add(consol);

            this.Representation = rep;
        }
    }
}