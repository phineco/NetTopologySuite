﻿using System;
using System.Diagnostics;
using GeoAPI.Geometries;
using GeoAPI.Operations.Buffer;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using GisSharpBlog.NetTopologySuite.Noding;
using GisSharpBlog.NetTopologySuite.Operation.Buffer;
using NUnit.Framework;

namespace GisSharpBlog.NetTopologySuite.Tests.Various
{
    [TestFixture]
    public class Issue36Tests
    {
        private readonly IGeometryFactory factory = GeometryFactory.Default;

        private WKTReader reader;        

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            reader = new WKTReader(factory);
        }

        [Test]
        public void Buffer()
        {
            var geometry = reader.Read(
                @"POLYGON((719068.76798974432 6178827.370335687 31.0995,719070.73569863627 
6178830.5852228012 31.0995,719076.87100000086 6178826.8299 31.0995,719078.2722488807 
6178825.9722172953 31.0995,719076.30480000074 6178822.7577000009 
31.0995,719068.76798974432 6178827.370335687 31.0995))");
            Assert.IsNotNull(geometry);
            Assert.IsTrue(geometry.IsValid);

            var buffered = geometry.Buffer(0.01);
            Assert.IsNotNull(buffered);
            Assert.IsTrue(buffered.IsValid);
            Assert.IsFalse(buffered.EqualsExact(geometry));
        }

        [Test]
        public void Buffer2()
        {
            var geometry = reader.Read(
                @"LINESTRING(1250.7665 446.9385,1137.8786 170.4488,1136.3666106287267 
166.74557327980631,1139.485009866369 125.36515638486206,1137.8786 121.7019)");
            Assert.IsNotNull(geometry);
            Assert.IsTrue(geometry.IsValid);

            var buffered = geometry.Buffer(5);
            Assert.IsNotNull(buffered);
            Assert.IsTrue(buffered.IsValid);

            var expected =
                reader.Read(
                    @"POLYGON ((1133.2495665997578 172.33878671409167, 1246.1374665997578 
448.82848671409164, 1246.595130295206 449.6952507660154, 1247.2130973890783 450.4560744320048, 
1247.967619752517 451.0817196852508, 1248.8297015142364 451.5481433295369, 1249.7662133555946 451.8374209657867, 
1250.7411656517302 451.9384358166678, 1251.7170915326622 451.8473059381107, 1252.6564867140917 451.5675334002421, 
1253.5232507660153 451.109869704794, 1254.284074432005 450.4919026109218, 1254.9097196852508 449.737380247483, 
1255.376143329537 448.8752984857635, 1255.6654209657866 447.93878664440524, 1255.7664358166678 446.9638343482697, 
1255.6753059381106 445.98790846733783, 1255.3955334002421 445.0485132859083, 1142.5076334002422 168.55881328590834, 
1141.4410130256356 165.94640267866984, 1144.4708724926961 125.74088750067949, 1144.413756618301 124.52405267228002, 
1144.0640803474464 123.35714405190089, 1142.4576704810775 119.69388766703882, 1141.9779411531201 118.83913872627556, 
1141.3406764435852 118.09440409580651, 1140.5703660778781 117.4883035098286, 1139.696612651358 117.04412907395584, 
1138.7529940181994 116.77895016266294, 1137.7757729132873 116.70295745279682, 1136.8025033956515 116.81907130158044, 
1135.8705876670388 117.12282951892244, 1135.0158387262757 117.60255884687994, 1134.2711040958065 118.23982355641485, 
1133.6650035098287 119.01013392212188, 1133.2208290739559 119.88388734864198, 1132.955650162663 120.82750598180051, 
1132.879657452797 121.80472708671265, 1132.9957713015804 122.7779966043485, 1133.2995295189226 123.70991233296117, 
1134.4055086776207 126.23198674214223, 1131.3807480023995 166.36984216398886, 1131.427488670157 167.52343848559283, 
1131.7375772284845 168.63555999389797, 1133.2495665997578 172.33878671409167))");

            var result = buffered.EqualsExact(expected);
            Assert.IsTrue(result);
        }

        [Test]
        public void Buffer3()
        {
            var geometry = reader.Read(
                @"LINESTRING(1250.7665 446.9385,1137.8786 170.4488,1136.3666106287267 
166.74557327980631,1139.485009866369 125.36515638486206,1137.8786 121.7019)");
            Assert.IsNotNull(geometry);
            Assert.IsTrue(geometry.IsValid);

            var curveBuilder = new OffsetCurveBuilder(
                geometry.PrecisionModel, 
                OffsetCurveBuilder.DefaultQuadrantSegments)
                {
                    EndCapStyle =  BufferStyle.CapRound
                };
            var curveSetBuilder = new OffsetCurveSetBuilder(geometry, 5, curveBuilder);

            var bufferSegStrList = curveSetBuilder.GetCurves();
            Assert.AreEqual(1, bufferSegStrList.Count);
            
            var segmentString = (SegmentString) bufferSegStrList[0];
            Assert.AreEqual(78, segmentString.Count);

            for (var i = 0; i < segmentString.Coordinates.Length; i++)
            {
                var coord = segmentString.Coordinates[i];
                Debug.WriteLine(String.Format("{1:R} {2:R}", i, coord.X, coord.Y));
            }
        }

        [Test]
        public void TestIsValid()
        {
            var geom1 = reader.Read(
                    @"POLYGON((719068.76798974432 6178827.370335687 31.0995,719070.73569863627 6178830.5852228012 31.0995,719076.87100000086 6178826.8299 31.0995,719078.2722488807 6178825.9722172953 31.0995,719076.30480000074 6178822.7577000009 31.0995,719068.76798974432 6178827.370335687 31.0995))");
            Assert.IsNotNull(geom1);
            Assert.IsTrue(geom1.IsValid);

            var geom2 = reader.Read(
                    @"POINT(719080.36969999934 6178824.6883999994)");
            Assert.IsNotNull(geom2);
            Assert.IsTrue(geom2.IsValid);

            var expected = reader.Read(
                    @"POLYGON ((719068.7579976716 6178827.369937588, 719068.758112008 6178827.371894637, 719068.7586059568 6178827.373791773, 719068.759460535 6178827.375556088, 719070.727169427 6178830.590443202, 719070.7283517772 6178830.592006875, 719070.7298164692 6178830.593309835, 719070.7315072144 6178830.594302007, 719070.733359037 6178830.594945263, 719070.7353007706 6178830.595214883, 719070.7372577946 6178830.595100504, 719070.7391548998 6178830.594606523, 719070.74091918 6178830.593751923, 719070.7409191797 6178830.593751923, 719076.8762205446 6178826.838429122, 719078.2774694242 6178825.980746417, 719078.2774694244 6178825.980746417, 719078.2790330398 6178825.979564076, 719078.2803359522 6178825.97809941, 719078.2813280922 6178825.976408705, 719078.2819713342 6178825.974556932, 719078.2822409592 6178825.97261525, 719078.2821266062 6178825.970658276, 719078.2816326694 6178825.968761212, 719078.2807781298 6178825.966996959, 719076.31332925 6178822.752479665, 719076.3121469484 6178822.750916023, 719076.3106823174 6178822.749613076, 719076.30899164 6178822.748620895, 719076.3071398856 6178822.747977607, 719076.3051982138 6178822.747707932, 719076.3032412394 6178822.747822234, 719076.3013441653 6178822.748316121, 719076.2995798928 6178822.749170613, 719068.7627696363 6178827.361806299, 719068.7627696362 6178827.361806299, 719068.7612059064 6178827.36298861, 719068.759902887 6178827.364453278, 719068.7589106546 6178827.366144013, 719068.7582673416 6178827.36799584, 719068.7579976716 6178827.369937588))");
            Assert.IsNotNull(expected);
            Assert.IsTrue(expected.IsValid);

            var actual = geom1.Buffer(0.01);
            Assert.IsNotNull(actual);
            Assert.IsTrue(actual.IsValid);

            Assert.IsTrue(expected.EqualsExact(actual));
        }
    }
}