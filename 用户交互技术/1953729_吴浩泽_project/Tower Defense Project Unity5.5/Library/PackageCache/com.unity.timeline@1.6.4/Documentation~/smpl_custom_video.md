# VideoTrack sample

The Video Track sample provides a track capable of playing video clips in Timeline. It demonstrates how to do the following:

* Using built-in blending, speed and clip-in capabilities in custom clips.
* Using `ClipEditor`s to customize clip drawing.
* Using a mixer PlayableBehaviour to perform look-ahead operations.
* Managing `UnityEngine.Object` lifetime (`VideoPlayer`) with a `PlayableBehaviour`.
* Using ExposedReferences to reference components in the scene from a `PlayableAsset`.

## Usage

Drag and drop an imported video from the project window onto a timeline. The video track and clip will be created. The video clip has several playback options, including the option to specify the camera to render to video to, and an audio source to redirect the audio.

If no camera is specified, the main camera in the scene will be used. If no audio source is specified, the audio will play directly (i.e. no 3D audio).

## Known Issues

* The video track supports ease-in and ease-out of a video, but blending between videos will not give expected results.
* Editing a timeline containing video clips may cause the clip to flicker or change unexpectedly.
* Looping a timeline with video clips may cause the video to be de-synchronized.
