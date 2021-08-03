using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Ox.Engine
{
    /// <summary>
    /// Helper methods for Ox.
    /// </summary>
    public static class OxHelper
    {
        /// <summary>
        /// The same as Type.EmptyTypes except available on all platforms.
        /// </summary>
        public static Type[] EmptyTypes { get { return emptyTypes; } }

        /// <summary>
        /// Perform an action the specified number of times.
        /// </summary>
        public static void Times(this int numberOfTimes, Action action)
        {
            for (int i = 0; i < numberOfTimes; ++i) action();
        }

        /// <summary>
        /// Is the string empty?
        /// </summary>
        public static bool IsEmpty(this string str)
        {
            return str.Length == 0;
        }

        /// <summary>
        /// Is the string not empty?
        /// </summary>
        public static bool IsNotEmpty(this string str)
        {
            return str.Length != 0;
        }

        /// <summary>
        /// Cast an object to type T.
        /// </summary>
        /// <remarks>
        /// It's rather difficult to find C-style casts on object references. Thus, I use and
        /// recommend Ox's clients to use this method to cast object references.
        /// </remarks>
        /// <exception cref="InvalidCastException" />
        public static T Cast<T>(object obj) where T : class
        {
            return (T)obj;
        }

        /// <summary>
        /// Swaps the contents of two values.
        /// </summary>
        public static void Swap<T>(ref T x, ref T y) where T : struct
        {
            T temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// Swaps the contents of two references.
        /// </summary>
        public static void SwapRef<T>(ref T x, ref T y) where T : class
        {
            T temp = x;
            x = y;
            y = temp;
        }

        /// <summary>
        /// Check if the argument is null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void ArgumentNullCheck(object obj)
        {
            if (obj == null) throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void ArgumentNullCheck(object obj, object obj2)
        {
            if (obj == null || obj2 == null) throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void ArgumentNullCheck(object obj, object obj2, object obj3)
        {
            if (obj == null || obj2 == null || obj3 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void ArgumentNullCheck(object obj, object obj2, object obj3, object obj4)
        {
            if (obj == null || obj2 == null || obj3 == null || obj4 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Check if any of the arguments are null.
        /// </summary>
        /// <exception cref="ArgumentNullException" />
        public static void ArgumentNullCheck(
            object obj, object obj2, object obj3, object obj4, object obj5)
        {
            if (obj == null || obj2 == null || obj3 == null || obj4 == null || obj5 == null)
                throw new ArgumentNullException(na, throwMessage);
        }

        /// <summary>
        /// Convert a position value to a safe zone position.
        /// </summary>
        public static Vector2 ToSafePosition(Vector2 vector)
        {
            return vector * OxConfiguration.SafeZoneMultiplier + OxConfiguration.SafeZone.Position;
        }

        /// <summary>
        /// Convert a scale value to a safe zone scale.
        /// </summary>
        public static Vector2 ToSafeScale(Vector2 vector)
        {
            return vector * OxConfiguration.SafeZoneMultiplier;
        }

        /// <summary>
        /// Strip the GUID, if any, from a name.
        /// </summary>
        public static string StripGuid(string name)
        {
            int separatorIndex = name.IndexOf(OxConfiguration.NameGuidSeparator, 0);
            if (separatorIndex == -1) return name;
            return name.Substring(0, separatorIndex);
        }

        /// <summary>
        /// Append a GUID to a name.
        /// </summary>
        public static string AffixGuid(string name, Guid guid)
        {
            return name + OxConfiguration.NameGuidSeparator + guid.ToString();
        }

        /// <summary>
        /// Strip the suffix "Token" from a string.
        /// </summary>
        public static string StripToken(string stringWithToken)
        {
            if (!stringWithToken.EndsWith(stringToken)) throw new ArgumentException(
                "\"" + stringToken + "\" not present at end of " + stringWithToken + ".\n" +
                "Note that all token class names must end with \"" + stringToken + "\".");            
            return stringWithToken.Substring(0, stringWithToken.Length - stringToken.Length);
        }

        /// <summary>
        /// Append the suffix "Token" to a string.
        /// </summary>
        public static string AffixToken(string stringWithoutToken)
        {
            return stringWithoutToken + stringToken;
        }

        /// <summary>
        /// Generate the current bounding box of a model.
        /// </summary>
        public static BoundingBox GenerateBoundingBox(this Model model)
        {
            Vector3 min = new Vector3(float.MaxValue);
            Vector3 max = new Vector3(float.MinValue);
            Matrix[] bonesAbsolute = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(bonesAbsolute);
            foreach (ModelMesh mesh in model.Meshes)
            {
                Matrix boneAbsolute = bonesAbsolute[mesh.ParentBone.Index];
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    int stride = part.VertexStride;
                    int vertexCount = part.NumVertices;
                    byte[] vertexData = new byte[stride * vertexCount];
                    mesh.VertexBuffer.GetData(vertexData);
                    for (int index = 0; index < vertexData.Length; index += stride)
                    {
                        float x = BitConverter.ToSingle(vertexData, index);
                        float y = BitConverter.ToSingle(vertexData, index + 4);
                        float z = BitConverter.ToSingle(vertexData, index + 8);
                        Vector3 vertex = new Vector3(x, y, z);
                        Vector3 vertexWorld = Vector3.Transform(vertex, boneAbsolute);
                        if (vertexWorld.X < min.X) min.X = vertexWorld.X;
                        if (vertexWorld.X > max.X) max.X = vertexWorld.X;
                        if (vertexWorld.Y < min.Y) min.Y = vertexWorld.Y;
                        if (vertexWorld.Y > max.Y) max.Y = vertexWorld.Y;
                        if (vertexWorld.Z < min.Z) min.Z = vertexWorld.Z;
                        if (vertexWorld.Z > max.Z) max.Z = vertexWorld.Z;
                    }
                }
            }
            return new BoundingBox(min, max);		
        }

        private static readonly Type[] emptyTypes = new Type[0];
        private const string throwMessage = "One or more arguments are null that shouldn't be.";
        private const string stringToken = "Token";
        private const string na = "[Not Available]";
    }
}
