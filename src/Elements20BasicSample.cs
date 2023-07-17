using Elements;
using Elements.Geometry;
using System.Collections.Generic;

namespace Elements20BasicSample
{
    public static class Elements20BasicSample
    {
        /// <summary>
        /// The Elements20BasicSample function.
        /// </summary>
        /// <param name="model">The input model.</param>
        /// <param name="input">The arguments to the execution.</param>
        /// <returns>A Elements20BasicSampleOutputs instance containing computed results and the model with any new elements.</returns>
        public static Elements20BasicSampleOutputs Execute(Dictionary<string, Model> inputModels, Elements20BasicSampleInputs input)
        {
            var output = new Elements20BasicSampleOutputs();

            // Create lineworks via override input
            var lineworks = input.Overrides.Lines.CreateElements(
                input.Overrides.Additions.Lines,
                input.Overrides.Removals.Lines,
                (add) => new Linework(add),
                (linework, identity) => linework.Match(identity),
                (linework, edit) => linework.Update(edit)
            );

            // Create polylineworks via override input
            var polylineworks = input.Overrides.Polylines.CreateElements(
              input.Overrides.Additions.Polylines,
              input.Overrides.Removals.Polylines,
              (add) => new Polylinework(add),
              (polylinework, identity) => polylinework.Match(identity),
              (polylinework, edit) => polylinework.Update(edit)
            );

            // Create bezierworks via override input
            var bezierworks = input.Overrides.Beziers.CreateElements(
              input.Overrides.Additions.Beziers,
              input.Overrides.Removals.Beziers,
              (add) => new Bezierwork(add),
              (bezierwork, identity) => bezierwork.Match(identity),
              (bezierwork, edit) => bezierwork.Update(edit)
            );

            // Add drawn lineworks, polylineworks, and bezierworks to the model
            output.Model.AddElements(lineworks);
            output.Model.AddElements(polylineworks);
            output.Model.AddElements(bezierworks);

            List<object> curves = new List<object>();

            /// CIRCLE
            // Create circle
            var circle = new Circle(new Vector3(5, 5, 0), 4);
            var circlework = new Circlework(circle);
            output.Model.AddElement(circlework);

            /// ARC
            // Create arc
            var arc = new Arc(new Vector3(15, 5, 0), 4, 0.0, 270.0);
            var arcwork = new Arcwork(arc);
            output.Model.AddElement(arcwork);

            /// ELLIPSE
            // Create ellipse
            var ellipse = new Ellipse(new Vector3(25, 5, 0), 2, 4);
            var divisions = 40; // Number of divisions for the polyline ellipse approximation
            var points = new List<Vector3>();

            for (var i = 0; i <= divisions; i++)
            {
                var t = (double)i / divisions;
                var point = ellipse.PointAt(t * (2 * Math.PI));
                points.Add(point);
            }

            var polylineEllipse = new Polyline(points);

            // Add the polyline to the model
            output.Model.AddElement(new Polylinework(polylineEllipse, false));
            var ellipsework = ellipse;

            /// BEZIER
            // Create bezier
            var bezier = new Bezier(
                new List<Vector3>()
                {
                    new Vector3(31, 5, 0),
                    new Vector3(35, 20, 0),
                    new Vector3(35, -10, 0),
                    new Vector3(39, 5.01, 0),
                }
            );
            var bezierwork = new Bezierwork(bezier);
            output.Model.AddElement(bezierwork);
            bezierworks.Add(bezierwork);

            /// LINE
            // Create line
            var line = new Line(new Vector3(45, 1, 0), new Vector3(45, 9, 0));
            var linework = new Linework(line);
            lineworks.Add(linework);
            output.Model.AddElement(linework);

            /// POLYLINE
            // Create polyline
            var polyline = new Polyline(
                new List<Vector3>()
                {
                    new Vector3(56.5, 9, 0),
                    new Vector3(54, 6.5, 0),
                    new Vector3(55, 6.5, 0),
                    new Vector3(52, 4, 0),
                    new Vector3(53, 4, 0),
                    new Vector3(51, 1, 0),
                    new Vector3(55.5, 4.5, 0),
                    new Vector3(54.5, 4.5, 0),
                    new Vector3(57.5, 7, 0),
                    new Vector3(56.5, 7, 0),
                    new Vector3(59, 9, 0),
                }
            );

            var polylinework = new Polylinework(polyline);
            polylineworks.Add(polylinework);
            output.Model.AddElement(polylinework);

            curves.AddRange(lineworks);
            curves.AddRange(polylineworks);
            curves.AddRange(bezierworks);
            curves.Add(circlework);
            curves.Add(arcwork);
            curves.Add(ellipsework);

            var parameter = input.Parameter;
            var directionMod = parameter == 0.0 ? 0.01 : -0.01;
            var size = 1.0;
            var subsize = 0.5;
            foreach (var curve in curves)
            {
                if (curve is Polylinework _polylinework)
                {
                    // Find the point at the given parameter
                    // Polyline parameterization is vertex based, 0->n
                    // where n = Polyline.Vertices.Count() - 1

                    ///
                    /// TEMPORARY METHOD TO FIND LENGTH BASED PARAMETER ///
                    ///
                    var length = _polylinework.Polyline.Length() * parameter;
                    var parameterMod = 0.0;
                    double countingLength = 0.0;
                    foreach (Line segment in _polylinework.Polyline.Segments())
                    {
                        // Find the parameter value via length
                        if (countingLength + segment.Length() > length)
                        {
                            var distanceToMid = length - countingLength;
                            parameterMod += distanceToMid / segment.Length();
                            break;
                        }
                        else
                        {
                            countingLength += segment.Length();
                            parameterMod++;
                        }
                    }
                    ///
                    /// TEMPORARY METHOD TO FIND LENGTH BASED PARAMETER ///
                    ///

                    var point = _polylinework.Polyline.PointAt(parameterMod);
                    // Find an appropriate direction to orient our mass
                    var direction = _polylinework.Polyline.PointAt(parameterMod) - _polylinework.Polyline.PointAt(parameterMod + directionMod);
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);

                    // Find the point at the midpoint
                    // Polyline parameterization is vertex based, 0->n
                    // where n = Polyline.Vertices.Count() - 1

                    ///
                    /// TEMPORARY METHOD TO FIND LENGTH BASED PARAMETER ///
                    ///
                    var midlength = _polylinework.Polyline.Length() / 2.0;
                    var midparameter = 0.0;
                    countingLength = 0.0;
                    foreach (Line segment in _polylinework.Polyline.Segments())
                    {
                        // Find the parameter value via length
                        if (countingLength + segment.Length() > midlength)
                        {
                            var distanceToMid = midlength - countingLength;
                            midparameter += distanceToMid / segment.Length();
                            break;
                        }
                        else
                        {
                            countingLength += segment.Length();
                            midparameter++;
                        }
                    }
                    ///
                    /// TEMPORARY METHOD TO FIND LENGTH BASED PARAMETER ///
                    ///

                    var midpoint = _polylinework.Polyline.PointAt(midparameter);
                    // Find an appropriate direction to orient our mass
                    var middirection = _polylinework.Polyline.PointAt(midparameter) - _polylinework.Polyline.PointAt(midparameter + directionMod);
                    var midmass = MassAtPointAndOrientation(size, midpoint, middirection);
                    output.Model.AddElement(midmass);

                    if (_polylinework.Polyline.Segments().Count() > 1)
                    {
                        foreach (var segment in _polylinework.Polyline.Segments())
                        {
                            // Find the point at the given parameter
                            var segmentpoint = segment.PointAt(parameter * segment.Length());
                            // Find an appropriate direction to orient our mass
                            var segmentdirection = segment.Direction();
                            var segmentmass = MassAtPointAndOrientation(subsize, segmentpoint, segmentdirection);
                            output.Model.AddElement(segmentmass);

                            // Find the midpoint
                            var segmentmidpoint = segment.Mid();
                            var segmentmidmass = MassAtPointAndOrientation(subsize, segmentmidpoint, segmentdirection);
                            output.Model.AddElement(segmentmidmass);
                        }
                    }
                }
                else if (curve is Linework _linework)
                {
                    // Find the point at the given parameter
                    // Line parameterization is length based, 0->length
                    var parameterMod = parameter * _linework.Line.Length();
                    var point = _linework.Line.PointAt(parameterMod);
                    // Find an appropriate direction to orient our mass
                    var direction = _linework.Line.Direction();
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);
                }
                else if (curve is Bezierwork _bezierwork)
                {
                    // Find the point at the given parameter
                    // Bezier parameterization is domain based, 0->1
                    var point = _bezierwork.Bezier.PointAt(parameter);
                    // Find an appropriate direction to orient our mass
                    var direction = _bezierwork.Bezier.PointAt(parameter) - _bezierwork.Bezier.PointAt(parameter + directionMod);
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);
                }
                else if (curve is Circlework _circlework)
                {
                    // Find the point at the given parameter
                    // Circle parameterization is domain based, 0->2PI
                    var parameterMod = parameter * (2 * Math.PI);
                    var point = _circlework.Circle.PointAt(parameterMod);
                    // Find an appropriate direction to orient our mass
                    var direction = _circlework.Circle.PointAt(parameterMod) - _circlework.Circle.PointAt(parameterMod + directionMod);
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);
                }
                else if (curve is Arcwork _arcwork)
                {
                    // Find the point at the given parameter
                    // Arc parameterization is domain based, 0->2PI
                    var parameterMod = parameter * _arcwork.Arc.Domain.Max;
                    var point = _arcwork.Arc.PointAt(parameterMod);
                    // Find an appropriate direction to orient our mass
                    var direction = _arcwork.Arc.PointAt(parameterMod) - _arcwork.Arc.PointAt(parameterMod + directionMod);
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);
                }
                else if (curve is Ellipse _ellipse)
                {
                    // Find the point at the given parameter
                    // Ellipse parameterization is domain based, 0->2PI
                    var parameterMod = parameter * (2 * Math.PI);
                    var point = _ellipse.PointAt(parameterMod);
                    // Find an appropriate direction to orient our mass
                    var direction = _ellipse.PointAt(parameterMod) - _ellipse.PointAt(parameterMod + directionMod);
                    var mass = MassAtPointAndOrientation(size, point, direction);
                    output.Model.AddElement(mass);
                }
            }
            return output;
        }

        public static Mass MassAtPointAndOrientation(Double size, Vector3 point, Vector3 direction, Material? material = null)
        {
            var mass = new Mass(Polygon.Rectangle(size, size), size);
            var center = mass.Bounds.Center() + new Vector3(0, 0, size / 2.0) + mass.Transform.Origin;
            mass.Transform = new Transform(-1 * center).Concatenated(new Transform(new Plane(point, direction)));
            if (material != null)
            {
                mass.Material = material;
            }
            return mass;
        }
    }
}