using System.Collections.Generic;

using UnityEngine;

namespace Unity.PlasticSCM.Editor.UI.Avatar
{
    internal static class AvatarImages
    {
        internal static void Dispose()
        {
            foreach (Texture2D image in mAvatars.Values)
                UnityEngine.Object.DestroyImmediate(image, true);

            mAvatars.Clear();
        }

        internal static bool HasGravatar(string email)
        {
            return mAvatars.ContainsKey(email);
        }

        internal static void AddGravatar(string email, Texture2D image)
        {
            if (mAvatars.ContainsKey(email))
                return;

            mAvatars.Add(email, image);
        }

        internal static void UpdateGravatar(string email, byte[] rawImage)
        {
            if (!mAvatars.ContainsKey(email))
                return;

            Texture2D result = GetTexture(rawImage);

            mAvatars[email] = result;
        }

        internal static Texture2D GetAvatar(string email)
        {
            Texture2D image = GetGravatarImage(email);

            if (image != null)
                return image;

            return Images.GetEmptyGravatar();
        }

        static Texture2D GetGravatarImage(string email)
        {
            Texture2D avatar;
            mAvatars.TryGetValue(email, out avatar);
            return avatar;
        }

        static Texture2D GetTexture(byte[] rawImage)
        {
            Texture2D result = Images.GetNewTextureFromBytes(32, 32, rawImage);
            Texture2D maskImage = ApplyCircleMask.For(result);

            UnityEngine.Object.DestroyImmediate(result, true);

            return maskImage;
        }

        static readonly Dictionary<string, Texture2D> mAvatars =
            new Dictionary<string, Texture2D>();
    }
}