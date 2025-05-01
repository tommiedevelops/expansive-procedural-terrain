using System.Collections;
using NUnit.Framework;
using UnityEngine;

namespace EditModeTests {
    public class QTViewerTests {
        float DegToRad(float angleInDeg) { return angleInDeg * Mathf.PI / 180f; }
        [Test]
        public void Can_Initialise_A_QTViewer() {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            go.transform.LookAt(new Vector3(0f, 0f, 1f));
            float fov = 60;
            float rd = 10;

            var viewer = new QTViewer(go.transform, fov, rd);

            Assert.That(viewer, Is.Not.Null);
            Assert.That(viewer.GetFOV(), Is.EqualTo(fov));
            Assert.That(viewer.GetRenderDist(), Is.EqualTo(rd));
        }

        [Test]
        public void Can_Compute_The_Correct_View_Triangle() {
            var go = new GameObject();
            go.transform.position = Vector3.zero;
            go.transform.LookAt(new Vector3(0f, 0f, 1f));
            float fov = 60;
            float rd = 10;

            var viewer = new QTViewer(go.transform, fov, rd);

            var point_1 = Vector3.zero;
            var point_2 = new Vector3(-10f*Mathf.Tan(DegToRad(60/2f)), 0f, 10f);
            var point_3 = new Vector3(10f*Mathf.Tan(DegToRad(60/2f)), 0f, 10f);

            Vector3[] expectedViewTriangle = { point_1, point_2, point_3 };
            var actualViewTriangle = viewer.GetViewTriangle();

            Assert.That(expectedViewTriangle, Is.EqualTo(actualViewTriangle));

        }


    }
}
