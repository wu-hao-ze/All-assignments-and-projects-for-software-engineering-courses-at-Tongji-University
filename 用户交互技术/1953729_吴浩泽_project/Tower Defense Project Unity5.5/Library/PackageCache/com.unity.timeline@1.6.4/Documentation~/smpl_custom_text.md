# Text Track sample

_This track requires the TextMeshPro package to be installed in the project._

This type of track can be used to display different messages to the screen using a TextMeshPro Text Component. It is ideal for things like subtitles. It demonstrates the following:

* Registering custom previewable properties in a custom track.
* Perform custom blending of clips using a mixer PlayableBehaviour.
* Provide custom clip data that can be animated using the inline curve editor using a PlayableBehaviour template.
* Using a ClipEditor to react to changes in a clip.

## Usage

To use this custom track, drag a TextMeshPro Text Component into the hierarchy view of the Timeline. A TextTrack will be created, and use the track context menu to create clips. Clip properties such as Message, FontSize and Color can be modified in the inspector by selecting the clip. Clips can be overlapped to create transitions.
