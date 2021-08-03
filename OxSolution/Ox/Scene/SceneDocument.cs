using System;
using Microsoft.Xna.Framework;
using Ox.Engine;
using Ox.Engine.DocumentNamespace;
using Ox.Engine.Utility;

namespace Ox.Scene
{
    /// <summary>
    /// A manipulable document composed of groups of SceneComponentTokens.
    /// </summary>
    public class SceneDocument : GroupedDocument
    {
        /// <summary>
        /// Create a SceneDocument.
        /// </summary>
        /// <param name="engine">The engine.</param>
        public SceneDocument(OxEngine engine) : base(engine) { }

        /// <summary>
        /// The position of the camera in the editor.
        /// </summary>
        public Vector3 CameraPosition { get; set; }

        /// <summary>
        /// The orientation of the camera in the editor.
        /// </summary>
        public Vector3 CameraOrientation { get; set; }

        /// <summary>
        /// The position snap in the editor.
        /// </summary>
        public float PositionSnap { get; set; }

        /// <summary>
        /// The scale snap in the editor.
        /// </summary>
        public float ScaleSnap { get; set; }

        /// <summary>
        /// The orientation snap in the editor.
        /// </summary>
        public float OrientationSnap { get; set; }

        /// <summary>
        /// The creation depth in the editor.
        /// </summary>
        public float CreationDepth { get; set; }

        /// <inheritdoc />
        protected override ConstructionDictionary ConstructionDictionary
        {
            get { return SceneConfiguration.SceneConstructionDictionary; }
        }

        /// <inheritdoc />
        protected override Type DocumentTokenTypeHook
        {
            get { return typeof(SceneDocumentToken); }
        }

        /// <inheritdoc />
        protected override DocumentToken CreateDocumentTokenHook()
        {
            return new SceneDocumentToken(Groups, Components, CameraPosition, CameraOrientation,
                PositionSnap, ScaleSnap, OrientationSnap, CreationDepth);
        }

        /// <inheritdoc />
        protected override void LoadHook(DocumentToken documentToken)
        {
            base.LoadHook(documentToken);
            SceneDocumentToken sceneDocumentToken = OxHelper.Cast<SceneDocumentToken>(documentToken);
            CameraPosition = sceneDocumentToken.CameraPosition;
            CameraOrientation = sceneDocumentToken.CameraOrientation;
            PositionSnap = sceneDocumentToken.PositionSnap;
            ScaleSnap = sceneDocumentToken.ScaleSnap;
            OrientationSnap = sceneDocumentToken.OrientationSnap;
            CreationDepth = sceneDocumentToken.CreationDepth;
        }
    }
}
