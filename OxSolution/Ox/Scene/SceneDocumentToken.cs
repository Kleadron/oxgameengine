using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Ox.Engine.Component;
using Ox.Engine.DocumentNamespace;

namespace Ox.Scene
{
    /// <summary>
    /// The token to which a scene document will serialize.
    /// </summary>
    public class SceneDocumentToken : GroupedDocumentToken
    {
        public SceneDocumentToken() { }

        public SceneDocumentToken(List<GroupToken> groups, List<ComponentToken> components,
            Vector3 cameraPosition, Vector3 cameraOrientation, float positionSnap, float scaleSnap,
            float orientationSnap, float creationDepth)
            : base(groups, components)
        {
            CameraPosition = cameraPosition;
            CameraOrientation = cameraOrientation;
            PositionSnap = positionSnap;
            ScaleSnap = scaleSnap;
            OrientationSnap = orientationSnap;
            CreationDepth = creationDepth;
        }

        public Vector3 CameraPosition = new Vector3(0, 0, 100);
        public Vector3 CameraOrientation;
        public float PositionSnap = 1;
        public float ScaleSnap = 1.0f / 8.0f;
        public float OrientationSnap = 15;
        public float CreationDepth = 100;
    }
}