using Elements.Geometry;
using Elements.Geometry.Solids;
using Elements20BasicSample;
using Newtonsoft.Json;

namespace Elements
{
    public class Ellipsework : GeometricElement
    {
        public Polyline Polyline { get; set; }

        public Ellipsework(string id, Polyline polyline, bool showpoints = true)
        {
            Polyline = polyline;
            SetMaterial();
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

                var circle = new Elements.Geometry.Circle(Vector3.Origin, circleRadius).ToPolygon(10);
                circle.Transform(new Transform(new Plane(start, direction)));

                // Create an extruded circle along the line segment
                var extrusion = new Extrude(circle, length, direction, false);

                rep.SolidOperations.Add(extrusion);
            }

            var consol = new ConstructedSolid(solidRep);
            rep.SolidOperations.Add(consol);

            this.Representation = rep;
        }
    }
}