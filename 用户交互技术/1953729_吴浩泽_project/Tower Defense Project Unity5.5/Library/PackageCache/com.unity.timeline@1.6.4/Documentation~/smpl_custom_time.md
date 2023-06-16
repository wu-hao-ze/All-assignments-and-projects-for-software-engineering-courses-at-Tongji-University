# Time Dilation Track sample

This type of track can be used to adjust Unity's global `Time.timeScale` for the duration of the clip. The most common use would be to create bullet-time style effects. The sample demonstrates the following:

* Creating a custom TrackMixer PlayableBehaviour that performs custom blending of clip values.
* Setting and restoring Unity global values in a PlayableBehaviour.
* How to support blending and extrapolation on custom clips.
* Provide custom clip data that can be animated using the inline curve editor using a PlayableBehaviour template.

## Usage

Create a TimeDilationTrack in timeline using the Add Track menu, found under the Timeline.Samples submenu. Add clips to the track, and use the inspector to set scale values for the clip, or use the inline curve editor to animate the scale values. Clips can also be overlapped to create transitions between clips.
