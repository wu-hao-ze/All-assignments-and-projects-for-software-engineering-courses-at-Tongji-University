using System;

using UnityEngine;

using PlasticGui;

namespace Unity.PlasticSCM.Editor.UI.Avatar
{
    internal static class GetAvatar
    {
        internal static Texture2D ForEmail(
            string email,
            Action avatarLoadedAction)
        {
            if (string.IsNullOrEmpty(email))
                return Images.GetEmptyGravatar();

            if (AvatarImages.HasGravatar(email))
                return AvatarImages.GetAvatar(email);

            Texture2D defaultImage =
                Images.GetEmptyGravatar();

            AvatarImages.AddGravatar(email, defaultImage);

            LoadAvatar.ForEmail(
                email, avatarLoadedAction,
                AfterDownloadSucceed);

            return defaultImage;
        }

        static void AfterDownloadSucceed(
            string email,
            byte[] avatarBytes,
            Action avatarLoadedAction)
        {
            AvatarImages.UpdateGravatar(email, avatarBytes);

            avatarLoadedAction();
        }
    }
}